using Bogus;

namespace NewsletterIntegrationTests.Extension;

internal static class EnumExtensions
{
    private static readonly Faker _faker = new Faker();

    public static T GetRandomExcept<T>(this T enumValue, params T[] excludedValues) where T : Enum
    {
        return _faker.PickRandom(
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(value => !excludedValues.Contains(value))
        );
    }
}