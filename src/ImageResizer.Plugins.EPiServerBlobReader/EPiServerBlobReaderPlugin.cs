using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ImageResizer.Configuration;

namespace ImageResizer.Plugins.EPiServerBlobReader
{
    /// <summary>
    ///     Copyright:
    ///     https://raw.githubusercontent.com/Igelkottegrodan/ImageResizer.Plugins.EPiServerBlobPlugin/master/ImageResizer.Plugins.EPiServerBlobPlugin/EPiServerBlobPlugin.cs
    /// </summary>
    public class EPiServerBlobReaderPlugin : IVirtualImageProvider, IVirtualImageProviderAsync, IPlugin
    {
        private static readonly Regex PathRegex = new Regex(@",,[\d_]+", RegexOptions.Compiled);

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

        private void OnPostAuthorizeRequestStart(IHttpModule sender, HttpContext context)
        {
            // A hack to get ImageResizer working with images in edit mode.
            // In edit mode image URLs look like http://host.com/ui/CMS/Content/contentassets/0c51b2fa1f464956aab221c6ecc21804/file.jpg,,82777?epieditmode=False&width=100
            // For some reason, URLs that have commas in them aren't processed by ImageResizer, so they need to be removed from the end of the path.
            // Episerver can still route content even without the commas and the content ID.

            // Thumbnail URLs should not be modified, e.g. 
            // http://host.com/ui/CMS/Content/contentassets/0c51b2fa1f464956aab221c6ecc21804/file.jpg,,82777/Thumbnail?epieditmode=False?1470652477313

            var pipelineConfig = Config.Current.Pipeline;

            if (PipelineIsUsingResizer(pipelineConfig))
            {
                var fixedUrl = PathRegex.Replace(context.Request.Url.AbsolutePath, string.Empty);
                pipelineConfig.PreRewritePath = fixedUrl;
            }
        }

        private bool PipelineIsUsingResizer(PipelineConfig config)
        {
            foreach (string key in config.ModifiedQueryString.Keys)
            {
                if (config.SupportedQuerystringKeys.Contains(key))
                {
                    return true;
                }
            }

            return false;
        }

        public Task<bool> FileExistsAsync(string virtualPath, NameValueCollection queryString)
        {
            var blobImage = GetBlobFile(virtualPath, queryString);
            var exists = blobImage != null && blobImage.BlobExists;
            return Task.FromResult(exists);
        }

        public Task<IVirtualFileAsync> GetFileAsync(string virtualPath, NameValueCollection queryString)
        {
            IVirtualFileAsync blobImage = GetBlobFile(virtualPath, queryString);
            return Task.FromResult(blobImage);
        }
    }
}
