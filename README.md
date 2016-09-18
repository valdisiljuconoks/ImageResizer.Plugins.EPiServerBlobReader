## ImageResizer.Plugins.EPiServerBlobReader

Nothing much to describe here :)
Just install NuGet package and it will register EPiServer Blob reader plugin for ImageResizer in order to serve and process images from EPiServer Media folders by ImageResizer.

## Render Image in Markup
Most convenient way to render image in markup would be use `HtmlHelper` extension method:

```
@using ImageResizer.Plugins.EPiServer

<img src="@Html.ResizeImage(Model.CurrentPage.MainImage, 100, 100)" />
```

This will make sure that markup for visitors would be (assuming that image is `png`):

```
<image src="/.../image.png?w=100&h=100">
```

And also for the edit mode it would be generated something like this:

```
<image src="/.../image.png,,{CONTENT-ID}?epieditmode=False&w=100&h=100">
```

`ResizeImage` returns back `UrlBuilder` type, so you can fluently chain any additional paramters if needed:

```
<img src="@Html.ResizeImage(Model.CurrentPage.MainImage, 100, 150).Add("gradient", "true").Add("bgcolor", "red)" />
```
