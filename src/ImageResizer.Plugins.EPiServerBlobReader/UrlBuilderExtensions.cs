using System;
using System.Collections.Specialized;
using EPiServer;

namespace ImageResizer.Plugins.EPiServer
{
    public static class UrlBuilderExtensions
    {
        public static UrlBuilder Clone(this UrlBuilder builder)
        {
            return new UrlBuilder(builder.ToString());
        }

        public static UrlBuilder Add(this UrlBuilder target, string key, string value)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if(!target.IsEmpty)
                target.QueryCollection.Add(key, value);

            return target;
        }

        public static UrlBuilder Add(this UrlBuilder target, NameValueCollection collection)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsEmpty)
                target.QueryCollection.Add(collection);

            return target;
        }

        public static UrlBuilder Width(this UrlBuilder target, int width)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsEmpty)
                target.QueryCollection.Add("w", width.ToString());

            return target;
        }

        public static UrlBuilder Height(this UrlBuilder target, int height)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsEmpty)
                target.QueryCollection.Add("h", height.ToString());

            return target;
        }

        public static UrlBuilder Scale(this UrlBuilder target, ScaleMode mode)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsEmpty)
                target.QueryCollection.Add("scale", AddScaleString(mode));

            return target;
        }

        public static UrlBuilder Quality(this UrlBuilder target, int quality)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsEmpty)
                target.QueryCollection.Add("quality", quality.ToString());

            return target;
        }

        public static UrlBuilder FitMode(this UrlBuilder target, FitMode mode)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsEmpty)
                target.QueryCollection.Add("mode", mode.ToString().ToLower());

            return target;
        }

        private static string AddScaleString(ScaleMode value)
        {
            switch (value) {
                case ScaleMode.Both:
                    return "both";
                case ScaleMode.DownscaleOnly:
                    return "down";
                case ScaleMode.UpscaleCanvas:
                    return "canvas";
                case ScaleMode.UpscaleOnly:
                    return "up";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
