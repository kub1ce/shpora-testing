using FluentAssertions;
using NUnit.Framework;
using TDD.Task;

namespace TDD;

[TestFixture]
public class GameTests
{
    [Test]
    public void HaveZeroScore_BeforeAnyRolls()
    {
        new Game()
            .GetScore()
            .Should().Be(0);
    }

    [Test]
    public void Score_IsSumOfPins_WhenThereAreTwoRolls()
    {
        var game = new Game();
        
        game.Roll(1);
        game.Roll(4);
        game.GetScore().Should().Be(5);
    }
    
    [Test]
    public void Score_GutterGame_IsZero()
    {
        var game = new Game();

        RollMany(game, 0, 20);

        game.GetScore().Should().Be(0);
    }

    [Test]
    public void Score_WithoutSparesOrStrikes_IsSumOfPins()
    {
        var game = new Game();

        RollMany(game, 1, 20);

        game.GetScore().Should().Be(20);
    }
    
    [Test]
    public void Score_Spare_AddsNextRollAsBonus()
    {
        var game = new Game();

        game.Roll(5);
        game.Roll(5);
        game.Roll(3);

        RollMany(game, 0, 17);

        game.GetScore().Should().Be(16);
    }
    
    [Test]
    public void Score_Strike_AddsNextTwoRollsAsBonus()
    {
        var game = new Game();

        game.Roll(10);
        game.Roll(3);
        game.Roll(4);

        RollMany(game, 0, 16);

        game.GetScore().Should().Be(24);
    }
    
    [Test]
    public void Score_SpareInTenthFrame_IncludesExtraRoll()
    {
        var game = new Game();

        for (int frame = 0; frame < 9; frame++)
        {
            game.Roll(1);
            game.Roll(2);
        }

        game.Roll(2);
        game.Roll(8);
        game.Roll(6);

        game.GetScore().Should().Be(43);
    }
    
    [Test]
    public void Score_StrikeInTenthFrame_IncludesTwoExtraRolls()
    {
        var game = new Game();

        for (int frame = 0; frame < 9; frame++)
        {
            game.Roll(1);
            game.Roll(2);
        }

        game.Roll(10);
        game.Roll(5);
        game.Roll(4);

        game.GetScore().Should().Be(46);
    }
    
    [Test]
    public void Score_StrikeInNinthAndSpareInTenthFrame()
    {
        var game = new Game();

        RollMany(game, 0, 16);

        game.Roll(10);

        game.Roll(2);
        game.Roll(8);
        game.Roll(6);

        game.GetScore().Should().Be(36);
    }
    
    [Test]
    public void Score_AllSpares_Is150()
    {
        var game = new Game();

        for (int frame = 0; frame < 10; frame++)
        {
            game.Roll(5);
            game.Roll(5);
        }

        game.Roll(5);

        game.GetScore().Should().Be(150);
    }
    
    [Test]
    public void Score_PerfectGame_Is300()
    {
        var game = new Game();

        RollMany(game, 10, 12);

        game.GetScore().Should().Be(300);
    }

    private static void RollMany(Game game, int pins, int count)
    {
        for (int i = 0; i < count; i++)
        {
            game.Roll(pins);
        }
    }
}