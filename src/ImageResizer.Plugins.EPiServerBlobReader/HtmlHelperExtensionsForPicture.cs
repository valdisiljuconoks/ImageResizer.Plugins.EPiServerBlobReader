using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;

namespace ImageResizer.Plugins.EPiServer
{
    public static class HtmlHelperExtensionsForPicture
    {
        public static MvcHtmlString ResizePicture(this HtmlHelper helper, UrlBuilder url, PictureProfile profile)
        {
            var imgUrl = url.Clone();
            imgUrl.QueryCollection["w"] = profile.DefaultWidth.ToString();

            var sourceSets = profile.SrcSetWidths.Select(w =>
                                                         {
                                                             var sourceUrl = url.Clone();
                                                             sourceUrl.QueryCollection["w"] = w.ToString();
                                                             return $"{sourceUrl} {w}w";
                                                         }).ToArray();

            return ResizePicture2(profile, imgUrl.ToString(), sourceSets);
        }

        public static MvcHtmlString ResizePicture(this HtmlHelper helper, ContentReference image, PictureProfile profile)
        {
            var imgUrl = helper.ResizeImage(image, profile.DefaultWidth);
            var sourceSets = profile.SrcSetWidths.Select(w => $"{helper.ResizeImage(image, w).ToString()} {w}w").ToArray();

            return ResizePicture2(profile, imgUrl.ToString(), sourceSets);
        }

        private static MvcHtmlString ResizePicture2(PictureProfile profile, string imgUrl, string[] sourceSets)
        {
            var picture = new TagBuilder("picture");
            var source = new TagBuilder("source");

            if(profile.SrcSetSizes != null && profile.SrcSetSizes.Length > 0)
                source.Attributes.Add("sizes", string.Join(", ", profile.SrcSetSizes));

            source.Attributes.Add("srcset", string.Join(", ", sourceSets));

            var img = new TagBuilder("img");
            img.Attributes.Add("src", imgUrl);
            img.Attributes.Add("alt", "");

            picture.InnerHtml = source.ToString() + img;

            return new MvcHtmlString(picture.ToString());
        }
    }
}
