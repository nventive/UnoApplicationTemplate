// Code imported from a library made by Jean-Philippe Lévesque. It will eventually be open-sourced on github.

//#define TESTING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicData;
using Uno.Extensions;

namespace CollectionTracking
{
	[Flags]
	public enum CollectionOperationOptions
	{
		/// <summary>
		/// All operations (Insert, Remove, Move, Replace) are permitted.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Replace operations are not permitter.
		/// </summary>
		NoReplace = 1,

		/// <summary>
		/// Move operations are not permitted.
		/// </summary>
		NoMove = 2,
	}

	public static class CollectionTrackingExtensions
	{
		/// <summary>
		/// Applies a collection operation on this <paramref name="list"/>.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">The list to modify.</param>
		/// <param name="operation">The operation to apply.</param>
		public static void ApplyOperation<T>(this IList<T> list, CollectionOperation<T> operation)
		{
			switch (operation.Type)
			{
				case CollectionOperationType.Insert:
					operation.Items.For((item, i) =>
					{
						list.Insert(operation.Index + i, item);
					});
					break;
				case CollectionOperationType.Remove:
					operation.Items.For((item, i) =>
					{
						list.RemoveAt(operation.Index);
					});
					break;
				case CollectionOperationType.Replace:
					list[operation.Index] = operation.Items.First();
					break;
				case CollectionOperationType.Move:
					list.Move(operation.FromIndex, operation.Index);
					break;
			}
		}

		private static void For<TSource>(this IEnumerable<TSource> enumerable, Action<TSource, int> action)
		{
			int cpt = 0;
			foreach (var val in enumerable)
			{
				action(val, cpt++);
			}
		}

		private static void Move<T>(this IList<T> list, int fromIndex, int toIndex)
		{
			var item = list[fromIndex];

			list.RemoveAt(fromIndex);
			list.Insert(toIndex, item);
		}

		private static int IndexOf<T>(this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer = null)
		{
			comparer = comparer ?? EqualityComparer<T>.Default;

			int index = 0;
			foreach (var i in enumerable)
			{
				if (comparer.Equals(i, item))
				{
					return index;
				}
				++index;
			}
			return -1;
		}

		/// <summary>
		/// Applies a series of collection operations on this <paramref name="list"/>.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">The list to modify.</param>
		/// <param name="operations">The operations to apply.</param>
		public static void ApplyOperations<T>(this IList<T> list, IEnumerable<CollectionOperation<T>> operations)
		{
			operations.ForEach(o => list.ApplyOperation(o));
		}

