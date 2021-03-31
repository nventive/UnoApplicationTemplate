using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationTemplate.Tests
{
	public partial class PostsPageViewModelTests
	{
		private class TestListObserver<T> : IObserver<T>
		{
			private readonly int _valuesCount;
			private readonly TaskCompletionSource<object> _taskCompletionSource;

			public TestListObserver(int expectedNotifications)
			{
				_valuesCount = expectedNotifications;
				_taskCompletionSource = new TaskCompletionSource<object>();
			}

			public bool IsCompleted { get; private set; }

			public Exception Error { get; private set; }

			public IList<T> Values { get; } = new List<T>();

			public Task ExpectedNotifications => _taskCompletionSource.Task;

			public void OnCompleted()
			{
				IsCompleted = true;
			}

			public void OnError(Exception error)
			{
				Error = error;
			}

			public void OnNext(T value)
			{
				Values.Add(value);

				if (Values.Count >= _valuesCount)
				{
					_taskCompletionSource.SetResult(null);
				}
			}
		}
	}
}
