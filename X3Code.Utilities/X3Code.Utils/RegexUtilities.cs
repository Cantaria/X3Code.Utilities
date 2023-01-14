using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using X3Code.Utils.Extensions;

namespace X3Code.Utils;

/// <summary>
/// Provides some regex helpers
/// </summary>
public static class RegexUtilities
{
    /// <summary>
    /// Checks via reges if the given email string is a valid email.
    /// <see cref="https://docs.microsoft.com/de-de/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format"/>
    /// <remarks>
    /// RegexMatchTimeoutException and ArgumentException will be eaten. Instead of throwing, this method will return false!
    /// </remarks>
    /// </summary>
    /// <param name="email">The email that should be validated</param>
    /// <returns></returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException )
        {
            return false;
        }
        catch (ArgumentException )
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Tries to replace the placeholder in the input string with data from the entity
    /// </summary>
    /// <param name="dataEntity">Source entity which contains the data for the placeholder</param>
    /// <param name="input">Input string with containing placeholder. Valid placeholder is: %property% </param>
    /// <param name="numberFormat">If the property is a number (decimal, float, double), it's possible to provide a format string</param>
    /// <param name="dateTimeFormat">If the property is a date, it's possible to provide a format string</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string FillPlaceholderFromEntity<T>(this T dataEntity, string input, string numberFormat = null, string dateTimeFormat = null) where T : class
    {
        var result = PlaceholderRegex.Replace(input, matches => {

            var name = matches.Groups["name"].Value;
            return dataEntity.TryReadPropertyAsString(name, numberFormat, dateTimeFormat);
        });

        return result;
    }
        
    private static readonly Regex PlaceholderRegex = new ("%(?<name>.+?)%");
}