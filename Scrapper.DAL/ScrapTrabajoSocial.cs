using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RestSharp;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;

public class ScrapTrabajoSocial
{
    public ScrapTrabajoSocial()
    {
    }

    public async Task ProcessAgremiadoData()
    {
        string baseUrl = "https://trabajosocial.or.cr/estado-de-las-personas-agremiadas/";
        var client = new RestClient(baseUrl);

        try
        {
            var initialRequest = new RestRequest("?listpage=1&instance=1", Method.Get);
            var initialResponse = await client.ExecuteAsync(initialRequest);

            if (!initialResponse.IsSuccessful)
            {
                Console.WriteLine("Error al obtener los datos iniciales, abortando ejecución.");
                return;
            }

            int totalPages = 100;  
            List<int> pagesList = Enumerable.Range(1, totalPages).ToList();

            
            int colegioId = 0;
            using (var context = new ColegiosProfesionalesContext())
            {
                colegioId = await context.Colegios
                                         .Where(c => c.Profesion == "Trabajo Social")
                                         .Select(c => c.Id)
                                         .FirstOrDefaultAsync();
            }

            if (colegioId == 0)
            {
                Console.WriteLine("No se encontró el colegio 'Trabajo Social'. Verifique la profesión.");
                return;
            }

            await Parallel.ForEachAsync(pagesList, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async (currentPage, ct) =>
            {
                try
                {
                    var request = new RestRequest($"?listpage={currentPage}&instance=1", Method.Get);
                    var response = await client.ExecuteAsync(request, ct);

                    if (response.IsSuccessful)
                    {
                        var htmlContent = response.Content;
                        if (!string.IsNullOrWhiteSpace(htmlContent))
                        {
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(htmlContent);
                            var tableRows = doc.DocumentNode.SelectNodes("//table[contains(@class,'wp-list-table widefat')]/tbody/tr");

                            if (tableRows != null && tableRows.Count > 0)
                            {
                                using (var _context = new ColegiosProfesionalesContext())
                                {
                                    foreach (var row in tableRows)
                                    {
                                        string nombre = row.SelectSingleNode(".//td[1]").InnerText.Trim();
                                        string identificacion = row.SelectSingleNode(".//td[2]").InnerText.Trim();
                                        string numeroCarne = row.SelectSingleNode(".//td[3]").InnerText.Trim();

                                        var existingAgremiado = await _context.Colegiados.FirstOrDefaultAsync(a => a.Identificacion == identificacion, ct);
                                        if (existingAgremiado == null)
                                        {
                                            var agremiado = new Colegiado
                                            {
                                                Nombre = nombre,
                                                Identificacion = identificacion,
                                                NumeroCarne = numeroCarne,
                                                CondicionColegiadoId = 1,
                                                ColegioId = colegioId  
                                            };

                                            _context.Colegiados.Add(agremiado);
                                            Console.WriteLine($"Se añadió: Nombre: {nombre}, Cédula: {identificacion}, Carné: {numeroCarne}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Duplicado: Nombre: {nombre}, Cédula: {identificacion}");
                                        }
                                    }
                                    await _context.SaveChangesAsync(ct);
                                }
                                Console.WriteLine($"Datos de la página {currentPage} guardados en la base de datos.");
                            }
                            else
                            {
                                Console.WriteLine("No hay más datos disponibles en esta página.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Contenido vacío o inválido. Deteniendo el proceso.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Fallo al obtener los datos de la página {currentPage}: {response.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar la página {currentPage}: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Se produjo un error durante la configuración inicial: {ex.Message}");
        }

        Console.WriteLine("Todas las páginas procesadas.");
    }
}
