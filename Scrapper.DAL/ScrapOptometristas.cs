using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;
public class ScrapOptometristas
{
    private const string BaseUrl = "https://www.colegiodeoptometristas.com/lista-de-colegiados-2/";

    public async Task ProcessColegiadosData()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest();

        try
        {
            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Content);

                var titles = doc.DocumentNode.SelectNodes("//h4[contains(@class,'rt_heading')]//p[not(normalize-space()='')]");
                var tables = doc.DocumentNode.SelectNodes("//div[@class='wpb_wrapper']//table");

                if (tables != null && titles != null && titles.Count == tables.Count)
                {
                    using (var context = new ColegiosProfesionalesContext())
                    {
                        // Obtener el ID del colegio "Optometrista"
                        int colegioId = await context.Colegios
                                                     .Where(c => c.Profesion == "Optometrista")
                                                     .Select(c => c.Id)
                                                     .FirstOrDefaultAsync();

                        if (colegioId == 0)
                        {
                            Console.WriteLine("No se encontró el colegio 'Optometrista'. Verifique la profesión.");
                            return;
                        }

                        for (int i = 0; i < tables.Count; i++)
                        {
                            string estado = HtmlEntity.DeEntitize(titles[i].InnerText.Replace("Colegiados", "").Trim());
                            var rows = tables[i].SelectNodes(".//tr/td");

                            if (rows != null)
                            {
                                foreach (var cell in rows)
                                {
                                    string nombre = HtmlEntity.DeEntitize(cell.InnerText.Trim());
                                    if (!context.Colegiados.Any(c => c.Nombre == nombre))
                                    {
                                        context.Colegiados.Add(new Colegiado
                                        {
                                            Nombre = nombre,
                                            CondicionColegiadoId = 1,
                                            ColegioId = colegioId 
                                        });
                                        Console.WriteLine($"Añadido: {nombre} - {estado}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Ya existe: {nombre} - {estado}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"No se encontraron nombres en la tabla {i + 1}.");
                            }
                        }
                        await context.SaveChangesAsync();
                        Console.WriteLine("Todos los datos han sido guardados en la base de datos.");
                    }
                }
                else
                {
                    Console.WriteLine("No se pudieron encontrar las tablas o títulos en la página, o su cantidad no coincide.");
                }
            }
            else
            {
                Console.WriteLine("Fallo al recuperar la página o contenido vacío.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Se produjo un error durante el proceso: {ex.Message}");
        }
    }
}
