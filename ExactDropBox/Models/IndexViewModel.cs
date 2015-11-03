using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExactDropBox.Models
{
    public class IndexViewModel
    {
        public bool DropboxIsAuthenticated { get; set; }
        public bool ExactIsAuthenticated { get; set; }

        public IEnumerable<UploadedFile> FileList { get; set; }
    }
}