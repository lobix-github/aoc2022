abstract class d23
{
    protected HashSet<Pos> plain = new HashSet<Pos>();
    Dictionary<string, Elf> elfs = new Dictionary<string, Elf>();

    protected abstract int rounds { get; }
    protected abstract bool isPart2 { get; }

    protected abstract void Result(int result);

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\23.txt").ToArray();
        for (int y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    var pos = new Pos(x, y);
                    plain.Add(pos);
                    var elf = new Elf(pos, plain);
                    elfs[elf.Id] = elf;
                }
            }
        }

        int i = 0;
        for (i = 0; i < rounds; i++)
        {
            var maybes = elfs.Values.Where(elf => !elf.SkipsRound).ToArray();
            var moves = maybes.Select(x => x.Proposal()).ToArray().GroupBy(x => x.Item1).Where(g => g.Count() == 1).Select(g => g.First()).ToArray();
            if (moves.Length == 0 && isPart2)
            {
                break;
            }
            foreach ((var newPos, var id) in moves)
            {
                var elf = elfs[id];
                plain.Remove(elf.Pos);
            }
            foreach ((var newPos, var id) in moves)
            {
                var elf = elfs[id];
                plain.Add(newPos);
                elf.Pos = newPos;
            }

            elfs.Values.ToList().ForEach(elf => elf.SwapPropositionIndex());
        }

        Result(i + 1);
    }
}

class Elf
{
    private readonly HashSet<Pos> plain;
    int checkIdx = 0;

    List<Func<bool>> checks = new List<Func<bool>>();
    List<Func<(Pos, string)>> proposals = new List<Func<(Pos, string)>>();

    public string Id { get; init; }
    public Pos Pos { get; set; }

    public Elf(Pos pos, HashSet<Pos> plain)
    {
        Id = pos.ToString();
        Pos = pos;

        this.plain = plain;

        checks.Add(checkNorth);
        checks.Add(checkSouth);
        checks.Add(checkWest);
        checks.Add(checkEast);

        proposals.Add(() => (new Pos(Pos.x, Pos.y - 1), Id));
        proposals.Add(() => (new Pos(Pos.x, Pos.y + 1), Id));
        proposals.Add(() => (new Pos(Pos.x - 1, Pos.y), Id));
        proposals.Add(() => (new Pos(Pos.x + 1, Pos.y), Id));
    }

    bool checkNorth() => !plain.Contains(new Pos(Pos.x - 1, Pos.y - 1)) &&
                        !plain.Contains(new Pos(Pos.x, Pos.y - 1)) &&
                        !plain.Contains(new Pos(Pos.x + 1, Pos.y - 1));
    bool checkSouth() => !plain.Contains(new Pos(Pos.x - 1, Pos.y + 1)) &&
                        !plain.Contains(new Pos(Pos.x, Pos.y + 1)) &&
                        !plain.Contains(new Pos(Pos.x + 1, Pos.y + 1));
    bool checkWest() => !plain.Contains(new Pos(Pos.x - 1, Pos.y - 1)) &&
                       !plain.Contains(new Pos(Pos.x - 1, Pos.y)) &&
                       !plain.Contains(new Pos(Pos.x - 1, Pos.y + 1));
    bool checkEast() => !plain.Contains(new Pos(Pos.x + 1, Pos.y - 1)) &&
                        !plain.Contains(new Pos(Pos.x + 1, Pos.y)) &&
                        !plain.Contains(new Pos(Pos.x + 1, Pos.y + 1));

    public bool SkipsRound => checks.All(p => p()) || checks.All(p => !p());

    public (Pos, string) Proposal()
    {
        checkIdx %= 4;
        for (int i = checkIdx; i < checkIdx + 4; i++)
        {
            if (checks[i % 4]()) return proposals[i % 4]();
        }

        throw new InvalidOperationException();
    }

    public void SwapPropositionIndex() => checkIdx++;
}

class d23_1 : d23
{
    protected override int rounds => 10; 
    protected override bool isPart2 => false;

    protected override void Result(int ignore)
    {
        var xspan = plain.Max(p => p.x) - plain.Min(p => p.x) + 1;
        var yspan = plain.Max(p => p.y) - plain.Min(p => p.y) + 1;
        var result = xspan * yspan - plain.Count();

        Console.WriteLine(result);
    }
}

class d23_2 : d23
{
    protected override int rounds => int.MaxValue;
    protected override bool isPart2 => true;

    protected override void Result(int result)
    {
        Console.WriteLine(result);
    }
}

