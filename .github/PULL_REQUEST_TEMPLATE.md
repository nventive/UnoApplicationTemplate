GitHub Issue: #

## Proposed Changes
<!-- Please check one or more that apply to this PR. -->

 - [ ] Bug fix
 - [ ] Feature
 - [ ] Code style update (formatting)
 - [ ] Refactoring (no functional changes, no api changes)
 - [ ] Build or CI related changes
 - [ ] Documentation content changes
 - [ ] Other, please describe:

## Description

<!-- (Please describe the changes that this PR introduces.) -->


## Impact on version
<!-- Please select one or more based on your commits. -->

- [ ] **Major**
  - The template structure was changed.
- [ ] **Minor**
  - New functionalities were added.
- [ ] **Patch**
  - A bug in behavior was fixed.
  - Documentation was changed.

## PR Checklist 

### Always applicable
No matter your changes, these checks always apply.
- [ ] Your conventional commits are aligned with the **Impact on version** section.
- [ ] Updated [CHANGELOG.md](../CHANGELOG.md).
  - Use the latest Major.Minor.X header if you do a **Patch** change.
  - Create a new Major.Minor.X header if you do a **Minor** or **Major** change.
  - If you create a new header, it aligns with the **Impact on version** section and matches what is generated in the build pipeline.
- [ ] Documentation files were updated according with the changes.
  - Update `README.md` and `TemplateConfig.md` if you made changes to **templating**.
  - Update `AzurePipelines.md` and `APP_README.md` if you made changes to **pipelines**.
  - Update `Diagnostics.md` if you made changes to **diagnostic tools**.
  - Update `Architecture.md` and its diagrams if you made **architecture decisions** or if you introduced new **recipes**.
  - ...and so forth: Make sure you update the documentation files associated to the recipes you changed. Review the topics by looking at the content of the `doc/` folder.
- [ ] Images about the template are referenced from the [wiki](https://github.com/nventive/UnoApplicationTemplate/wiki/Images) and added as images in this git.
- [ ] TODO comments are hints for next steps for users of the template and not planned work.

### Contextual
Based on your changes these checks may not apply.
- [ ] Automated tests for the changes have been added/updated.
- [ ] Tested on all relevant platforms

## Other information

<!-- Please provide any additional information if necessary -->

## Internal Issue (If applicable):
<!-- Link to relevant internal issue if applicable. All PRs should be associated with an issue (GitHub issue or internal) -->