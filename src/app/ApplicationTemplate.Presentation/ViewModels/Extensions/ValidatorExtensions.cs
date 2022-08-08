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
	/// Returns true if value is over 18 years olds.
	/// </summary>
	/// <typeparam name="T">Date of birth</typeparam>
	/// <param name="ruleBuilder"> Rule builder</param>
	/// <returns>Validation result.</returns>
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
	/// Returns true if value is over 18 years olds.
	/// </summary>
	/// <typeparam name="T">Date of birth</typeparam>
	/// <param name="ruleBuilder"> Rule builder</param>
	/// <returns>Validation result.</returns>
	public static IRuleBuilderOptions<T, string> MustBePhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		var charsToTrim = new char[] { '(', ')', '-', ' ' };

		return ruleBuilder.Must(phonenumber =>
		{
			phonenumber = phonenumber ?? string.Empty;

			var strippedNumber = new string(phonenumber
				.Where(x => x.ToString(CultureInfo.InvariantCulture).IsNumber() &&
					!x.ToString(CultureInfo.InvariantCulture).IsNullOrWhiteSpace())
				.ToArray());

			var trimmedNumber = phonenumber.Trim(charsToTrim);
			var result = strippedNumber.Length == 10 || strippedNumber.Length == 0; // 10 = "(000) 000-0000".Length - "() -".Length;

			return result;
		})
		.Must(phonenumber =>
		{
			phonenumber = phonenumber ?? string.Empty;
			var result = phonenumber
				.All(pn => pn.ToString(CultureInfo.InvariantCulture).IsNumber() ||
					pn == '(' || pn == ')' || pn == '-' || pn == ' ');
			return result;
		});
	}
}
