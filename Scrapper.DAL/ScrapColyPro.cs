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
                    Console.WriteLine("PDF has been processed successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to download PDF.");
                }
            }
            else
            {
                Console.WriteLine("PDF URL could not be found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async Task<string> GetPdfUrlAsync()
    {
        try
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
                    Console.WriteLine("No download link found on the page.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to load the web page. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the PDF URL: {ex.Message}");
        }
        return null;
    }

    private async Task<string> DownloadPdfAsync(string pdfUrl)
    {
        try
        {
            var client = new RestClient(pdfUrl);
            var request = new RestRequest();
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "downloaded.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, response.RawBytes);
                Console.WriteLine("PDF downloaded successfully.");
                return filePath;
            }
            else
            {
                Console.WriteLine($"Failed to download the PDF. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while downloading the PDF: {ex.Message}");
        }
        return null;
    }

    private async Task ProcessPdf(string filePath)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing the PDF: {ex.Message}");
        }
    }

    private async Task ExtractDataFromText(string text)
    {
        var lines = text.Split('\n');
        bool isTableStarted = false;
        ConcurrentBag<Colegiado> colegiados = new ConcurrentBag<Colegiado>();

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
                        string Estado = match.Groups[3].Value.Trim();  

                        colegiados.Add(new Colegiado
                        {
                            Identificacion = Identificacion,
                            Nombre = Nombre,
                            CondicionColegiadoId = 1 
                        });
                    }
                }
            }
        }

        using (var context = new ColegiosProfesionalesContext())
        {
            if (colegiados.Any())
            {
                await context.BulkInsertAsync(colegiados.ToList());
                Console.WriteLine("se inserto todo!!!");
            }
        }
    }
}
