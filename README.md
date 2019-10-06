# ZenHub.NET

This is a .NET library that makes REST calls to the ZenHub API. More information about the ZenHub API is can be found [here](https://github.com/ZenHubIO/API).

[![NuGet version](https://img.shields.io/nuget/v/zenhub.net.svg?style=flat)](https://www.nuget.org/packages/ZenHub.Net/)
[![Nuget downloads](https://img.shields.io/nuget/dt/zenhub.net.svg?style=flat)](https://www.nuget.org/packages/zenhub.net/)
[![MIT License](https://img.shields.io/github/license/AlexGhiondea/ZenHub.net.svg)](https://github.com/AlexGhiondea/zenhub.net/blob/master/LICENSE)
========

## ZenHub authentication

In order for the calls to be authorized with ZenHub, you first need to get an authentication token from ZenHub.
To do so, please visit [the ZenHub token page](https://app.zenhub.com/dashboard/tokens)

## Getting started

Once you have a token, constructing the client is done this way:

```csharp
ZenHubClient zenhubClient = new ZenHubClient("<token>");
```

The operations on the client are split depending on what they are operating on. 
Please see below for more details about each of the clients

### Repository client

To interact with ZenHub at the repository level, you would use the Repository client.
You can create a `ZenHubRepositoryClient` from a `ZenHubClient` by specifying the repository id, or a `Octokit.Repository` object.

```csharp
ZenHubClient zenhubClient = new ZenHubClient("<token>");

// Using the repoId directly
long repoId = 1234;
ZenHubRepositoryClient repoClient = zenhubClient.GetRepositoryClient(repoId);

// Using the Octokit object
Repository repo = ...;  
ZenHubRepositoryClient repoClient = zenhubClient.GetRepositoryClient(repo);
```

### Issue client

To interact with ZenHub at the issue level, you would use the Issue client.
You can create a `ZenHubIssueClient` from a `ZenHubClient` by specifying the repository id and issue number, or a `Octokit.Issue` object.

```csharp
ZenHubClient zenhubClient = new ZenHubClient("<token>");

// Using the repoId directly
long repoId = 1234;
int issueNumber = 456;
ZenHubIssueClient repoClient = zenhubClient.GetIssueClient(repoId, issueNumber);

// Using the Octokit object
Issue issue = ...;  
ZenHubIssueClient repoClient = zenhubClient.GetIssueClient(issue);
```

### Epic client

To interact with ZenHub at the epic level, you would use the Epic client.
You can create a `ZenHubEpicClient` from a `ZenHubClient` by specifying the repository id and issue number, or a `Octokit.Issue` object.

```csharp
ZenHubClient zenhubClient = new ZenHubClient("<token>");

// Using the repo Id and the epic number
long repoId = 1234;
int epicNumber = 456;
ZenHubEpicClient repoClient = zenhubClient.GetEpicClient(repoId, epicNumber);

// Using the Octokit object
Issue epic = ...;  
ZenHubEpicClient repoClient = zenhubClient.GetEpicClient(epic);
```

### Release client

To interact with ZenHub at the release level, you would use the Release client.

```csharp
ZenHubClient zenhubClient = new ZenHubClient("<token>");

string releaseId = "releaseId";
ZenHubReleaseClient repoClient = zenhubClient.GetReleaseClient(releaseId);
```