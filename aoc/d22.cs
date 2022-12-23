abstract class d22
{  
    protected HashSet<MapPoint> map = new HashSet<MapPoint>();
    protected HashSet<MapPoint> walls = new HashSet<MapPoint>();
    protected List<Step> steps = new List<Step>();
    protected MapPoint curPos = default;
    protected int curDirection = 0; // 0 = right, 1 = down, 2 = left, 3 = up

    protected abstract int CurMapDirection { get; }
    protected abstract void TryMoveEdge();
    protected abstract MapPoint NextPos();
    protected abstract void ChangeDirection(char dir);
    protected virtual void MakeCube() { }
    protected virtual string input => @"..\..\..\22.txt";

    public void Run()
    {
        var lines = File.ReadAllLines(input).ToArray();
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

        MakeCube();

        for (int i = 0; i < steps.Count; i++)
        {
            TryStep(steps[i]);
        }

        var result = 1000 * curPos.y + 4 * curPos.x + CurMapDirection;
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
        var next = NextPos();
        if (IsWall(next)) return;
        if (Exists(next))
        {
            ActualizeCurPos(next);
            return;
        }

        TryMoveEdge();
    }

    bool Exists(MapPoint point) => map.Contains(point);

    protected bool IsWall(MapPoint point) => walls.Contains(point);

    protected virtual void ActualizeCurPos(MapPoint newPoint) => curPos = newPoint;
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

    protected override MapPoint NextPos() => curDirection switch
    {
        int curDir when curDir == 0 => new MapPoint(curPos.x + 1, curPos.y),
        int curDir when curDir == 1 => new MapPoint(curPos.x, curPos.y + 1),
        int curDir when curDir == 2 => new MapPoint(curPos.x - 1, curPos.y),
        int curDir when curDir == 3 => new MapPoint(curPos.x, curPos.y - 1),
    };

    protected override void ChangeDirection(char dir)
    {
        curDirection = dir switch
        {
            char c when c == 'L' => curDirection - 1,
            char c when c == 'R' => curDirection + 1,
        };

        if (curDirection == -1) curDirection = 3;
        curDirection %= 4;
    }

    protected override int CurMapDirection => curDirection;
}

class d22_2 : d22
{
    Dictionary<CubePoint, MapPoint> points3D = new Dictionary<CubePoint, MapPoint>();
    int[] cur3DDirection; // (x, y, z) => -1: desc, 0: const, 1: asc
    CubePoint curPos3D = default;
    int curMapDirection;

    protected virtual int dim => 50;
    protected virtual int[] init3DDirection => new int[] { 0, 0, -1 }; // (x, y, z) => -1: desc, 0: const, 1: asc

    protected override void MakeCube()
    {
        cur3DDirection = init3DDirection;

        for (int x = 1; x <= dim; x++)
        {
            for (int y = 1; y <= dim; y++)
            {
                points3D[new CubePoint(x, y, dim, Sides.Face)] = getFaceMapPoint(x, y);
            }
        }

        for (int z = 1; z <= dim; z++)
        {
            for (int y = 1; y <= dim; y++)
            {
                points3D[new CubePoint(dim, y, z, Sides.Right)] = getRightMapPoint(z, y);
            }
        }

        for (int x = 1; x <= dim; x++)
        {
            for (int y = 1; y <= dim; y++)
            {
                points3D[new CubePoint(x, y, 1, Sides.Back)] = getBackMapPoint(x, y);
            }
        }

        for (int z = 1; z <= dim; z++)
        {
            for (int y = 1; y <= dim; y++)
            {
                points3D[new CubePoint(1, y, z, Sides.Left)] = getLeftMapPoint(z, y);
            }
        }

        for (int x = 1; x <= dim; x++)
        {
            for (int z = 1; z <= dim; z++)
            {
                points3D[new CubePoint(x, 1, z, Sides.Top)] = getTopMapPoint(x, z);
            }
        }

        for (int x = 1; x <= dim; x++)
        {
            for (int z = 1; z <= dim; z++)
            {
                points3D[new CubePoint(x, dim, z, Sides.Bottom)] = getBottomMapPoint(x, z);
            }
        }

        curPos3D = points3D.Single(x => x.Value == curPos).Key;
        
        if (points3D.Count != dim * dim * 6) throw new InvalidOperationException();
        var mapTest = map.ToHashSet();
        points3D.ToList().ForEach(kv => mapTest.Remove(kv.Value));
        if (mapTest.Any()) throw new InvalidOperationException();
    }

