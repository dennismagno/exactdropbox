using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExactOnline.Client.Models;
using ExactOnline.Client.Sdk;
using DotNetOpenAuth.OAuth2;
using ExactOnline.Client.Sdk.Controllers;
using DotNetOpenAuth.AspNet.Clients;

namespace ExactDropBox.Connector
{
    public class ExactConnector : IExactConnector
    {
        private string _endPoint;
        private UserAgentClient _oAuthClient;

        private ExactOnlineClient _client;
        private IAuthorizationState _authorization;

        public bool IsAuthenticated
        {
            get { return IsAccessTokenValid(); }
        }

        public Uri GetAuthorizationUri()
        {
            _endPoint = "https://start.exactonline.com";

      
            _authorization = new AuthorizationState
            {
                Callback = new Uri("http://localhost:19648/home/exoauth2")
            };


            var serverDescription = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(string.Format("{0}/api/oauth2/auth", _endPoint)),
                TokenEndpoint = new Uri(string.Format("{0}/api/oauth2/token", _endPoint))
            };

            _oAuthClient = new UserAgentClient(serverDescription, "eb93389b-9532-46ca-9b24-898a38e91c22", "jYzWXVFiE87C");
            _oAuthClient.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter("jYzWXVFiE87C");

            return _oAuthClient.RequestUserAuthorization(_authorization);
        }

        public void Authenticate(Uri responseUri)
        {
            _authorization = _oAuthClient.ProcessUserAuthorization(responseUri, _authorization);

            _client = new ExactOnlineClient(_endPoint, GetAccessToken);
        }

        public Guid CreateDocument(string subject, string body)
        {
            var document = new Document
            {
                Subject = subject,
                Body = body,
                Category = Guid.Parse("3b6d3833-b31b-423d-bc3c-39c62b8f2b12"),
                Type = 55,
                DocumentDate = DateTime.UtcNow.Date
            };

            if (_client.For<Document>().Insert(ref document))
            {
                return document.ID;
            }

            return Guid.Empty;
        }

        private string GetAccessToken()
        {
            return _authorization.AccessToken;
        }

        private bool IsAccessTokenValid()
        {
            if (_authorization == null) return false;

            return !string.IsNullOrWhiteSpace(_authorization.AccessToken)
                && _authorization.AccessTokenExpirationUtc.HasValue
                && _authorization.AccessTokenExpirationUtc.Value > DateTime.UtcNow;
        }

        public void Dispose()
        {
            
        }
    }
}