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
                throw new ArgumentNullException(nameof(image));

            var url = UrlResolver.Current.GetUrl(image);

            return helper.ResizeImage(url, width, height);
        }

        public static UrlBuilder ResizeImage(this HtmlHelper helper, string imageUrl, int? width = null, int? height = null)
        {
            if(string.IsNullOrEmpty(imageUrl))
                return new UrlBuilder(string.Empty);

            var builder = new UrlBuilder(imageUrl);

            if(width.HasValue)
                builder.Add("w", width.Value.ToString());

            if(height.HasValue)
                builder.Add("h", height.Value.ToString());

            return builder;
        }
    }
}
