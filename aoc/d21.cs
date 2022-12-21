abstract class d21
{
    HashSet<MonkeyInfo> monkeys = new HashSet<MonkeyInfo>();
    
    protected abstract void Result(MonkeyNode root, MonkeyNode humnNode);

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
        MonkeyNode humnNode = null;
        var rootNode = new MonkeyNode(monkeys, default, rootMonkeyInfo, ref humnNode);
        Result(rootNode, humnNode);
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

    private Dictionary<char, Func<long, long, long>> revOps = new Dictionary<char, Func<long, long, long>>()
    {
        { '+', (a, b) => a - b },
        { '-', (a, b) => a + b },
        { '*', (a, b) => a / b },
        { '/', (a, b) => a * b },
    };

    public string Name { get; init; }
    public bool IsNumber { get; init; }

    public MonkeyNode? Parent { get; init; }
    public long NumberValue { get; init; }

    public bool CalcIsNormalNode { get; set; } = true;
    public long CalcValue { get; set; }
    public MonkeyNode CalcRevNode => !left.CalcIsNormalNode ? left : right;
    public MonkeyNode CalcNormalNode => left.CalcIsNormalNode ? left : right;

    public MonkeyNode(HashSet<MonkeyInfo> monkeys, MonkeyNode? parent, MonkeyInfo info, ref MonkeyNode humnNode)
    {
        Parent = parent;

        var body = info.body;
        Name = info.name;
        IsNumber = char.IsNumber(body[0]);
        if (!IsNumber)
        {
            op = body.Split(' ')[1][0];
            var leftName = body.Split(' ')[0];
            left = new MonkeyNode(monkeys, this, monkeys.Single(x => x.name == leftName), ref humnNode);
            var rightName = body.Split(' ')[2];
            right = new MonkeyNode(monkeys, this, monkeys.Single(x => x.name == rightName), ref humnNode);
        }
        else
        {
            if (Name == "humn")
            {
                humnNode = this;
            }
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

    public long CalcRevValue(long value) => revOps[op](CalcValue, value);

    MonkeyNode? left;
    MonkeyNode? right;
}

class d21_1 : d21
{
    protected override void Result(MonkeyNode root, MonkeyNode humnNode)
    {
        var result = root.Value();
        Console.WriteLine(result);
    }
}

class d21_2 : d21
{
    protected override void Result(MonkeyNode root, MonkeyNode humnNode)
    {
        _ = root.Value();
        var firstNode = PrepareHumnBranch(humnNode);
        firstNode.CalcValue = -root.CalcNormalNode.Value();
        CalcHumnValue(firstNode);
        Console.WriteLine(humnNode.CalcValue);
    }

    MonkeyNode PrepareHumnBranch(MonkeyNode node)
    {
        do
        {
            node.CalcIsNormalNode = false;
            node = node.Parent;
        }
        while (node.Name != "root");

        return node.CalcRevNode;
    }

    void CalcHumnValue(MonkeyNode node)
    {
        do
        {
            node.CalcRevNode.CalcValue = node.CalcRevValue(node.CalcNormalNode.Value());
            node = node.CalcRevNode;
        }
        while (node.Name != "humn");
    }
}

record struct MonkeyInfo(string name, string body);
