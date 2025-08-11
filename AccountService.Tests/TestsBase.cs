using Bogus;

namespace AccountService.Tests;

public class TestsBase
{
    private static readonly Faker FakerInstance = new ();
    
    public Faker Faker => FakerInstance;

    public TEnum PickRandom<TEnum>(params TEnum[] exclude) where TEnum : struct, Enum
        => Faker.Random.Enum<TEnum>();
}