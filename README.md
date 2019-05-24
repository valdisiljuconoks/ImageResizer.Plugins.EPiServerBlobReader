[![Build status](https://ci.appveyor.com/api/projects/status/10j0nkukch6y5jyf?svg=true)](https://ci.appveyor.com/project/ValdisIljuconoks/imageresizer-plugins-episerverblobreader)

## ImageResizer.Plugins.EPiServerBlobReader


Nothing much to describe here :)
Just install NuGet package and it will register EPiServer Blob reader plugin for ImageResizer in order to serve and process images from EPiServer Media folders by ImageResizer.


## Breaking Changes (starting from v6.0)

If you use fluent API to resize the image and pass in `null`, `string.Empty` or `ContentReference.EmptyReference` you will get `ArgumentNullException` exception.

## Render Image in Markup
Most convenient way to render image in markup would be use `HtmlHelper` extension method:

```
@using ImageResizer.Plugins.EPiServer

<img src="@Html.ResizeImage(Model.CurrentPage.MainImage, 100, 100)" />
```

This will make sure that markup for visitors would be (assuming that image is `png`):

```
<img src="/.../image.png?w=100&h=100">
```

And also for the edit mode it would be generated something like this:

```
<img src="/.../image.png,,{CONTENT-ID}?epieditmode=False&w=100&h=100">
```

`ResizeImage` returns back `UrlBuilder` type, so you can fluently chain any additional paramters if needed:

```
<img src="@Html.ResizeImage(Model.CurrentPage.MainImage, 100, 150).Add("gradient", "true").Add("bgcolor", "red)" />
```

## Render Image Markup (Fluent)
You can also use some basic fluent api support as well:

```
<img src="@Html.ResizeImage(CurrentPage.MainImage).Width(200)
                                                  .Height(200)
                                                  .Scale(ScaleMode.Both)
                                                  .FitMode(FitMode.Crop)" />
```

## Render Image Markup with Fallback (Fluent)

If you need to fallback to other image in cases when given `ContentReference` is empty (and don't want to check for `null` or `ContentReference.EmptyReference` yourself) you can use resize image with fallback:

```
<img src="@Html.ResizeImageWithFallback(CurrentPage.MainImage, "/no-image.jpg").Width(200).Height(200).Scale(ScaleMode.Both).FitMode(FitMode.Crop)" />
```

## Render Picture Element

This is pretty simple as well.

1) We need to define picture profile. Profile is metadata how to render `<picture>` element.

```
public static PictureProfile SampleImage =
    new PictureProfile
    {
        SrcSetWidths = new[] { 480, 768, 992, 1200 },
        SrcSetSizes = new[]
        {
            "50vw",
        },
       DefaultWidth = 992
    };
```

Here we can specify couple of properties to customize <picture> element:
* Source set sizes (`SrcSetSizes`) - this regulates image size for various media conditions.
* Source set widths (`SrcSetWidths`) - this regulates various image sizes (resized by width specified here). Used to generate srcset attribute.
* Default width (`DefaultWidth`) - what is default width of the image. This is for old-school browsers those have no clue about `<picture>` element existence.

2) Call actual rendering method

```csharp
@Html.ResizePicture(Model.CurrentPage.MainImage, PictureProfiles.SampleImage)
```

3) Code above generates following markup:

```
<picture>
    <source sizes="50vw"
            srcset="/globalassets/batman.jpg?w=480 480w,
                    /globalassets/batman.jpg?w=768 768w,
                    /globalassets/batman.jpg?w=992 992w,
                    /globalassets/batman.jpg?w=1200 1200w">
    <img alt="" src="/globalassets/batman.jpg?w=992">
</picture>
```

More info about how to render picture element - [here](https://blog.tech-fellow.net/2018/10/25/episerver-images-got-responsive-resize/).

<br/>
Happy imaging!
