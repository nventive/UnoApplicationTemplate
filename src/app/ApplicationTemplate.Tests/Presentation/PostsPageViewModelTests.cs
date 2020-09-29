using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class PostsPageViewModelTests : TestBase
	{
		[Fact(Skip = "This test is no longer valid because 'viewModel.Posts' is a DataLoader and not a dynamic property.")]
		public async Task It_Should_GetAll()
		{
			// TODO #172359: Find a cuter way to test the reactive properties.

			var viewModel = new PostsPageViewModel();
			var testObserver = new TestObserver<ImmutableList<PostData>>(2);

			var postsProperty = viewModel.GetProperty<ImmutableList<PostData>>(nameof(viewModel.Posts));
			postsProperty.GetAndObserve().Subscribe(testObserver);

			await testObserver.ExpectedNotifications;

			testObserver.Values[0].Should().BeNull();
			testObserver.Values[1].Should().NotBeEmpty();
		}

		private class TestObserver<T> : IObserver<T>
		{
			private readonly int _valuesCount;
			private readonly TaskCompletionSource<object> _taskCompletionSource;

			public TestObserver(int expectedNotifications)
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
