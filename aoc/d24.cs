using System.Collections.Generic;
using System.Text;

abstract class d24
{
    List<HashSet<Blizzard>> blizzardsInTime = new List<HashSet<Blizzard>>();
    Dictionary<string, char> dirs = new Dictionary<string, char>();
    protected int dimx, dimy;
    protected int cycleIdx = 0;

    protected abstract void Result();

    public void Run()
    {
        var blizzards = new HashSet<Blizzard>();
        var lines = File.ReadAllLines(@"..\..\..\24.txt").ToArray();
        dimy = lines.Length;
        for (int y = 0; y < dimy; y++)
        {
            var line = lines[y];
            dimx = line.Length;
            for (int x = 0; x < dimx; x++)
            {
                if (line[x] != '.' && line[x] != '#')
                {
                    var pos = new Blizzard(new Pos(x, y), line[x]);
                    blizzards.Add(pos);
                }
            }
        }

        var seen = new HashSet<int>();
        while (true)
        {
            blizzardsInTime.Add(blizzards);
            var id = blizzards.GetHash();
            if (seen.Contains(id))
            {
                cycleIdx = blizzardsInTime.Count - 1;
                break;
            }
            blizzards = MoveBlizzards(blizzards);
            seen.Add(id);
        }

        Result();
    }

    protected int TraversePlain(Pos startPos, Pos endPos, int startIdx, int cycleIdx)
    {
        var seen = new HashSet<int>();
        Queue<BlizzardState> queue = new Queue<BlizzardState>();
        queue.Enqueue(new BlizzardState(startPos, 0, startIdx % cycleIdx));
        while (true)
        {
            var state = queue.Dequeue();
            var stateId = state.GetHash(blizzardsInTime[state.blizzardsInTimeIdx]);
            if (seen.Contains(stateId)) continue;
            seen.Add(stateId);

            var pos = state.pos;
            if (state.pos == endPos)
            {
                return state.minutes;
            }

            var newBlizzardsInTimeIdx = (state.blizzardsInTimeIdx + 1) % cycleIdx;
            var newBlizzards = blizzardsInTime[newBlizzardsInTimeIdx];
            move(newPos => true, pos);
            move(newPos => newPos.x < dimx - 1  && newPos.y > 0 && newPos.y < dimy - 1,     pos with { x = pos.x + 1 }); // right
            move(newPos => newPos.x > 0         && newPos.y > 0 && newPos.y < dimy - 1,     pos with { x = pos.x - 1 }); // left
            move(newPos => newPos.y < dimy - 1  || newPos == endPos,                        pos with { y = pos.y + 1 }); // down
            move(newPos => newPos.y > 0         || newPos == endPos,                        pos with { y = pos.y - 1 }); // up

            void move(Func<Pos, bool> check, Pos newPos)
            {
                if (check(newPos))
                {
                    if (!newBlizzards.Any(b => b.pos.x == newPos.x && b.pos.y == newPos.y)) queue.Enqueue(state with { pos = newPos, minutes = state.minutes + 1, blizzardsInTimeIdx = newBlizzardsInTimeIdx });
                }
            }
        }
    }

    HashSet<Blizzard> MoveBlizzards(HashSet<Blizzard> blizzards)
    {
        HashSet<Blizzard> result = new HashSet<Blizzard>();
        foreach (var blizzard in blizzards)
        {
            var newPos = blizzard.dir switch
            {
                '>' => blizzard.pos with { x = blizzard.pos.x + 1 },
                '<' => blizzard.pos with { x = blizzard.pos.x - 1 },
                'v' => blizzard.pos with { y = blizzard.pos.y + 1 },
                '^' => blizzard.pos with { y = blizzard.pos.y - 1 },
            };


            if (blizzard.dir == '>' && newPos.x == dimx - 1) newPos.x = 1;
            if (blizzard.dir == '<' && newPos.x == 0) newPos.x = dimx - 2;
            if (blizzard.dir == 'v' && newPos.y == dimy - 1) newPos.y = 1;
            if (blizzard.dir == '^' && newPos.y == 0) newPos.y = dimy - 2;
            result.Add(new Blizzard(newPos, blizzard.dir));
        }

        return result;
    }
}

public record struct Blizzard(Pos pos, char dir);
public record struct BlizzardState(Pos pos, int minutes, int blizzardsInTimeIdx);

class d24_1 : d24
{
    protected override void Result()
    {
        var startPos = new Pos(1, 0);
        var endPos = new Pos(dimx - 2, dimy - 1);
        var result = TraversePlain(startPos, endPos, 0, cycleIdx);
        
        Console.WriteLine(result);
    }
}

class d24_2 : d24
{
    protected override void Result()
    {
        var startPos = new Pos(1, 0);
        var endPos = new Pos(dimx - 2, dimy - 1);
        var result1 = TraversePlain(startPos, endPos, 0, cycleIdx);
        var result2 = TraversePlain(endPos, startPos, result1, cycleIdx);
        var result3 = TraversePlain(startPos, endPos, result1 + result2, cycleIdx);

        Console.WriteLine(result1 + result2 + result3);
    }
}

public static class ExtsD24
{
    public static int GetHash(this BlizzardState state, HashSet<Blizzard> blizzards)
    {
        var hash = state.pos.GetHashCode();
        foreach (var blizzard in blizzards)
        {
            hash ^= blizzard.GetHashCode();
        }
        return hash;
    }

    public static int GetHash(this HashSet<Blizzard> blizzards)
    {
        var hash = 0;
        foreach (var blizzard in blizzards)
        {
            hash ^= blizzard.GetHashCode();
        }
        return hash;
    }
}

