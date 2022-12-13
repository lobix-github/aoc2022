using System.Text;

abstract class d13
{
    protected abstract void Result(List<string> lines);

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\13.txt").ToList();

        Result(lines);
    }

    void MakeGroups(Group _left, Group _right)
    {
        // make all non-grouped values grouped in case their pair is a group
        for (int i = 0; i < Math.Min(_left.Length, _right.Length); i++) // enough to only compare to shorter length
        {
            if (_left[i].IsChar && !_right[i].IsChar) _left.MakeGroup(i);
            if (!_left[i].IsChar && _right[i].IsChar) _right.MakeGroup(i);
            if (!_left[i].IsChar && !_right[i].IsChar) MakeGroups(_left[i].Group, _right[i].Group);
        }
    }

    protected int Compare(string a, string b)
    {
        var leftGroup = BuildGroup(a);
        var rightGroup = BuildGroup(b);

        MakeGroups(leftGroup, rightGroup);

        var left = leftGroup.ToString();
        var right = rightGroup.ToString();

        // compare as strings
        return string.CompareOrdinal(left, right);
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
                    // cheat in order to convert all integers (including two-char '10' to a single char, leaving it still comparable
                    var valueChar = (char)('a' + Convert.ToInt32(lpartial.ToString())); 
                    group.Values.Add(new Value(valueChar));
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

    public Value this[int index] => Values[index];

    public void MakeGroup(int index)
    {
        Values[index] = new Value(new Group(this[index]));
    }

    public override string ToString()
    {
        var res = string.Join('_', Values.Select(v => v.ToString())); // '_' separators are greater than open/close brackets ('>', '<')

        return $">{res}<"; // '>' > '<' -> will make opening bracket greater than closing one
    }
}

class Value
{
    private readonly char value;
    public readonly Group? Group;

    public bool IsChar => this.Group == null;

    public Value(char value)
    {
        this.value = value;
    }

    public Value(Group group)
    {
        this.Group = group;
    }
    public int Length => Group?.Length ?? 1;


    public override string ToString()
    {
        if (IsChar) return $">{this.value}<"; // '>' > '<' -> will make opening bracket greater than closing one

        return this.Group.ToString();
    }
}
