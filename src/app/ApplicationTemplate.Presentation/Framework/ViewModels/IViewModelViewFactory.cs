using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate
{
	/// <summary>
	/// This factory is used to abstract the creation of <see cref="IViewModelView"/> objects.
	/// </summary>
	public interface IViewModelViewFactory
	{
		/// <summary>
		/// Creates a new <see cref="IViewModelView"/> using the provided <paramref name="view"/> reference.
		/// </summary>
		/// <param name="view">The native view object.</param>
		/// <returns>A new <see cref="IViewModelView"/> instance.</returns>
		IViewModelView Create(object view);
	}
}
