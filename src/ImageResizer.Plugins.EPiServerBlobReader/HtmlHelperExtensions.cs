using System;
using System.Collections.Specialized;
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
            if(string.IsNullOrEmpty(url))
                return new UrlBuilder("");

            var builder = new UrlBuilder(url);

            if(width.HasValue)
                builder.QueryCollection.Add("w", width.Value.ToString());

            if(height.HasValue)
                builder.QueryCollection.Add("h", height.Value.ToString());

            return builder;
        }
    }

    public static class UrlBuilderExtensions
    {
        public static UrlBuilder Add(this UrlBuilder target, string key, string value)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            target.QueryCollection.Add(key, value);
            return target;
        }

        public static UrlBuilder Add(this UrlBuilder target, NameValueCollection collection)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            target.QueryCollection.Add(collection);
            return target;
        }
    }
}
