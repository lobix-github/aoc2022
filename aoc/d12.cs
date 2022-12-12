abstract class d12
{
    protected List<List<Hill>> hills = new List<List<Hill>>();

    protected abstract void Result(DijkstraGraph graph);

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\12.txt");
        var graph = new DijkstraGraph();
        for (int row = 0; row < lines.Length; row++)
        {
            var l = lines[row].ToCharArray().Select((h, idx) => new Hill(flatten(h), row, idx, h == 'S', h == 'E')).ToList();
            hills.Add(l);
            l.ForEach(h => graph.InsertVertex(h.ToString()));
        }

        char flatten(char c)
        {
            if (c == 'S') return 'a';
            if (c == 'E') return 'z';
            return c;
        }

        for (int rowIdx = 0; rowIdx < hills.Count; rowIdx++)
        {
            var row = hills[rowIdx];
            for (int col = 0; col < row.Count; col++)
            {
                var source = row[col];
                if (col > 0)
                {
                    var dest = row[col - 1];
                    if (dest.height - source.height <= 1) graph.InsertEdge(source.ToString(), dest.ToString(), 1);
                }
                if (col < row.Count - 1)
                {
                    var dest = row[col + 1];
                    if (dest.height - source.height <= 1) graph.InsertEdge(source.ToString(), dest.ToString(), 1);
                }
                if (rowIdx > 0)
                {
                    var dest = hills[rowIdx - 1][col];
                    if (dest.height - source.height <= 1) graph.InsertEdge(source.ToString(), dest.ToString(), 1);
                }
                if (rowIdx < hills.Count - 1)
                {
                    var dest = hills[rowIdx + 1][col];
                    if (dest.height - source.height <= 1) graph.InsertEdge(source.ToString(), dest.ToString(), 1);
                }
            }
        }

        Result(graph);
    }
}

class d12_1 : d12
{
    protected override void Result(DijkstraGraph graph)
    {
        var S = hills.SelectMany(x => x).Single(h => h.isS);
        var result = graph.FindPaths(S.ToString());

        Console.WriteLine(result);
    }
}

class d12_2 : d12
{
    protected override void Result(DijkstraGraph graph)
    {

        var result = int.MaxValue;
        var lowest = hills.SelectMany(x => x).Where(x => x.height == 'a').ToArray();
        for (int i = 0; i < lowest.Count(); i++)
        {
            result = Math.Min(result, graph.FindPaths(lowest[i].ToString()));
        }

        Console.WriteLine(result);
    }
}

record struct Hill(char height, int y, int x, bool isS, bool isE)
{
    public override string ToString() => $"{height}: {y}x{x}{(isE ? "E" : string.Empty)}";
}

class Vertex
{
    public string name;
    public int status;
    public int predecessor;
    public int pathLength;

    public Vertex(string name)
    {
        this.name = name;
    }

    public override string ToString() => this.name;
}

class DijkstraGraph
{
    public readonly int MAX_VERTICES = 150*150;

    int n, e;
    int[,] adj;
    Vertex[] vertexList;

    private readonly int TEMPORARY = 1;
    private readonly int PERMANENT = 2;
    private readonly int NIL = -1;
    private readonly int INFINITY = 99999;

    public DijkstraGraph()
    {
        adj = new int[MAX_VERTICES, MAX_VERTICES];
        vertexList = new Vertex[MAX_VERTICES];
    }

    public int FindPaths(string source)
    {
        int s = GetIndex(source);

        Dijkstra(s);

        for (int v = 0; v < n; v++)
        {
            if (vertexList[v].pathLength != INFINITY)
            {
                if (vertexList[v].name.Contains("E"))
                {
                    return FindPath(s, v);
                }
            }
        }

        return int.MaxValue;
    }

    public void InsertVertex(string name) => vertexList[n++] = new Vertex(name);

    public void InsertEdge(string s1, string s2, int wt)
    {
        int u = GetIndex(s1);
        int v = GetIndex(s2);
        if (u == v)
            throw new InvalidOperationException("Not a valid edge");

        adj[u, v] = wt;
        e++;
    }

    private void Dijkstra(int s)
    {
        int v, c;

        for (v = 0; v < n; v++)
        {
            vertexList[v].status = TEMPORARY;
            vertexList[v].pathLength = INFINITY;
            vertexList[v].predecessor = NIL;
        }

        vertexList[s].pathLength = 0;

        while (true)
        {
            c = TempVertexMinPL();

            if (c == NIL)
                return;

            vertexList[c].status = PERMANENT;

            for (v = 0; v < n; v++)
            {
                if (IsAdjacent(c, v) && vertexList[v].status == TEMPORARY)
                    if (vertexList[c].pathLength + adj[c, v] < vertexList[v].pathLength)
                    {
                        vertexList[v].predecessor = c;
                        vertexList[v].pathLength = vertexList[c].pathLength + adj[c, v];
                    }
            }
        }
    }

    private int TempVertexMinPL()
    {
        int min = INFINITY;
        int x = NIL;
        for (int v = 0; v < n; v++)
        {
            if (vertexList[v].status == TEMPORARY && vertexList[v].pathLength < min)
            {
                min = vertexList[v].pathLength;
                x = v;
            }
        }
        return x;
    }

    private int FindPath(int s, int v)
    {
        int u;
        int[] path = new int[n];
        int sd = 0;
        int count = 0;

        while (v != s)
        {
            count++;
            path[count] = v;
            u = vertexList[v].predecessor;
            sd += adj[u, v];
            v = u;
        }
        count++;
        path[count] = s;

        return sd;
    }

    private int GetIndex(string s)
    {
        for (int i = 0; i < n; i++)
            if (s.Equals(vertexList[i].name))
                return i;
        throw new InvalidOperationException("Invalid Vertex");
    }

    private bool IsAdjacent(int u, int v) => adj[u, v] != 0;
}

