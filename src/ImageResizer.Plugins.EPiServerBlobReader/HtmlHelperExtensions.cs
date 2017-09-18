using System;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace ImageResizer.Plugins.EPiServer
{
    public static class HtmlHelperExtensions
    {
        public static UrlBuilder ResizeImage(this HtmlHelper helper, ContentReference image, int? width = null, int? height = null)
        {
            if(image == null || image == ContentReference.EmptyReference)
                throw new ArgumentNullException(nameof(image), "You might want to use `ResizeImageWithFallback()` instead");

            var url = UrlResolver.Current.GetUrl(image);

            return ConstructUrl(url, width, height);
        }

        public static UrlBuilder ResizeImageWithFallback(this HtmlHelper helper, ContentReference image, string imageFallback, int? width = null, int? height = null)
        {
            return ConstructUrl(image == null || image == ContentReference.EmptyReference ? imageFallback : UrlResolver.Current.GetUrl(image), width, height);
        }

        public static UrlBuilder ResizeImage(this HtmlHelper helper, string imageUrl, int? width = null, int? height = null)
        {
            if(string.IsNullOrEmpty(imageUrl))
                throw new ArgumentNullException(nameof(imageUrl), "You might want to use `ResizeImageWithFallback()` instead");

            return ConstructUrl(imageUrl, width, height);
        }

        public static UrlBuilder ResizeImageWithFallback(this HtmlHelper helper, string imageUrl, string imageFallback, int? width = null, int? height = null)
        {
            return ConstructUrl(string.IsNullOrEmpty(imageUrl) ? imageFallback : imageUrl, width, height);
        }

        private static UrlBuilder ConstructUrl(string url, int? width = null, int? height = null)
        {
            var builder = new UrlBuilder(url);

            if(width.HasValue)
                builder.Width(width.Value);

            if(height.HasValue)
                builder.Height(height.Value);

            return builder;
        }
    }
}
