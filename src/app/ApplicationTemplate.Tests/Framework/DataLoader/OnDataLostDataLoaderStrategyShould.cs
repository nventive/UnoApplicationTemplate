using System;
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
			var cpt = 0;
			Action<object> mockAction = _ => cpt++;

			var sut = new OnDataLostDataLoaderStrategy(mockAction);
			sut.InnerStrategy = new MockDelegatingDataLoaderStrategy(() => Task.FromResult(new object()));

			// Act
			await sut.Load(CancellationToken.None, null);

			// Assert
			cpt.Should().Be(0);
		}

		[Fact]
		public async Task DisposePreviousData_When_LoadNewReference()
		{
			// Arrange
			var cpt = 0;
			Action<object> mockAction = _ => cpt++;

			var sut = new OnDataLostDataLoaderStrategy(mockAction);
			sut.InnerStrategy = new MockDelegatingDataLoaderStrategy(() => Task.FromResult(new object()));
			await sut.Load(CancellationToken.None, null);

			// Act
			await sut.Load(CancellationToken.None, null);

			// Assert
			cpt.Should().Be(1);
		}

		[Fact]
		public async Task DisposePreviousData_When_LoadNewValue()
		{
			// Arrange
			var cpt = 0;
			Action<object> mockAction = _ => cpt++;

			var value = new Random().Next(int.MaxValue);
			var sut = new OnDataLostDataLoaderStrategy(mockAction);
			sut.InnerStrategy = new MockDelegatingDataLoaderStrategy(async () => value);
			await sut.Load(CancellationToken.None, null);

			// Act
			await sut.Load(CancellationToken.None, null);

			// Assert
			cpt.Should().Be(1);
		}

		[Fact]
		public async Task NotDisposePreviousData_When_LoadSameReference()
		{
			// Arrange
			var cpt = 0;
			Action<object> mockAction = _ => cpt++;

			var instance = new object();
			var sut = new OnDataLostDataLoaderStrategy(mockAction);
			sut.InnerStrategy = new MockDelegatingDataLoaderStrategy(() => Task.FromResult(instance));
			await sut.Load(CancellationToken.None, null);

			// Act
			await sut.Load(CancellationToken.None, null);

			// Assert
			cpt.Should().Be(0);
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
	}
}
