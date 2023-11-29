using System.Numerics;
using System.Text.RegularExpressions;

abstract class d15
{
    const string s = @"Sensor at x=(?<x>.+), y=(?<y>.+): closest beacon is at x=(?<bx>.+), y=(?<by>.+)";
    
    protected HashSet<CavePoint> cave = new HashSet<CavePoint>();
    protected HashSet<CavePoint> beacons = new HashSet<CavePoint>();

    protected abstract void Compute(CavePoint S, CavePoint B, int dist);
    protected abstract void Result();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\15.txt").ToArray();
        foreach (var line in lines)
        {
            var match = Regex.Match(line, s);
            var x = Convert.ToInt32(match.Groups[1].Value);
            var y = Convert.ToInt32(match.Groups[2].Value);
            var bx = Convert.ToInt32(match.Groups[3].Value);
            var by = Convert.ToInt32(match.Groups[4].Value);

            var S = new CavePoint(x, y);
            var B = new CavePoint(bx, by);
            beacons.Add(B);
            var dist = Math.Abs(x - bx) + Math.Abs(y - by);

            Compute(S, B, dist);
        }

        Result();
    }
}

class d15_1 : d15
{
    const int idxToCheck = 2000000;

    protected override void Compute(CavePoint S, CavePoint B, int dist)
    {
        for (int row = 0; row <= dist; row++)
        {
            if (S.y - row != idxToCheck && S.y + row != idxToCheck) continue;
            for (int col = 0; col <= dist - row; col++)
            {
                cave.Add(new CavePoint(S.x - col, S.y - row));
                cave.Add(new CavePoint(S.x - col, S.y + row));
                cave.Add(new CavePoint(S.x + col, S.y - row));
                cave.Add(new CavePoint(S.x + col, S.y + row));
            }
        }
    }

    protected override void Result()
    {
        var result = cave.Count(x => x.y == idxToCheck) - beacons.Count(x => x.y == idxToCheck);
        Console.WriteLine(result);
    }
}

class d15_2 : d15
{
    const int dim = 4000000;
    Dictionary<CavePoint, int> sensors = new Dictionary<CavePoint, int>();
    
    protected override void Compute(CavePoint S, CavePoint B, int dist)
    {
        sensors[S] = dist;
    }

    protected override void Result()
    {
        for (int row = 0; row <= dim; row++)
        {
            var rowToCheck = new List<Range>();
            foreach (var sensor in sensors)
            {
                var S = sensor.Key;
                var dist = sensor.Value;
                int xdist = dist - Math.Abs(S.y - row);
                if (xdist >= 0)
                {
                    var xstart = Math.Max(0, S.x - xdist);
                    var xend = Math.Min(dim, S.x + xdist);
                    var range = new Range(xstart, xend);
                    rowToCheck.Add(range);
                    CombineRanges(rowToCheck);
                }
            }
   
            if (rowToCheck.Count() > 1)
            {
                var result = new BigInteger(dim) * (rowToCheck[0].end + 1) + row;
                Console.WriteLine(result);
                break;
            }
        }
    }

    void CombineRanges(List<Range> row)
    {
        var change = true;
        do
        {
            change = false;
            row.Sort((r1, r2) => r1.start.CompareTo(r2.start));
            for (int i = 1; i < row.Count; i++)
            {
                if (row[i].start <= row[i - 1].end + 1)
                {
                    row[i - 1] = new Range(row[i - 1].start, Math.Max(row[i - 1].end, row[i].end));
                    row.RemoveAt(i);
                    change = true;
                    break;
                }
            }
        } 
        while (change);
    }
}

record struct Range(int start, int end);