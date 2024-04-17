﻿using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using MessageDialogService;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate.Presentation;

public class EditPostPageViewModel : ViewModel
{
	public EditPostPageViewModel(Post post = null)
	{
		IsNewPost = post is null;
		Title = post is null ? this.GetService<IStringLocalizer>()["EditPost_NewPost"] : post.Title;
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
				await this.GetService<IPostService>().Update(ct, post.Id, post);
			}
			else
			{
				await this.GetService<IPostService>().Create(ct, post);
			}

			await this.GetService<ISectionsNavigator>().NavigateBackOrCloseModal(ct);
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

		if (result is MessageDialogResult.Ok)
		{
			await this.GetService<ISectionsNavigator>().NavigateBackOrCloseModal(ct);
		}
	}
}
