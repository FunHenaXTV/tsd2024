namespace GoldSavings.App;

public class RandomizedList<T>
{
    private readonly List<T> _items = new List<T>();
    private readonly Random _random = new Random();

    public void Add(T element)
    {
        bool addAtEnd = _random.Next(0, 2) == 0;

        if (addAtEnd)
        {
            _items.Add(element);
            Console.WriteLine($"[Add] '{element}' added at the END   | List: [{string.Join(", ", _items)}]");
        }
        else
        {
            _items.Insert(0, element);
            Console.WriteLine($"[Add] '{element}' added at the START | List: [{string.Join(", ", _items)}]");
        }
    }

    public T Get(int index)
    {
        if (IsEmpty)
            throw new InvalidOperationException("Collection is empty.");

        int maxIndex = Math.Min(index, _items.Count - 1);

        int randomIndex = _random.Next(0, maxIndex + 1);

        Console.WriteLine($"[Get] Requested index: {index} | Actual random index used: {randomIndex}");

        return _items[randomIndex];
    }

    public bool IsEmpty => _items.Count == 0;

    public void PrintAll()
    {
        if (IsEmpty)
        {
            Console.WriteLine("[List] Empty");
            return;
        }
        Console.WriteLine($"[List] Count: {_items.Count} | Items: [{string.Join(", ", _items)}]");
    }

    public int Count => _items.Count;
}

class Task2
{
    public static void Run()
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("EXTRAS - Lambda & Generic Collection");
        Console.WriteLine(new string('=', 70));

        // ----------------------------------------------------------------
        // Task 1: Leap year lambda
        // ----------------------------------------------------------------
        Func<int, bool> isLeapYear = year =>
            (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);

        Console.WriteLine("\nTask 1: Leap Year Lambda");
        Console.WriteLine(new string('-', 70));

        int[] yearsToCheck = { 1900, 1996, 2000, 2001, 2004, 2100, 2024, 2025 };

        foreach (int year in yearsToCheck)
        {
            Console.WriteLine($"Year {year}: {(isLeapYear(year) ? "LEAP YEAR" : "not a leap year")}");
        }

        // ----------------------------------------------------------------
        // Task 2: RandomizedList<T>
        // ----------------------------------------------------------------

        Console.WriteLine("\nTask 2: RandomizedList<T>");
        Console.WriteLine(new string('-', 70));

        Console.WriteLine("\n[INT LIST TEST]");
        RandomizedList<int> intList = new RandomizedList<int>();

        Console.WriteLine($"IsEmpty: {intList.IsEmpty}"); // true

        intList.Add(10);
        intList.Add(20);
        intList.Add(30);
        intList.Add(40);
        intList.Add(50);

        Console.WriteLine($"\nIsEmpty: {intList.IsEmpty}"); // false
        intList.PrintAll();

        Console.WriteLine("\nGet(3) calls:");
        for (int i = 0; i < 5; i++)
        {
            int value = intList.Get(3);
            Console.WriteLine($"  -> Got value: {value}");
        }

        Console.WriteLine(new string('=', 70));
    }
}
