# Contributing to [LocalStack.NET Client](https://github.com/localstack-dotnet/localstack-dotnet-client)

All kind of pull requests even for things like typos, documentation, test cases, etc are always welcome. By submitting a pull request for this project, you agree to license your contribution under the MIT license to this project.

Please read these guidelines before contributing to LocalStack.NET Client:

 - [Question or Problem?](#question)
 - [Issues and Bugs](#issue)
 - [Feature Requests](#feature)
 - [Submitting a Pull Request](#submit-pull-request)
    - [Getting Started](#getting-started)
    - [Pull Requests](#pull-requests)

## <a name="question"></a> Got a Question or Problem?

If you have questions about how to use LocalStack.NET Client, you can ask by submitting an issue to the [GitHub Repository][github]

## <a name="issue"></a> Found an Issue?

If you find a bug in the source code or a mistake in the documentation, you can help by
submitting an issue to the [GitHub Repository][github]. Even better you can submit a Pull Request
with a fix.

When submitting an issue please include the following information:

- A description of the issue
- The exception message and stacktrace if an error was thrown
- If possible, please include code that reproduces the issue. [DropBox][dropbox] or GitHub's
[Gist][gist] can be used to share large code samples, or you could
[submit a pull request](#pullrequest) with the issue reproduced in a new test.

The more information you include about the issue, the more likely it is to be fixed!


## <a name="feature"></a> Want a Feature?

You can request a new feature by submitting an issue to the [GitHub Repository][github]

## <a name="submit-pull-request"></a> Submitting a Pull Request

Good pull requests, patches, improvements and new features are a fantastic
help. They should remain focused in scope and avoid containing unrelatedcahe
commits. When submitting a pull request to the [GitHub Repository][github] make sure to do the following:

- Check that new and updated code follows LocalStack.NET Client's existing code formatting and naming standard
- Run LocalStack.NET Client's unit tests to ensure no existing functionality has been affected
- Write new unit tests to test your changes. All features and fixed bugs must have tests to verify
they work

Read [GitHub Help][pullrequesthelp] for more details about creating pull requests.

### <a name="getting-started"></a> Getting Started

-   Make sure you have a [GitHub account](https://github.com/signup/free)
-   Submit a ticket for your issue, assuming one does not already exist.
    -   Clearly describe the issue including steps to reproduce the bug.
-   Fork the repository on GitHub

### <a name="pull-requests"></a> Pull requests

Adhering to the following process is the best way to get your work
included in the project:

1. [Fork](http://help.github.com/fork-a-repo/) the project, clone your fork,
   and configure the remotes:

   ```bash
   # Clone your fork of the repo into the current directory
   git clone git@github.com:<user-name>/localstack-dotnet-client.git
   # Navigate to the newly cloned directory
   cd <folder-name>
   # Assign the original repo to a remote called "upstream"
   git remote add upstream git@github.com:localstack-dotnet/localstack-dotnet-client.git
   ```

2. If you cloned a while ago, get the latest changes from upstream:

   ```bash
   git checkout master
   git pull upstream master
   ```

3. Create a new topic branch (off the main project development branch) to
   contain your feature, change, or fix:

   ```bash
   git checkout -b <topic-branch-name>
   ```

4. Commit your changes in logical chunks. Use Git's
   [interactive rebase](https://thoughtbot.com/blog/git-interactive-rebase-squash-amend-rewriting-history)
   feature to tidy up your commits before making them public.

5. Locally merge (or rebase) the upstream development branch into your topic branch:

   ```bash
   git pull [--rebase] upstream master
   ```

6. Push your topic branch up to your fork:

   ```bash
   git push origin <topic-branch-name>
   ```

7. [Open a Pull Request](https://help.github.com/articles/using-pull-requests/)
    with a clear title and description against the `master` branch.


[github]: https://github.com/localstack-dotnet/localstack-dotnet-client
[dropbox]: https://www.dropbox.com
[gist]: https://gist.github.com
[pullrequesthelp]: https://help.github.com/articles/using-pull-requests
