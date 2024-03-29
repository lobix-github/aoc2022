﻿abstract class d16
{
    Dictionary<string, Node> nodes = new Dictionary<string, Node>();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\16.txt").ToList();
        foreach (var line in lines)
        {
            var parts = line.Split(' ').Select(x => x.TrimEnd(',')).ToArray();
            var name = parts[1];
            var flowRate = Convert.ToInt32(parts[4].Split('=').Select(x => x.TrimEnd(';')).ToArray()[1]);
            var dests = parts[9..].ToList();
            var node = new Node(name, flowRate, dests);
            nodes[name] = node;
        }

        foreach (var node in nodes.Values)
        {
            node.minDists[node.name] = 0;
            CalcMinDist(node, node.name);
        }

        var superNodes = nodes.Values.Where(v => v.weight > 0).ToList();
        var result = Calculate(30, superNodes, "AA");
        Console.WriteLine(result);
    }
    
    void CalcMinDist(Node current, string targetName)
    {
        var visited = new HashSet<string>();

        while (visited.Count < nodes.Count)
        {
            visited.Add(current.name);
            int distance = current.minDists[targetName] + edgeWeight;
            foreach (var nodeName in current.nodes)
            {
                if (!visited.Contains(nodeName))
                {
                    var node = nodes[nodeName];
                    if (node.minDists.ContainsKey(targetName))
                    {
                        if (distance < node.minDists[targetName]) node.minDists[targetName] = distance;
                    }
                    else node.minDists[targetName] = distance;
                }
            }
            current = nodes.Values.Where(n => !visited.Contains(n.name) && n.minDists.ContainsKey(targetName)).OrderBy(n => n.minDists[targetName]).FirstOrDefault();
        }
    }

    int Calculate(int steps, List<Node> superNodes, string cur)
    {
        int max = 0;
        foreach (var node in superNodes)
        {
            int newSteps = steps - nodes[cur].minDists[node.name] - 1;
            if (newSteps > 0)
            {
                var res = newSteps * node.weight + Calculate(newSteps, superNodes.Where(n => n.name != node.name).ToList(), node.name);
                if (res > max) max = res;
            }
        }
        return max;
    }

    int edgeWeight => 1; // in this case always 1, normally Dictionary<(Node, Node), int>
}


record struct Node(string name, int weight, List<string> nodes)
{
    public Dictionary<string, int> minDists = new Dictionary<string, int>();
}

class d16_1 : d16
{
}

