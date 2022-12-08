abstract class d08
{
    protected List<List<Tree>> trees = new List<List<Tree>>();
    protected List<Tree> visibleTrees = new List<Tree>();

    public void Run()
    {
        var lines = File.ReadLines(@"..\..\..\08.txt").ToList();
        for (int row = 0; row < lines.Count; row++)
        {
            var line = new List<Tree>();
            trees.Add(line);
            for (int col = 0; col < lines[0].Length; col++)
            {
                line.Add(new Tree(lines[row][col], col, row));
            }
        }

        Console.WriteLine(Result());
    }

    abstract protected int Result();
}

class d08_1 : d08
{
    override protected int Result()
    {
        Parse(trees, false);
        Parse(Transpose(trees), false);
        Parse(trees, true);
        Parse(Transpose(trees), true);

        return visibleTrees.Distinct().Count();
    }

    void Parse(List<List<Tree>> lines, bool reverse)
    {
        for (int row = 0; row < lines.Count; row++)
        {
            var line = lines[row];
            if (reverse) line.Reverse();
            char maxHeight = ' ';
            for (int col = 0; col < line.Count; col++)
            {
                if (line[col].height > maxHeight)
                {
                    visibleTrees.Add(line[col]);
                    maxHeight = line[col].height;
                }
            }
            if (reverse) line.Reverse();
        }
    }

    List<List<Tree>> Transpose(List<List<Tree>> lines)
    {
        var res = new List<List<Tree>>();
        for (int col = 0; col < lines[0].Count; col++)
        {
            var line = new List<Tree>();
            for (int row = 0; row < lines.Count; row++)
            {
                line.Add(lines[row][col]);
            }
            res.Add(line);
        }

        return res;
    }
}

class d08_2 : d08
{
    override protected int Result()
    {
        return trees.Max(x => x.Max(CheckView));
    }

    int CheckView(Tree tree)
    {
        int view = 1;

        int localView = 0;
        for (int row = tree.y - 1; row >= 0; row--)
        {
            localView++;
            if (trees[row][tree.x].height >= tree.height) break;
        }
        view *= localView;

        localView = 0;
        for (int row = tree.y + 1; row < trees.Count; row++)
        {
            localView++;
            if (trees[row][tree.x].height >= tree.height) break;
        }
        view *= localView;

        localView = 0;
        for (int col = tree.x - 1; col >= 0; col--)
        {
            localView++;
            if (trees[tree.y][col].height >= tree.height) break;
        }
        view *= localView;

        localView = 0;
        for (int col = tree.x + 1; col < trees[0].Count; col++)
        {
            localView++;
            if (trees[tree.y][col].height >= tree.height) break;
        }
        view *= localView;

        return view;
    }
}

record struct Tree(char height, int x, int y);
