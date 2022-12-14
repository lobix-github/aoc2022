using System.Text;

abstract class d13
{
    protected abstract void Result(List<string> lines);

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\13.txt").ToList();

        Result(lines);
    }

    protected int Compare(string a, string b)
    {
        var g1 = BuildGroup(a);
        var g2 = BuildGroup(b);

        return Compare(g1, g2);
    }

    int Compare(Group g1, Group g2)
    {
        var res = g1.Values.Zip(g2.Values).Select(z =>
        {
            (var v1, var v2) = z;
            return (v1.IsInt, v2.IsInt) switch
            {
                (true, true) => v1.Char.CompareTo(v2.Char),
                (true, false) => Compare(new Group(new Value(v1.Char)), v2.Group),
                (false, true) => Compare(v1.Group, new Group(new Value(v2.Char))),
                (false, false) => Compare(v1.Group, v2.Group),
            };
        }).ToList();

        return res.All(x => x == 0) ? g1.Length - g2.Length : res.FirstOrDefault(x => x != 0);
    }

    Group BuildGroup(string line)
    {
        var group = new Group();
        StringBuilder lpartial = new StringBuilder();
        for (int left = 1; left < line.Length; left++)
        {
            if (line[left] == '[')
            {
                int leftPos = left;
                int counter = 1;
                while(counter > 0) // look for according closing bracket
                {
                    left++;
                    if (line[left] == '[') counter++;
                    if (line[left] == ']') counter--;
                }
                group.Values.Add(new Value(BuildGroup(line.Substring(leftPos, left - leftPos + 1))));
            }
            else if (char.IsNumber(line[left]))
            {
                lpartial.Append(line[left]);
            }
            else // both ',' and ']'
            {
                if (lpartial.Length > 0)
                {
                    var valueInt = Convert.ToInt32(lpartial.ToString()); 
                    group.Values.Add(new Value(valueInt));
                    lpartial.Clear();
                }

                if (line[left] == ']')
                {
                    return group;
                }
            }
        }

        return group;
    }
}

class d13_1 : d13
{
    protected override void Result(List<string> lines)
    {
        var sum = 0;
        var pairIdx = 1;
        for (int idx = 0; idx < lines.Count; idx += 3, pairIdx++)
        {
            if (Compare(lines[idx], lines[idx + 1]) < 0)
            {
                sum += pairIdx;
            }
        }

        Console.WriteLine(sum);
    }
}

class d13_2 : d13
{
    protected override void Result(List<string> lines)
    {
        lines = lines.Where(l => l != "").ToList();
        lines.Add("[[2]]");
        lines.Add("[[6]]");
        lines.Sort(Compare);

        var result = (lines.IndexOf("[[2]]") + 1) * (lines.IndexOf("[[6]]") + 1);
        Console.WriteLine(result);
    }
}

class Group
{
    public Group() { }

    public Group(Value value)
    {
        Values.Add(value);
    }

    public List<Value> Values = new List<Value>();

    public int Length => Values.Count;
}

class Value
{
    public readonly int Char;
    public readonly Group? Group;

    public bool IsInt => Group == null;

    public Value(int value)
    {
        Char = value;
    }

    public Value(Group group)
    {
        Group = group;
    }
}
