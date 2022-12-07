using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

abstract class d07
{
    protected List<Dir7> dirs = new List<Dir7>();
    protected Dir7 tree = new Dir7("/");
    protected Dir7? current;

    public void Run()
    {
        var lines = File.ReadLines(@"..\..\..\07.txt").Skip(1).ToArray();
        current = tree;

        foreach (var line in lines)
        {
            var words = line.Split(' ');
            if (words[0] == "dir")
            {
                current.TryCreateDir(words[1]);
                continue;
            }

            if (char.IsDigit(words[0][0]))
            {
                current.TryCreateFile(Convert.ToInt32(words[0]), words[1]);
                continue;
            }

            if (words[0] == "$" && words[1] == "cd")
            {
                current = words[2] != ".." ? current.Dirs[words[2]] : current.Parent ?? throw new NullReferenceException("root");
                continue;
            }
        }

        void Traverse(Dir7 dir)
        {
            if (Filter(dir))
            {
                dirs.Add(dir);
            }
            foreach (var _dir in dir.Dirs)
            {
                Traverse(_dir.Value);
            }
        }

        Traverse(tree);
        Console.WriteLine(Result);
    }

    abstract protected int Result { get; }
    abstract protected bool Filter(Dir7 dir);
}

public class Dir7
{
    public string Name;
    public Dir7? Parent;

    public Dictionary<string, Dir7> Dirs = new Dictionary<string, Dir7>();
    public Dictionary<string, int> Files = new Dictionary<string, int>();

    public Dir7(string name) : this(null, name) { }
    private Dir7(Dir7? parent, string name) { Parent = parent; Name = name; }

    public void TryCreateDir(string name) => Dirs[name] = new Dir7(this, name);

    public void TryCreateFile(int size, string name) => Files[name] = size;

    public int Size => Dirs.Sum(x => x.Value.Size) + Files.Sum(x => x.Value);
}

class File7
{   
    public string Name;
    public int Size;

    public File7(string name, int size) { Name = name; Size = size; }
}

class d07_1 : d07
{
    override protected int Result => dirs.Sum(x => x.Size);

    protected override bool Filter(Dir7 dir) => dir.Size <= 100000;
}

class d07_2 : d07
{
    override protected int Result => dirs.OrderBy(x => x.Size).First().Size;

    protected override bool Filter(Dir7 dir) => dir.Size >= 30000000 - (70000000 - tree.Size);
}