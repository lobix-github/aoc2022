abstract class d11
{
    protected List<Monkey> monkeys = new List<Monkey>();
    private Dictionary<int, long> times = new Dictionary<int, long>();
    private long commonDivider = 1;

    protected abstract int rounds { get; }
    protected abstract long TransformWorryLevel(long worryLevel);

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\11.txt");

        var idx = 0;
        for (int i = 0; i < lines.Length; i += 7)
        {
            var monkey = GetMonkey(new[] { lines[i+1], lines[i+2], lines[i+3], lines[i+4], lines[i+5] });
            monkeys.Add(monkey);
            times[idx++] = 0;
            commonDivider *= monkey.divBy;
        }

        for (int round = 0; round < rounds; round++)
        {
            for (int i = 0; i < monkeys.Count; i++)
            {
                var m = monkeys[i];
                while (m.items.Any())
                {
                    var worryLevel = TransformWorryLevel(m.worryLevel % commonDivider);
                    monkeys[worryLevel % m.divBy == 0 ? m.toTrue : m.toFalse].items.Add(worryLevel);
                    m.items.RemoveAt(0);
                    times[i]++;
                }
            };
        }

        var ordered = times.Values.OrderByDescending(x => x).ToArray();
        Console.WriteLine(ordered[0] * ordered[1]);
    }

    Monkey GetMonkey(string[] ms)
    {
        var items = new List<long>();
        var itemsInfo = ms[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimEnd(',')).ToArray();
        for (int i = 2; i < itemsInfo.Length; i++) items.Add(Convert.ToInt32(itemsInfo[i]));

        var op = ms[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[4][0];
        var opArg = ms[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[5];
        var divBy = Convert.ToInt32(ms[2].Split(' ', StringSplitOptions.RemoveEmptyEntries)[3]);
        var toTrue = Convert.ToInt32(ms[3].Split(' ', StringSplitOptions.RemoveEmptyEntries)[5]);
        var toFalse = Convert.ToInt32(ms[4].Split(' ', StringSplitOptions.RemoveEmptyEntries)[5]);

        return new Monkey(items, op, opArg, divBy, toTrue, toFalse, commonDivider);
    }
}

class d11_1 : d11
{
    protected override int rounds => 20;
    protected override long TransformWorryLevel(long worryLevel) => (long)Math.Floor(worryLevel / (double)3);
}

class d11_2 : d11
{
    protected override int rounds => 10000;
    protected override long TransformWorryLevel(long worryLevel) => worryLevel;
}

record struct Monkey(List<long> items, char op, string opArg, int divBy, int toTrue, int toFalse, long commonDivider)
{
    Dictionary<char, Func<long, long, long>> ops = new Dictionary<char, Func<long, long, long>>()
    {
        { '+', (a, b) => a + b },
        { '*', (a, b) => a * b },
    };

    long argValue => opArg == "old" ? items[0] : Convert.ToInt32(opArg);

    public long worryLevel => ops[op](items[0], argValue);
}

