using Bogus;

namespace AccountService.Tests;

public class TestsBase
{
    private static Faker Faker { get; } = new();

    protected static TEnum PickRandom<TEnum>(params TEnum[] exclude) where TEnum : struct, Enum
        => Faker.Random.Enum(exclude);
}