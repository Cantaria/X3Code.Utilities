using System.IO;

namespace X3Code.Utils.Web;

/// <summary>
/// Mime Type Converter helps convert file extension to their specific mime-type
/// </summary>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
public static class MimeTypeConverter
{
    /// <summary>
    /// Determines for a given filename with extension a fitting mime-type
    /// </summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
    /// <param name="fileName">The filename with extension</param>
    /// <returns>"application/octet-stream" if unknown, else the mime type</returns>
    public static string GetContentTypeForFileExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
        
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension)) return string.Empty;

        return extension.ToLowerInvariant() switch
        {
            //Images
            MimeTypeFileExtension.Jpg or MimeTypeFileExtension.Jpeg => MimeTypes.Jpeg,
            MimeTypeFileExtension.Png => MimeTypes.Png,
            MimeTypeFileExtension.Gif => MimeTypes.Gif,
            MimeTypeFileExtension.Bmp => MimeTypes.Bmp,
            MimeTypeFileExtension.Svg => MimeTypes.Svg,
            MimeTypeFileExtension.Tiff or MimeTypeFileExtension.Tif => MimeTypes.Tiff,
            MimeTypeFileExtension.Webp => MimeTypes.Webp,
            //Binary files
            MimeTypeFileExtension.Pdf => MimeTypes.Pdf,
            MimeTypeFileExtension.Zip => MimeTypes.Zip,
            MimeTypeFileExtension.SevenZip => MimeTypes.SevenZip,
            MimeTypeFileExtension.Gz => MimeTypes.Gz,
            MimeTypeFileExtension.Bz => MimeTypes.Bz,
            MimeTypeFileExtension.Bz2 => MimeTypes.Bz2,
            MimeTypeFileExtension.Rar => MimeTypes.Rar,
            MimeTypeFileExtension.Jar => MimeTypes.Jar,
            MimeTypeFileExtension.Tar => MimeTypes.Tar,
            MimeTypeFileExtension.Mpkg => MimeTypes.Mpkg,
            //Audio
            MimeTypeFileExtension.Aac => MimeTypes.Aac,
            MimeTypeFileExtension.Oga => MimeTypes.Oga,
            MimeTypeFileExtension.Mid => MimeTypes.Mid,
            MimeTypeFileExtension.Midi => MimeTypes.Midi,
            MimeTypeFileExtension.Mp3 => MimeTypes.Mp3,
            MimeTypeFileExtension.Wav => MimeTypes.Wav,
            MimeTypeFileExtension.Weba => MimeTypes.Weba,
            //Video
            MimeTypeFileExtension.Avi => MimeTypes.Avi,
            MimeTypeFileExtension.Mp4 => MimeTypes.Mp4,
            MimeTypeFileExtension.Ogv => MimeTypes.Ogv,
            MimeTypeFileExtension.Webm => MimeTypes.Webm,
            //Office
            MimeTypeFileExtension.Csv => MimeTypes.Csv,
            MimeTypeFileExtension.Rtf => MimeTypes.Rtf,
            MimeTypeFileExtension.Doc => MimeTypes.Doc,
            MimeTypeFileExtension.Xls => MimeTypes.Xls,
            MimeTypeFileExtension.Ppt => MimeTypes.Ppt,
            MimeTypeFileExtension.Docx => MimeTypes.Docx,
            MimeTypeFileExtension.Pptx => MimeTypes.Pptx,
            MimeTypeFileExtension.Xlsx => MimeTypes.Xlsx,
            MimeTypeFileExtension.Abi => MimeTypes.Abi,
            MimeTypeFileExtension.Ics => MimeTypes.Ics,
            //Web
            MimeTypeFileExtension.Css => MimeTypes.Css,
            MimeTypeFileExtension.Html => MimeTypes.Html,
            MimeTypeFileExtension.Php => MimeTypes.Php,
            MimeTypeFileExtension.Js => MimeTypes.Js,
            MimeTypeFileExtension.Json => MimeTypes.Json,
            MimeTypeFileExtension.Jsonld => MimeTypes.Jsonld,
            MimeTypeFileExtension.Ico => MimeTypes.Ico,
            MimeTypeFileExtension.Epub => MimeTypes.Epub,
            //Other
            MimeTypeFileExtension.Azw => MimeTypes.Azw,
            MimeTypeFileExtension.Otf => MimeTypes.Otf,
            MimeTypeFileExtension.Ttf => MimeTypes.Ttf,
            MimeTypeFileExtension.Woff => MimeTypes.Woff,
            MimeTypeFileExtension.Woff2 => MimeTypes.Woff2,
            MimeTypeFileExtension.Txt => MimeTypes.Txt,
            MimeTypeFileExtension.Xml => MimeTypes.Xml,
            _ => MimeTypes.Default
        };
    }
        
    /// <summary>
    /// Determines for a given content type a fitting file extension
    /// </summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
    /// <param name="contentType">The content type</param>
    /// <returns>The file extension is known, else string.Empty</returns>
    public static string GetFileExtensionByContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return string.Empty;

        return contentType.ToLowerInvariant() switch
        {
            //Images
            MimeTypes.Jpeg => MimeTypeFileExtension.Jpeg,
            MimeTypes.Png => MimeTypeFileExtension.Png,
            MimeTypes.Gif => MimeTypeFileExtension.Gif,
            MimeTypes.Bmp => MimeTypeFileExtension.Bmp,
            MimeTypes.Svg => MimeTypeFileExtension.Svg,
            MimeTypes.Tiff => MimeTypeFileExtension.Tiff,
            MimeTypes.Webp => MimeTypeFileExtension.Webp,
            //Binary files
            MimeTypes.Pdf => MimeTypeFileExtension.Pdf,
            MimeTypes.Zip => MimeTypeFileExtension.Zip,
            MimeTypes.SevenZip => MimeTypeFileExtension.SevenZip,
            MimeTypes.Gz => MimeTypeFileExtension.Gz,
            MimeTypes.Bz => MimeTypeFileExtension.Bz,
            MimeTypes.Bz2 => MimeTypeFileExtension.Bz2,
            MimeTypes.Rar => MimeTypeFileExtension.Rar,
            MimeTypes.Jar => MimeTypeFileExtension.Jar,
            MimeTypes.Tar => MimeTypeFileExtension.Tar,
            MimeTypes.Mpkg => MimeTypeFileExtension.Mpkg,
            //Audio
            MimeTypes.Aac => MimeTypeFileExtension.Aac,
            MimeTypes.Oga => MimeTypeFileExtension.Oga,
            MimeTypes.Mid => MimeTypeFileExtension.Mid,
            MimeTypes.Midi => MimeTypeFileExtension.Midi,
            MimeTypes.Mp3 => MimeTypeFileExtension.Mp3,
            MimeTypes.Wav => MimeTypeFileExtension.Wav,
            MimeTypes.Weba => MimeTypeFileExtension.Weba,
            //Video
            MimeTypes.Avi => MimeTypeFileExtension.Avi,
            MimeTypes.Mp4 => MimeTypeFileExtension.Mp4,
            MimeTypes.Ogv => MimeTypeFileExtension.Ogv,
            MimeTypes.Webm => MimeTypeFileExtension.Webm,
            //Office
            MimeTypes.Csv => MimeTypeFileExtension.Csv,
            MimeTypes.Rtf => MimeTypeFileExtension.Rtf,
            MimeTypes.Doc => MimeTypeFileExtension.Doc,
            MimeTypes.Xls => MimeTypeFileExtension.Xls,
            MimeTypes.Ppt => MimeTypeFileExtension.Ppt,
            MimeTypes.Docx => MimeTypeFileExtension.Docx,
            MimeTypes.Pptx => MimeTypeFileExtension.Pptx,
            MimeTypes.Xlsx => MimeTypeFileExtension.Xlsx,
            MimeTypes.Abi => MimeTypeFileExtension.Abi,
            MimeTypes.Ics => MimeTypeFileExtension.Ics,
            //Web
            MimeTypes.Css => MimeTypeFileExtension.Css,
            MimeTypes.Html => MimeTypeFileExtension.Html,
            MimeTypes.Php => MimeTypeFileExtension.Php,
            MimeTypes.Js => MimeTypeFileExtension.Js,
            MimeTypes.Json => MimeTypeFileExtension.Json,
            MimeTypes.Jsonld => MimeTypeFileExtension.Jsonld,
            MimeTypes.Ico => MimeTypeFileExtension.Ico,
            MimeTypes.Epub => MimeTypeFileExtension.Epub,
            //Other
            MimeTypes.Azw => MimeTypeFileExtension.Azw,
            MimeTypes.Otf => MimeTypeFileExtension.Otf,
            MimeTypes.Ttf => MimeTypeFileExtension.Ttf,
            MimeTypes.Woff => MimeTypeFileExtension.Woff,
            MimeTypes.Woff2 => MimeTypeFileExtension.Woff2,
            MimeTypes.Txt => MimeTypeFileExtension.Txt,
            MimeTypes.Xml => MimeTypeFileExtension.Xml,
            _ => string.Empty
        };
    }
}