using System.Numerics;
using System.Text;

abstract class d25
{
    protected abstract void Result();

    Dictionary<char, int> values = new Dictionary<char, int>()
    {
        { '0', 0 },
        { '1', 1 },
        { '2', 2 },
        { '-', 1 },
        { '=', 2 },
    };

    Dictionary<char, int> signs = new Dictionary<char, int>()
    {
        { '0', 1 },
        { '1', 1 },
        { '2', 1 },
        { '-', -1 },
        { '=', -1 },
    };

    public void Run()
    {
        var lines = File.ReadAllLines(@"..\..\..\25.txt").ToArray();

        BigInteger sum = 0;
        foreach (var line in lines)
        {
            sum += ToDecimal(line);
        }

        var result = ToSNAFU(sum);
        Console.WriteLine(result);
        //Result();
    }

    BigInteger ToDecimal(string word)
    {
        BigInteger res = 0;
        var pow = word.Length - 1;
        for (int i = 0; i < word.Length; i++)
        {
            res += new BigInteger(Math.Pow(5, pow--)) * new BigInteger(values[word[i]]) * signs[word[i]];
        }

        return res;
    }

    string ToSNAFU(BigInteger dec)
    {
        var f = To5(dec);
        var next = 0;
        StringBuilder result = new StringBuilder();
        for (int i = f.Length - 1; i >= 0; i--)
        {
            var c = f[i];
            (c, next) = Add(c, next);
            result.Insert(0, c);
        }

        if (next == 1)
        {
            result.Insert(0, "1");
        }

        return result.ToString();
    }

    (char, int) Add(char c, int next)
    {
        if (next == 1)
        {
            if (c == '0') return ('1', 0);
            if (c == '1') return ('2', 0);
            if (c == '2') return ('=', 1);
            if (c == '3') return ('-', 1);
            if (c == '4') return ('0', 1);
        }

        if (c == '0') return ('0', 0);
        if (c == '1') return ('1', 0);
        if (c == '2') return ('2', 0);
        if (c == '3') return ('=', 1);
        if (c == '4') return ('-', 1);

        throw new InvalidOperationException();
    }

    string To5(BigInteger dec)
    {
        StringBuilder result = new StringBuilder();
        BigInteger next = 0;
        while (dec > 0)
        {
            var reminder = dec % 5;
            result.Insert(0, (char)(reminder + '0'));
            dec /= 5;
        }

        return result.ToString();
    }
}

class d25_1 : d25
{
    protected override void Result()
    {
        //Console.WriteLine(result);
    }
}

class d25_2 : d25
{
    protected override void Result()
    {
        //Console.WriteLine(result1 + result2 + result3);
    }
}
