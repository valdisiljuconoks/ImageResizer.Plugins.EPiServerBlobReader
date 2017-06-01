using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Internal;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Internal;
using SampleAlloy.Models.Media;
using MimeMapping = EPiServer.Web.MimeMapping;

namespace SampleAlloy.Experiment
{
    /// <summary>
    /// A HTTP Handler that deliver binary large objects from <see cref="T:EPiServer.Framework.Blobs.Blob" />.
    /// </summary>
    public abstract class BlobHttpHandler2 : MediaHandlerBase, IHttpAsyncHandler, IHttpHandler
    {
        internal Injected<IRoutableEvaluator> RoutableEvaluator;

        bool IHttpHandler.IsReusable
        {
            get
            {
                return true;
            }
        }

        protected override bool ProcessRequestInternal(HttpContextBase context)
        {
            Blob blob = (Blob)null;
            bool flag = this.ProccessBlobRequest(context, out blob);
            if (blob == null)
                return flag;
            this.DownloadBlobData(context, blob);
            return true;
        }

        IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            if (context.Request.HttpMethod == "GET" || context.Request.HttpMethod == "HEAD")
                return this.ProcessRequestAsyncInternal(new HttpContextWrapper(context), cb, extraData);
            throw new HttpException(404, "Not Found.");
        }

        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
        {
            bool? asyncState = result.AsyncState as bool?;
            if (asyncState.HasValue && asyncState.HasValue)
            {
                if (!asyncState.Value)
                    throw new HttpException(404, "Not Found.");
            }
            else if (!this.EndTransmitStream(result))
                throw new HttpException(404, "Not Found.");
        }

        /// <summary>Gets the routed BLOB from the request.</summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// </returns>
        protected abstract Blob GetBlob(HttpContextBase httpContext);

        protected virtual IAsyncResult ProcessRequestAsyncInternal(HttpContextWrapper context, AsyncCallback cb, object extraData)
        {
            Blob blob = (Blob)null;
            bool flag = this.ProccessBlobRequest((HttpContextBase)context, out blob);
            if (blob != null)
            {
                context.Response.ContentType = MimeMapping.GetMimeMapping(blob.ID.LocalPath);
                if (blob is FileBlob)
                {
                    this.TransmitFile((HttpContextBase)context, ((FileBlob)blob).FilePath);
                }
                else
                {
                    Stream stream = blob.OpenRead();
                    if (stream.CanSeek)
                        return this.BeginTransmitStream((HttpContextBase)context, stream, (AsyncCallback)(r => this.DisposeOnCallback(r, cb, (IDisposable)stream)), (object)stream);
                    try
                    {
                        this.TransmitStream((HttpContextBase)context, stream);
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
            AsyncResult asyncResult = new AsyncResult(cb, (object)flag);
            asyncResult.SetCompleted();
            return (IAsyncResult)asyncResult;
        }

        private void DownloadBlobData(HttpContextBase context, Blob blob)
        {
            context.Response.ContentType = MimeMapping.GetMimeMapping(blob.ID.LocalPath);
            if (blob is FileBlob)
            {
                this.TransmitFile(context, ((FileBlob)blob).FilePath);
            }
            else
            {
                using (Stream source = blob.OpenRead())
                    this.TransmitStream(context, source);
            }
        }

        public bool IsRoutable(IContent content)
        {
            var _publishedStateAssessor = ServiceLocator.Current.GetInstance<IPublishedStateAssessor>();
            var _contextModeResolver = ServiceLocator.Current.GetInstance<IContextModeResolver>();

            if (!_publishedStateAssessor.IsPublished(content))
                return _contextModeResolver.CurrentMode.EditOrPreview();
            return true;
        }


        private bool ProccessBlobRequest(HttpContextBase context, out Blob blob)
        {
            blob = (Blob)null;
            IContent content = ServiceLocator.Current.GetInstance<IContentRouteHelper>().Content;
            if (content == null || !IsRoutable(content))
                return false;
            if (!content.QueryDistinctAccess(AccessLevel.Read))
            {
                DefaultAccessDeniedHandler.CreateAccessDeniedDelegate()((object)this);
                return true;
            }
            IContent assetOwner = ServiceLocator.Current.GetInstance<ContentAssetHelper>().GetAssetOwner(content.ContentLink);
            if (assetOwner != null && !assetOwner.QueryDistinctAccess(AccessLevel.Read))
            {
                DefaultAccessDeniedHandler.CreateAccessDeniedDelegate()((object)this);
                return true;
            }
            DateTime modifiedDate = DateTime.Today;
            if (content is IChangeTrackable)
                modifiedDate = ((IChangeTrackable)content).Changed;
            this.SetCachePolicy(context, modifiedDate.ToUniversalTime());
            if (this.NotModifiedHandling(context, modifiedDate))
                return true;
            blob = this.GetBlob(context);
            return blob != null;
        }

        private void DisposeOnCallback(IAsyncResult result, AsyncCallback cb, IDisposable disposable)
        {
            if (result != null && result.IsCompleted)
                disposable.Dispose();
            if (cb == null)
                return;
            cb(result);
        }
    }

    [TemplateDescriptor(Inherited = true, TemplateTypeCategory = TemplateTypeCategories.HttpHandler)]
    public class ImageFileHttpHandler : BlobHttpHandler2, IRenderTemplate<ImageFile>
    {
        /// <summary>Unsupported INTERNAL API! Not covered by semantic versioning; might change without notice. Gets the routed BLOB from the request.
        /// </summary>
        /// <remarks>The implementation returns IContentMedia.BinaryData.</remarks>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        /// <exclude />
        protected override Blob GetBlob(HttpContextBase httpContext)
        {
            string customRouteData = httpContext.Request.RequestContext.GetCustomRouteData<string>(DownloadMediaRouter.DownloadSegment);

            if (!string.IsNullOrEmpty(customRouteData))
                httpContext.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"; filename*=UTF-8''{1}", (object)customRouteData, (object)Uri.EscapeDataString(customRouteData)));

            IBinaryStorable content = ServiceLocator.Current.GetInstance<IContentRouteHelper>().Content as IBinaryStorable;
            if (content == null)
                return (Blob)null;

            return content.BinaryData;
        }
    }
}