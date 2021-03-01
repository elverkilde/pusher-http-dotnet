﻿using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PusherServer.RestfulClient;
using PusherServer.Tests.Helpers;
using System.Threading.Tasks;

namespace PusherServer.Tests.UnitTests
{
    [TestClass]
    public class When_using_async_Get_to_retrieve_a_list_of_application_channels
    {
        private IPusher _pusher;
        private IPusherRestClient _subPusherClient;
        private IApplicationConfig _config;

        [SetUp]
        public void Setup()
        {
            _subPusherClient = Substitute.For<IPusherRestClient>();

            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subPusherClient
            };

            _config = new ApplicationConfig
            {
                AppId = "test-app-id",
                AppKey = "test-app-key",
                AppSecret = "test-app-secret",
            };

            _pusher = new Pusher(_config.AppId, _config.AppKey, _config.AppSecret, options);
        }

        [TestMethod]
        public async Task url_is_in_expected_format()
        {
            await _pusher.GetAsync<object>("/channels");

#pragma warning disable 4014
            _subPusherClient.Received().ExecuteGetAsync<object>(
#pragma warning restore 4014
                Arg.Is<IPusherRestRequest>(
                    x => x.ResourceUri.StartsWith("/apps/" + _config.AppId + "/channels")
                )
            );
        }

        [TestMethod]
        public async Task GET_request_is_made()
        {
            await _pusher.GetAsync<object>("/channels");

#pragma warning disable 4014
            _subPusherClient.Received().ExecuteGetAsync<object>(
#pragma warning restore 4014
                Arg.Is<IPusherRestRequest>(
                    x => x.Method == PusherMethod.GET
                )
            );
        }

        [TestMethod]
        public async Task additional_parameters_should_be_added_to_query_string()
        {
            await _pusher.GetAsync<object>("/channels", new { filter_by_prefix = "presence-", info = "user_count" });

#pragma warning disable 4014
            _subPusherClient.Received().ExecuteGetAsync<object>(
#pragma warning restore 4014
                Arg.Is<IPusherRestRequest>(
                    x => x.ResourceUri.Contains("&filter_by_prefix=presence-") &&
                         x.ResourceUri.Contains("&info=user_count")
                )
            );
        }
    }
}
