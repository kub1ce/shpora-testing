using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Id берётся из статического счетчика где для каждого созданного экземпляра он разный
        actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
            .Excluding(ctx => ctx.DeclaringType == typeof(Person) && ctx.Name == nameof(Person.Id)));
        
            // ИЛИ еще думал: .Excluding(ctx => Regex.IsMatch(ctx.Path, @"^(Parent\.)*Id$")));
            // но этот вариант сломается ели в Person добавить ещё одно поле типа Person
        
        // 1. новое поле класса проверяется автоматически
        // 2. FluentAssertions показывает, какое именно поле упало, в отличие от AreEqual
        // 3. Исключение только Id объектов Person в цепочке Parent, т.к. они генерируются автоматически
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 
        // 1. при добавлении поля в Person нужно вручную дописывать его, тк тест 
        //    не будет проверять новое поле и может продолжать проходить.
        // 2. непонятно, какое именно поле отличается при падении теста
        // 3. нужно самому писать и поддерживать рекурсивный обход Parent
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
