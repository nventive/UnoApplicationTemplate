using System;
using Chinook.DynamicMvvm.Implementations;

namespace Chinook.DynamicMvvm;

/// <summary>
/// This is an implementation of a <see cref="IDynamicProperty"/> using a <see cref="IDynamicProperty"/> that ensures <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
/// </summary>
public class ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty : ValueChangedOnBackgroundTaskDynamicProperty
{
	private readonly IDynamicProperty _source;
	private readonly Func<object, object> _selector;
	private bool _isDisposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty"/> class.
	/// </summary>
	/// <param name="name">The name of the this property.</param>
	/// <param name="source">Source.</param>
	/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
	/// <param name="selector">The optional projection. Setting <see cref="Value"/> is not supported when providing this parameter.</param>
	/// <param name="initialValue">The initial value of this property.</param>
	public ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty(string name, IDynamicProperty source, IViewModel viewModel, Func<object, object> selector, object initialValue)
		: base(name, viewModel, initialValue)
	{
		_source = source;
		_selector = selector;
		_source.ValueChanged += OnSourceValueChanged;
	}

	public override object Value
	{
		get => base.Value;
		set
		{
			ObjectDisposedException.ThrowIf(_isDisposed, this);

			if (!Equals(value, base.Value))
			{
				if (_selector == null)
				{
					//Set the source only if there's no projection.
					_source.Value = value;
				}

				base.Value = value;
			}
		}
	}

	/// <inheritdoc />
	protected override void Dispose(bool isDisposing)
	{
		if (_isDisposed)
		{
			return;
		}

		if (isDisposing && _source != null)
		{
			_source.ValueChanged -= OnSourceValueChanged;
		}

		_isDisposed = true;

		base.Dispose(isDisposing);
	}

	private void OnSourceValueChanged(IDynamicProperty property)
	{
		if (_selector == null)
		{
			Value = property.Value;
		}
		else
		{
			Value = _selector(property.Value);
		}
	}
}

/// <summary>
/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using a <see cref="IDynamicProperty{T}"/> that ensures <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
/// </summary>
/// <typeparam name="T">The type of value.</typeparam>
public class ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty<T> : ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty, IDynamicProperty<T>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty{T}"/> class.
	/// </summary>
	/// <param name="name">The name of the this property.</param>
	/// <param name="source">Source.</param>
	/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
	/// <param name="initialValue">The initial value of this property.</param>
	public ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty(string name, IDynamicProperty<T> source, IViewModel viewModel, T initialValue = default)
		: base(name, source, viewModel, null, initialValue)
	{
	}

	/// <inheritdoc />
	public new T Value
	{
		get => (T)base.Value;
		set => base.Value = value;
	}
}

/// <summary>
/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using a <see cref="IDynamicProperty{T}"/> that ensures <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
/// </summary>
/// <typeparam name="TSource">Type of the parent property.</typeparam>
/// <typeparam name="TResult">Type of current property.</typeparam>
public class ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty<TSource, TResult> : ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty, IDynamicProperty<TResult>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty{TSource, TResult}"/> class.
	/// </summary>
	/// <param name="name">The name of the this property.</param>
	/// <param name="source">Source.</param>
	/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
	/// <param name="selector">The optional projection. Setting <see cref="Value"/> is not supported when providing this parameter.</param>
	/// <param name="initialValue">The initial value of this property.</param>
	public ValueChangedOnBackgroundTaskDynamicPropertyFromDynamicProperty(string name, IDynamicProperty<TSource> source, IViewModel viewModel, Func<TSource, TResult> selector, TResult initialValue)
		: base(name, source, viewModel, x => selector((TSource)x), initialValue)
	{
	}

	/// <inheritdoc />
	public new TResult Value
	{
		get => (TResult)base.Value;
		set => base.Value = value;
	}
}
