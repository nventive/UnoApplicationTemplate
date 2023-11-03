using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FluentValidation;
using Uno.Extensions;

namespace Presentation;

public static class ValidatorExtensions
{
	/// <summary>
	/// Adds a validation rule that checks whether the age is over 18 years old based on the date of birth.
	/// </summary>
	/// <typeparam name="T">The type containing the property to validate.</typeparam>
	/// <param name="ruleBuilder">The rule builder.</param>
	/// <returns>The modified rule builder.</returns>
	public static IRuleBuilderOptions<T, DateTimeOffset> MustBe18OrOlder<T>(this IRuleBuilder<T, DateTimeOffset> ruleBuilder)
	{
		return ruleBuilder.Must(dateOfBirth =>
		{
			var diff = DateTimeOffset.Now.Year - dateOfBirth.DateTime.Year;
			if (diff == 18)
			{
				if (DateTimeOffset.Now.Month == dateOfBirth.DateTime.Month)
				{
					return DateTimeOffset.Now.Day >= dateOfBirth.DateTime.Day;
				}
				else
				{
					return DateTimeOffset.Now.Month > dateOfBirth.DateTime.Month;
				}
			}
			else
			{
				return diff > 18;
			}
		});
	}

	/// <summary>
	/// Adds a validation rule that checks whether a string is a phone number.
	/// </summary>
	/// <typeparam name="T">The type containing the property to validate.</typeparam>
	/// <param name="ruleBuilder">The rule builder.</param>
	/// <returns>The modified rule builder.</returns>
	public static IRuleBuilderOptions<T, string> MustBePhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		var charsToTrim = new char[] { '(', ')', '-', ' ' };

		return ruleBuilder.Must(phoneNumber =>
		{
			phoneNumber = phoneNumber ?? string.Empty;

			var strippedNumber = new string(phoneNumber
				.Where(x => x.ToString(CultureInfo.InvariantCulture).IsNumber() &&
					!x.ToString(CultureInfo.InvariantCulture).IsNullOrWhiteSpace())
				.ToArray());

			var trimmedNumber = phoneNumber.Trim(charsToTrim);
			var result = strippedNumber.Length == 10 || strippedNumber.Length == 0; // 10 = "(000) 000-0000".Length - "() -".Length;

			return result;
		})
		.Must(phoneNumber =>
		{
			phoneNumber = phoneNumber ?? string.Empty;
			var result = phoneNumber
				.All(pn => pn.ToString(CultureInfo.InvariantCulture).IsNumber() ||
					pn == '(' || pn == ')' || pn == '-' || pn == ' ');
			return result;
		});
	}
}
