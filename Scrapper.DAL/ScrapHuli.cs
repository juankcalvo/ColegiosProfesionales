using Newtonsoft.Json.Linq;
using RestSharp;
using HtmlAgilityPack;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;  // Incluir para WebUtility

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
                colegioId = await context.Colegios
                                         .Where(c => c.Profesion == "medicos y cirujanos")
                                         .Select(c => c.Id)
                                         .FirstOrDefaultAsync();
            }

            if (colegioId == 0)
            {
                Console.WriteLine("No se encontró el colegio especificado. Verifique la profesión.");
                return;
            }

            await Parallel.ForEachAsync(pagesList, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, async (page, ct) =>
            {
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
                            using var _context = new ColegiosProfesionalesContext();

                            foreach (var article in articles)
                            {
                                var nombreHtml = article.SelectSingleNode(".//div[contains(@class, 'name')]")?.InnerText.Trim();
                                var nombre = WebUtility.HtmlDecode(nombreHtml);  

                                var EspecialidadHtml = article.SelectSingleNode(".//div[contains(@class, 'specialty')]")?.InnerText.Trim();
                                var Especialidad = WebUtility.HtmlDecode(EspecialidadHtml);

                                //var rawNumeroCarne = article.SelectSingleNode(".//span[contains(@class, 'license-number')]")?.InnerText.Trim();
                                //var match = Regex.Match(rawNumeroCarne, @"\d+");
                                //var numeroCarne = match.Success ? match.Value : "";

                                var colegiado = new Colegiado
                                {
                                    Nombre = nombre,
                                    Especialidad = Especialidad,
                                    NumeroCarne = article.SelectSingleNode(".//span[contains(@class, 'license-number')]")?.InnerText.Trim(),
                                    CondicionColegiadoId = 1,
                                    ColegioId = colegioId
                                };

                                var existingColegiado = await _context.Colegiados.FirstOrDefaultAsync(c => c.NumeroCarne == colegiado.NumeroCarne);
                                if (existingColegiado == null)
                                {
                                    var photoUrl = article.SelectSingleNode(".//img[@itemprop='image']")?.GetAttributeValue("src", string.Empty).Trim();
                                    if (!string.IsNullOrEmpty(photoUrl) && !photoUrl.Contains("gray-120x150.png"))
                                    {
                                        colegiado.FotoColegiado = new FotoColegiado
                                        {
                                            FotoColegiado1 = await DownloadImageAsByteArray(photoUrl)
                                        };
                                    }

                                    _context.Colegiados.Add(colegiado);
                                    await _context.SaveChangesAsync();
                                    Console.WriteLine($"Added: {colegiado.Nombre}");
                                }
                                else
                                {
                                    Console.WriteLine($"Duplicate found, skipping: {colegiado.Nombre}");
                                }
                            }
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
