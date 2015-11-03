using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExactDropBox.Models;
using System.IO;
using Dropbox.Api.Files;
using ExactDropBox.Connector;

namespace ExactDropBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDropboxConnector _dbx;
        private readonly IExactConnector _ext;
        UploadedFileDBContext db = new UploadedFileDBContext();

        public HomeController(IDropboxConnector dbx, IExactConnector ext)
        {
            if (dbx == null) throw new ArgumentNullException("dbx");
            if (ext == null) throw new ArgumentNullException("ext");
            _dbx = dbx;
            _ext = ext;
        }

        public ActionResult Index()
        {
            var model = GetOrCreateModel();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                string content = new StreamReader(file.InputStream).ReadToEnd();
                FileMetadata metaData = await _dbx.Upload(fileName, content);
                if (metaData.Rev != null)
                {
                    Guid _extDoc = _ext.CreateDocument(fileName, content);
                    var _file = new UploadedFile();
                    _file.FileName = fileName;
                    _file.DropBoxRev = metaData.Rev;
                    _file.ExactDocID = _extDoc.ToString();
                    _file.DateUploaded = DateTime.UtcNow.Date;
                    db.UploadedFile.Add(_file);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("Index");
        }


        // GET: /Home/DropBoxConnect
        public ActionResult DropBoxConnect()
        {
            var redirect = _dbx.GetAuthorizationUri();
            return Redirect(redirect.ToString());
        }

        // GET: /Home/AuthAsync
        public async Task<ActionResult> AuthAsync(string code, string state)
        {
            await _dbx.Authenticate(code);

            return this.RedirectToAction("Index");
        }

        // GET: /Home/ExactConnect
        public ActionResult ExactConnect()
        {
            var redirect = _ext.GetAuthorizationUri();
            return Redirect(redirect.ToString());
        }

        // GET: /Home/exoauth2
        public ActionResult exoauth2(string code, string state)
        {
            _ext.Authenticate(this.Request.Url);

            return this.RedirectToAction("Index");
        }

        private IndexViewModel GetOrCreateModel(IndexViewModel model = null)
        {
            if (model == null)
                model = new IndexViewModel();

            model.DropboxIsAuthenticated = _dbx.IsAuthenticated;
            model.ExactIsAuthenticated = _ext.IsAuthenticated;
            if (_dbx.IsAuthenticated)
            {
                model.FileList = db.UploadedFile.AsEnumerable();
            }

            return model;
        }
    }
}
