using System;
using System.Runtime.CompilerServices;

namespace Chinook.DynamicMvvm;

public static class ChinookViewModelExtensionsForPropertiesFromDynamicProperty
{
	/// <summary>
	/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.<br/>
	/// Can be used with a static dynamic property so instances of a same class share a property.
	/// </summary>
	/// <typeparam name="T">The property type.</typeparam>
	/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
	/// <param name="source">The dynamic property that feeds the property.</param>
	/// <param name="name">The property's name.</param>
	/// <returns>The property's value.</returns>
	public static T GetFromDynamicProperty<T>(this IViewModel viewModel, IDynamicProperty<T> source, [CallerMemberName] string name = null)
	{
		ArgumentNullException.ThrowIfNull(source);

		return viewModel.Get<T>(viewModel.GetOrCreateDynamicProperty(name, n => new ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty<T>(name, source, viewModel, source.Value)));
	}

	/// <summary>
	/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.<br/>
	/// Can be used with a static dynamic property so instances of a same class share a property.
	/// </summary>
	/// <typeparam name="TSource">Type of the parent property.</typeparam>
	/// <typeparam name="TResult">Type of current property.</typeparam>
	/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
	/// <param name="source">The dynamic property that feeds the property.</param>
	/// <param name="selector">The selection operation to project <typeparamref name="TSource"/> into <typeparamref name="TResult"/>.</param>
	/// <param name="name">The property's name.</param>
	/// <returns>The property's value.</returns>
	public static TResult GetFromDynamicProperty<TSource, TResult>(this IViewModel viewModel, IDynamicProperty<TSource> source, Func<TSource, TResult> selector, [CallerMemberName] string name = null)
	{
		ArgumentNullException.ThrowIfNull(source);

		return viewModel.Get<TResult>(viewModel.GetOrCreateDynamicProperty(name, n =>
			new ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty<TSource, TResult>(name, source, viewModel, selector, selector(source.Value))));
	}
}
