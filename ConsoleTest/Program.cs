class Program
{
    static async Task Main(string[] args)
    {
        ScrapHuli scrapHuli = new ScrapHuli();
        await scrapHuli.ProcessColegiadoData();
    }
}
