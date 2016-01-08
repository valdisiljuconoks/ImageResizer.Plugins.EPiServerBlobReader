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
    // Copyright: http://world.episerver.com/Code/Martin-Pickering/ImageResizingNet-integration-for-CMS75/
    public class EPiServerBlobFile : IVirtualFileWithModifiedDate
    {
        private readonly ContentRouteHelper _contentRouteHelper;
        private Blob _blob;
        private IContent _content;

        public EPiServerBlobFile(string virtualPath, NameValueCollection queryString)
            : this(virtualPath, queryString, ServiceLocator.Current.GetInstance<ContentRouteHelper>()) { }

        public EPiServerBlobFile(string virtualPath, NameValueCollection queryString, ContentRouteHelper contentRouteHelper)
        {
            _contentRouteHelper = contentRouteHelper;
            VirtualPath = virtualPath;
            QueryString = queryString;
        }

        public NameValueCollection QueryString { get; private set; }

        public Blob Blob
        {
            get { return _blob ?? (_blob = GetBlob()); }
        }

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

        public bool BlobExists
        {
            get { return Content != null; }
        }

        public string VirtualPath { get; }

        public DateTime ModifiedDateUTC
        {
            get
            {
                if (Content != null)
                {
                    var trackable = Content as IChangeTrackable;
                    if (trackable != null)
                    {
                        return trackable.Changed.ToUniversalTime();
                    }
                }
                return DateTime.MinValue.ToUniversalTime();
            }
        }

        public Stream Open()
        {
            return Blob != null ? Blob.OpenRead() : null;
        }

        private Blob GetBlob()
        {
            if (Content == null)
            {
                return null;
            }
            var binaryStorable = Content as IBinaryStorable;
            if (binaryStorable == null || binaryStorable.BinaryData == null)
            {
                return null;
            }
            return binaryStorable.BinaryData;
        }
    }
}
