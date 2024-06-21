using HtmlAgilityPack;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using RestSharp;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using EFCore.BulkExtensions;

public class ScrapColyPro
{
    private const string BaseUrl = "https://www.colypro.com/buscador-de-colegiados/";

    public async Task DownloadAndProcessPdfAsync()
    {
        try
        {
            string pdfUrl = await GetPdfUrlAsync();
            if (!string.IsNullOrEmpty(pdfUrl))
            {
                string filePath = await DownloadPdfAsync(pdfUrl);
                if (!string.IsNullOrEmpty(filePath))
                {
                    await ProcessPdf(filePath);
                    Console.WriteLine("Se proceso completamente el PDF.");
                }
                else
                {
                    Console.WriteLine("Fallo la descarga del PDF");
                }
            }
            else
            {
                Console.WriteLine("No se puede encontrar el PDF");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async Task<string> GetPdfUrlAsync()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest();
        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(response.Content);
            var downloadLinkNode = doc.DocumentNode.SelectSingleNode("//a[@class='boton descargar']");
            if (downloadLinkNode != null)
            {
                return downloadLinkNode.GetAttributeValue("href", null);
            }
            else
            {
                Console.WriteLine("No se encontro el link para encontrar la pagina");
            }
        }
        else
        {
            Console.WriteLine($"Fallo al cargar la pagina. Estado: {response.StatusCode}");
        }
        return null;
    }

    private async Task<string> DownloadPdfAsync(string pdfUrl)
    {
        var client = new RestClient(pdfUrl);
        var request = new RestRequest();
        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "downloaded.pdf");
            await System.IO.File.WriteAllBytesAsync(filePath, response.RawBytes);
            Console.WriteLine("Se descargo el PDF.");
            return filePath;
        }
        else
        {
            Console.WriteLine($"Fallo la descarga del PDF. Estado: {response.StatusCode}");
        }
        return null;
    }

    private async Task ProcessPdf(string filePath)
    {
        using (PdfReader reader = new PdfReader(filePath))
        {
            StringBuilder text = new StringBuilder();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            }
            await ExtractDataFromText(text.ToString());
        }
    }

    private async Task ExtractDataFromText(string text)
    {
        var lines = text.Split('\n');
        bool isTableStarted = false;
        ConcurrentBag<Colegiado> colegiados = new ConcurrentBag<Colegiado>();

        using (var context = new ColegiosProfesionalesContext())
        {

            var colegio = context.Colegios.FirstOrDefault(c => c.Descripcion == "Colegio de Licenciados y Profesores");
            if (colegio == null)
            {
                colegio = new Colegio
                {
                    Descripcion = "Colegio de Licenciados y Profesores",
                    Profesion = "Educación",
                    Web = BaseUrl,
                    Fecha = DateTime.Now,
                    Modulo = false,
                    Titulo = "Licenciados y Profesores"
                };
                context.Colegios.Add(colegio);
                await context.SaveChangesAsync();
            }
            int colegioId = colegio.Id;

            foreach (var line in lines)
            {
                if (line.Contains("Cédula"))
                {
                    isTableStarted = true;
                    continue;
                }
                if (isTableStarted && !string.IsNullOrWhiteSpace(line))
                {
                    var matches = Regex.Matches(line, @"(\d+)\s+(.*?)(Retiro Indefinido|Suspendido|Activo|Retiro Temporal)(?=\d|$)");
                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            string Identificacion = match.Groups[1].Value.Trim();
                            string Nombre = match.Groups[2].Value.Trim();
                            colegiados.Add(new Colegiado
                            {
                                Identificacion = Identificacion,
                                Nombre = Nombre,
                                CondicionColegiadoId = 1,  
                                ColegioId = colegioId
                            });
                        }
                    }
                }
            }

            
            if (colegiados.Any())
            {
                await context.BulkInsertAsync(colegiados.ToList());
                Console.WriteLine("Todos los colegiados se guardaron conrrectamente");
            }
        }
    }
}
