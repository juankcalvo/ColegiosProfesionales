using Newtonsoft.Json.Linq;
using RestSharp;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;

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

            await Parallel.ForEachAsync(pagesList, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, async (page, ct) =>
            {
                using ColegiosProfesionalesContext _context = new ColegiosProfesionalesContext();

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
                        var articles = doc.DocumentNode.SelectNodes("//article[contains(@class, 'Colegiado')]");

                        if (articles != null)
                        {
                            foreach (var article in articles)
                            {
                                var Colegiado = new Colegiado
                                {
                                    Nombre = article.SelectSingleNode(".//div[contains(@class, 'name')]")?.InnerText.Trim(),
                                    Especialidad = article.SelectSingleNode(".//div[contains(@class, 'specialty')]")?.InnerText.Trim(),
                                    //LicenseType = article.SelectSingleNode(".//span[contains(@class, 'professional-license-type')]")?.InnerText.Trim(),
                                    NumeroCarne = article.SelectSingleNode(".//span[contains(@class, 'license-number')]")?.InnerText.Trim()
                                };

                                var existingColegiado = await _context.Colegiados
                                    .FirstOrDefaultAsync(d => d.NumeroCarne == Colegiado.NumeroCarne);

                                if (existingColegiado == null)
                                {
                                    var photoUrl = article.SelectSingleNode(".//img[@itemprop='image']")?.GetAttributeValue("src", string.Empty).Trim();
                                    if (!string.IsNullOrEmpty(photoUrl) && !photoUrl.Contains("gray-120x150.png"))
                                    {
                                        Colegiado.FotoColegiado = new FotoColegiado { FotoColegiado1 = await DownloadImageAsByteArray(photoUrl) };
                                    }

                                    _context.Colegiados.Add(Colegiado);
                                    Console.WriteLine($"Added: {Colegiado.Nombre}");
                                    Console.WriteLine($"Specialty: {Colegiado.Especialidad}");
                                    Console.WriteLine($"License Number: {Colegiado.NumeroCarne}");
                                    Console.WriteLine($"Photo URL: {photoUrl}");
                                    Console.WriteLine($"Photo varbinary: {(Colegiado.FotoColegiado?.FotoColegiado1 == null || Colegiado.FotoColegiado.FotoColegiado1.Length == 0 ? "no tiene foto de perfil" : Colegiado.FotoColegiado.FotoColegiado1.Length.ToString())}");
                                    Console.WriteLine("-_-_-_-_-_-_-_-_-_-_s-_-_-_-_-_-_-_-_-_-_-_-");
                                }
                                else
                                {
                                    Console.WriteLine($"Duplicate found, skipping: {Colegiado.Nombre} with License Number: {Colegiado.NumeroCarne}");
                                }
                            }
                            await _context.SaveChangesAsync();
                            Console.WriteLine("Data saved to database.");
                        }
                        else
                        {
                            Console.WriteLine("No Colegiados found on this page.");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
            }
            return null;
        }
    }
}
