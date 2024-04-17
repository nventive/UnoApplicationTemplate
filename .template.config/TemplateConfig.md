﻿# Template.config

## Special Files and Folders
There are several manipulations on the files of the template when packaging it and running it. Most files are copied as-is and modified by the renaming phase, but that's not true for everything.

### Template files
When running the template, the `.template.config/` folder is discarded.

### GitHub files
When running the template, everything related to GitHub is discarded.
- The `.github/` folder
- `CODE_OF_CONDUCT.md`
- `CONTRIBUTING.md`
- `CHANGELOG.md`
- `LICENSE`
- `.mergify.yml`

### READMEs
When packaging this template, the `README.md` (which describes the template) is renamed to `TEMPLATE_README.md` and `APP_README.md` is modified and renamed to `README.md`.
- `README.md` --> `TEMPLATE_README.md`
- `APP_README` --> _Injected with versioning information_ --> `README.md`

## Platform pragmas

- If you want pragma directives (e.g. `__ANDROID__`) to be in the output without being interpreted as a symbol, you need to use the following comment.

    ```csharp
    //-:cnd:noEmit
    #if __ANDROID__
    ...
    #endif
    //+:cnd:noEmit
    ```

- You can run `tools\escape-if-directives.linq` in linqpad to automatically escape #if directives.
- To remove the escape syntax, you use the search-and-replace feature of vscode:
  ```
  Search: //[-+]:cnd.+
  ^ with regex enabled
  Replace: **LEAVE_THIS_EMPTY**
  files to include: ./src/library/, *.cs
  ^ limits to cs files from src/library folder
  ```
  > note: you can use `collapse all` to confirm the affected files, before doing `replace all`.

## Pipeline conditionals
The pipelines also use some special conditionals.
The `.azure-pipelines.yml` has additional stages that are removed when running the template.
These conditions use the `#-if false` syntax and are configured in `.template.config/template.json`

## GUIDs

- You can generate/replace guids as described in this [github sample](https://github.com/dotnet/dotnet-template-samples/tree/master/14-guid).
  - For example, it is important for manifests to make sure generated apps have different identifiers.

## GitVersion

For the template itself, we use the **MainLine** mode for GitVersion. This is the same mode as most of our open source packages.
The configuration is located at `.template.config/build/gitversion-config.yml`.

For the generated app, we use the **ContinuousDeployment** mode for GitVersion.
The configuration is located at `build/gitversion-config.yml`.

## References

- [Creating custom templates](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)
- [Properties of template.json](https://github.com/dotnet/templating/wiki/Reference-for-template.json)
- [Comment syntax](https://github.com/dotnet/templating/wiki/Reference-for-comment-syntax)
- [Supported files](https://github.com/dotnet/templating/blob/5b5eb6278bd745149a57d0882d655b29d02c70f4/src/Microsoft.TemplateEngine.Orchestrator.RunnableProjects/SimpleConfigModel.cs#L387)
