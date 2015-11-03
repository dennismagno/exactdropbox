using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExactDropBox.Connector
{
    public interface IExactConnector : IDisposable
    {
        bool IsAuthenticated { get; }

        Uri GetAuthorizationUri();
        void Authenticate(Uri responseUri);
        Guid CreateDocument(string subject, string body);
    }
}