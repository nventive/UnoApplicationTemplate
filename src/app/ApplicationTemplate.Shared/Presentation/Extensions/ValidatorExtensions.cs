using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation;
using Uno.Extensions;

namespace Presentation
{
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

		/// <summary>
		/// Returns true if the string is a valid email address.
		/// </summary>
		/// <typeparam name="T">email</typeparam>
		/// <param name="ruleBuilder"> Rule builder</param>
		/// <returns>Validation result.</returns>
		public static IRuleBuilderOptions<T, string> IsValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
		{
			return ruleBuilder.Must(email =>
			{
				if (string.IsNullOrWhiteSpace(email))
				{
					return false;
				}

				try
				{
					// Normalize the domain
					email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
										  RegexOptions.None, TimeSpan.FromMilliseconds(200));

					// Examines the domain part of the email and normalizes it.
					string DomainMapper(Match match)
					{
						// Use IdnMapping class to convert Unicode domain names.
						var idn = new IdnMapping();

						// Pull out and process domain name (throws ArgumentException on invalid)
						string domainName = idn.GetAscii(match.Groups[2].Value);

						return match.Groups[1].Value + domainName;
					}
				}
				catch (RegexMatchTimeoutException e)
				{
					return false;
				}
				catch (ArgumentException e)
				{
					return false;
				}

				try
				{
					return Regex.IsMatch(email,
						@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
						RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
				}
				catch (RegexMatchTimeoutException)
				{
					return false;
				}
			});
		}
	}
}
