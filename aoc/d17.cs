public abstract class d17
{
    HashSet<CavePoint> cave = new HashSet<CavePoint>() { new (0, -1), new(1, -1), new(2, -1), new(3, -1), new(4, -1), new(5, -1), new(6, -1) };
    Dictionary<int, int> cyclesHeights = new Dictionary<int, int>();

    //string jetPattern = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
    string _jetPattern;
    string jetPattern => _jetPattern ??= File.ReadAllText(@"..\..\..\17.txt");
    protected abstract long rounds { get; set; }

    public void Run()
    {
        int count = 0;
        int step = 0;
        while (count < rounds)
        {
            var lowestY = cave.Max(p => p.y);
            (var patternHeight, var patternCycles) = MaybePattern(count, lowestY);
            if (patternHeight != default)
            {
                var reps = (rounds - count) / patternCycles;
                var skippedHeight = reps * patternHeight;
                var skippedRounds = reps * patternCycles;
                var fakeCycle = count - patternCycles;
                while (count < rounds - skippedRounds)
                {
                    skippedHeight += cyclesHeights[fakeCycle++];
                    count++;
                }

                var r = lowestY + skippedHeight + 1;
            }

            var rock = RockFabric.GetRock(lowestY + 1, count);
            while(rock.Fall(cave, jetFabric(step++)));
            cyclesHeights[count] = cave.Max(p => p.y) - lowestY;
            count++;
        }

        var result = cave.Max(p => p.y) + 1;
        Console.WriteLine(result);
    }

    int prevRockIndex = -1;
    int prevHeight;
    List<(int, int)> deltas = new List<(int, int)>();
    (int, int) MaybePattern(int cycle, int lowestY)
    {
        if (cycle is > 0 && Enumerable.Range(0, 6).All(x => cave.Contains(new CavePoint(x, lowestY))))
        {
            var rockIndex = cycle % 5;
            if (prevRockIndex is -1)
            {
                prevRockIndex = rockIndex;
                prevHeight = lowestY;
            }
            else if (prevRockIndex == rockIndex)
            {
                var delta = lowestY - prevHeight;
                deltas.Add((delta, cycle));
                prevHeight = lowestY;
                if (deltas.Count % 2 is 0)
                {
                    // maybe pattern? let's check!
                    bool isPattern = true;
                    for (int i = 0; i < deltas.Count / 2; i++)
                    {
                        isPattern &= deltas[i].Item1 == deltas[deltas.Count / 2 + i].Item1;
                    }

                    if (isPattern)
                    {
                        // we have a pattern!
                        return (deltas.Sum(x => x.Item1) / 2, deltas[deltas.Count / 2].Item2 - deltas[0].Item2);
                    }
                }
            }
        }

        return default;
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
    protected override long rounds { get; set; } = 2022;
}

class d17_2 : d17
{
    protected override long rounds { get; set; } = 1_000_000_000_000;
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