    protected virtual MapPoint getFaceMapPoint(int x, int y) => new MapPoint(x, y + 2 * dim);
    protected virtual MapPoint getRightMapPoint(int z, int y) => new MapPoint(2 * dim + 1 - z, y + 2 * dim);
    protected virtual MapPoint getBackMapPoint(int x, int y) => new MapPoint(x + 2 * dim, dim - y + 1);
    protected virtual MapPoint getLeftMapPoint(int z, int y) => new MapPoint(2 * dim + 1 - z, dim + 1 - y);
    protected virtual MapPoint getTopMapPoint(int x, int z) => new MapPoint(2 * dim + 1 - z, x + dim);
    protected virtual MapPoint getBottomMapPoint(int x, int z) => new MapPoint(x, 4 * dim + 1 - z);

    protected override void TryMoveEdge()
    {
        // do nothing, exists and not a wall for sure if we are here
    }

    protected override void ActualizeCurPos(MapPoint newPoint)
    {
        curMapDirection = newPoint switch
        {
            MapPoint p when p.x > curPos.x => 0,
            MapPoint p when p.y > curPos.y => 1,
            MapPoint p when p.x < curPos.x => 2,
            MapPoint p when p.y < curPos.y => 3,
            _ => curMapDirection,
        };

        base.ActualizeCurPos(newPoint);

        curPos3D = points3D.Single(kv => kv.Value == newPoint).Key;
    }

    protected override void ChangeDirection(char dir)
    {
        var side = curPos3D.side;
        if (cur3DDirection[0] != 0)
        {
            ChangeDirectionInX(dir, side);
        }
        else if (cur3DDirection[1] != 0)
        {
            ChangeDirectionInY(dir, side);
        }
        else
        {
            ChangeDirectionInZ(dir, side);
        }
    }

    void ChangeDirectionInX(char dir, Sides side)
    {
        if (cur3DDirection[1] != 0 || cur3DDirection[2] != 0) throw new InvalidOperationException();

        if (side == Sides.Face)
        {
            if (cur3DDirection[0] < 0)
            {
                cur3DDirection[1] = dir == 'L' ? 1 : -1;
            }
            else
            {
                cur3DDirection[1] = dir == 'L' ? -1 : 1;
            }
        }
        else if (side == Sides.Right)
        {
            throw new InvalidOperationException();
        }
        else if (side == Sides.Back)
        {
            if (cur3DDirection[0] < 0)
            {
                cur3DDirection[1] = dir == 'L' ? -1 : 1;
            }
            else
            {
                cur3DDirection[1] = dir == 'L' ? 1 : -1;
            }
        }
        else if (side == Sides.Left)
        {
            throw new InvalidOperationException();
        }
        else if (side == Sides.Top)
        {
            if (cur3DDirection[0] < 0)
            {
                cur3DDirection[2] = dir == 'L' ? 1 : -1;
            }
            else
            {
                cur3DDirection[2] = dir == 'L' ? -1 : 1;
            }
        }
        else
        {
            // Bottom
            if (cur3DDirection[0] < 0)
            {
                cur3DDirection[2] = dir == 'L' ? -1 : 1;
            }
            else
            {
                cur3DDirection[2] = dir == 'L' ? 1 : -1;
            }
        }

        cur3DDirection[0] = 0;
    }

