using System.Text;

abstract class d10
{
    protected Dictionary<int, int> states = new Dictionary<int, int>();
    private int currentCycle = 0;
    private int currentValue = 1;

    protected abstract void Result();

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\10.txt");

        states[currentCycle] = currentValue;
        foreach (var line in lines)
        {
            switch (line.Split(' '))
            {
                case string[] words when words[0] == "noop":
                    Noop();
                    break;

                case string[] words when words[0] == "addx":
                    AddX(Convert.ToInt32(words[1]));
                    break;
            }
        }

        Result();
    }

    void Noop()
    {
        ExecuteCycle();
    }

    void AddX(int value)
    {
        ExecuteCycle();
        currentValue += value;
        ExecuteCycle();
    }

    void ExecuteCycle()
    {
        currentCycle++;
        states[currentCycle] = currentValue;
    }

    protected int ValueDuring(int cycle) => states[cycle];
}

class d10_1 : d10
{
    protected override void Result()
    {
        Console.WriteLine(SignalStrength(20) + SignalStrength(60) + SignalStrength(100) + SignalStrength(140) + SignalStrength(180) + SignalStrength(220));
    }

    int SignalStrength(int nthCycle) => nthCycle * ValueDuring(nthCycle - 1);
}

class d10_2 : d10
{
    protected override void Result()
    {
        var line = new StringBuilder();
        var crt = new List<StringBuilder>();
        for (int cycle = 0; cycle < states.Count; cycle++)
        {
            if (cycle % 40 == 0)
            {
                crt.Add(line);
                line = new StringBuilder("                                        ");
            }

            int[] sprite = new[] { ValueDuring(cycle) - 1, ValueDuring(cycle), ValueDuring(cycle) + 1 };
            if (sprite.Contains(cycle % 40))
            {
                line[cycle % 40] = '#';
            }
        }
        crt.Add(line);

        crt.Skip(1).ToList().ForEach(Console.WriteLine);
    }
}