		/// <summary>
		/// Gets the operations (add, remove, replace, move) that have to be applied on <paramref name="source"/> to obtain <paramref name="targetCollection"/>.
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="source">The source collection.</param>
		/// <param name="targetCollection">the target collection.</param>
		/// <param name="comparer">The equality comparer.</param>
		/// <param name="options">The operations options.</param>
		public static IEnumerable<CollectionOperation<T>> GetOperations<T>(this IEnumerable<T> source, IEnumerable<T> targetCollection, IEqualityComparer<T> comparer = null, CollectionOperationOptions options = CollectionOperationOptions.Default)
		{
			bool isMoveDisabled = options.HasFlag(CollectionOperationOptions.NoMove);
			bool isReplaceDisabled = options.HasFlag(CollectionOperationOptions.NoReplace);
			comparer = comparer ?? EqualityComparer<T>.Default;

			List<CollectionOperation<T>> operations = new List<CollectionOperation<T>>();

			var sourceList = new LinkedList<T>(source);
			var targetList = new LinkedList<T>(targetCollection);

			var count = targetList.Count;
			int i;
			for (i = 0; i < count; i++)
			{
#if TESTING
				var testlist = source.ToList();
				string testTarget, testCurrent;
				bool equals = true;
				foreach (var operation in operations)
				{
					testlist.ApplyOperation(operation);
					testTarget = string.Join("", targetCollection);
					testCurrent = string.Join("", testlist);
					equals = testTarget.Take(i).SequenceEqual(testCurrent.Take(i));
				}
				if (!equals)
				{

				}
#endif

				var targetItem = targetList.First.Value;
				if (sourceList.First == null)
				{
					// If the source list is empty, add all items from the target list
					PositiveMoveOrAdd();
					continue;
				}
				var sourceItem = sourceList.First.Value;

				if (comparer.Equals(targetItem, sourceItem))
				{
					// If both heads are equals, move both heads
					targetList.RemoveFirst();
					sourceList.RemoveFirst();
#if TESTING
					operations.Add(new CollectionOperation<T>()
					{
						Type = CollectionOperationType.Equality,
						Index = i,
						Items = new List<T> { sourceItem },
					});
#endif
					continue;
				}

				var indexInTargetList = targetList.IndexOf(sourceItem, comparer);
				if (indexInTargetList == -1)
				{
					NegativeMoveOrRemove(sourceItem);
					continue;
				}

				var fromIndex = sourceList.IndexOf(targetItem, comparer);
				if (fromIndex != -1)
				{
					// If the element exists, it was moved

					// This condition is not optimal, but it's good enough. We could improve the algorithm by using Damerau-Levenshtein, but that would be a significant change.
					if ((indexInTargetList - i) > (fromIndex - i))
					{
						// Start an remove that will become a positive move.
						operations.Add(new CollectionOperation<T>()
						{
							Type = CollectionOperationType.Remove,
							Index = i,
							Items = new List<T> { sourceItem },
						});

						sourceList.Remove(sourceItem);
						--i;
						continue;
					}
					else
					{
						// Start an insert that will become a negative move.
						operations.Add(new CollectionOperation<T>()
						{
							Type = CollectionOperationType.Insert,
							Index = i,
							Items = new List<T> { targetItem },
						});

						targetList.RemoveFirst();
						continue;
					}
				}

				// Otherwise it was added (or moved)
				PositiveMoveOrAdd();

				void PositiveMoveOrAdd()
				{
					if (!TryPositiveMove())
					{
						// If it wasn't moved, it was added.
						operations.Add(new CollectionOperation<T>()
						{
							Type = CollectionOperationType.Insert,
							Index = i,
							Items = new List<T> { targetItem },
						});
						targetList.RemoveFirst();
					}
				}
				bool TryPositiveMove()
				{
					if (isMoveDisabled)
					{
						return false;
					}

					var addCount = 0;
					var removeCount = 0;
					for (int j = operations.Count - 1; j >= 0; --j)
					{
						var operation = operations[j];
						if (operation.Type == CollectionOperationType.Insert)
						{
							++addCount;
							continue;
						}

						if (operation.Type == CollectionOperationType.Remove)
						{
							if (comparer.Equals(operation.Items[0], targetItem))
							{
								operation.Type = CollectionOperationType.Move;
								operation.FromIndex = operation.Index;
								operation.Index = i - addCount + removeCount;
								targetList.RemoveFirst();
								return true;
							}
							else
							{
								++removeCount;
							}
						}
					}

					return false;
				}
			}

			if (sourceList.Count > 0)
			{
				// All remaining items in sourceList have been removed
				while (sourceList.Count > 0)
				{
					var sourceItem = sourceList.First.Value;
					NegativeMoveOrRemove(sourceItem);
					++i;
				}
			}

			void NegativeMoveOrRemove(T sourceItem)
			{
				if (!TryNegativeMove(sourceItem))
				{
					operations.Add(new CollectionOperation<T>()
					{
						Type = CollectionOperationType.Remove,
						Index = i,
						Items = new List<T> { sourceItem },
					});
				}
				sourceList.Remove(sourceItem);
				--i;
			}
			bool TryNegativeMove(T sourceItem)
			{
				if (isMoveDisabled)
				{
					return false;
				}

				var addCount = 0;
				var removeCount = 0;
				for (int j = operations.Count - 1; j >= 0; --j)
				{
					var operation = operations[j];
					if (operation.Type == CollectionOperationType.Remove)
					{
						++removeCount;
						continue;
					}

					if (operation.Type == CollectionOperationType.Insert)
					{
						++addCount;
						if (comparer.Equals(operation.Items[0], sourceItem))
						{
							operation.Type = CollectionOperationType.Move;
							operation.Index = operation.Index;
							operation.FromIndex = i + removeCount - addCount;
							return true;
						}
					}
				}

				return false;
			}

			// Group consecutive inserts and removes.
			for (int j = 0; j < operations.Count - 1; j++)
			{
				var operation = operations[j];
				var next = operations[j + 1];

#if TESTING
				if (operation.Type == CollectionOperationType.Equality)
				{
					operations.RemoveAt(j);
					--j;
				}
#endif

				if (operation.Type == next.Type
					&& ((operation.Type == CollectionOperationType.Insert && operation.Index + operation.Items.Count == next.Index)
					|| (operation.Type == CollectionOperationType.Remove && operation.Index == next.Index)))
				{
					operation.Items.AddRange(next.Items);
					operations.RemoveAt(j + 1);
					--j;
				}
				else if ((!isReplaceDisabled) && operation.Type == CollectionOperationType.Remove && next.Type == CollectionOperationType.Insert && operation.Index == next.Index && operation.Items.Count == 1 && next.Items.Count == 1)
				{
					operation.Type = CollectionOperationType.Replace;
					operation.PreviousItem = operation.Items[0];
					operation.Items = next.Items;
					operations.RemoveAt(j + 1);
				}
			}

#if TESTING
			if (operations.LastOrDefault()?.Type == CollectionOperationType.Equality)
			{
				operations.RemoveAt(operations.Count - 1);
			}
#endif

			return operations;
		}

