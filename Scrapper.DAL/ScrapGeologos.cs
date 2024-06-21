using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;

public class ScrapGeologos
{
    private const string BaseUrl = "https://www.geologos.or.cr/agremiados/";

    public async Task ProcessAgremiadosData()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest
        {
            Method = Method.Get
        };

        try
        {
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Content);
                var agremiadosDiv = doc.DocumentNode.SelectSingleNode("//div[@class='sow-tabs-panel-content']");

                if (agremiadosDiv != null)
                {
                    var pContent = agremiadosDiv.SelectSingleNode("./div/p");

                    if (pContent != null)
                    {
                        var namesHtml = pContent.InnerHtml;
                        var names = namesHtml.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                        using (var context = new ColegiosProfesionalesContext())
                        {
                            
                            int colegioId = await context.Colegios
                                                        .Where(c => c.Profesion == "Geologo")
                                                        .Select(c => c.Id)
                                                        .FirstOrDefaultAsync();

                            if (colegioId == 0)
                            {
                                Console.WriteLine("No se encontró el colegio 'Colypro'. Verifique la profesión.");
                                return;
                            }

                            foreach (var name in names)
                            {
                                var cleanName = HtmlEntity.DeEntitize(name).Trim();
                                if (!context.Colegiados.Any(a => a.Nombre == cleanName))
                                {
                                    var colegiado = new Colegiado
                                    {
                                        Nombre = cleanName,
                                        CondicionColegiadoId = 1,
                                        ColegioId = colegioId  
                                    };
                                    context.Colegiados.Add(colegiado);
                                    Console.WriteLine($"Añadiendo a la base de datos: {cleanName}");
                                }
                                else
                                {
                                    Console.WriteLine($"El nombre ya existe y no será añadido: {cleanName}");
                                }
                            }
                            await context.SaveChangesAsync();
                            Console.WriteLine("Todos los datos han sido procesados y actualizados en la base de datos.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se encontraron etiquetas <p> con datos.");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontró el div de agremiados.");
                }
            }
            else
            {
                Console.WriteLine("Fallo al recuperar la página o contenido vacío.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error durante la solicitud HTTP: {ex.Message}");
        }
    }
}
