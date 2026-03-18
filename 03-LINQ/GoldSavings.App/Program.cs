using System.Xml.Linq;
using GoldSavings.App.Model;
using GoldSavings.App.Services;

namespace GoldSavings.App;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Investor!");
        Console.WriteLine("Loading gold prices...\n");

        GoldDataService dataService = new GoldDataService();

        DateTime startDate = new DateTime(2019, 1, 1);
        DateTime endDate = DateTime.Now;

        List<GoldPrice> goldPrices = await GetGoldPricesInChunks(dataService, startDate, endDate, 93);

        if (goldPrices.Count == 0)
        {
            Console.WriteLine("No data found. Exiting.");
            return;
        }

        Console.WriteLine($"Retrieved {goldPrices.Count} records.");
        Console.WriteLine($"Data range: {goldPrices.Min(g => g.Date):yyyy-MM-dd} -> {goldPrices.Max(g => g.Date):yyyy-MM-dd}");
        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // a) What are the TOP 3 highest and TOP 3 lowest prices of gold within the last year?
        //    - method syntax
        //    - query syntax
        // ------------------------------------------------------------

        DateTime latestDate = goldPrices.Max(g => g.Date);
        DateTime lastYearStart = latestDate.AddYears(-1);

        // METHOD SYNTAX
        var top3HighestMethod = goldPrices
            .Where(g => g.Date >= lastYearStart && g.Date <= latestDate)
            .OrderByDescending(g => g.Price)
            .Take(3)
            .ToList();

        var top3LowestMethod = goldPrices
            .Where(g => g.Date >= lastYearStart && g.Date <= latestDate)
            .OrderBy(g => g.Price)
            .Take(3)
            .ToList();

        // QUERY SYNTAX
        var top3HighestQuery =
            (from g in goldPrices
             where g.Date >= lastYearStart && g.Date <= latestDate
             orderby g.Price descending
             select g)
            .Take(3)
            .ToList();

        var top3LowestQuery =
            (from g in goldPrices
             where g.Date >= lastYearStart && g.Date <= latestDate
             orderby g.Price ascending
             select g)
            .Take(3)
            .ToList();

        Console.WriteLine("a) TOP 3 highest and TOP 3 lowest prices of gold within the last year");
        Console.WriteLine($"Last year window used: {lastYearStart:yyyy-MM-dd} -> {latestDate:yyyy-MM-dd}\n");

        Console.WriteLine("Method syntax - TOP 3 highest:");
        PrintPriceList(top3HighestMethod);

        Console.WriteLine("\nMethod syntax - TOP 3 lowest:");
        PrintPriceList(top3LowestMethod);

        Console.WriteLine("\nQuery syntax - TOP 3 highest:");
        PrintPriceList(top3HighestQuery);

        Console.WriteLine("\nQuery syntax - TOP 3 lowest:");
        PrintPriceList(top3LowestQuery);

        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // b)   If one had bought gold in January 2020,
        //      is it possible that they would have earned more than 5%?
        //      On which days?
        // ------------------------------------------------------------

        var january2020Buys = goldPrices
            .Where(g => g.Date.Year == 2020 && g.Date.Month == 1)
            .OrderBy(g => g.Date)
            .ToList();

        var profitOver5Percent = january2020Buys
            .SelectMany(buy => goldPrices
                .Where(sell => sell.Date > buy.Date && sell.Price >= buy.Price * 1.05)
                .Select(sell => new
                {
                    BuyDate = buy.Date,
                    BuyPrice = buy.Price,
                    SellDate = sell.Date,
                    SellPrice = sell.Price,
                    RoiPercent = (sell.Price - buy.Price) / buy.Price * 100
                }))
            .OrderBy(x => x.BuyDate)
            .ThenBy(x => x.SellDate)
            .ToList();

        bool isPossibleMoreThan5Percent = profitOver5Percent.Any();

        Console.WriteLine("b) If one had bought gold in January 2020, could they earn more than 5%?");
        Console.WriteLine($"Answer: {(isPossibleMoreThan5Percent ? "YES" : "NO")}");

        if (isPossibleMoreThan5Percent)
        {
            var sellDays = profitOver5Percent
                .Select(x => x.SellDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            Console.WriteLine("\nDays on which selling would give more than 5% profit");
            Console.WriteLine("(for at least one January 2020 buying day):");

            Console.WriteLine("\nFirst profitable sell day for each January 2020 buy date:");

            var firstProfitableDayPerBuy = january2020Buys
                .Select(buy => new
                {
                    BuyDate = buy.Date,
                    BuyPrice = buy.Price,
                    FirstSell = goldPrices
                        .Where(sell => sell.Date > buy.Date && sell.Price >= buy.Price * 1.05)
                        .OrderBy(sell => sell.Date)
                        .Select(sell => new
                        {
                            sell.Date,
                            sell.Price,
                            RoiPercent = (sell.Price - buy.Price) / buy.Price * 100
                        })
                        .FirstOrDefault()
                })
                .Where(x => x.FirstSell != null)
                .ToList();

            foreach (var item in firstProfitableDayPerBuy)
            {
                Console.WriteLine(
                    $"Buy: {item.BuyDate:yyyy-MM-dd} at {item.BuyPrice:F2} " +
                    $"-> First sell over 5%: {item.FirstSell!.Date:yyyy-MM-dd} at {item.FirstSell.Price:F2} " +
                    $"({item.FirstSell.RoiPercent:F2}%)");
            }
        }

        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // c) Which 3 dates of 2022-2019 opens the second ten
        //    of the prices ranking?
        // ------------------------------------------------------------

        var secondTenFirstThree = goldPrices
            .Where(g => g.Date.Year >= 2019 && g.Date.Year <= 2022)
            .OrderByDescending(g => g.Price)
            .Skip(10)
            .Take(3)
            .Select(g => new
            {
                g.Date,
                g.Price
            })
            .ToList();

        Console.WriteLine("c) Which 3 dates of 2019-2022 opens the second ten of the prices ranking?");
        Console.WriteLine("Ranking by price descending, positions 11-13.\n");

        if (secondTenFirstThree.Any())
        {
            int rank = 11;
            foreach (var item in secondTenFirstThree)
            {
                Console.WriteLine($"Rank {rank}: {item.Date:yyyy-MM-dd} | Price = {item.Price:F2}");
                rank++;
            }
        }
        else
        {
            Console.WriteLine("No data found for this ranking.");
        }

        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // d) Query syntax: averages of gold prices in 2020, 2023, 2024
        // ------------------------------------------------------------

        var yearlyAverages =
            from g in goldPrices
            where g.Date.Year == 2020 || g.Date.Year == 2023 || g.Date.Year == 2024
            group g by g.Date.Year into yearGroup
            orderby yearGroup.Key
            select new
            {
                Year = yearGroup.Key,
                AveragePrice = yearGroup.Average(x => x.Price)
            };

        Console.WriteLine("d) Average gold prices in 2020, 2023, 2024 (query syntax)");
        foreach (var item in yearlyAverages)
        {
            Console.WriteLine($"{item.Year}: {item.AveragePrice:F2}");
        }

        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // e) Best buy/sell between 2020 and 2024 + ROI
        // ------------------------------------------------------------

        var prices2020to2024 =
            from price in goldPrices
            where price.Date >= new DateTime(2020, 1, 1)
            where price.Date <= new DateTime(2024, 12, 31)
            select price;

        var bestTrade = prices2020to2024
            .SelectMany(buy => goldPrices
                .Where(sell => sell.Date > buy.Date)
                .Select(sell => new
                {
                    BuyDate = buy.Date,
                    BuyPrice = buy.Price,
                    SellDate = sell.Date,
                    SellPrice = sell.Price,
                    RoiPercent = (sell.Price - buy.Price) / buy.Price * 100,
                    Profit = sell.Price - buy.Price
                }))
            .OrderByDescending(x => x.RoiPercent)
            .FirstOrDefault();

        Console.WriteLine("e) Best time to buy and sell gold between 2020 and 2024");

        if (bestTrade != null)
        {
            Console.WriteLine($"Best BUY date : {bestTrade.BuyDate:yyyy-MM-dd}");
            Console.WriteLine($"Buy price     : {bestTrade.BuyPrice:F2}");
            Console.WriteLine($"Best SELL date: {bestTrade.SellDate:yyyy-MM-dd}");
            Console.WriteLine($"Sell price    : {bestTrade.SellPrice:F2}");
            Console.WriteLine($"Profit        : {bestTrade.Profit:F2}");
            Console.WriteLine($"ROI           : {bestTrade.RoiPercent:F2}%");
        }
        else
        {
            Console.WriteLine("Not enough data to determine best trade.");
        }

        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // 3) Save prices to XML file
        // ------------------------------------------------------------

        string xmlFilePath = "goldprices.xml";
        SavePricesToXml(goldPrices, xmlFilePath);
        Console.WriteLine(new string('-', 70));

        // ------------------------------------------------------------
        // 4) Read prices from XML file using one instruction (one semicolon)
        // ------------------------------------------------------------

        XDocument loadedDoc = ReadPricesFromXml(xmlFilePath);

        Console.WriteLine("4) Reading XML file content (one instruction):");
        // Console.WriteLine(loadedDoc);
        Console.WriteLine(new string('-', 70));

        Console.WriteLine("Gold Analysis Queries with LINQ Completed.");
    }

    // ------------------------------------------------------------
    // 3)   Write a method that saves the list of prices to a file in XML format
    // ------------------------------------------------------------

    static void SavePricesToXml(List<GoldPrice> prices, string filePath)
    {
        XDocument doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("GoldPrices",
                prices.Select(p =>
                    new XElement("GoldPrice",
                        new XElement("Date", p.Date.ToString("yyyy-MM-dd")),
                        new XElement("Price", p.Price)
                    )
                )
            )
        );

        doc.Save(filePath);
        Console.WriteLine($"3) Saved {prices.Count} records to '{filePath}'.");
    }

    // ------------------------------------------------------------
    // 4)   Write a method that reads the contents of the XML file from the previous set using one
    //      instruction (you cannot use more than one semicolon)
    // ------------------------------------------------------------

    static XDocument ReadPricesFromXml(string filePath) =>
        XDocument.Load(filePath);

    static void PrintPriceList(IEnumerable<GoldPrice> prices)
    {
        foreach (var item in prices)
        {
            Console.WriteLine($"{item.Date:yyyy-MM-dd} | Price = {item.Price:F2}");
        }
    }
    static async Task<List<GoldPrice>> GetGoldPricesInChunks(
        GoldDataService dataService,
        DateTime startDate,
        DateTime endDate,
        int maxDaysPerRequest)
    {
        var result = new List<GoldPrice>();

        DateTime chunkStart = startDate;

        while (chunkStart <= endDate)
        {
            DateTime chunkEnd = chunkStart.AddDays(maxDaysPerRequest - 1);
            if (chunkEnd > endDate)
                chunkEnd = endDate;

            Console.WriteLine($"Downloading: {chunkStart:yyyy-MM-dd} -> {chunkEnd:yyyy-MM-dd}");

            var chunk = await dataService.GetGoldPrices(chunkStart, chunkEnd);

            if (chunk != null && chunk.Any())
            {
                result.AddRange(chunk);
            }

            chunkStart = chunkEnd.AddDays(1);
        }

        return result
            .GroupBy(x => x.Date.Date)
            .Select(g => g.First())
            .OrderBy(x => x.Date)
            .ToList();
    }
}
