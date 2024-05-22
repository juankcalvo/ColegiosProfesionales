using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RestSharp;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

public class ScrapHuli
{
    public async Task ProcessColegiadoData()
    {
        int totalPages;
        string baseUrl = "https://medicoscr.hulilabs.com/es/api/search/keywords";
        var client = new RestClient(baseUrl);
        var initialRequest = new RestRequest("?page=1&l_id=1-13707&kl=1&sort=name_asc&ro=9619", Method.Get);
        var initialResponse = await client.ExecuteAsync(initialRequest);

        Console.WriteLine("Initial request made...");

        if (initialResponse.IsSuccessful)
        {
            var initialData = JObject.Parse(initialResponse.Content);
            int totalCount = (int)initialData["total_count"];
            int pageCount = (int)initialData["page_count"];
            totalPages = (int)Math.Ceiling((double)totalCount / pageCount);
            List<int> pagesList = Enumerable.Range(1, totalPages).ToList();

            Console.WriteLine($"Total pages calculated: {totalPages}");

            int colegioId = 0;
            using (var context = new ColegiosProfesionalesContext())
            {
                var colegio = await context.Colegios
                                          .Where(c => c.Profesion == "Medicos y cirujanos")
                                          .FirstOrDefaultAsync();
                if (colegio == null)
                {
                    colegio = new Colegio
                    {
                        Descripcion = "Colegio de Médicos y Cirujanos",
                        Profesion = "Medicos y cirujanos",
                        Web = baseUrl,
                        Fecha = DateTime.Now,
                        Modulo = false,
                        Titulo = "Medicos y cirujanos"
                    };
                    context.Colegios.Add(colegio);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Colegio creado y añadido a la base de datos.");
                }
                colegioId = colegio.Id;
            }

            await Parallel.ForEachAsync(pagesList, new ParallelOptions() { MaxDegreeOfParallelism = 20 }, async (page, ct) =>
            {
                using var _context = new ColegiosProfesionalesContext();
                var request = new RestRequest($"?page={page}&l_id=1-13707&kl=1&sort=name_asc&ro=9619", Method.Get);
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var data = JObject.Parse(response.Content);
                    var htmlContent = data["content"]?.ToString();

                    Console.WriteLine($"Processing page {page}...");

                    if (!string.IsNullOrWhiteSpace(htmlContent))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(htmlContent);
                        var articles = doc.DocumentNode.SelectNodes("//article[contains(@class, 'doctor')]");

                        if (articles != null && articles.Count > 0)
                        {
                            foreach (var article in articles)
                            {
                                var nombre = WebUtility.HtmlDecode(article.SelectSingleNode(".//div[contains(@class, 'name')]")?.InnerText.Trim());
                                var especialidad = WebUtility.HtmlDecode(article.SelectSingleNode(".//div[contains(@class, 'specialty')]")?.InnerText.Trim());
                                var numeroCarne = article.SelectSingleNode(".//span[contains(@class, 'license-number')]")?.InnerText.Trim();

                                if (!_context.Colegiados.Any(c => c.NumeroCarne == numeroCarne))
                                {
                                    var photoUrl = article.SelectSingleNode(".//img[@itemprop='image']")?.GetAttributeValue("src", string.Empty).Trim();
                                    byte[] photoData = null;
                                    if (!string.IsNullOrEmpty(photoUrl) && !photoUrl.Contains("gray-120x150.png"))
                                    {
                                        photoData = await DownloadImageAsByteArray(photoUrl);
                                    }

                                    var colegiado = new Colegiado
                                    {
                                        Nombre = nombre,
                                        Especialidad = especialidad,
                                        NumeroCarne = numeroCarne,
                                        CondicionColegiadoId = 1,
                                        ColegioId = colegioId,
                                        FotoColegiado = new FotoColegiado
                                        {
                                            FotoColegiado1 = photoData
                                        }
                                    };

                                    _context.Colegiados.Add(colegiado);
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("No colegiados found on this page.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("HTML content is empty or not available.");
                    }
                }
                else
                {
                    Console.WriteLine($"Error in request: {response.ErrorMessage}");
                }
            });
        }
        else
        {
            Console.WriteLine("Error obtaining total pages, aborting execution.");
        }
    }

    private async Task<byte[]> DownloadImageAsByteArray(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    Console.WriteLine("Failed to download image.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
            }
            return null;
        }
    }
}
