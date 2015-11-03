using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExactDropBox.Connector
{
    public class DropboxConnector : IDropboxConnector
    {
        private string _accessToken;
        private string oauth2State;
        string RedirectUri = "http://localhost:19648/home/AuthAsync";

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(_accessToken); }
        }

        public DropboxConnector()
        {

        }

        public Uri GetAuthorizationUri()
        {
            oauth2State = Guid.NewGuid().ToString("N");
            var authRequestUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, "unqwltt7mf1x5mh", RedirectUri, state: oauth2State);

            return authRequestUrl;
        }

        public async Task Authenticate(string authCode)
        {
            OAuth2Response response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(authCode, "unqwltt7mf1x5mh", "yehgvm5kg1qlel3", RedirectUri);

            _accessToken = response.AccessToken;
        }

        public async Task<FileMetadata> Upload(string file, string content)
        {
            using (var dbx = new DropboxClient(_accessToken))
            {
                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    var updated = await dbx.Files.UploadAsync(
                        "/" + file,
                       WriteMode.Overwrite.Instance,
                       body: mem);

                    return updated;
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