    void ChangeDirectionInY(char dir, Sides side)
    {
        if (cur3DDirection[0] != 0 || cur3DDirection[2] != 0) throw new InvalidOperationException();

        if (side == Sides.Face)
        {
            if (cur3DDirection[1] < 0)
            {
                cur3DDirection[0] = dir == 'L' ? -1 : 1;
            }
            else
            {
                cur3DDirection[0] = dir == 'L' ? 1 : -1;
            }
        }
        else if (side == Sides.Right)
        {
            if (cur3DDirection[1] < 0)
            {
                cur3DDirection[2] = dir == 'L' ? 1 : -1;
            }
            else
            {
                cur3DDirection[2] = dir == 'L' ? -1 : 1;
            }
        }
        else if (side == Sides.Back)
        {
            if (cur3DDirection[1] < 0)
            {
                cur3DDirection[0] = dir == 'L' ? 1 : -1;
            }
            else
            {
                cur3DDirection[0] = dir == 'L' ? -1 : 1;
            }
        }
        else if (side == Sides.Left)
        {
            if (cur3DDirection[1] < 0)
            {
                cur3DDirection[2] = dir == 'L' ? -1 : 1;
            }
            else
            {
                cur3DDirection[2] = dir == 'L' ? 1 : -1;
            }
        }
        else if (side == Sides.Top)
        {
            throw new InvalidOperationException();
        }
        else
        {
            // Bottom
            throw new InvalidOperationException();
        }

        cur3DDirection[1] = 0;
    }

    void ChangeDirectionInZ(char dir, Sides side)
    {
        if (cur3DDirection[0] != 0 || cur3DDirection[1] != 0) throw new InvalidOperationException();

        if (side == Sides.Face)
        {
            throw new InvalidOperationException();
        }
        else if (side == Sides.Right)
        {
            if (cur3DDirection[2] < 0)
            {
                cur3DDirection[1] = dir == 'L' ? -1 : 1;
            }
            else
            {
                cur3DDirection[1] = dir == 'L' ? 1 : -1;
            }
        }
        else if (side == Sides.Back)
        {
            throw new InvalidOperationException();
        }
        else if (side == Sides.Left)
        {
            if (cur3DDirection[2] < 0)
            {
                cur3DDirection[1] = dir == 'L' ? 1 : -1;
            }
            else
            {
                cur3DDirection[1] = dir == 'L' ? -1 : 1;
            }
        }
        else if (side == Sides.Top)
        {
            if (cur3DDirection[2] < 0)
            {
                cur3DDirection[0] = dir == 'L' ? -1 : 1;
            }
            else
            {
                cur3DDirection[0] = dir == 'L' ? 1 : -1;
            }
        }
        else
        {
            // Bottom
            if (cur3DDirection[2] < 0)
            {
                cur3DDirection[0] = dir == 'L' ? 1 : -1;
            }
            else
            {
                cur3DDirection[0] = dir == 'L' ? -1 : 1;
            }
        }

        cur3DDirection[2] = 0;
    }

    protected override MapPoint NextPos() 
    {
        CubePoint next = default;
        if (cur3DDirection[0] != 0)
        {
            next = NextPosX(cur3DDirection[0]);
        }
        else if (cur3DDirection[1] != 0)
        {
            next = NextPosY(cur3DDirection[1]);
        }
        else
        {
            next = NextPosZ(cur3DDirection[2]);
        }

        return points3D[next];
    }

    CubePoint NextPosX(int dir)
    {
        if (curPos3D.side == Sides.Right || curPos3D.side == Sides.Left)
        {
            throw new InvalidOperationException();
        }
        else
        {
            CubePoint next = default;
            if (dir < 0)
            {
                if (curPos3D.x > 1)
                {
                    next = curPos3D with { x = curPos3D.x - 1 };
                }
                else
                {
                    if (curPos3D.x != 1) throw new InvalidOperationException();
                    next = curPos3D with { side = Sides.Left };
                    cur3DDirection = new3DDirection(next);
                }
            }
            else
            {
                if (curPos3D.x < dim)
                {
                    next = curPos3D with { x = curPos3D.x + 1 };
                }
                else
                {
                    if (curPos3D.x != dim) throw new InvalidOperationException();
                    next = curPos3D with { side = Sides.Right };
                    cur3DDirection = new3DDirection(next);
                }
            }
            return next;

            int[] new3DDirection(CubePoint next) => IsWall(points3D[next]) ? cur3DDirection : curPos3D.side switch
            {
                Sides.Face => new int[] { 0, 0, -1 },
                Sides.Back => new int[] { 0, 0, 1 },
                Sides.Top => new int[] { 0, 1, 0 },
                Sides.Bottom => new int[] { 0, -1, 0 },
            };
        }
    }

