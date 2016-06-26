using System.Collections.Specialized;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using ImageResizer.Configuration;

namespace ImageResizer.Plugins.EPiServerBlobReader
{
    /// <summary>
    ///     Copyright:
    ///     https://raw.githubusercontent.com/Igelkottegrodan/ImageResizer.Plugins.EPiServerBlobPlugin/master/ImageResizer.Plugins.EPiServerBlobPlugin/EPiServerBlobPlugin.cs
    /// </summary>
    public class EPiServerBlobReaderPlugin : IVirtualImageProvider, IPlugin
    {
        private readonly UrlResolver _urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

        public IPlugin Install(Config config)
        {
            config.Plugins.add_plugin(this);

            return this;
        }

        public bool Uninstall(Config config)
        {
            config.Plugins.remove_plugin(this);

            return true;
        }

        public bool FileExists(string virtualPath, NameValueCollection queryString)
        {
            var blobImage = GetBlobFile(virtualPath, queryString);

            return blobImage != null && blobImage.BlobExists;
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
    }
}
