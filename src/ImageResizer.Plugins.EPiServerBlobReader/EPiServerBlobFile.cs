using System;
using System.Collections.Specialized;
using System.IO;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace ImageResizer.Plugins.EPiServerBlobReader
{
    // Original code (or at least as much as left from it):
    // http://world.episerver.com/Code/Martin-Pickering/ImageResizingNet-integration-for-CMS75/
    public class EPiServerBlobFile : IVirtualFileWithModifiedDate
    {
        private readonly IContentRouteHelper _contentRouteHelper;
        private Blob _blob;
        private IContent _content;

        public EPiServerBlobFile(string virtualPath, NameValueCollection queryString) : this(virtualPath, queryString, ServiceLocator.Current.GetInstance<IContentRouteHelper>()) { }

        public EPiServerBlobFile(string virtualPath, NameValueCollection queryString, IContentRouteHelper contentRouteHelper)
        {
            _contentRouteHelper = contentRouteHelper;
            VirtualPath = virtualPath;
            QueryString = queryString;
        }

        public NameValueCollection QueryString { get; private set; }

        public Blob Blob => _blob ?? (_blob = GetBlob());

        private IContent Content
        {
            get
            {
                if (_content != null)
                {
                    return _content;
                }

                _content = _contentRouteHelper.Content;
                if (!_content.QueryDistinctAccess(AccessLevel.Read))
                {
                    _content = null;
                }

                return _content;
            }
        }

        public bool BlobExists => Content != null;

        public string VirtualPath { get; }

        public DateTime ModifiedDateUTC
        {
            get
            {
                var trackable = Content as IChangeTrackable;
                return trackable?.Saved.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime();
            }
        }

        public Stream Open()
        {
            return Blob?.OpenRead();
        }

        private Blob GetBlob()
        {
            var binaryStorable = Content as IBinaryStorable;
            return binaryStorable?.BinaryData;
        }
    }
}
