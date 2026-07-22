namespace DRSSoftware.TextTemplateProcessor.Core;

using System.ComponentModel.DataAnnotations;
using System.Reflection;

/// <summary>
/// Static class that provides helper functions for <see cref="Enum" /> types.
/// </summary>
internal static class EnumHelper
{
    /// <summary>
    /// <see cref="Enum" /> extension that gets the friendly name for the given
    /// <paramref name="enumValue" />.
    /// </summary>
    /// <param name="enumValue">
    /// The enum value for which we want to get the friendly name.
    /// </param>
    /// <returns>
    /// A <see langword="string" /> value equal to the <see cref="DisplayAttribute.Name" /> property
    /// if one was defined on the given <paramref name="enumValue" />. <br /> Otherwise, the
    /// <see langword="string" /> value that is returned from the <see cref="Enum.ToString()" />
    /// method.
    /// </returns>
    public static string GetFriendlyName(this Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        string enumName = enumValue.ToString();
        MemberInfo[] memberInfo = enumType.GetMember(enumName);

        if (memberInfo.Length > 0)
        {
            object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length > 0)
            {
                string? friendlyName = ((DisplayAttribute)attributes[0]).Name;

                if (friendlyName is not null)
                {
                    return friendlyName;
                }
            }
        }

        return enumName;
    }
}