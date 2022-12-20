abstract class d20
{
    private List<string> words = File.ReadAllLines(@"..\..\..\20.txt").ToList();
    private List<WordPos> orderedWordsCorrect;
    int len;

    public void Run()
    {
        orderedWordsCorrect = words.Select((w, idx) => new WordPos(w, idx)).ToList();
        len = words.Count();
        WordPos zeroWord = default;
        for (int i = 0; i < len; i++)
        {
            if (i % 100 == 0) Console.WriteLine(i);

            var wordPos = new WordPos(words[i], i);
            var pos = orderedWordsCorrect.IndexOf(wordPos);
            var delta = Convert.ToInt32(wordPos.word);

            if (delta > 0)
            {
                while (delta > 0)
                {
                    if (pos == len - 1)
                    {
                        pos = MoveForward(pos, 1);
                        delta--;
                        continue;
                    }
                    var offset = Math.Min(len - pos - 1, delta);
                    pos = MoveForward(pos, offset);
                    delta -= offset;
                }
            }

            if (delta < 0)
            {
                while (delta < 0)
                {
                    if (pos == 0)
                    {
                        pos = MoveBackward(pos, 1);
                        delta++;
                        continue;
                    }
                    var offset = Math.Min(pos, -delta);
                    pos = MoveBackward(pos, offset);
                    delta += offset;
                }
            }
        }

        zeroWord = orderedWordsCorrect.Single(x => x.word == "0");
        var zeroIdx = orderedWordsCorrect.IndexOf(zeroWord);
        var result = Convert.ToInt32(orderedWordsCorrect[(zeroIdx + 1000) % len].word)
                    + Convert.ToInt32(orderedWordsCorrect[(zeroIdx + 2000) % len].word)
                    + Convert.ToInt32(orderedWordsCorrect[(zeroIdx + 3000) % len].word);
        Console.WriteLine(result);
        //17490
    }

    int MoveForward(int idx, int offset)
    {
        var w = orderedWordsCorrect[idx];
        if (idx + offset < len)
        {
            orderedWordsCorrect.RemoveAt(idx);
            orderedWordsCorrect.Insert(idx + offset, w);
            return idx + offset;
        }
        else
        {
            orderedWordsCorrect.RemoveAt(idx);
            orderedWordsCorrect.Insert(1, w);
            return 1;
        }
    }

    int MoveBackward(int idx, int offset)
    {
        var w = orderedWordsCorrect[idx];
        if (idx - offset >= 0)
        {
            orderedWordsCorrect.RemoveAt(idx);
            orderedWordsCorrect.Insert(idx - offset, w);
            return idx - offset;
        }
        else
        {
            orderedWordsCorrect.RemoveAt(idx);
            orderedWordsCorrect.Insert(len - 2, w);
            return len - 2;
        }
    }
}

class d20_1 : d20
{

}

class d20_2 : d20
{

}

record struct WordPos(string word, int pos);