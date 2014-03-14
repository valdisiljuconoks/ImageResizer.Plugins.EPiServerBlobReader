using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using ImageResizer.Configuration;

namespace ImageResizer.Plugins.EPiServerBlobReader
{
    // Copyright: http://world.episerver.com/Code/Martin-Pickering/ImageResizingNet-integration-for-CMS75/
    public class EPiServerBlobReaderPlugin : IVirtualImageProvider, IPlugin
    {
        //ToDo: get rid of these constants in favour of something that uses the Site Definition?
        //ToDo: the file extension list here is fixed, ought it not to be driven by the registered extensions for ImageResizing.Net?
        private readonly Regex _isAssetImageRegex =
                new Regex(@".*/(?:globalassets|contentassets|siteassets)/.*\.(?:jpg|jpeg|gif|png)",
                        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline |
                        RegexOptions.Compiled);

        private readonly Regex _isEditModeImageUrlRegex =
                new Regex(@"(.*/(?:globalassets|contentassets|siteassets)/.*\.(?:jpg|jpeg|gif|png)),{2}\d+",
                        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline |
                        RegexOptions.Compiled);

        public IPlugin Install(Config c)
        {
            c.Pipeline.PostAuthorizeRequestStart += PostAuthorizeRequestStarted;
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            c.Pipeline.PostAuthorizeRequestStart -= PostAuthorizeRequestStarted;
            return true;
        }

        public bool FileExists(string virtualPath, NameValueCollection queryString)
        {
            if (!PathIsInScope(virtualPath))
            {
                return false;
            }
            var blob = new EPiServerBlobFile(virtualPath, queryString);
            return blob.BlobExists;
        }

        public IVirtualFile GetFile(string virtualPath, NameValueCollection queryString)
        {
            if (!PathIsInScope(virtualPath))
            {
                return null;
            }
            var blob = new EPiServerBlobFile(virtualPath, queryString);
            return blob.Blob != null ? blob : null;
        }

        protected virtual void PostAuthorizeRequestStarted(IHttpModule httpModule, HttpContext httpContext)
        {
            var path = HttpUtility.UrlDecode(httpContext.Request.Url.AbsolutePath);
            if (path == null || !this._isEditModeImageUrlRegex.IsMatch(path))
            {
                return;
            }

            Config.Current.Pipeline.PreRewritePath = this._isEditModeImageUrlRegex.Match(path).Groups[1].Value;
        }

        protected bool PathIsInScope(string virtualPath)
        {
            return this._isAssetImageRegex.IsMatch(virtualPath);
        }
    }
}
