using System.Numerics;
using System.Text.RegularExpressions;

abstract class d18
{
    protected HashSet<Cube> lava = new HashSet<Cube>();

    protected abstract void Result();

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

        long result = 0;
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

        Result();
    }

    Cube GetNextCube(Cube cube, int dim)
    {
        switch (dim)
        {
            case 0: return new Cube(cube.x + 1, cube.y, cube.z);
            case 1: return new Cube(cube.x, cube.y + 1, cube.z);
            case 2: return new Cube(cube.x, cube.y, cube.z + 1);
        }
        throw new NotImplementedException();
    }

    Cube GetPrevCube(Cube cube, int dim)
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
    protected override void Result()
    {
    }
}

class d18_2 : d18
{
    protected override void Result()
    {
    }
}

record struct Cube(int x, int y, int z)
{
    private int[] _dim;
    public int[] dim => _dim ??= new int[3] { x, y, z };
}