    CubePoint NextPosY(int dir)
    {
        if (curPos3D.side == Sides.Top || curPos3D.side == Sides.Bottom)
        {
            throw new InvalidOperationException();
        }
        else
        {
            CubePoint next = default;
            if (dir < 0)
            {
                if (curPos3D.y > 1)
                {
                    next = curPos3D with { y = curPos3D.y - 1 };
                }
                else
                {
                    if (curPos3D.y != 1) throw new InvalidOperationException();
                    next = curPos3D with { side = Sides.Top };
                    cur3DDirection = new3DDirection(next);
                }
            }
            else
            {
                if (curPos3D.y < dim)
                {
                    next = curPos3D with { y = curPos3D.y + 1 };
                }
                else
                {
                    if (curPos3D.y != dim) throw new InvalidOperationException();
                    next = curPos3D with { side = Sides.Bottom };
                    cur3DDirection = new3DDirection(next);
                }
            }
            return next;

            int[] new3DDirection(CubePoint next) => IsWall(points3D[next]) ? cur3DDirection : curPos3D.side switch
            {
                Sides.Face => new int[] { 0, 0, -1 },
                Sides.Right => new int[] { -1, 0, 0 },
                Sides.Back => new int[] { 0, 0, 1 },
                Sides.Left => new int[] { 1, 0, 0 },
            };
        }
    }

    CubePoint NextPosZ(int dir)
    {
        if (curPos3D.side == Sides.Face || curPos3D.side == Sides.Back)
        {
            throw new InvalidOperationException();
        }
        else
        {
            CubePoint next = default;
            if (dir < 0)
            {
                if (curPos3D.z > 1)
                {
                    next = curPos3D with { z = curPos3D.z - 1 };
                }
                else
                {
                    if (curPos3D.z != 1) throw new InvalidOperationException();
                    next = curPos3D with { side = Sides.Back };
                    cur3DDirection = new3DDirection(next);
                }
            }
            else
            {
                if (curPos3D.z < dim)
                {
                    next = curPos3D with { z = curPos3D.z + 1 };
                }
                else
                {
                    if (curPos3D.z != dim) throw new InvalidOperationException();
                    next = curPos3D with { side = Sides.Face };
                    cur3DDirection = new3DDirection(next);
                }
            }
            return next;

            int[] new3DDirection(CubePoint next) => IsWall(points3D[next]) ? cur3DDirection : curPos3D.side switch
            {
                Sides.Right => new int[] { -1, 0, 0 },
                Sides.Left => new int[] { 1, 0, 0 },
                Sides.Top => new int[] { 0, 1, 0 },
                Sides.Bottom => new int[] { 0, -1, 0 },
            };
        }
    }

    protected override int CurMapDirection => curMapDirection;
}

class d22_1_test : d22_1
{
    protected override string input => @"..\..\..\22_test.txt";
}

class d22_2_test : d22_2
{
    protected override string input => @"..\..\..\22_test.txt";
    protected override int dim => 4;
    protected override int[] init3DDirection => new int[] { 1, 0, 0 }; // (x, y, z) => -1: desc, 0: const, 1: asc

    protected override MapPoint getFaceMapPoint(int x, int y) => new MapPoint(x + 2 * dim, y);
    protected override MapPoint getRightMapPoint(int z, int y) => new MapPoint(3 * dim + z, 3 * dim + 1 - y);
    protected override MapPoint getBackMapPoint(int x, int y) => new MapPoint(x + 2 * dim, 3 * dim + 1 - y);
    protected override MapPoint getLeftMapPoint(int z, int y) => new MapPoint(dim + y, 2 * dim + 1 - z);
    protected override MapPoint getTopMapPoint(int x, int z) => new MapPoint(dim + 1 - x, 2 * dim + 1 - z);
    protected override MapPoint getBottomMapPoint(int x, int z) => new MapPoint(2 * dim + x, 2 * dim + 1 - z);
}

enum Sides
{
    Face,
    Right,
    Back, 
    Left,
    Top,
    Bottom
}

record struct MapPoint(int x, int y);
record struct CubePoint(int x, int y, int z, Sides side);
record struct Step(int steps, char dir)
{
    public bool isDirection => steps == int.MinValue;
}
