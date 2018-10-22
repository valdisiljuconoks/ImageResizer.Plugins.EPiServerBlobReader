using EPiServer;
using EPiServer.Core;

namespace ImageResizer.Plugins.EPiServer
{
    public static class ContentReferenceExtensions
    {
        public static UrlBuilder ResizeImage(this ContentReference image, int? width = null, int? height = null)
        {
            return HtmlHelperExtensions.ResizeImage(null, image, width, height);
        }
    }
}
