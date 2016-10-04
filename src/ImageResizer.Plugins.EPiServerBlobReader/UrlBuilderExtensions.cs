using System;
using System.Collections.Specialized;
using EPiServer;

namespace ImageResizer.Plugins.EPiServer
{
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
