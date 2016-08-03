## ImageResizer.Plugins.EPiServerBlobReader

Nothing much to describe here :)
Just install NuGet package and it will register EPiServer Blob reader plugin for ImageResizer in order to serve and process images from EPiServer Media folders by ImageResizer.

## I'm seeing unscaled images in Episerver Edit mode

ImageResizer does not support image URLs that Episerver generates in edit mode (Because of special characters). Use the code below to create image links that are supported by ImageResizer regardless of context:
```
<img src="@UrlResolver.Current.GetUrl(new UrlBuilder(...), ContextMode.Default)?w=.."/>
```
