abstract class d21
{
    HashSet<MonkeyInfo> monkeys = new HashSet<MonkeyInfo>();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\21.txt").ToArray();
        foreach (var line in lines)
        {
            var words = line.Split(':').Select(x => x.Trim()).ToArray();
            monkeys.Add(new MonkeyInfo(words[0], words[1]));
        }
        var rootMonkeyInfo = monkeys.Single(x => x.name== "root");
        monkeys.Remove(rootMonkeyInfo);
        var rootNode = new MonkeyNode(monkeys, rootMonkeyInfo);
        var result = rootNode.Value();
        Console.WriteLine(result);
    }
}

class MonkeyNode
{
    private char op;
    private Dictionary<char, Func<long, long, long>> ops = new Dictionary<char, Func<long, long, long>>()
    {
        { '+', (l, r) => l + r },
        { '-', (l, r) => l - r },
        { '*', (l, r) => l * r },
        { '/', (l, r) => l / r },
    };

    public string Name { get; init; }
    public bool IsNumber { get; init; }

    public long NumberValue { get; init; }

    public MonkeyNode(HashSet<MonkeyInfo> monkeys,MonkeyInfo info)
    {
        var body = info.body;
        Name = info.name;
        IsNumber = char.IsNumber(body[0]);
        if (!IsNumber)
        {
            op = body.Split(' ')[1][0];
            var leftName = body.Split(' ')[0];
            left = new MonkeyNode(monkeys, monkeys.Single(x => x.name == leftName));
            var rightName = body.Split(' ')[2];
            right = new MonkeyNode(monkeys, monkeys.Single(x => x.name == rightName));
        }
        else
        {
            NumberValue = long.Parse(body);
        }

        monkeys.Remove(info);
    }

    public long Value()
    {
        if (IsNumber)
        {
            return NumberValue;
        }

        return ops[op](left.Value(), right.Value());
    }

    MonkeyNode? left;
    MonkeyNode? right;
}

class d21_1 : d21
{

}

class d21_2 : d21
{

}

record struct MonkeyInfo(string name, string body);
