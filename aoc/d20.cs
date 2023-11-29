abstract class d20
{
    private List<string> words;
    private List<WordPos> orderedWords;
    int len;

    protected abstract long multiplier { get; }
    protected abstract long rounds { get; }

    public void Run()
    {
        words = File.ReadAllLines(@"..\..\..\20.txt").Select(w => (Convert.ToInt64(w) * multiplier).ToString()).ToList();
        orderedWords = words.Select((w, idx) => new WordPos(w, idx)).ToList();
        len = words.Count();

        for (int round = 0; round < rounds; round++)
        {
            for (int i = 0; i < len; i++)
            {
                var wordPos = new WordPos(words[i], i);
                var pos = orderedWords.IndexOf(wordPos);
                var delta = Convert.ToInt64(wordPos.word);

                if (delta < 0)
                {
                    delta = transformNegDelta(delta);
                }
                if (delta > 0)
                {
                    var off = (int)calcOffset(pos, delta);
                    orderedWords.RemoveAt(pos);
                    orderedWords.Insert(off, wordPos);
                }
            }
        }

        var zeroWord = orderedWords.Single(x => x.word == "0");
        var zeroIdx = orderedWords.IndexOf(zeroWord);
        var result = Convert.ToInt64(orderedWords[(zeroIdx + 1000) % len].word)
                    + Convert.ToInt64(orderedWords[(zeroIdx + 2000) % len].word)
                    + Convert.ToInt64(orderedWords[(zeroIdx + 3000) % len].word);
        Console.WriteLine(result);
    }

    long calcOffset(long pos, long delta)
    {
        var x = (pos + delta) % len;
        var y = (pos + delta) / len;
        var result = x + y;
        while (result >= len)
        {
            result = calcOffset(x, y);
        }

        return result;
    }

    long transformNegDelta(long delta)
    {
        var x = -delta / len + 1;
        var result = x * len + delta - x;
        while (result < 0)
        {
            result = transformNegDelta(result);
        }
        return result;
    }
}

class d20_1 : d20
{
    protected override long multiplier => 1;
    protected override long rounds => 1;
}

class d20_2 : d20
{
    protected override long multiplier => 811589153;
    protected override long rounds => 10;
}

record struct WordPos(string word, int pos);