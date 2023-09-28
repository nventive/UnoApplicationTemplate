using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationTemplateHttpClientBuilderExtensions
{
	/// <summary>
	/// Adds a <see cref="HttpMessageHandler"/> to the <see cref="HttpClient"/> pipeline only when <see paramref="addHandler"/> is true.
	/// </summary>
	/// <typeparam name="THandler">The type of <see cref="DelegatingHandler"/>.</typeparam>
	/// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
	/// <param name="addHandler">The condition deciding whether the <typeparamref name="THandler"/> should be added.</param>
	/// <returns>The provided <paramref name="builder"/>.</returns>
	public static IHttpClientBuilder AddConditionalHttpMessageHandler<THandler>(this IHttpClientBuilder builder, bool addHandler)
		where THandler : DelegatingHandler
	{
		if (addHandler)
		{
			return builder.AddHttpMessageHandler<THandler>();
		}

		return builder;
	}
}
