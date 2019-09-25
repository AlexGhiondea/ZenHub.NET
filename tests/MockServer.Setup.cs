using Octokit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ZenHub.Tests
{
    internal class MockServer
    {
        public FluentMockServer Server { get; }
        public string EndPoint => Server.Urls[0];

        public MockServer()
        {
            Server = FluentMockServer.Start();
        }

        public const long repoId = 1;
        public const int issueNumber = 2;
        public const int milestoneNumber = 1;
        public const string ZenHubWorkspaceId = "dummyWorkspace";
        public const string ZenHubReleaseId = "dummyRelease";
        public const string ZenHubPipelineId = "pipelineDummy";

        public void RegisterServer()
        {
            // GetIssueData
            Server.Given(Request.Create()
                                 .WithPath($"/p1/repositories/{repoId}/issues/{issueNumber}")
                                 .UsingGet())
                   .RespondWith(
                       Response.Create()
                       .WithStatusCode(200)
                       .WithBody(@"{
                            ""estimate"": {
                                ""value"": 8
                            },
                            ""plus_ones"": [
                                {
                                ""created_at"": ""2015-12-11T18:43:22.296Z""
                                }
                            ],
                            ""pipeline"": {
                                ""name"": ""QA"",
                                ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a7"",
                                ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac""
                            },
                            ""pipelines"": [
                                {
                                ""name"": ""QA"",
                                ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a7"",
                                ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac""
                                },
                                {
                                ""name"": ""Done"",
                                ""pipeline_id"": ""5d0a7cea41fd098f6b7f58b7"",
                                ""workspace_id"": ""5d0a7cea41fd098f6b7f58b8""
                                }
                            ],
                            ""is_epic"": true
                            }")
                         );



            // Get Issue Events
            Server.Given(Request.Create()
                           .WithPath($"/p1/repositories/{repoId}/issues/{issueNumber}/events")
                           .UsingGet())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"[
                                {
                                    ""user_id"": 16717,
                                    ""type"": ""estimateIssue"",
                                    ""created_at"": ""2015-12-11T19:43:22.296Z"",
                                    ""from_estimate"": {
                                    ""value"": 8
                                    }
                                },
                                {
                                    ""user_id"": 16717,
                                    ""type"": ""estimateIssue"",
                                    ""created_at"": ""2015-12-11T18:43:22.296Z"",
                                    ""from_estimate"": {
                                    ""value"": 4
                                    },
                                    ""to_estimate"": {
                                    ""value"": 8
                                    }
                                },
                                {
                                    ""user_id"": 16717,
                                    ""type"": ""estimateIssue"",
                                    ""created_at"": ""2015-12-11T13:43:22.296Z"",
                                    ""to_estimate"": {
                                    ""value"": 4
                                    }
                                },
                                {
                                    ""user_id"": 16717,
                                    ""type"": ""transferIssue"",
                                    ""created_at"": ""2015-12-11T12:43:22.296Z"",
                                    ""from_pipeline"": {
                                    ""name"": ""Backlog""
                                    },
                                    ""to_pipeline"": {
                                    ""name"": ""In progress""
                                    },
                                    ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac""
                                },
                                {
                                    ""user_id"": 16717,
                                    ""type"": ""transferIssue"",
                                    ""created_at"": ""2015-12-11T11:43:22.296Z"",
                                    ""to_pipeline"": {
                                    ""name"": ""Backlog""
                                    }
                                }
                                ]")
                           );


            // Get Epics for Repo
            Server.Given(Request.Create()
                           .WithPath($"/p1/repositories/{repoId}/epics")
                           .UsingGet())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"{
                                ""epic_issues"": [
                                    {
                                    ""issue_number"": 3953,
                                    ""repo_id"": 1234567,
                                    ""issue_url"": ""https://github.com/RepoOwner/RepoName/issues/3953""
                                    },
                                    {
                                    ""issue_number"": 1342,
                                    ""repo_id"": 1234567,
                                    ""issue_url"": ""https://github.com/RepoOwner/RepoName/issues/1342""
                                    }
                                ]
                                }")
                           );

            // Get Epic Data
            Server.Given(Request.Create()
                        .WithPath($"/p1/repositories/{repoId}/epics/{issueNumber}")
                        .UsingGet())
                    .RespondWith(
                        Response.Create()
                        .WithStatusCode(200)
                        .WithBody(@"{
                        ""total_epic_estimates"": { ""value"": 60 },
                        ""estimate"": { ""value"": 10 },
                        ""pipeline"": {
                            ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac"",
                            ""name"": ""Backlog"",
                            ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a8""
                        },
                        ""pipelines"": [
                            {
                            ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac"",
                            ""name"": ""Backlog"",
                            ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a8""
                            },
                            {
                            ""workspace_id"": ""5d0a7cea41fd098f6b7f58b8"",
                            ""name"": ""In Progress"",
                            ""pipeline_id"": ""5d0a7cea41fd098f6b7f58b5""
                            }
                        ],
                        ""issues"": [
                            {
                            ""issue_number"": 3161,
                            ""is_epic"": true,
                            ""repo_id"": 1099029,
                            ""estimate"": { ""value"": 40 },
                            ""pipelines"": [
                                {
                                ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac"",
                                ""name"": ""Backlog"",
                                ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a8""
                                },
                                {
                                ""workspace_id"": ""5d0a7cea41fd098f6b7f58b8"",
                                ""name"": ""In Progress"",
                                ""pipeline_id"": ""5d0a7cea41fd098f6b7f58b5""
                                }
                            ],
                            ""pipeline"": {
                                ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac"",
                                ""name"": ""Backlog"",
                                ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a8""
                            }
                            },
                            {
                            ""issue_number"": 2,
                            ""is_epic"": false,
                            ""repo_id"": 1234567,
                            ""estimate"": { ""value"": 10 },
                            ""pipelines"": [
                                {
                                ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac"",
                                ""name"": ""Backlog"",
                                ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a8""
                                },
                                {
                                ""workspace_id"": ""5d0a7cea41fd098f6b7f58b8"",
                                ""name"": ""In Progress"",
                                ""pipeline_id"": ""5d0a7cea41fd098f6b7f58b5""
                                }
                            ],
                            ""pipeline"": {
                                ""workspace_id"": ""5d0a7a9741fd098f6b7f58ac"",
                                ""name"": ""Backlog"",
                                ""pipeline_id"": ""5d0a7a9741fd098f6b7f58a8""
                            }
                            }
                        ]
                        }")
                            );


            // Get workspaces
            Server.Given(Request.Create()
                                .WithPath($"/p2/repositories/{repoId}/workspaces")
                                .UsingGet())
                  .RespondWith(
                            Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"[
                        {
                            ""name"": ""Design and UX"",
                            ""description"": null,
                            ""id"": ""5d0a7a9741fd098f6b7f58ac"",
                            ""repositories"": [12345678, 912345]
                        },
                        {
                            ""name"": ""Roadmap"",
                            ""description"": ""Feature planning and enhancements"",
                            ""id"": ""5d0a7cea41fd098f6b7f58b8"",
                            ""repositories"": [12345678]
                        }
                        ]")
                           );


            Server.Given(Request.Create()
                                .WithPath($"/p2/workspaces/{ZenHubWorkspaceId}/repositories/{repoId}/board")
                                .UsingGet())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"{
                        ""pipelines"": [
                            {
                            ""id"": ""595d430add03f01d32460080"",
                            ""name"": ""New Issues"",
                            ""issues"": [
                                {
                                ""issue_number"": 279,
                                ""estimate"": { ""value"": 40 },
                                ""position"": 0,
                                ""is_epic"": true
                                },
                                {
                                ""issue_number"": 142,
                                ""is_epic"": false
                                }
                            ]
                            },
                            {
                            ""id"": ""595d430add03f01d32460081"",
                            ""name"": ""Backlog"",
                            ""issues"": [
                                {
                                ""issue_number"": 303,
                                ""estimate"": { ""value"": 40 },
                                ""position"": 3,
                                ""is_epic"": false
                                }
                            ]
                            },
                            {
                            ""id"": ""595d430add03f01d32460082"",
                            ""name"": ""To Do"",
                            ""issues"": [
                                {
                                ""issue_number"": 380,
                                ""estimate"": { ""value"": 1 },
                                ""position"": 0,
                                ""is_epic"": true
                                },
                                {
                                ""issue_number"": 284,
                                ""position"": 2,
                                ""is_epic"": false
                                },
                                {
                                ""issue_number"": 329,
                                ""estimate"": { ""value"": 8 },
                                ""position"": 7,
                                ""is_epic"": false
                                }
                            ]
                            }
                        ]
                        }")
                           );


            // Get ZenHub Board (old)
            Server.Given(Request.Create()
                                         .WithPath($"/p1/repositories/{repoId}/board")
                                         .UsingGet())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"{
                      ""pipelines"": [
                        {
                          ""id"": ""595d430add03f01d32460080"",
                          ""name"": ""New Issues"",
                          ""issues"": [
                            {
                              ""issue_number"": 279,
                              ""estimate"": { ""value"": 40 },
                              ""position"": 0,
                              ""is_epic"": true
                            },
                            {
                              ""issue_number"": 142,
                              ""is_epic"": false
                            }
                          ]
                        },
                        {
                          ""id"": ""595d430add03f01d32460081"",
                          ""name"": ""Backlog"",
                          ""issues"": [
                            {
                              ""issue_number"": 303,
                              ""estimate"": { ""value"": 40 },
                              ""position"": 3,
                              ""is_epic"": false
                            }
                          ]
                        },
                        {
                          ""id"": ""595d430add03f01d32460082"",
                          ""name"": ""To Do"",
                          ""issues"": [
                            {
                              ""issue_number"": 380,
                              ""estimate"": { ""value"": 1 },
                              ""position"": 0,
                              ""is_epic"": true
                            },
                            {
                              ""issue_number"": 284,
                              ""position"": 2,
                              ""is_epic"": false
                            },
                            {
                              ""issue_number"": 329,
                              ""estimate"": { ""value"": 8 },
                              ""position"": 7,
                              ""is_epic"": false
                            }
                          ]
                        }
                      ]
                    }")
                           );


            // Get dependencies for Repository

            Server.Given(Request.Create()
                                          .WithPath($"/p1/repositories/{repoId}/dependencies")
                                          .UsingGet())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                                .WithBody(@"{
                      ""dependencies"": [
                        {
                          ""blocking"": {
                            ""issue_number"": 3953,
                            ""repo_id"": 1234567
                          },
                          ""blocked"": {
                            ""issue_number"": 1342,
                            ""repo_id"": 1234567
                          }
                        },
                        {
                          ""blocking"": {
                            ""issue_number"": 5,
                            ""repo_id"": 987
                          },
                          ""blocked"": {
                            ""issue_number"": 1342,
                            ""repo_id"": 1234567
                          }
                        }
                      ]
                    }")
                            );



            // Get release report
            Server.Given(Request.Create()
                                         .WithPath($"/p1/reports/release/{ZenHubReleaseId}")
                                         .UsingGet())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"{
                      ""release_id"": ""59d3cd520a430a6344fd3bdb"",
                      ""title"": ""Test release"",
                      ""description"": """",
                      ""start_date"": ""2017-10-01T19:00:00.000Z"",
                      ""desired_end_date"": ""2017-10-03T19:00:00.000Z"",
                      ""created_at"": ""2017-10-03T17:48:02.701Z"",
                      ""closed_at"": null,
                      ""state"": ""open"",
                      ""repositories"": [105683718]
                    }")
                           );


            // Get release report for repositoryt

            Server.Given(Request.Create()
                                       .WithPath($"/p1/repositories/{repoId}/reports/releases")
                                       .UsingGet())
                         .RespondWith(
                             Response.Create()
                             .WithStatusCode(200)
                             .WithBody(@"[
                      {
                        ""release_id"": ""59cbf2fde010f7a5207406e8"",
                        ""title"": ""Great title for release 1"",
                        ""description"": ""Great description for release"",
                        ""start_date"": ""2000-10-10T00:00:00.000Z"",
                        ""desired_end_date"": ""2010-10-10T00:00:00.000Z"",
                        ""created_at"": ""2017-09-27T18:50:37.418Z"",
                        ""closed_at"": null,
                        ""state"": ""open""
                      },
                      {
                        ""release_id"": ""59cbf2fde010f7a5207406e8"",
                        ""title"": ""Great title for release 2"",
                        ""description"": ""Great description for release"",
                        ""start_date"": ""2000-10-10T00:00:00.000Z"",
                        ""desired_end_date"": ""2010-10-10T00:00:00.000Z"",
                        ""created_at"": ""2017-09-27T18:50:37.418Z"",
                        ""closed_at"": null,
                        ""state"": ""open""
                      }
                    ]")
                         );

            // Edit release report
            Server.Given(Request.Create()
                                         .WithPath($"/p1/reports/release/{ZenHubReleaseId}")
                                         .UsingPatch())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"{
                      ""release_id"": ""59d3d6438b3f16667f9e7174"",
                      ""title"": ""Amazing title"",
                      ""description"": ""Amazing description"",
                      ""start_date"": ""2007-01-01T00:00:00.000Z"",
                      ""desired_end_date"": ""2007-01-01T00:00:00.000Z"",
                      ""created_at"": ""2017-10-03T18:26:11.700Z"",
                      ""closed_at"": ""2017-10-03T18:26:11.700Z"",
                      ""state"": ""closed"",
                      ""repositories"": [105683567, 105683718]
                    }")
                           );

            // Get issues for release
            Server.Given(Request.Create()
                              .WithPath($"/p1/reports/release/{ZenHubReleaseId}/issues")
                              .UsingGet())
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"[
                      { ""repo_id"": 103707262, ""issue_number"": 2 },
                      { ""repo_id"": 103707262, ""issue_number"": 3 }
                    ]")
                );


            // Set issue estimate
            Server.Given(Request.Create()
                              .WithPath($"/p1/repositories/{repoId}/issues/{issueNumber}/estimate")
                              .UsingPut())
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"{ ""estimate"": 15 }")
                );

            // Add issue to release report
            Server.Given(Request.Create()
                                         .WithPath($"/p1/reports/release/{ZenHubReleaseId}/issues")
                                         .UsingPatch())
                           .RespondWith(
                               Response.Create()
                               .WithStatusCode(200)
                               .WithBody(@"{
                      ""added"": [{ ""repo_id"": 103707262, ""issue_number"": 3 }],
                      ""removed"": []
                    }")
                           );


            // add issue to epic
            Server
                .Given(Request.Create()
                              .WithPath($"/p1/repositories/{repoId}/epics/{issueNumber}/update_issues")
                              .UsingPost())
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"{
                      ""remove_issues"": [{ ""repo_id"": 13550592, ""issue_number"": 3 }],
                      ""add_issues"": [
                        { ""repo_id"": 13550592, ""issue_number"": 2 },
                        { ""repo_id"": 13550592, ""issue_number"": 1 }
                      ]
                    }")
                );


            // Move issue to pipeline

            Server.Given(Request.Create()
                                          .WithPath($"/p2/workspaces/{ZenHubWorkspaceId}/repositories/{repoId}/issues/{issueNumber}/moves")
                                          .WithBody(@"{""pipeline_id"":""pipelineDummy"",""position"":1}")
                                          .UsingPost())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                            );

            // Move issue to pipeline old
            Server.Given(Request.Create()
                                          .WithPath($"/p1/repositories/{repoId}/issues/{issueNumber}/moves")
                                          .WithBody(@"{""pipeline_id"":""pipelineDummy"",""position"":1}")
                                          .UsingPost())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                            );


            // Convert issue to epic
            Server.Given(Request.Create()
                                          .WithPath($"/p1/repositories/{repoId}/epics/{issueNumber}/convert_to_epic")
                                          .WithBody(@"{""issues"":[]}")
                                          .UsingPost())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                            );

            Server.Given(Request.Create()
                                          .WithPath($"/p1/repositories/{repoId}/epics/{issueNumber}/convert_to_epic")
                                          .WithBody(@"{""issues"":[{""repo_id"":13550592,""issue_number"":3},{""repo_id"":13550592,""issue_number"":1}]}")
                                          .UsingPost())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                            );

            // convert epic to issue
            Server.Given(Request.Create()
                                .WithPath($"/p1/repositories/{repoId}/epics/{issueNumber}/convert_to_issue")
                                .UsingPost())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                            );


            // get milestone start
            Server.Given(Request.Create()
                                .WithPath($"/p1/repositories/{repoId}/milestones/{milestoneNumber}/start_date")
                                .UsingGet())
                            .RespondWith(
                                Response.Create()
                                .WithBody(@"{ ""start_date"": ""2010-11-13T01:38:56.842Z"" }")
                                .WithStatusCode(200)
                            );


            // set milestone start
            Server.Given(Request.Create()
                    .WithPath($"/p1/repositories/{repoId}/milestones/{milestoneNumber}/start_date")
                    .WithBody(@"{""start_date"":""2019-11-01T07:00:00Z""}")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                    .WithBody(@"{""start_date"":""2019-11-01T07:00:00Z""}")
                    .WithStatusCode(200)
                );


            // create dependency
            Server.Given(Request.Create()
                    .WithPath($"/p1/dependencies")
                    .WithBody(@"{""blocking"":{""repo_id"":1,""issue_number"":2},""blocked"":{""repo_id"":1,""issue_number"":2}}")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                    .WithBody(@"{""blocking"":{""repo_id"":1,""issue_number"":2},""blocked"":{""repo_id"":1,""issue_number"":2}}")
                    .WithStatusCode(200)
                );

            // create dependency 
            // WireMock bug https://github.com/WireMock-Net/WireMock.Net/issues/352
            Server.Given(Request.Create()
                    .WithPath($"/p1/dependencies")
                    //.WithBody(@"{""blocking"":{""repo_id"":1,""issue_number"":2},""blocked"":{""repo_id"":1,""issue_number"":2}}")
                    .UsingDelete())
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(204)
                );


            // create release report
            Server.Given(Request.Create()
                    .WithPath($"/p1/repositories/{repoId}/reports/release")
                    .WithBody(@"{""title"":"""",""description"":"""",""start_date"":""2019-11-19T08:00:00Z"",""desired_end_date"":""2019-11-19T08:00:00Z"",""repositories"":[]}")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                    .WithBody(@"{
                          ""release_id"": ""59dff4f508399a35a276a1ea"",
                          ""title"": ""Great title"",
                          ""description"": ""Amazing description"",
                          ""start_date"": ""2007-01-01T00:00:00.000Z"",
                          ""desired_end_date"": ""2007-01-01T00:00:00.000Z"",
                          ""created_at"": ""2017-10-12T23:04:21.795Z"",
                          ""closed_at"": null,
                          ""state"": ""open"",
                          ""repositories"": [103707262]
                        }")
                    .WithStatusCode(200)
                );


            // add repo to release report
            Server.Given(Request.Create()
                    .WithPath($"/p1/reports/release/{ZenHubReleaseId}/repository/{repoId}")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(200)
                );

            // remove repo to release report
            Server.Given(Request.Create()
                    .WithPath($"/p1/reports/release/{ZenHubReleaseId}/repository/{repoId}")
                    .UsingDelete())
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(204)
                );
        }

        internal void Stop()
        {
            Server.Stop();
        }
    }
}
