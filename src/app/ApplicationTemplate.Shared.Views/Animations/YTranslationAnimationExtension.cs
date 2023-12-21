using System;
using DynamicData;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace ApplicationTemplate.Views.Animations
{
	public class YTranslationAnimationExtension
	{
		public static readonly DependencyProperty AnimationTriggerProperty =
			DependencyProperty.RegisterAttached("AnimationTrigger", typeof(object), typeof(YTranslationAnimationExtension), new PropertyMetadata(default, OnAnimationTriggered));

		public static object GetAnimationTrigger(FrameworkElement element)
		{
			return element.GetValue(AnimationTriggerProperty);
		}

		public static void SetAnimationTrigger(FrameworkElement element, object obj)
		{
			element.SetValue(DelayTriggerProperty, obj);
		}

		public static readonly DependencyProperty DelayTriggerProperty =
			DependencyProperty.RegisterAttached("DelayTrigger", typeof(TimeSpan), typeof(YTranslationAnimationExtension), new PropertyMetadata(default));

		public static TimeSpan GetDelayTrigger(FrameworkElement element)
		{
			var delay = element.GetValue(DelayTriggerProperty);
			return delay == null ? default : (TimeSpan)delay;
		}

		public static void SetDelayTrigger(FrameworkElement element, TimeSpan time)
		{
			element.SetValue(DelayTriggerProperty, time);
		}

		public static readonly DependencyProperty AnimationDurationProperty =
			DependencyProperty.RegisterAttached("AnimationDuration", typeof(TimeSpan), typeof(YTranslationAnimationExtension), new PropertyMetadata(default));

		public static TimeSpan GetAnimationDuration(FrameworkElement element)
		{
			var duration = element.GetValue(AnimationDurationProperty);
			return duration == null ? default : (TimeSpan)duration;
		}

		public static void SetAnimationDuration(FrameworkElement element, TimeSpan time)
		{
			element.SetValue(AnimationDurationProperty, time);
		}

		private static void OnAnimationTriggered(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d == null) return;

			var element = d as FrameworkElement;

			CompositeTransform compositeTransform = new CompositeTransform() { TranslateY = 100 };
			element.RenderTransform = compositeTransform;

			// Create the DoubleAnimation for the Y-axis translation
			DoubleAnimation translateYAnimation = new DoubleAnimation
			{
				To = 0,
				Duration = GetAnimationDuration(element),
				BeginTime = GetDelayTrigger(element)
			};
			DoubleAnimation ScaleYAnimation = new DoubleAnimation
			{
				To = 1.1,
				Duration = TimeSpan.FromSeconds(0.5),
				BeginTime = TimeSpan.FromSeconds(2),
				AutoReverse = true
			};
			DoubleAnimation ScaleXAnimation = new DoubleAnimation
			{
				To = 1.2,
				Duration = TimeSpan.FromSeconds(0.5),
				BeginTime = TimeSpan.FromSeconds(2),
				AutoReverse = true
			};
			DoubleAnimation CenterYAnimation = new DoubleAnimation
			{
				To = element.ActualHeight / 2,
				Duration = TimeSpan.FromSeconds(0.5),
				BeginTime = TimeSpan.FromSeconds(2),
				AutoReverse = true
			};
			DoubleAnimation CenterXAnimation = new DoubleAnimation
			{
				To = element.ActualWidth / 2,
				Duration = TimeSpan.FromSeconds(0.5),
				BeginTime = TimeSpan.FromSeconds(2),
				AutoReverse = true
			};

			Storyboard storyboard = new Storyboard();

			// Set the target property and target element for the animation
			Storyboard.SetTarget(translateYAnimation, compositeTransform);
			Storyboard.SetTarget(ScaleXAnimation, compositeTransform);
			Storyboard.SetTarget(ScaleYAnimation, compositeTransform);
			Storyboard.SetTarget(CenterYAnimation, compositeTransform);
			Storyboard.SetTarget(CenterXAnimation, compositeTransform);

			Storyboard.SetTargetProperty(translateYAnimation, "TranslateY");
			Storyboard.SetTargetProperty(ScaleXAnimation, "ScaleX");
			Storyboard.SetTargetProperty(ScaleYAnimation, "ScaleY");
			Storyboard.SetTargetProperty(CenterYAnimation, "CenterY");
			Storyboard.SetTargetProperty(CenterXAnimation, "CenterX");

			// Check if the element already has a RenderTransform, otherwise create one
			if (element.RenderTransform == null)
			{
				element.RenderTransform = new TranslateTransform();
			}

			storyboard.Children.Add(translateYAnimation);
			storyboard.Children.Add(ScaleXAnimation);
			storyboard.Children.Add(ScaleYAnimation);
			storyboard.Children.Add(CenterYAnimation);
			storyboard.Children.Add(CenterXAnimation);

			// Begin the animation
			storyboard.Begin();
			//------------------------------------------------------------------------------------------------------------------------------------------
			//DoubleAnimation bounceYAnimation = new DoubleAnimation
			//{
			//	To = -5,
			//	Duration = new Duration(TimeSpan.FromSeconds(1)),
			//	BeginTime = TimeSpan.FromSeconds(3),
			//	EasingFunction = new BounceEase() { Bounces = 1, Bounciness = 1 },
			//	AutoReverse = true,
			//};

			//// Create a Storyboard and add the animation to it
			//Storyboard storyboard2 = new Storyboard();
			//storyboard2.Children.Add(bounceYAnimation);

			//// Set the target property and target element for the animation
			//Storyboard.SetTarget(bounceYAnimation, element);
			//Storyboard.SetTargetProperty(bounceYAnimation, "UIElement.RenderTransform.(TranslateTransform.Y)");

			//// Check if the element already has a RenderTransform, otherwise create one
			//if (element.RenderTransform == null || element.RenderTransform.GetType() != typeof(TranslateTransform))
			//{
			//	element.RenderTransform = new TranslateTransform();
			//}

			//// Begin the animation
			//storyboard2.Begin();
		}
	}
}
