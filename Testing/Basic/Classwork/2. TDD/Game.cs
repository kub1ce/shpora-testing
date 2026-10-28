namespace TDD.Task;
public class Game
{
    private const int FramesPerGame = 10;
    private readonly List<int> rolls = new();

    public void Roll(int pins)
    {
        rolls.Add(pins);
    }

    public int GetScore()
    {
        int score = 0;
        int rollIndex = 0;

        for (int frame = 0;
             frame < FramesPerGame && rollIndex < rolls.Count;
             frame++)
        {
            if (rolls[rollIndex] == 10)
            {
                score += 10;

                if (!HasRoll(rollIndex + 1))
                    break;

                score += rolls[rollIndex + 1];

                if (!HasRoll(rollIndex + 2))
                    break;

                score += rolls[rollIndex + 2];

                rollIndex++;
                continue;
            }

            if (!HasRoll(rollIndex + 1))
            {
                score += rolls[rollIndex];
                break;
            }

            int frameScore =
                rolls[rollIndex] + rolls[rollIndex + 1];

            score += frameScore;

            if (frameScore == 10)
            {
                if (!HasRoll(rollIndex + 2))
                    break;

                score += rolls[rollIndex + 2];

                if (!HasRoll(rollIndex + 3))
                    break;
            }

            rollIndex += 2;
        }

        return score;
    }

    private bool HasRoll(int index)
    {
        return index < rolls.Count;
    }
}