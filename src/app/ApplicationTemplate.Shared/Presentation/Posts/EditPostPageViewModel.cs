﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using MessageDialogService;
using Microsoft.Extensions.Localization;
using Uno;

namespace ApplicationTemplate.Presentation
{
	public partial class EditPostPageViewModel : ViewModel
	{
		[Inject] private ISectionsNavigator _sectionsNavigator;
		[Inject] private IPostService _postService;

		public EditPostPageViewModel(PostData post = null)
		{
			IsNewPost = post == null;
			Title = post == null ? this.GetService<IStringLocalizer>()["EditPost_NewPost"] : post.Title;
			Form = this.AttachChild(new PostFormViewModel(post));

			this.RegisterBackHandler(OnBackRequested);
		}

		public string Title { get; }

		public bool IsNewPost { get; }

		public PostFormViewModel Form { get; }

		public IDynamicCommand Save => this.GetCommandFromTask(async ct =>
		{
			var validationResult = await Form.Validate(ct);

			if (validationResult.IsValid)
			{
				var post = Form.GetPost();

				if (post.Exists)
				{
					await _postService.Update(ct, post.Id, post);
				}
				else
				{
					await _postService.Create(ct, post);
				}

				await _sectionsNavigator.NavigateBackOrCloseModal(ct);
			}
		});

		public IDynamicCommand Cancel => this.GetCommandFromTask(async ct =>
		{
			await OnBackRequested(ct);
		});

		private async Task OnBackRequested(CancellationToken ct)
		{
			var result = await this.GetService<IMessageDialogService>()
				.ShowMessage(ct, mdb => mdb
					.Title("Warning")
					.Content("Are you sure you want to leave this page?")
					.OkCommand()
					.CancelCommand()
				);

			if (result == MessageDialogResult.Ok)
			{
				await _sectionsNavigator.NavigateBackOrCloseModal(ct);
			}
		}
	}
}
