using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ApplicationTemplate.Views.Helpers
{
	public static class DependencyObjectExtensions
	{
		public static IEnumerable<DependencyObject> GetChildren(this DependencyObject obj)
		{
			var count = VisualTreeHelper.GetChildrenCount(obj);

			for (int i = 0; i < count; i++)
			{
				yield return VisualTreeHelper.GetChild(obj, i);
			}
		}

		public static T FindFirstChild<T>(this DependencyObject element, int? childLevelLimit = null, bool includeCurrent = true)
			where T :
//-:cnd:noEmit
#if HAS_UNO
//+:cnd:noEmit
			class,
//-:cnd:noEmit
#endif
//+:cnd:noEmit
			DependencyObject
		{
			return element.FindFirstChild<T>(x => true, childLevelLimit, includeCurrent);
		}

		public static T FindFirstChild<T>(this DependencyObject element, Func<T, bool> selector, int? childLevelLimit = null, bool includeCurrent = true)
			where T :
//-:cnd:noEmit
#if HAS_UNO
//+:cnd:noEmit
			class,
//-:cnd:noEmit
#endif
//+:cnd:noEmit
			DependencyObject
		{
			return InnerFindFirstChild(new[] { element }.Trim(), selector, childLevelLimit, includeCurrent);
		}

		private static T InnerFindFirstChild<T>(IEnumerable<DependencyObject> elements, Func<T, bool> selector, int? childLevelLimit, bool includeCurrentLevel)
			where T :
//-:cnd:noEmit
#if HAS_UNO
//+:cnd:noEmit
			class,
//-:cnd:noEmit
#endif
//+:cnd:noEmit
			DependencyObject
		{
			if (elements.None() || (childLevelLimit.HasValue && childLevelLimit <= 0))
			{
				return null;
			}
			else if (includeCurrentLevel)
			{
				return elements.OfType<T>().FirstOrDefault(selector)
					?? InnerFindFirstChild(elements.SelectMany(GetChildren), selector, childLevelLimit.HasValue ? childLevelLimit - 1 : null, true);
			}
			else
			{
				return InnerFindFirstChild(elements.SelectMany(GetChildren), selector, childLevelLimit.HasValue ? childLevelLimit - 1 : null, true);
			}
		}

		public static T FindFirstParent<T>(this DependencyObject element, bool includeCurrent = true)
			where T :
//-:cnd:noEmit
#if HAS_UNO
//+:cnd:noEmit
			class,
//-:cnd:noEmit
#endif
//+:cnd:noEmit
			DependencyObject
		{
			return element.GetParentHierarchy(includeCurrent).OfType<T>().FirstOrDefault();
		}

		public static T FindFirstParent<T>(this DependencyObject element, Func<T, bool> selector, bool includeCurrent = true)
			where T :
//-:cnd:noEmit
#if HAS_UNO
//+:cnd:noEmit
			class,
//-:cnd:noEmit
#endif
//+:cnd:noEmit
			DependencyObject
		{
			return element.GetParentHierarchy(includeCurrent).OfType<T>().FirstOrDefault(selector);
		}

		public static IEnumerable<DependencyObject> GetParentHierarchy(this DependencyObject element, bool includeCurrent = true)
		{
			if (includeCurrent)
			{
				yield return element;
			}

			for (var parent = (element as FrameworkElement).SelectOrDefault(e => e.Parent) ?? VisualTreeHelper.GetParent(element);
				 parent != null;
				 parent = VisualTreeHelper.GetParent(parent))
			{
				yield return parent;
			}
		}

		public static IEnumerable<T> FindAllChildren<T>(this DependencyObject element, int? childLevelLimit = null, bool includeCurrent = true)
			where T :
			DependencyObject
		{
			return element.FindAllChildren<T>(x => true, childLevelLimit, includeCurrent);
		}

		public static IEnumerable<T> FindAllChildren<T>(this DependencyObject element, Func<T, bool> selector, int? childLevelLimit = null, bool includeCurrent = true)
			where T :
			DependencyObject
		{
			return InnerFindAllChildren<T>(element, selector, childLevelLimit, includeCurrent);
		}

		// Remark : Could be even more optimal to use full yield with no recursion (ie.: Stack)
		private static IEnumerable<T> InnerFindAllChildren<T>(DependencyObject reference, Func<T, bool> selector, int? childLevelLimit, bool includeCurrentLevel)
			where T :
			DependencyObject
		{
			IEnumerable<T> innerChildren = null;
			// should we check the current object?
			if (includeCurrentLevel)
			{
				var current = (T)reference;
				// if object match desired type + selector
				if (current != null && selector(current))
				{
					innerChildren = Enumerable.Repeat(current, 1);
				}
			}
			// still have some more children to check?
			if (childLevelLimit.HasValue &&
				childLevelLimit <= 0)
			{
				return innerChildren ?? Enumerable.Empty<T>();
			}
			if (innerChildren == null)
			{
				innerChildren = Enumerable.Empty<T>();
			}
			// check how many children exist for current object, if no more, return what we found!
			var count = VisualTreeHelper.GetChildrenCount(reference);
			if (count == 0)
			{
				return innerChildren;
			}
			// get all the children of current object's children
			var otherChilds = reference
								.GetChildrenInternal(count)
								.SelectMany(child => InnerFindAllChildren(child, selector, childLevelLimit.HasValue ? childLevelLimit - 1 : null, true));
			return innerChildren.Concat(otherChilds);
		}

		private static IEnumerable<DependencyObject> GetChildrenInternal(this DependencyObject reference, int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return VisualTreeHelper.GetChild(reference, i);
			}
		}
	}
}
