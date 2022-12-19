
abstract class d18
{
    protected HashSet<Cube> lava = new HashSet<Cube>();

    protected abstract int Result();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\18.txt").ToArray();
        foreach (var line in lines)
        {
            var words = line.Split(',');
            var x = Convert.ToInt32(words[0]);
            var y = Convert.ToInt32(words[1]);
            var z = Convert.ToInt32(words[2]);

            var cube = new Cube(x, y, z);
            lava.Add(cube);
        }

        var result = Result();
        Console.WriteLine(result);
    }

    protected int Result1()
    {
        int result = 0;
        for (int dim = 0; dim < 3; dim++)
        {
            var sortedLava = lava.ToList();
            sortedLava.Sort((c1, c2) => c1.dim[dim].CompareTo(c2.dim[dim]));
            for (int i = 0; i < lava.Count; i++)
            {

                if (i == 0) result++;
                var cube1 = sortedLava[i];
                var prevCube = GetPrevCube(cube1, dim);
                var nextCube = GetNextCube(cube1, dim);
                if (i == 0)
                {
                    if (!lava.Contains(nextCube))
                    {
                        result++;
                    }
                }
                else
                {
                    if (!lava.Contains(prevCube))
                    {
                        result++;
                    }
                    if (i < lava.Count - 1)
                    {
                        if (!lava.Contains(nextCube))
                        {
                            result++;
                        }
                    }
                }

                if (i == lava.Count - 1) result++;
            }
        }

        return result;
    }

    protected Cube GetNextCube(Cube cube, int dim)
    {
        switch (dim)
        {
            case 0: return new Cube(cube.x + 1, cube.y, cube.z);
            case 1: return new Cube(cube.x, cube.y + 1, cube.z);
            case 2: return new Cube(cube.x, cube.y, cube.z + 1);
        }
        throw new NotImplementedException();
    }

    protected Cube GetPrevCube(Cube cube, int dim)
    {
        switch (dim)
        {
            case 0: return new Cube(cube.x - 1, cube.y, cube.z);
            case 1: return new Cube(cube.x, cube.y - 1, cube.z);
            case 2: return new Cube(cube.x, cube.y, cube.z - 1);
        }
        throw new NotImplementedException();
    }
}

class d18_1 : d18
{
    protected override int Result()
    {
        return Result1();
    }
}

class d18_2 : d18
{
    const int worldSize = 22;

    protected override int Result()
    {
        var result1 = Result1();

        var world = new HashSet<Cube>();
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    world.Add(new Cube(x, y, z));
                }
            }
        }

        var used = new HashSet<Cube>();
        Queue<Cube> toCheck = new Queue<Cube>();
        toCheck.Enqueue(new Cube(0, 0, 0));
        while (toCheck.Any())
        {
            var c = toCheck.Dequeue();
            if (used.Contains(c)) continue;
            used.Add(c);

            Cube candidate;
            for (int dim = 0; dim < 3; dim++)
            {
                candidate = GetPrevCube(c, dim);
                if (candidate.x >= 0 && candidate.y >= 0 && candidate.z >= 0
                    && candidate.x < worldSize && candidate.y < worldSize && candidate.z < worldSize
                    && !lava.Contains(candidate) && !used.Contains(candidate))
                {
                    toCheck.Enqueue(candidate);
                }

                candidate = GetNextCube(c, dim);
                if (candidate.x >= 0 && candidate.y >= 0 && candidate.z >= 0
                    && candidate.x < worldSize && candidate.y < worldSize && candidate.z < worldSize
                    && !lava.Contains(candidate) && !used.Contains(candidate))
                {
                    toCheck.Enqueue(candidate);
                }
            }
        }

        var result2 = 0;
        var air = world.Except(used).Except(lava).ToHashSet();
        foreach (var c in air)
        {
            for (int dim = 0; dim < 3; dim++)
            {
                var prev = GetPrevCube(c, dim);
                if (!air.Contains(prev)) result2++;

                var next = GetNextCube(c, dim);
                if (!air.Contains(prev)) result2++;
            }
        }

        return result1 - result2;
    }
}

record struct Cube(int x, int y, int z)
{
    private int[] _dim;
    public int[] dim => _dim ??= new int[3] { x, y, z };
}
