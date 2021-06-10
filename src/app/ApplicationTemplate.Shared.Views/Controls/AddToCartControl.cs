using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Controls
{
	[TemplateVisualState(Name = "Normal", GroupName = "VisualStates")]
	[TemplateVisualState(Name = "QuantityPicker", GroupName = "VisualStates")]
	public partial class AddToCartControl : Control
	{

		public AddToCartControl()
		{
			DefaultStyleKey = typeof(AddToCartControl);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// TODO Aller chercher les PART

			var addToCartButton = this.GetTemplateChild("PART_AddToCartButton") as Button;

			addToCartButton.Click += OnAddToCartClick;
		}

		private void OnAddToCartClick(object sender, RoutedEventArgs e)
		{

			VisualStateManager.GoToState(this, "QuantityPicker", useTransitions: true);

			// TODO dissapear

			// TODO call AddToCartCommand
		}

		public ICommand AddToCartCommand
		{
			get { return (ICommand)GetValue(AddToCartCommandProperty); }
			set { SetValue(AddToCartCommandProperty, value); }
		}

		public static readonly DependencyProperty AddToCartCommandProperty =
			DependencyProperty.Register("AddToCartCommand", typeof(ICommand), typeof(AddToCartControl), new PropertyMetadata(default(ICommand)));



		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}

		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(int), typeof(AddToCartControl), new PropertyMetadata(0, (o,e) => OnQuantityChanged(o, e)));

		private static void OnQuantityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var control = (AddToCartControl)o;

			// adapt to quantity changing
		}
	}
}
