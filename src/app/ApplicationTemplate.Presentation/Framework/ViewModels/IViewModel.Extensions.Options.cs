using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chinook.DynamicMvvm;

/// <summary>
/// This class exposes extensions methods on <see cref="IViewModel"/> related to the <see href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options">configuration options pattern</see>.
/// </summary>
public static class ChinookViewModelExtensionsForOptions
{
	/// <summary>
	/// Gets the current value of a <see cref="IOptionsMonitor{TOptions}"/>.
	/// </summary>
	/// <typeparam name="TOptions">The type of options.</typeparam>
	/// <param name="viewModel">The viewModel.</param>
	/// <returns>The current value of the options monitor.</returns>
	public static TOptions GetOptionsValue<TOptions>(this IViewModel viewModel)
		where TOptions : class, new()
	{
		return viewModel.GetService<IOptionsMonitor<TOptions>>().CurrentValue;
	}

	/// <summary>
	/// Gets an <see cref="IOptionsMonitor{TOptions}"/>.
	/// </summary>
	/// <typeparam name="TOptions">The type of options.</typeparam>
	/// <param name="viewModel">The viewModel.</param>
	/// <returns>The <see cref="IOptionsMonitor{TOptions}"/>.</returns>
	public static IOptionsMonitor<TOptions> GetOptionsMonitor<TOptions>(this IViewModel viewModel)
		where TOptions : class, new()
	{
		return viewModel.GetService<IOptionsMonitor<TOptions>>();
	}

	/// <summary>
	/// Gets a value from a <see cref="IDynamicProperty{T}"/> bound to an <see cref="IOptionsMonitor{TOptions}"/>.<br/>
	/// When the <see cref="IOptionsMonitor{TOptions}"/> detects a change, the underlying <see cref="IDynamicProperty{T}.Value"/> is updated.<br/>
	/// When the value of the underlying <see cref="IDynamicProperty{T}"/> is set, the <see cref="IConfiguration"/> is updated.
	/// </summary>
	/// <typeparam name="TOptions">The type of options for the <see cref="IOptionsMonitor{TOptions}"/>.</typeparam>
	/// <typeparam name="TValue">The type of the <see cref="IDynamicProperty{T}"/>.</typeparam>
	/// <param name="viewModel">The viewModel owning the property.</param>
	/// <param name="expression">The expression to resolve the property from the <typeparamref name="TOptions"/>.</param>
	/// <param name="valueToString">
	/// The optional value-to-string converter.
	/// This is used when setting the IConfiguration value in reaction to the dynamic property value changing.
	/// When not provided, <see cref="object.ToString"/> is used.
	/// </param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The value of the underlying generated <see cref="IDynamicProperty{T}"/>.</returns>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> must resolve a property on the <typeparamref name="TOptions"/> type.</exception>
	public static TValue GetFromOptionsMonitor<TOptions, TValue>(this IViewModel viewModel, Expression<Func<TOptions, TValue>> expression, Func<TValue, string> valueToString = null, [CallerMemberName] string propertyName = null)
		where TOptions : class, new()
	{
		return viewModel.Get<TValue>(viewModel.GetOrCreateDynamicProperty(propertyName, n =>
		{
			var selector = expression.Compile();
			valueToString ??= GetValueAsString;
			var optionPropertyName = GetOptionPropertyName(expression);
			var monitor = viewModel.GetOptionsMonitor<TOptions>();
			var property = viewModel.GetDynamicPropertyFactory().Create<TValue>(n, initialValue: selector(monitor.CurrentValue), viewModel);

			// Update the dynamic property when the option changes.
			viewModel.AddDisposable(
				propertyName + "MonitorOnChangeSubscription",
				monitor.OnChange((options, s) =>
				{
					property.Value = selector(options);
				})
			);

			// Update the config when the dynamic property is set.
			viewModel.AddDisposable(
				propertyName + "ValueChangedSubscription",
				property.Subscribe(dp =>
				{
					var monitorValue = selector(monitor.CurrentValue);
					if (!Equals(dp.Value, monitorValue))
					{
						var config = viewModel.GetService<IConfiguration>();
						var configName = $"{ApplicationTemplateConfigurationExtensions.DefaultOptionsName<TOptions>()}:{optionPropertyName}";
						config[configName] = valueToString((TValue)dp.Value);
					}
				})
			);

			return property;
		}));

		static string GetOptionPropertyName(Expression<Func<TOptions, TValue>> expression)
		{
			var sb = new StringBuilder();

			if (!(expression.Body is MemberExpression member))
			{
				throw new ArgumentException($"Expression '{expression}' doesn't refer to member property.");
			}

			var propInfo = member.Member as PropertyInfo;
			if (propInfo == null)
			{
				throw new ArgumentException($"Expression '{expression}' doesn't refer to a property.");
			}

			var innerExpression = member.Expression as MemberExpression;
			while (innerExpression != null)
			{
				sb.Append(innerExpression.Member.Name);
				sb.Append(':');
				innerExpression = innerExpression.Expression as MemberExpression;
			}

			sb.Append(propInfo.Name);

			return sb.ToString();
		}

		string GetValueAsString(TValue value)
		{
			if (value is not string && value is not bool)
			{
				viewModel.GetService<ILogger<TOptions>>().LogWarning("Serialization of type {TypeName} may be wrong. Consider using the valueToString parameter from the GetFromOptionsMonitor method.", typeof(TValue).Name);
			}

			return value.ToString();
		}
	}
}
