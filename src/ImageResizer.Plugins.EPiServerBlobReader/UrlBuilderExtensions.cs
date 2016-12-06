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

        public static UrlBuilder Width(this UrlBuilder target, int width)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            target.QueryCollection.Add("w", width.ToString());
            return target;
        }

        public static UrlBuilder Height(this UrlBuilder target, int height)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            target.QueryCollection.Add("h", height.ToString());
            return target;
        }

        public static UrlBuilder Scale(this UrlBuilder target, ScaleMode mode)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            target.QueryCollection.Add("scale", AddScaleString(mode));
            return target;
        }

        public static UrlBuilder FitMode(this UrlBuilder target, FitMode mode)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.QueryCollection.Add("mode", mode.ToString().ToLower());
            return target;
        }

        private static string AddScaleString(ScaleMode value)
        {
            if(value == ScaleMode.Both)
                return "both";

            if(value == ScaleMode.DownscaleOnly)
                return "down";

            if(value == ScaleMode.UpscaleCanvas)
                return "canvas";

            if(value == ScaleMode.UpscaleOnly)
                return "up";

            throw new NotImplementedException("Unrecognized ScaleMode value: " + value);
        }
    }
}