		/// <summary>
		/// Creates a <see cref="IChangeSet{T}"/> from a series of <see cref="CollectionOperation{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collectionOperations">The collection operations.</param>
		/// <returns>A <see cref="IChangeSet{T}"/> equivalent to the provided <paramref name="collectionOperations"/>.</returns>
		public static IChangeSet<T> ToChangeSet<T>(this IEnumerable<CollectionOperation<T>> collectionOperations)
		{
			return new ChangeSet<T>(collectionOperations.Select(co => co.ToChange()));
		}

		/// <summary>
		/// Creates a <see cref="Change{T}"/> from a <see cref="CollectionOperation{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collectionOperation">The collection operation.</param>
		/// <returns>A <see cref="Change{T}"/> equivalent to the provided <paramref name="collectionOperation"/>.</returns>
		public static Change<T> ToChange<T>(this CollectionOperation<T> collectionOperation)
		{
			switch (collectionOperation.Type)
			{
				case CollectionOperationType.Insert:
					if (collectionOperation.Items.Count > 1)
					{
						return new Change<T>(ListChangeReason.AddRange, collectionOperation.Items, collectionOperation.Index);
					}
					else
					{
						return new Change<T>(ListChangeReason.Add, collectionOperation.Items.First(), collectionOperation.Index);
					}
				case CollectionOperationType.Remove:
					if (collectionOperation.Items.Count > 1)
					{
						return new Change<T>(ListChangeReason.RemoveRange, collectionOperation.Items, collectionOperation.Index);
					}
					else
					{
						return new Change<T>(ListChangeReason.Remove, collectionOperation.Items.First(), collectionOperation.Index);
					}
				case CollectionOperationType.Move:
					return new Change<T>(collectionOperation.Items.First(), collectionOperation.Index, collectionOperation.FromIndex);
				case CollectionOperationType.Replace:
					return new Change<T>(ListChangeReason.Replace, collectionOperation.Items.First(), collectionOperation.PreviousItem, collectionOperation.Index, collectionOperation.Index);
				default:
					throw new NotSupportedException();
			}
		}
	}
}
