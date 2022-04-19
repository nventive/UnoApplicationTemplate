#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using NativeValue = System.Double;

namespace Nventive.View
{
	internal static class SizeExtensions
	{
		public static double GetOrientedValue(this Size size, Orientation orientation)
		{
			return (orientation == Orientation.Horizontal)
				? size.Width
				: size.Height;
		}

		public static Size AddOriented(this Size size, Size added, Orientation orientation)
		{
			return (orientation == Orientation.Horizontal)
				? new Size(size.Width + added.Width, Math.Max(size.Height, added.Height))
				: new Size(Math.Max(size.Width, added.Width), size.Height + added.Height);
		}

		public static Size ReplaceOrientedValue(this Size size, double newValue, Orientation orientation)
		{
			return (orientation == Orientation.Horizontal)
				? new Size(newValue, size.Height)
				: new Size(size.Width, newValue);
		}

		public static Size SubtractOriented(this Size size, Size subtracted, Orientation orientation, bool isClippedAtZero = true)
		{
			// Infinity minus something is infinity.
			return (orientation == Orientation.Horizontal)
				? new Size(Math.Max(size.Width - subtracted.Width, isClippedAtZero ? 0 : double.NegativeInfinity), size.Height)
				: new Size(size.Width, Math.Max(size.Height - subtracted.Height, isClippedAtZero ? 0 : double.NegativeInfinity));
		}

		public static Rect GetOrientedRect(this Size size, Size position, Orientation orientation)
		{
			return (orientation == Orientation.Horizontal)
				? new Rect(position.Width, 0, size.Width, size.Height)
				: new Rect(0, position.Height, size.Width, size.Height);
		}

		/// <summary>
		/// Gets an element's final rectangle based on its target size, the target position and space available, and its
		/// horizontal and vertical alignment. Those alignments can be null when layouting elements in an infinite direction.
		/// </summary>
		/// <param name="elementSize">The desired or calculated size of the element, but not yet adjusted with alignments.</param>
		/// <param name="x">The horizontal starting position of that element in its parent.</param>
		/// <param name="y">The vertical starting position of that element in its parent.</param>
		/// <param name="availableSize">The space reserved for that element.</param>
		/// <param name="horizontalAlignment">The horizontal alignment of that element, or null if it should not be adjusted horizontally.</param>
		/// <param name="verticalAlignment">The vertical alignment of that element, or null if it should not be adjusted vertically.</param>
		/// <returns>The area this element should cover.</returns>
		public static Rect GetFinalRect(this Size elementSize, NativeValue x, NativeValue y, Size availableSize, HorizontalAlignment? horizontalAlignment, VerticalAlignment? verticalAlignment)
		{
			NativeValue finalX;
			NativeValue finalY;
			NativeValue finalWidth;
			NativeValue finalHeight;

			if (horizontalAlignment.HasValue)
			{
				switch (horizontalAlignment.Value)
				{
					case HorizontalAlignment.Left:
						finalX = x;
						finalWidth = elementSize.Width;
						break;

					case HorizontalAlignment.Right:
						finalX = x
							+ ((availableSize.Width >= 0)
								? availableSize.Width - elementSize.Width
								: 0);
						finalWidth = elementSize.Width;
						break;

					case HorizontalAlignment.Center:
						finalX = x
							+ ((availableSize.Width >= 0)
								? (availableSize.Width - elementSize.Width) / 2
								: 0);
						finalWidth = elementSize.Width;
						break;

					case HorizontalAlignment.Stretch:
						finalX = x;
						finalWidth = (availableSize.Width >= 0)
							? availableSize.Width
							: elementSize.Width;
						break;

					default:
						throw new NotSupportedException("Unknown horizontal alignment.");
				}
			}
			else
			{
				finalX = x;
				finalWidth = elementSize.Width;
			}

			if (verticalAlignment.HasValue)
			{
				switch (verticalAlignment.Value)
				{
					case VerticalAlignment.Top:
						finalY = y;
						finalHeight = elementSize.Height;
						break;

					case VerticalAlignment.Bottom:
						finalY = y
							+ ((availableSize.Height >= 0)
								? availableSize.Height - elementSize.Height
								: 0);
						finalHeight = elementSize.Height;
						break;

					case VerticalAlignment.Center:
						finalY = y
							+ ((availableSize.Height >= 0)
								? (availableSize.Height - elementSize.Height) / 2
								: 0);
						finalHeight = elementSize.Height;
						break;

					case VerticalAlignment.Stretch:
						finalY = y;
						finalHeight = (availableSize.Height >= 0)
							? availableSize.Height
							: elementSize.Height;
						break;

					default:
						throw new NotSupportedException("Unknown vertical alignment.");
				}
			}
			else
			{
				finalY = y;
				finalHeight = elementSize.Height;
			}

			return new Rect(finalX, finalY, finalWidth, finalHeight);
		}
	}
}
#endif
