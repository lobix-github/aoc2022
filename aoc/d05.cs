using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

abstract class d05
{
    const int cols = 9;
    const string s = @"move (?<qty>\d+) from (?<from>\d+) to (?<to>\d+)";

    protected List<char>[] stacks = new List<char>[cols];

    public void Run()
    {
        var lines = File.ReadLines(@"..\..\..\05.txt").ToArray();

        for (int i = 0; i < cols; i++)
        {
            stacks[i] = new List<char>();
        }

        foreach (var line in lines)
        {
            if (line == "")
            {
                for (int i = 0; i < cols; i++)
                {
                    stacks[i].Reverse();
                }
                continue;
            }

            if (line[1] == '1')
            {
                continue;
            }

            if (!line.StartsWith("move"))
            {
                for (int i = 0; i < cols; i++)
                {
                    var c = line[4 * i + 1];
                    if (c != 32) stacks[i].Add(c);
                }
                continue;
            }

            var match = Regex.Match(line, s);
            var qty = Convert.ToInt32(match.Groups[1].Value);
            var from = Convert.ToInt32(match.Groups[2].Value) - 1;
            var to = Convert.ToInt32(match.Groups[3].Value) - 1;

            Arrange(qty, from, to);
        }

        string res = "";
        for (int i = 0; i < cols; i++)
        {
            res += stacks[i].Last();
        }
        Console.WriteLine(res);
    }

    abstract protected void Arrange(int qty, int from, int to);
}

class d05_1 : d05
{
    override protected void Arrange(int qty, int from, int to)
    {
        for (int i = 0; i < qty; i++)
        {
            var val = stacks[from].Last();
            stacks[from].RemoveAt(stacks[from].Count - 1);
            stacks[to].Add(val);
        }
    }
}

class d05_2 : d05
{
    override protected void Arrange(int qty, int from, int to)
    {
        for (int i = qty - 1; i >= 0; i--)
        {
            var val = stacks[from][stacks[from].Count - 1 - i];
            stacks[to].Add(val);
        }
        for (int i = 0; i < qty; i++)
        {
            stacks[from].RemoveAt(stacks[from].Count - 1);
        }
    }
}