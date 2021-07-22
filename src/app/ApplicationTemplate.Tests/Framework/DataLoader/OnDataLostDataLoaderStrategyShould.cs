using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DataLoader;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests.Framework
{
	public class OnDataLostDataLoaderStrategyShould
	{
		[Fact]
		public async Task NotDisposePreviousData_When_FirstLoad()
		{
			// Arrange
			var methodCount = 0;
			Action<object> mockAction = data => methodCount++;

			var sut = new OnDataLostDataLoaderStrategy(mockAction);
			sut.InnerStrategy = new MockDelegatingDataLoaderStrategy(() => Task.FromResult(new object()));

			// Act
			await sut.Load(CancellationToken.None, null);

			// Assert
			methodCount.Should().Be(0);
		}

		[Theory]
		[ClassData(typeof(OnDataLostDataLoaderStrategyTestData))]
		public async Task DisposePreviousData_If_LoadReturnsNewReference(object firstLoad, object secondLoad, int expectedMethodCount)
		{
			// Arrange
			var methodCount = 0;
			var isFirstLoad = true;
			Action<object> mockAction = data => methodCount++;

			var sut = new OnDataLostDataLoaderStrategy(mockAction);
			sut.InnerStrategy = new MockDelegatingDataLoaderStrategy(() =>
			{
				var result = isFirstLoad ? firstLoad : secondLoad;
				return Task.FromResult(result);
			});
			await sut.Load(CancellationToken.None, null);
			isFirstLoad = false;

			// Act
			await sut.Load(CancellationToken.None, null);

			// Assert
			methodCount.Should().Be(expectedMethodCount);
		}

		public class MockDelegatingDataLoaderStrategy : DelegatingDataLoaderStrategy
		{
			private Func<Task<object>> _innerFunc;

			public MockDelegatingDataLoaderStrategy(Func<Task<object>> innerFunc)
			{
				_innerFunc = innerFunc;
			}

			public override async Task<object> Load(CancellationToken ct, IDataLoaderRequest request)
			{
				return await _innerFunc();
			}
		}

		public class OnDataLostDataLoaderStrategyTestData : IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
			{
				var list = new List<object[]>();

				var value = new Random().Next(int.MaxValue);
				var instance1 = new object();
				var instance2 = new object();

				list.Add(new object[] { instance1, instance2, 1 }); // Different reference, will call dispose.
				list.Add(new object[] { instance1, instance1, 0 }); // Same reference, won't call dispose.
				list.Add(new object[] { value, value, 1 }); // Same value, but will still call dispose.

				return list.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
