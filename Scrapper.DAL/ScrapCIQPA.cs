using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using ScrapperData.Database.Contexto;
using ScrapperData.Database.Models;

public class ScrapCIQPA
{
    public ScrapCIQPA()
    {
    }

    public async Task ProcessColegiadoData()
    {
        string url = "https://www.ciqpacr.com/colegiados/";
        var client = new RestClient(url);
        var request = new RestRequest();

        try
        {
            var response = await client.ExecuteAsync(request, Method.Get);

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Content);
                var rows = doc.DocumentNode.SelectNodes("//table[@id='table_2']/tbody/tr");

                if (rows != null)
                {
                    using (var _context = new ColegiosProfesionalesContext())
                    {
                        // Obtener el ID del colegio "Químicos"
                        int colegioId = await _context.Colegios
                                                      .Where(c => c.Profesion == "Quimico")
                                                      .Select(c => c.Id)
                                                      .FirstOrDefaultAsync();
                        if (colegioId == 0)
                        {
                            Console.WriteLine("No se encontró el colegio 'Químicos'. Verifique la profesión.");
                            return;
                        }

                        foreach (var row in rows)
                        {
                            string nombre = row.SelectSingleNode(".//td[3]").InnerText.Trim();
                            string primerApellido = row.SelectSingleNode(".//td[4]").InnerText.Trim();
                            string segundoApellido = row.SelectSingleNode(".//td[5]").InnerText.Trim();

                            string NombreCompleto = $"{nombre} {primerApellido} {segundoApellido}";

                            string profesion = row.SelectSingleNode(".//td[7]").InnerText.Trim();
                            string estado = row.SelectSingleNode(".//td[8]").InnerText.Trim();

                            var existingColegiado = await _context.Colegiados.FirstOrDefaultAsync(c => c.Nombre == NombreCompleto);
                            if (existingColegiado == null)
                            {
                                var colegiado = new Colegiado
                                {
                                    Nombre = NombreCompleto,
                                    Especialidad = profesion,
                                    CondicionColegiadoId = 1,
                                    ColegioId = colegioId  // Asignar el ID del colegio encontrado
                                };

                                _context.Colegiados.Add(colegiado);
                                Console.WriteLine($"Añadiendo a la base de datos: Nombre: {NombreCompleto}, Profesión: {profesion}, Estado: {estado}");
                            }
                            else
                            {
                                Console.WriteLine($"Duplicado encontrado, se omite: Nombre: {NombreCompleto}");
                            }
                        }
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Todos los datos han sido procesados y guardados en la base de datos.");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos para procesar.");
                }
            }
            else
            {
                Console.WriteLine("Fallo al recuperar la página o contenido vacío. Deteniendo...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Se produjo un error durante el proceso: {ex.Message}");
        }
    }
}
