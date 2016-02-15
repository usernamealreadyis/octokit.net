﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Octokit.Internal;
using Xunit;

namespace Octokit.Tests.Models
{
    public class ReadOnlyPagedCollectionTests
    {
        public class TheGetNextPageMethod
        {
            [Fact]
            public async Task ReturnsTheNextPage()
            {
                var nextPageUrl = new Uri("https://example.com/page/2");
                var nextPageResponse = Task.Factory.StartNew<IApiResponse<List<object>>>(() =>
                    new ApiResponse<List<object>>(new Response(), new List<object> { new object(), new object() }));
                var links = new Dictionary<string, Uri> { { "next", nextPageUrl } };
                var scopes = new List<string>();
                var httpResponse = Substitute.For<IResponse>();
                httpResponse.ApiInfo.Returns(new ApiInfo(links, scopes, scopes, "etag", new RateLimit(new Dictionary<string, string>())));
                var response = new ApiResponse<List<object>>(httpResponse, new List<object>());
                var connection = Substitute.For<IConnection>();
                connection.Get<List<object>>(nextPageUrl, null, null).Returns(nextPageResponse);
                var pagedCollection = new ReadOnlyPagedCollection<object>(
                    response,
                    nextPageUri => connection.Get<List<object>>(nextPageUrl, null, null));

                var nextPage = await pagedCollection.GetNextPage();

                Assert.NotNull(nextPage);
                Assert.Equal(2, nextPage.Count);
            }
        }
    }
}
