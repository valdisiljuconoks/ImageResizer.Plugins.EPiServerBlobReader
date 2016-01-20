using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using ImageResizer.Configuration;

namespace ImageResizer.Plugins.EPiServerBlobReader
{
    /// <summary>
    /// Copyright: https://raw.githubusercontent.com/Igelkottegrodan/ImageResizer.Plugins.EPiServerBlobPlugin/master/ImageResizer.Plugins.EPiServerBlobPlugin/EPiServerBlobPlugin.cs
    /// </summary>
    public class EPiServerBlobReaderPlugin : IVirtualImageProvider, IPlugin
    {
        private readonly UrlResolver __urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

        public IPlugin Install(Config config)
        {
            config.Plugins.add_plugin(this);
            config.Pipeline.PostAuthorizeRequestStart += OnPostAuthorizeRequestStart;

            return this;
        }

        public bool Uninstall(Config config)
        {
            config.Plugins.remove_plugin(this);
            config.Pipeline.PostAuthorizeRequestStart -= OnPostAuthorizeRequestStart;

            return true;
        }

        public bool FileExists(string virtualPath, NameValueCollection queryString)
        {
            var blobImage = GetBlobFile(virtualPath, queryString);

            return (blobImage != null && blobImage.BlobExists);
        }

        public IVirtualFile GetFile(string virtualPath, NameValueCollection queryString)
        {
            return GetBlobFile(virtualPath, queryString);
        }

        private EPiServerBlobFile GetBlobFile(string virtualPath, NameValueCollection queryString)
        {
            var blobFile = new EPiServerBlobFile(virtualPath, queryString);

            return blobFile;
        }

        private void OnPostAuthorizeRequestStart(IHttpModule sender, HttpContext context)
        {
            var absolutePath = CleanEditModePath(context.Request.Url.AbsolutePath);
            var resolvedContent = __urlResolver.Route(new UrlBuilder(absolutePath));

            if (resolvedContent == null)
            {
                return;
            }

            var isMediaContent = resolvedContent is MediaData;

            if (!isMediaContent)
            {
                return;
            }

            var previewOrEditMode = RequestSegmentContext.CurrentContextMode == ContextMode.Edit || RequestSegmentContext.CurrentContextMode == ContextMode.Preview;

            // Disable cache if editing or previewing
            if (!previewOrEditMode)
            {
                return;
            }

            Config.Current.Pipeline.PreRewritePath = absolutePath;
            var modifiedQueryString = new NameValueCollection(Config.Current.Pipeline.ModifiedQueryString)
            {
                {
                    "process", ProcessWhen.Always.ToString()
                },
                {
                    "cache", ServerCacheMode.No.ToString()
                }
            };

            Config.Current.Pipeline.ModifiedQueryString = modifiedQueryString;
        }

        private string CleanEditModePath(string path)
        {
            return Regex.Replace(path, @",.*$", string.Empty);
        }
    }
}
