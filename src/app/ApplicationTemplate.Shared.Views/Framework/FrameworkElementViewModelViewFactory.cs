using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using Windows.UI.Xaml;

namespace ApplicationTemplate.Views
{
	/// <summary>
	/// This implementation of <see cref="IViewModelViewFactory"/> uses the <see cref="ViewModelView"/> implementation.
	/// </summary>
	public class FrameworkElementViewModelViewFactory : IViewModelViewFactory
	{
		public IViewModelView Create(object view)
		{
			return new ViewModelView((FrameworkElement)view);
		}
	}
}
