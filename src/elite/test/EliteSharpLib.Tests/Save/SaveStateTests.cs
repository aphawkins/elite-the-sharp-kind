// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Reflection;
using EliteSharpLib.Save;

namespace EliteSharpLib.Tests.Save;

// The saved commander's copy constructor. It names every property one by one,
// so a property added to the class and not added to it would be dropped
// silently the next time a save was copied - which is the one mistake this
// class can make.
public class SaveStateTests
{
    // The settable properties, which are the ones the copy constructor has to
    // carry. The init-only collections are set up by the class itself and
    // shared by a copy rather than assigned.
    private static IEnumerable<PropertyInfo> Properties
        => typeof(SaveState)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true);

    [Fact]
    public void TheCopyConstructorCarriesEveryProperty()
    {
        SaveState original = new();
        foreach (PropertyInfo property in Properties)
        {
            property.SetValue(original, DistinctValueFor(property.PropertyType));
        }

        SaveState copy = new(original);

        // Reflected over rather than listed, so a property added to SaveState
        // is compared here without this test being touched.
        Assert.All(
            Properties,
            property => Assert.Equal(property.GetValue(original), property.GetValue(copy)));
    }

    [Fact]
    public void ACopyIsAnObjectOfItsOwn()
    {
        SaveState original = new() { CommanderName = "JAMESON" };

        SaveState copy = new(original) { CommanderName = "ELITE" };

        Assert.Equal("JAMESON", original.CommanderName);
        Assert.Equal("ELITE", copy.CommanderName);
    }

    [Fact]
    public void ThereIsNothingToCopyFromNothing()
        => Assert.Throws<ArgumentNullException>(() => new SaveState(null!));

    [Fact]
    public void ANewSaveIsStampedWithTodaysFormat()
    {
        SaveState save = new();

        Assert.Equal(SaveState.CurrentFileType, save.FileType);
        Assert.Equal(SaveState.CurrentVersion, save.Version);
    }

    // A value nothing on a fresh SaveState already holds, so a property the
    // copy constructor misses comes back as its default and fails.
    private static object DistinctValueFor(Type type)
    {
        if (type == typeof(string))
        {
            return "carried";
        }

        if (type == typeof(bool))
        {
            return true;
        }

        if (type == typeof(int))
        {
            return 42;
        }

        if (type == typeof(float))
        {
            return 4.2f;
        }

        if (type == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(1984, 9, 20, 0, 0, 0, TimeSpan.Zero);
        }

        // The keyed collections are declared as the interface, so the type to
        // build is the dictionary behind it.
        if (type.IsInterface && type.IsGenericType)
        {
            return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments()))!;
        }

        // The nested states are carried by reference, so a fresh instance is
        // distinct from the one the constructor made.
        return Activator.CreateInstance(type)
            ?? throw new InvalidOperationException($"No test value for {type.Name}.");
    }
}
