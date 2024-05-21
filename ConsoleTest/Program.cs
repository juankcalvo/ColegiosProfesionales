class Program
{
    static async Task Main(string[] args)
    {
        //ScrapHuli scrapHuli = new ScrapHuli();
        //await scrapHuli.ProcessColegiadoData();


        //ScrapTrabajoSocial scrapTS = new ScrapTrabajoSocial();
        //await scrapTS.ProcessAgremiadoData();


        //ScrapCIQPA scrapCIQPA = new ScrapCIQPA();
        //await scrapCIQPA.ProcessColegiadoData();


        //ScrapGeologos scrapGeologos = new ScrapGeologos();
        //await scrapGeologos.ProcessAgremiadosData();


        ScrapOptometristas scrapOptometristas = new ScrapOptometristas();
        await scrapOptometristas.ProcessColegiadosData();



        Console.ReadKey();
    }
}
