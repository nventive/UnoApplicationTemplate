using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ApplicationTemplate.Client;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate.Presentation
{
	public class HttpLogItemViewModel : ViewModel
	{
		public HttpLogItemViewModel(HttpLog log)
		{
			Data = log;
		}

		public HttpLog Data
		{
			get => this.Get<HttpLog>();
			set => this.Set(value);
		}

		public IImmutableList<(string Key, string Value)> RequestHeaders => Data.Request
			.Headers
			.Select(v => (v.Key, v.Value))
			.ToImmutableList();

		public IImmutableList<(string Key, string Value)> ResponseHeaders => Data.Response.Headers.Select(v => (v.Key, v.Value)).ToImmutableList();

		public bool IsRequestBodyVisible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public bool IsResponseBodyVisible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public bool IsResponseDetailsVisible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public bool IsRequestDetailsVisible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public IDynamicCommand SwitchResponseDetailsVisibility => this.GetCommandFromTask(async ct => IsResponseDetailsVisible = !IsResponseDetailsVisible);

		public IDynamicCommand SwitchRequestDetailsVisibility => this.GetCommandFromTask(async ct => IsRequestDetailsVisible = !IsRequestDetailsVisible);

		public IDynamicCommand ChangeRequestBodyVisibility => this.GetCommandFromTask(async ct => IsRequestBodyVisible = !IsRequestBodyVisible);

		public IDynamicCommand ChangeResponseBodyVisibility => this.GetCommandFromTask(async ct => IsResponseBodyVisible = !IsResponseBodyVisible);
	}
}
