using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExactDropBox.Connector
{
    public interface IDropboxConnector : IDisposable
    {
        bool IsAuthenticated { get; }

        Uri GetAuthorizationUri();

        Task Authenticate(string authCode);

        Task<FileMetadata> Upload(string file, string content);
    }
}