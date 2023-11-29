using System.Text.RegularExpressions;

abstract class d19
{
    const string s = @"Blueprint (?<id>\d+): Each ore robot costs (?<orc>\d+) ore. Each clay robot costs (?<crc>\d+) ore. Each obsidian robot costs (?<orco>\d+) ore and (?<orcc>\d+) clay. Each geode robot costs (?<grcore>\d+) ore and (?<grcobs>\d+) obsidian.";

    protected HashSet<Cube> lava = new HashSet<Cube>();

    protected abstract int Result();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\19.txt").ToArray();
        int result = 0;
        foreach (var line in lines)
        {
            var match = Regex.Match(line, s);
            var id = Convert.ToInt32(match.Groups[1].Value);
            var oreRcore = Convert.ToInt32(match.Groups[2].Value);
            var clayRcore = Convert.ToInt32(match.Groups[3].Value);
            var obsRcore = Convert.ToInt32(match.Groups[4].Value);
            var obsRcclay = Convert.ToInt32(match.Groups[5].Value);
            var geoRcore = Convert.ToInt32(match.Groups[6].Value);
            var geoRcobs = Convert.ToInt32(match.Groups[7].Value);

            Console.WriteLine("---------------------------------------------------- >" + id);
            result += id * Run(oreRcore, clayRcore, obsRcore, obsRcclay, geoRcore, geoRcobs);
        }

        Console.WriteLine(result);
    }

    int Run(int oreRcore, int clayRcore, int obsRcore, int obsRcclay, int geoRcore, int geoRcobs)
    {
        var minutes = 0;
        var maxGeo = 0;
        var seen = new HashSet<state19>();
        Queue<state19> queue = new Queue<state19>();
        queue.Enqueue(new state19(0, 0, 0, 0, 1, 0, 0, 0, 0));
        while (queue.Any())
        {
            var state = queue.Dequeue();
            if (seen.Contains(state)) continue;
            if (state.geo > maxGeo)
            {
                maxGeo = state.geo;
            }
            if (state.minutes == 24) continue;
            seen.Add(state);

            if (minutes < state.minutes)
            {
                minutes = state.minutes;
                Console.WriteLine(state.minutes);
            }

            queue.Enqueue(new state19(state.ore + state.oreR, state.clay + state.clayR, state.obs + state.obsR, state.geo + state.geoR, state.oreR, state.clayR, state.obsR, state.geoR, state.minutes + 1));

            //if (state.minutes == 20 && (state.ore < obsRcore || state.clay < obsRcclay)) continue;
            //if (state.minutes == 22 && (state.ore < geoRcore || state.obs < geoRcobs)) continue;

            if (state.ore >= oreRcore) queue.Enqueue(new state19(state.ore + state.oreR - oreRcore, state.clay + state.clayR, state.obs + state.obsR, state.geo + state.geoR, state.oreR + 1, state.clayR, state.obsR, state.geoR, state.minutes + 1));
            if (state.minutes <= 18 && state.ore >= clayRcore) queue.Enqueue(new state19(state.ore + state.oreR - clayRcore, state.clay + state.clayR, state.obs + state.obsR, state.geo + state.geoR, state.oreR, state.clayR + 1, state.obsR, state.geoR, state.minutes + 1));
            if (state.minutes <= 20 && state.ore >= obsRcore && state.clay >= obsRcclay) queue.Enqueue(new state19(state.ore + state.oreR - obsRcore, state.clay + state.clayR - obsRcclay, state.obs + state.obsR, state.geo + state.geoR, state.oreR, state.clayR, state.obsR + 1, state.geoR, state.minutes + 1));
            if (state.minutes <= 22 && state.ore >= geoRcore && state.obs >= geoRcobs) queue.Enqueue(new state19(state.ore + state.oreR - geoRcore, state.clay + state.clayR, state.obs + state.obsR - geoRcobs, state.geo + state.geoR, state.oreR, state.clayR, state.obsR, state.geoR + 1, state.minutes + 1));
        }

        return maxGeo;
    }
}

record struct state19(int ore, int clay, int obs, int geo, int oreR, int clayR, int obsR, int geoR, int minutes);

class d19_1 : d19
{
    protected override int Result()
    {
        return 0;
    }
}

class d19_2 : d19
{
    protected override int Result()
    {
        return 0;
    }
}
