using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Presentation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Content.Diagnostics;

/// <summary>
/// This view shows the current Diagnostics tab.
/// It maps ViewModels to the corresponding view.
/// </summary>
public sealed partial class DiagnosticsTabPresenter : ContentControl
{
	public TabViewModel ViewModel
	{
		get { return (TabViewModel)GetValue(ViewModelProperty); }
		set { SetValue(ViewModelProperty, value); }
	}

	public static readonly DependencyProperty ViewModelProperty =
		DependencyProperty.Register("ViewModel", typeof(TabViewModel), typeof(DiagnosticsTabPresenter), new PropertyMetadata(null, OnViewModelChanged));

	private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var that = (DiagnosticsTabPresenter)d;
		that.Content = e.NewValue switch
		{
			HttpDebuggerViewModel => new HttpDebuggerView() { DataContext = e.NewValue },
			NavigationDebuggerViewModel => new NavigationDebuggerView() { DataContext = e.NewValue },
			_ => new Border(),
		};
	}
}
