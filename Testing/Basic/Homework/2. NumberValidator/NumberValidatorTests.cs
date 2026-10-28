using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    // инвалидные аргументы
    [TestCase(-1, 2, true)] // precision < 0
    [TestCase(0, 0, false)] // precision == 0
    [TestCase(1, -1, false)] // scale < 0
    [TestCase(1, 1, false)] // scale == precision
    [TestCase(2, 3, true)] // scale > precision
    public void Constructor_Throws_OnInvalidArguments(int precision, int scale, bool onlyPositive)
    {
        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
    }

    // валидные аргументы
    [TestCase(1, 0, true)]
    [TestCase(1, 0, false)]
    [TestCase(2, 1, false)] // scale == precision - 1, граница
    [TestCase(17, 2, true)]
    public void Constructor_DoesNotThrow_OnValidArguments(int precision, int scale, bool onlyPositive)
    {
        Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
    }

    [TestCase(17, 2, true, "0")]
    [TestCase(17, 2, true, "0.0")]
    [TestCase(3, 2, true, "1,23")] // , разделитель
    [TestCase(3, 2, true, "1.23")] // ровно precision цифр
    [TestCase(4, 2, true, "+1.23")] // знак + входит в precision
    [TestCase(17, 2, false, "-1.23")] // отрицательные только if onlyPositive == false
    [TestCase(4, 2, false, "-1.23")] // знак - входит в precision, ровно на границе
    [TestCase(3, 0, false, "123")] // целое число при scale == 0
    [TestCase(3, 0, false, "-12")]
    public void IsValidNumber_ReturnsTrue_ForValidNumber(int precision, int scale, bool onlyPositive, string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        Assert.That(validator.IsValidNumber(value), Is.True,
            $"value \"{value}\" must be valid for precision={precision}, scale={scale}, onlyPositive={onlyPositive}");
    }

    [TestCase(17, 2, true, "")]
    [TestCase(17, 2, true, " ")]
    [TestCase(3, 2, true, "a.sd")]
    [TestCase(17, 2, true, "1.")] // нет цифр после разделителя
    [TestCase(17, 2, true, ".5")] // и без цифр до разделителя
    [TestCase(17, 2, true, "1.2.3")]
    [TestCase(3, 2, true, "00.00")] // 4 цифры > precision
    [TestCase(3, 2, true, "+0.00")] // знак + учитывается в precision
    [TestCase(3, 2, true, "+1.23")]
    [TestCase(3, 2, false, "-1.23")] // знак - учитывается в precision
    [TestCase(17, 2, true, "0.000")] // дробная часть длиннее scale
    [TestCase(3, 0, false, "1.0")] // дробная часть при scale == 0
    [TestCase(3, 0, false, "1234")] // целая часть длиннее precision
    [TestCase(17, 2, true, "-1.23")] // минусовое при onlyPositive == true
    [TestCase(17, 2, true, "-0.00")]
    public void IsValidNumber_ReturnsFalse_ForInvalidNumber(int precision, int scale, bool onlyPositive, string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        Assert.That(validator.IsValidNumber(value), Is.False,
            $"value \"{value}\" must be invalid for precision={precision}, scale={scale}, onlyPositive={onlyPositive}");
    }

    [Test]
    public void IsValidNumber_ReturnsFalse_ForNull()
    {
        var validator = new NumberValidator(17, 2, true);

        Assert.That(validator.IsValidNumber(null!), Is.False);
    }
}