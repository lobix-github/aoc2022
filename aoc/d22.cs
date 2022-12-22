abstract class d22
{  
    protected HashSet<MapPoint> map = new HashSet<MapPoint>();
    protected HashSet<MapPoint> walls = new HashSet<MapPoint>();
    protected List<Step> steps = new List<Step>();
    protected MapPoint curPos = default;
    protected int curDirection = 0; // 0 = right, 1 = down, 2 = left, 3 = up

    protected abstract void TryMoveEdge();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\22.txt").ToArray();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line != "")
            {
                for (int x = 0; x < line.Length; x++)
                {
                    var c = line[x];
                    if (c != ' ')
                    {
                        var p = new MapPoint(x + 1, i + 1);
                        var isWall = c == '#';
                        if (isWall)
                        {
                            walls.Add(p);
                        }
                        if (curPos == default && !isWall) curPos = p;
                        map.Add(p);
                    }
                }
            }
            else
            {
                var directions = lines[i + 1];
                for (int idx = directions.Length - 1; idx >= 0; idx--)
                {
                    int val = 0;
                    var b = 0;
                    var c = directions[idx];
                    if (char.IsDigit(c))
                    {
                        while (idx >= 0 && char.IsDigit(c = directions[idx]))
                        {
                            val += (int)Math.Pow(10, b++) * (c - '0');
                            idx--;
                        }
                        steps.Insert(0, new Step(val, (char)0));
                        idx++;
                        continue;
                    }

                    steps.Insert(0, new Step(int.MinValue, c));
                }
                break;
            }
        }

        for (int i = 0; i < steps.Count; i++)
        {
            TryStep(steps[i]);
        }

        var result = 1000 * curPos.y + 4 * curPos.x + curDirection;
        Console.WriteLine(result);
    }

    void TryStep(Step step)
    {
        if (step.isDirection)
        {
            ChangeDirection(step.dir);
        }
        else
        {
            for (int i = 0; i < step.steps; i++)
            {
                TryMove();
            }
        }
    }

    void TryMove()
    {
        var next = NextPos;
        if (IsWall(next)) return;
        if (Exists(next))
        {
            curPos = next;
            return;
        }

        TryMoveEdge();
    }

    protected bool IsWall(MapPoint point) => walls.Contains(point);
    bool Exists(MapPoint point) => map.Contains(point);

    MapPoint NextPos => curDirection switch
    {
        int curDir when curDir == 0 => new MapPoint(curPos.x + 1, curPos.y),
        int curDir when curDir == 1 => new MapPoint(curPos.x, curPos.y + 1),
        int curDir when curDir == 2 => new MapPoint(curPos.x - 1, curPos.y),
        int curDir when curDir == 3 => new MapPoint(curPos.x, curPos.y - 1),
    };

    void ChangeDirection(char dir)
    {
        curDirection = dir switch
        {
            char c when c == 'L' => curDirection - 1,
            char c when c == 'R' => curDirection + 1,
        };

        if (curDirection == -1) curDirection = 3;
        curDirection %= 4;
    }
}

class d22_1 : d22
{
    protected override void TryMoveEdge()
    {
        if (curDirection == 0) // right
        {
            var leftMost = map.Where(p => p.y == curPos.y).OrderBy(p => p.x).First();
            if (IsWall(leftMost)) return;

            curPos = leftMost;
        }
        else if (curDirection == 1) // down
        {
            var topMost = map.Where(p => p.x == curPos.x).OrderBy(p => p.y).First();
            if (IsWall(topMost)) return;

            curPos = topMost;
        }
        else if (curDirection == 2) // left
        {
            var rightMost = map.Where(p => p.y == curPos.y).OrderBy(p => p.x).Last();
            if (IsWall(rightMost)) return;

            curPos = rightMost;
        }
        else if (curDirection == 3) // up
        {
            var bottomMost = map.Where(p => p.x == curPos.x).OrderBy(p => p.y).Last();
            if (IsWall(bottomMost)) return;

            curPos = bottomMost;
        }
    }
}

class d22_2 : d22
{
    protected override void TryMoveEdge()
    {

    }
}

record struct MapPoint(int x, int y);
record struct Step(int steps, char dir)
{
    public bool isDirection => steps == int.MinValue;
}
