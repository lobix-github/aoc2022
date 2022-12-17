public abstract class d17
{
    HashSet<CavePoint> cave = new HashSet<CavePoint>() { new (0, -1), new(1, -1), new(2, -1), new(3, -1), new(4, -1), new(5, -1), new(6, -1) };

    //string jetPattern = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
    string jetPattern => File.ReadAllText(@"..\..\..\17.txt");
    protected abstract long rounds { get; }

    public void Run()
    {
        int count = 0;
        int step = 0;
        while (count < rounds)
        {
            var lowestY = cave.Max(p => p.y);
            var rock = RockFabric.GetRock(lowestY + 1, count++);
            while(rock.Fall(cave, jetFabric(step++)));
        }

        var result = cave.Max(p => p.y) + 1;
        Console.WriteLine(result);
    }

    int jetFabric(int i) => jetPattern[i % jetPattern.Length] == '>' ? 1 : -1;
}

class Rock
{
    const int wide = 7;
    private readonly CavePoint[] points;

    public Rock(CavePoint[] points)
    {
        this.points = points;
    }

    public bool Fall(HashSet<CavePoint> cave, int jet)
    {
        TryMoveJet(cave, jet);
        return TryFallDown(cave);
    }

    void TryMoveJet(HashSet<CavePoint> cave, int jet)
    {
        if (leftMost == 0 && jet == -1) return;
        if (RightMost == wide - 1 && jet == 1) return;

        Transform(jet, 0);
        if (cave.Intersect(points).Any())
        {
            Transform(-jet, 0);
        }
    }

    bool TryFallDown(HashSet<CavePoint> cave)
    {
        Transform(0, -1);
        var canFall = !cave.Intersect(points).Any();
        if (!canFall)
        {
            Transform(0, 1);
            points.ToList().ForEach(p => cave.Add(p));
        }

        return canFall;
    }

    int leftMost => points.Min(p => p.x);
    int RightMost => points.Max(p => p.x);

    void Transform(int dx, int dy)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x += dx;
            points[i].y += dy;
        }
    }
}

class d17_1 : d17
{
    protected override long rounds => 2022;
}

class d17_2 : d17
{
    protected override long rounds => 1_000_000_000_000;
}

static class RockFabric
{

    public static Rock GetRock(int lowestY, int i) => i switch
    {
        int when i % 5 == 0 => new Rock(new CavePoint[] { tr(new(0, 0), lowestY), tr(new(1, 0), lowestY), tr(new(2, 0), lowestY), tr(new(3, 0), lowestY) }),
        int when i % 5 == 1 => new Rock(new CavePoint[] { tr(new(1, 0), lowestY), tr(new(0, 1), lowestY), tr(new(1, 1), lowestY), tr(new(2, 1), lowestY), tr(new(1, 2), lowestY) }),
        int when i % 5 == 2 => new Rock(new CavePoint[] { tr(new(0, 0), lowestY), tr(new(1, 0), lowestY), tr(new(2, 0), lowestY), tr(new(2, 1), lowestY), tr(new(2, 2), lowestY) }),
        int when i % 5 == 3 => new Rock(new CavePoint[] { tr(new(0, 0), lowestY), tr(new(0, 1), lowestY), tr(new(0, 2), lowestY), tr(new(0, 3), lowestY) }),
        int when i % 5 == 4 => new Rock(new CavePoint[] { tr(new(0, 0), lowestY), tr(new(1, 0), lowestY), tr(new(0, 1), lowestY), tr(new(1, 1), lowestY) }),
    };

    static CavePoint tr(CavePoint p, int lowestY) => p switch
    {
        CavePoint => new (p.x + 2, p.y + 3 + lowestY)
    };
}


