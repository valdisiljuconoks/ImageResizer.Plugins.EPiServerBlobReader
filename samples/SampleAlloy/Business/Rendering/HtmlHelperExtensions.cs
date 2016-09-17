using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace SampleAlloy.Business.Rendering
{
    public static class HtmlHelperExtensions
    {
        public static string Resize(this HtmlHelper helper, ContentReference image, int? width = null, int? height = null)
        {
            var url = UrlResolver.Current.GetUrl(image);
            var builder = new UrlBuilder(url);

            if(width.HasValue)
                builder.QueryCollection.Add("w", width.Value.ToString());

            if (height.HasValue)
                builder.QueryCollection.Add("h", height.Value.ToString());

            return builder.ToString();
        }
    }
}