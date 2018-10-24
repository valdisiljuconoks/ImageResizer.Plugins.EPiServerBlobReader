namespace ImageResizer.Plugins.EPiServer
{
    public class PictureProfile
    {
        public int DefaultWidth { get; set; }

        public int[] SrcSetWidths { get; set; }

        public string[] SrcSetSizes { get; set; }

        public string[] SrcMedias { get; set; }
    }
}
