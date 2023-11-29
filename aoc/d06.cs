using System;
using System.IO;
using System.Linq;

abstract class d06
{
    protected abstract int len { get; }
    public void Run()
    {
        var line = File.ReadAllText(@"..\..\..\06.txt");
        for (int i = 0; i < line.Length - len; i++)
        {
            var chunk = line.Skip(i).Take(len);
            if (chunk.Distinct().Count() == len)
            {
                Console.WriteLine(i);
                break;
            }
        }
    }
}

class d06_1 : d06
{
    protected override int len => 4;
}

class d06_2 : d06
{
    protected override int len => 14;
}
