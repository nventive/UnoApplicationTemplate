using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate
{
	public class ChuckNorrisItemViewModel : ViewModel
	{
		public ChuckNorrisItemViewModel(IViewModel parent, ChuckNorrisQuote quote)
		{
			Parent = parent;
			Quote = quote;
		}

		public IViewModel Parent { get; }

		public ChuckNorrisQuote Quote { get; }

		public bool IsFavorite
		{
			get => this.Get(initialValue: Quote.IsFavorite);
			set => this.Set(value);
		}
	}
}
