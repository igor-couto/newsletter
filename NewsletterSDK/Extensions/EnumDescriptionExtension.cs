using System.ComponentModel;

namespace NewsletterSDK.Extensions;

public static class EnumDescriptionExtension
{
    static public string GetDescription(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());

        if (field is null)
           return enumValue.ToString();

        field?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) is DescriptionAttribute attribute
            ? attribute.Description
            : enumValue.ToString();
    }
}