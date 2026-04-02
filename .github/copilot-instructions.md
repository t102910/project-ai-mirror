# QolmsJotoWebView Working Instructions

## Purpose
- The overall goal of this repository is screen migration from QolmsYappli to QolmsJotoWebView.
- The current target is QolmsJotoWebView.
- QolmsYappli is reference-only.
- HTML mocks provided by the design owner are the source of truth for screen structure and appearance.
- API connections, DB connections, repositories, workers, and real data integration are out of scope until the mock screen is established.

## Core Working Rules
- Do not assert by guess.
- State only facts confirmed in code or files.
- If inference is included, mark it clearly as 要確認 or 推測.
- Before changing code, first identify target files, change policy, and impact scope unless the user explicitly asks for immediate implementation.
- If the user is asking for planning only, do not edit code.
- Prefer the provided HTML mock over existing Razor structure when they differ.
- Keep changes as small as possible.
- Respect existing responsibility boundaries and avoid unnecessary large refactors.
- Reuse existing implementation patterns in QolmsJotoWebView whenever possible.
- Always show referenced file paths in explanations.

## Naming And Namespace Rules
- Match namespaces to existing QolmsJotoWebView files in the same layer and folder.
- Match prefixes and suffixes to neighboring classes of the same type.
- Do not rename existing classes broadly just because Qy should be read as Qj.
- If proposing a new naming style, explain the reason.
- Before deciding names, inspect surrounding files and base the choice on those files.
- Recheck namespace, prefix, suffix, and related class consistency before finishing.

## Controller And Action Renaming Rules
- Controller names and action names may be changed when appropriate for screen migration.
- If changing them, always provide:
  - old name
  - new name
  - reason
  - impact scope including routing, views, callers, and related links
- Prefer names that are explicit at feature level.
- Avoid overly generic names.
- If choosing between integrating into an existing controller and creating a new controller, explain the reason.

## Screen Migration Policy
- Proceed one screen at a time.
- Identify the corresponding old implementation in QolmsYappli when needed.
- Decide the destination placement in QolmsJotoWebView based on existing structure.
- Build or adjust Razor Views from the HTML mock.
- Use dummy data in the controller or view model.
- Keep CSS and JS changes minimal.
- API integration comes later.

## Mock URL List Is The Tracking Baseline
- Use the user-provided mock URL list as the baseline for progress tracking.
- When asked what screens remain, compare the list against actual controllers and views in this repository.

## Persistent Instruction Handling
- When the user gives instructions that are important for future work, update this instruction file.
- Treat this file as the repository-level source for ongoing project rules.
- Do not create ad hoc summaries elsewhere unless the user asks for them.