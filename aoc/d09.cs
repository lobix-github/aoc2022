abstract class d09
{
    private Pos[] rope;
    private List<Pos> tailPositions = new List<Pos>() { new Pos(0, 0) };

    protected abstract int len { get; }

    public d09() 
    { 
        rope = new Pos[len];
        for (int i = 0; i < len; i++) { rope[i] = new Pos(0, 0); }
    }

    public void Run()
    {
        File.ReadAllLines(@"..\..\..\09.txt").Select(line => new Move(line[0], Convert.ToInt32(line.Split(' ')[1]))).ToList().ForEach(MakeMove); ;

        Console.WriteLine(tailPositions.Distinct().Count());
    }

    void MakeMove(Move move)
    {
        switch (move.dir)
        {
            case 'R': for (int i = 0; i < move.dist; i++) { rope[0].x++; for (int idx = 1; idx < len; idx++) KnotMove(idx); tailPositions.Add(rope[len - 1]); }; break;
            case 'L': for (int i = 0; i < move.dist; i++) { rope[0].x--; for (int idx = 1; idx < len; idx++) KnotMove(idx); tailPositions.Add(rope[len - 1]); }; break;
            case 'U': for (int i = 0; i < move.dist; i++) { rope[0].y--; for (int idx = 1; idx < len; idx++) KnotMove(idx); tailPositions.Add(rope[len - 1]); }; break;
            case 'D': for (int i = 0; i < move.dist; i++) { rope[0].y++; for (int idx = 1; idx < len; idx++) KnotMove(idx); tailPositions.Add(rope[len - 1]); }; break;
        }
    }

    void KnotMove(int idx)
    {
        if (Math.Abs(rope[idx - 1].x - rope[idx].x) > 1)
        {
            rope[idx].x = rope[idx - 1].x > rope[idx].x ? rope[idx - 1].x - 1 : rope[idx - 1].x + 1;
            if (rope[idx - 1].y != rope[idx].y)
            {
                rope[idx].y = rope[idx - 1].y > rope[idx].y ? rope[idx].y + 1 : rope[idx].y - 1;
            }
        }
        if (Math.Abs(rope[idx - 1].y - rope[idx].y) > 1)
        {
            if (rope[idx - 1].x != rope[idx].x)
            {
                rope[idx].x = rope[idx - 1].x > rope[idx].x ? rope[idx].x + 1 : rope[idx].x - 1;
            }
            rope[idx].y = rope[idx - 1].y > rope[idx].y ? rope[idx - 1].y - 1 : rope[idx - 1].y + 1;
        }
    }
}

class d09_1 : d09
{
    protected override int len => 2;
}

class d09_2 : d09
{
    protected override int len => 10;
}

record struct Move(char dir, int dist);
record struct Pos(int x, int y);

