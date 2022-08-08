using System;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using FluentValidation;

namespace ApplicationTemplate.Presentation;

public class PostFormViewModel : ViewModel
{
	private readonly Post _post;

	public PostFormViewModel(Post post)
	{
		_post = post ?? new Post();

		this.AddValidation(this.GetProperty(x => x.Title));
		this.AddValidation(this.GetProperty(x => x.Body));
	}

	public string Title
	{
		get => this.Get(_post?.Title);
		set => this.Set(value);
	}

	public string Body
	{
		get => this.Get(_post?.Body);
		set => this.Set(value);
	}

	public Post GetPost()
	{
		return _post with
		{
			Title = Title,
			Body = Body
		};
	}
}

public class PostFormValidator : AbstractValidator<PostFormViewModel>
{
	public PostFormValidator()
	{
		RuleFor(x => x.Title).NotEmpty();
		RuleFor(x => x.Body).NotEmpty();
	}
}
