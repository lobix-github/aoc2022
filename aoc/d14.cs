abstract class d14
{
    HashSet<CavePoint> cave = new HashSet<CavePoint>();
    protected int lowestRock;
    protected CavePoint curPoint;

    CavePoint startPoint = new CavePoint(500, 0);

    protected virtual bool CheckIfEmpty => !cave.Contains(curPoint);
    protected abstract void MaybeCheckIfBelowLowestRock();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\14.txt").ToArray();
        lowestRock = lines.SelectMany(line => line.Split("->").Select(point => Convert.ToInt32(point.Split(',')[1]))).Max();
        BuildCave(lines);

        int count = 0;
        try
        {
            while (true)
            {
                curPoint = curPoint != default ? curPoint : startPoint;
                while (Fall());
                curPoint.x--;
                if (CheckIfEmpty)
                {
                    continue;
                }
                else
                {
                    curPoint.x += 2;
                    if (CheckIfEmpty)
                    {
                        continue;
                    }
                    else
                    {
                        curPoint.x--;
                        curPoint.y--;
                        cave.Add(curPoint);
                        count++;
                        if (curPoint == startPoint) throw new InvalidOperationException();
                        curPoint = default;
                    }
                }
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine(count);
        }
    }

    bool Fall()
    {
        curPoint.y++;
        MaybeCheckIfBelowLowestRock();

        return CheckIfEmpty;
    }

    void BuildCave(string[] lines)
    {
        foreach (var line in lines)
        {
            var paths = line.Split("->");
            var x0 = Convert.ToInt32(paths[0].Split(',')[0]);
            var y0 = Convert.ToInt32(paths[0].Split(',')[1]);

            for (int i = 1; i < paths.Length; i++)
            {
                var x1 = Convert.ToInt32(paths[i].Split(',')[0]);
                var y1 = Convert.ToInt32(paths[i].Split(',')[1]);

                if (x0 == x1)
                {
                    for (int y = 0; y <= Math.Abs(y0 - y1); y++)
                    {
                        var voffset = Math.Min(y0, y1);
                        cave.Add(new CavePoint(x0, y + voffset));
                    }
                }

                if (y0 == y1)
                {
                    for (int x = 0; x <= Math.Abs(x0 - x1); x++)
                    {
                        var hoffset = Math.Min(x0, x1);
                        cave.Add(new CavePoint(x + hoffset, y0));
                    }
                }

                x0 = x1;
                y0 = y1;
            }
        }
    }
}

class d14_1 : d14
{
    protected override void MaybeCheckIfBelowLowestRock() 
    { 
        if (curPoint.y > lowestRock) throw new InvalidOperationException(); 
    }
}

class d14_2 : d14
{
    protected override bool CheckIfEmpty => base.CheckIfEmpty && curPoint.y < lowestRock + 2;

    protected override void MaybeCheckIfBelowLowestRock() { }
}

record struct CavePoint(int x, int y);
