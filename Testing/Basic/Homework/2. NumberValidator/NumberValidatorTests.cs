using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    // проверка precision
    [TestCase(-1, Description = "Отрицательная точность")] // precision < 0
    [TestCase(0, Description = "Нулевая точность")] // precision == 0
    public void Constructor_Throws_OnInvalidPrecision(int precision)
    {
        Action action = () => new NumberValidator(precision);
        action.Should().Throw<ArgumentException>();
    }

    [TestCase(1, Description = "Минимально допустимая точность")]
    [TestCase(17, Description = "Обычное значение")]
    public void Constructor_DoesNotThrow_OnValidPrecision(int precision)
    {
        Action action = () => new NumberValidator(precision);
        action.Should().NotThrow();
    }

    // проверка scale
    [TestCase(-1, Description = "scale меньше нуля")] // scale < 0
    [TestCase(2, Description = "scale равен precision")] // scale == precision
    [TestCase(3, Description = "scale больше precision")] // scale > precision
    public void Constructor_Throws_OnInvalidScale(int scale)
    {
        Action action = () => new NumberValidator(2, scale);
        action.Should().Throw<ArgumentException>();
    }

    [TestCase(0, Description = "Нулевой scale")]
    [TestCase(1, Description = "Максимальный допустимый scale")] // scale == precision - 1
    public void Constructor_DoesNotThrow_OnValidScale(int scale)
    {
        Action action = () => new NumberValidator(2, scale);
        action.Should().NotThrow();
    }

    // number format
    [TestCase("0", Description = "Целое число")]
    [TestCase("0.0", Description = "Дробное число с нулём")]
    [TestCase("1,23", Description = "Дробное число с запятой")] // , разделитель
    [TestCase("1.23", Description = "Дробное число с точкой")]
    [TestCase("+1.23", Description = "Число с положительным знаком")]
    [TestCase("-1.23", Description = "Число с отрицательным знаком")]
    public void IsValidNumber_ReturnsTrue_ForValidFormat(string value)
    {
        var validator = new NumberValidator(17, 16);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("", Description = "Пустая строка")]
    [TestCase(" ", Description = "Строка из пробела")]
    [TestCase("a.sd", Description = "Буквы вместо цифр")]
    [TestCase("1.", Description = "Нет цифр после разделителя")] // нет дробной части
    [TestCase(".5", Description = "Нет цифр до разделителя")] // нет целой части
    [TestCase("1.2.3", Description = "Несколько разделителей")]
    [TestCase("1:23", Description = "Некорректный разделитель")]
    [TestCase("*1", Description = "Недопустимый знак в начале числа")]
    [TestCase("++1", Description = "Два знака плюса")]
    [TestCase("--1", Description = "Два знака минуса")]
    [TestCase("1-2", Description = "Знак находится не в начале числа")]
    [TestCase("+", Description = "Знак без числа")]
    [TestCase("-", Description = "Знак без числа")]
    [TestCase(".", Description = "Разделитель без цифр")]
    [TestCase(",", Description = "Разделитель без цифр")]
    [TestCase(" 1", Description = "Пробел перед числом")]
    [TestCase("1 ", Description = "Пробел после числа")]
    public void IsValidNumber_ReturnsFalse_ForInvalidFormat(string value)
    {
        var validator = new NumberValidator(17, 16);
        validator.IsValidNumber(value).Should().BeFalse();
    }


    // positive
    [TestCase("-1", Description = "Отрицательное целое число")]
    [TestCase("-1.23", Description = "Отрицательное дробное число")]
    [TestCase("-0.00", Description = "Отрицательный ноль")]
    public void IsValidNumber_ReturnsFalse_WhenOnlyPositiveAndNumberIsNegative(string value)
    {
        var validator = new NumberValidator(17, 2, onlyPositive: true);
        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("-1", Description = "Отрицательное целое число")]
    [TestCase("-1.23", Description = "Отрицательное дробное число")]
    public void IsValidNumber_ReturnsTrue_WhenOnlyPositiveIsDisabled(string value)
    {
        var validator = new NumberValidator(17, 2, onlyPositive: false);
        validator.IsValidNumber(value).Should().BeTrue();
    }


    // precision
    [TestCase("1.23", Description = "Количество знаков равно precision")]
    [TestCase("123", Description = "Количество знаков целого числа равно precision")]
    [TestCase("+1.2", Description = "Знак плюса учитывается в precision")]
    [TestCase("-1.2", Description = "Знак минуса учитывается в precision")]
    public void IsValidNumber_ReturnsTrue_WhenPrecisionIsNotExceeded(string value)
    {
        var validator = new NumberValidator(3, 2, onlyPositive: false);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("00.00", Description = "Общее количество знаков превышает precision")] // 4 цифры > precision
    [TestCase("+1.23", Description = "Знак плюса учитывается в precision")] // знак + входит в precision
    [TestCase("-1.23", Description = "Знак минуса учитывается в precision")] // знак - входит в precision
    [TestCase("1234", Description = "Длина целого числа превышает precision")] // длина целой части > precision
    public void IsValidNumber_ReturnsFalse_WhenPrecisionIsExceeded(string value)
    {
        var validator = new NumberValidator(3, 2, onlyPositive: false);
        validator.IsValidNumber(value).Should().BeFalse();
    }


    // scale
    [TestCase("1.23", 2, Description = "Количество дробных знаков равно scale")]
    [TestCase("1.2", 2, Description = "Количество дробных знаков меньше scale")]
    [TestCase("123", 0, Description = "Целое число при нулевом scale")] // scale == 0
    public void IsValidNumber_ReturnsTrue_WhenScaleIsNotExceeded(string value, int scale)
    {
        var validator = new NumberValidator(17, scale);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("0.000", 2, Description = "Количество дробных знаков превышает scale")] // дробная часть длиннее scale
    [TestCase("1.0", 0, Description = "Дробная часть при нулевом scale")] // scale == 0
    public void IsValidNumber_ReturnsFalse_WhenScaleIsExceeded(string value, int scale)
    {
        var validator = new NumberValidator(17, scale);
        validator.IsValidNumber(value).Should().BeFalse();
    }
    
    // null
    [Test]
    public void IsValidNumber_ReturnsFalse_ForNull()
    {
        var validator = new NumberValidator(17, 2);
        validator.IsValidNumber(null!).Should().BeFalse();
    }
}