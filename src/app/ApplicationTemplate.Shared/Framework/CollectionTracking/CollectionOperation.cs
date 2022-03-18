// Code imported from a library made by Jean-Philippe Lévesque. It will eventually be open-sourced on github.

using System.Collections.Generic;

namespace CollectionTracking
{
	public enum CollectionOperationType
	{
		Insert,
		Remove,
		Replace,
		Move,
		Equality
	}

	public class CollectionOperation<T>
	{
		public CollectionOperationType Type { get; internal set; }

		public IList<T> Items { get; internal set; }

		public T PreviousItem { get; internal set; }

		public int Index { get; internal set; }

		public int FromIndex { get; internal set; }

		public override string ToString()
		{
			switch (Type)
			{
				case CollectionOperationType.Insert:
					return $"Insert '{string.Join(",", Items)}' at [{Index}]";
				case CollectionOperationType.Move:
					return $"Move '{Items[0]}' from [{FromIndex}] to [{Index}]";
				case CollectionOperationType.Remove:
					return $"Remove '{string.Join(",", Items)}' from [{Index}]";
				case CollectionOperationType.Replace:
					return $"Replace '{PreviousItem}' by '{Items[0]}' at [{Index}]";
				case CollectionOperationType.Equality:
					return $"Equality of '{Items[0]}' at [{Index}]";
			}
			return base.ToString();
		}
	}
}
