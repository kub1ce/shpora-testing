using Basic.Task.WordsStatistics.WordsStatistics;
using FluentAssertions;
using NUnit.Framework;

namespace Basic.Task.WordsStatistics;

// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki

[TestFixture]
public class WordsStatisticsTests
{

    private IWordsStatistics wordsStatistics;

    [SetUp]
    public void SetUp()
    {

        wordsStatistics = CreateStatistics();
    }

    public virtual IWordsStatistics CreateStatistics()
    {
        // меняется на разные реализации при запуске exe
        return new WordsStatisticsImpl();
    }


    [Test]
    public void GetStatistics_IsEmpty_AfterCreation()
    {
        wordsStatistics.GetStatistics().Should().BeEmpty();
    }

    [Test]
    public void GetStatistics_ContainsItem_AfterAddition()
    {
        wordsStatistics.AddWord("abc");
        wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 1));
    }

    [Test]
    public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
    {
        wordsStatistics.AddWord("abc");
        wordsStatistics.AddWord("def");
        wordsStatistics.GetStatistics().Should().HaveCount(2);
    }

    [Test]
    public void AddWord_Null_ThrowsArgumentNullException()
    {
        Action act = () => wordsStatistics.AddWord(null!);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("word");
    }

    [Test]
    public void AddWord_EmptyOrWhitespace_IsIgnored()
    {
        wordsStatistics.AddWord("");
        wordsStatistics.AddWord(" ");
        wordsStatistics.AddWord("\t");
        wordsStatistics.AddWord("\n");
        wordsStatistics.AddWord("\r\n");

        wordsStatistics.GetStatistics().Should().BeEmpty();
    }

    [Test]
    public void AddWord_WhitespaceInNonEmptyWord_IsPreserved()
    {
        wordsStatistics.AddWord("СКБ Контур");
        wordsStatistics.AddWord("скб контур");
        wordsStatistics.AddWord(" a ");
        wordsStatistics.AddWord("a");

        var result = wordsStatistics.GetStatistics().ToArray();

        result.Should().HaveCount(3);
        result.Should().Contain(new WordCount("скб контур", 2));
        result.Should().Contain(new WordCount(" a ", 1));
        result.Should().Contain(new WordCount("a", 1));
    }

    [Test]
    public void AddWord_DifferentCases_AreCountedAsOneWord()
    {
        wordsStatistics.AddWord("APPLE");
        wordsStatistics.AddWord("Apple");
        wordsStatistics.AddWord("aPpLe");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(new WordCount("apple", 3));
    }

    [Test]
    public void GetStatistics_Word_IsConvertedToLowerCase()
    {
        wordsStatistics.AddWord("HELLO");
        wordsStatistics.AddWord("WoRlD");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(
                new WordCount("hello", 1),
                new WordCount("world", 1));
    }

    [Test]
    public void GetStatistics_IsSortedByFrequencyAndThenByWord()
    {
        wordsStatistics.AddWord("banana");
        wordsStatistics.AddWord("banana");

        wordsStatistics.AddWord("apple");
        wordsStatistics.AddWord("apple");

        wordsStatistics.AddWord("cherry");
        wordsStatistics.AddWord("cherry");
        wordsStatistics.AddWord("cherry");

        wordsStatistics.AddWord("date");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(
                new WordCount("cherry", 3),
                new WordCount("apple", 2),
                new WordCount("banana", 2),
                new WordCount("date", 1));
    }

    [Test]
    public void AddWord_WordLongerThanTenCharacters_IsTruncated()
    {
        wordsStatistics.AddWord("abcdefghij");
        wordsStatistics.AddWord("abcdefghijk");
        wordsStatistics.AddWord("ABCDEFGHIJK");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(new WordCount("abcdefghij", 3));
    }

    [Test]
    public void AddWord_HashCollision_DoesNotMergeDifferentWords()
    {
        var (first, second) = FindHashCollision();

        first.Should().NotBe(second);
        first.GetHashCode().Should().Be(second.GetHashCode());

        wordsStatistics.AddWord(first);
        wordsStatistics.AddWord(second);

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(
                new WordCount(first, 1),
                new WordCount(second, 1));
    }

    [Test]
    public void AddWord_DifferentInstances_HaveIndependentStatistics()
    {
        var anotherStatistics = CreateStatistics();

        wordsStatistics.AddWord("first");

        anotherStatistics.GetStatistics().Should().BeEmpty();

        anotherStatistics.AddWord("second");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(new WordCount("first", 1));

        anotherStatistics.GetStatistics()
            .Should()
            .Equal(new WordCount("second", 1));
    }

    [Test]
    public void GetStatistics_ReturnsUpdatedDataAfterFurtherAdditions()
    {
        wordsStatistics.GetStatistics().Should().BeEmpty();

        wordsStatistics.AddWord("pear");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(new WordCount("pear", 1));

        wordsStatistics.AddWord("apple");
        wordsStatistics.AddWord("pear");

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(
                new WordCount("pear", 2),
                new WordCount("apple", 1));
    }

    [Test]
    [Timeout(5000)]
    public void AddWord_ManyDifferentWords_CompletesInReasonableTime()
    {
        const int wordCount = 10_000;

        for (int i = 0; i < wordCount; i++)
        {
            wordsStatistics.AddWord($"w{i:D9}");
        }

        var result = wordsStatistics.GetStatistics().ToArray();

        result.Should().HaveCount(wordCount);
        result.Should().OnlyContain(item => item.Count == 1);
    }

    [Test]
    [Timeout(5000)]
    public void AddWord_SameWordManyTimes_CompletesInReasonableTime()
    {
        const int repetitions = 100_000;

        for (int i = 0; i < repetitions; i++)
        {
            wordsStatistics.AddWord("same");
        }

        wordsStatistics.GetStatistics()
            .Should()
            .Equal(new WordCount("same", repetitions));
    }

    private static (string First, string Second) FindHashCollision()
    {
        var seen = new Dictionary<int, string>();

        for (int i = 0; i < 300_000; i++)
        {
            string word = $"w{i:D9}";
            int hash = word.GetHashCode();

            if (seen.TryGetValue(hash, out string previous))
            {
                return (previous, word);
            }

            seen.Add(hash, word);
        }

        Assert.Fail("Не удалось найти коллизию хеш-кода среди проверенных слов.");

        return (null!, null!);
    }
}