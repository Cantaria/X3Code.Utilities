﻿using X3Code.Utils.Web;
using Xunit;

namespace X3Code.UnitTests.Web;

// ReSharper disable once InconsistentNaming
public class MimeTypeConverter_specs
{
    [Theory]
    [InlineData(MimeTypeFileExtension.Jpg, MimeTypes.Jpeg)]
    [InlineData(MimeTypeFileExtension.Jpeg, MimeTypes.Jpeg)]
    [InlineData(MimeTypeFileExtension.Png, MimeTypes.Png)]
    [InlineData(MimeTypeFileExtension.Gif, MimeTypes.Gif)]
    [InlineData(MimeTypeFileExtension.Bmp, MimeTypes.Bmp)]
    [InlineData(MimeTypeFileExtension.Svg, MimeTypes.Svg)]
    [InlineData(MimeTypeFileExtension.Tiff, MimeTypes.Tiff)]
    [InlineData(MimeTypeFileExtension.Tif, MimeTypes.Tiff)]
    [InlineData(MimeTypeFileExtension.Webp, MimeTypes.Webp)]
    [InlineData(MimeTypeFileExtension.Pdf, MimeTypes.Pdf)]
    [InlineData(MimeTypeFileExtension.Zip, MimeTypes.Zip)]
    [InlineData(MimeTypeFileExtension.SevenZip, MimeTypes.SevenZip)]
    [InlineData(MimeTypeFileExtension.Gz, MimeTypes.Gz)]
    [InlineData(MimeTypeFileExtension.Bz, MimeTypes.Bz)]
    [InlineData(MimeTypeFileExtension.Bz2, MimeTypes.Bz2)]
    [InlineData(MimeTypeFileExtension.Rar, MimeTypes.Rar)]
    [InlineData(MimeTypeFileExtension.Jar, MimeTypes.Jar)]
    [InlineData(MimeTypeFileExtension.Tar, MimeTypes.Tar)]
    [InlineData(MimeTypeFileExtension.Mpkg, MimeTypes.Mpkg)]
    [InlineData(MimeTypeFileExtension.Aac, MimeTypes.Aac)]
    [InlineData(MimeTypeFileExtension.Oga, MimeTypes.Oga)]
    [InlineData(MimeTypeFileExtension.Mid, MimeTypes.Mid)]
    [InlineData(MimeTypeFileExtension.Midi, MimeTypes.Midi)]
    [InlineData(MimeTypeFileExtension.Mp3, MimeTypes.Mp3)]
    [InlineData(MimeTypeFileExtension.Wav, MimeTypes.Wav)]
    [InlineData(MimeTypeFileExtension.Weba, MimeTypes.Weba)]
    [InlineData(MimeTypeFileExtension.Avi, MimeTypes.Avi)]
    [InlineData(MimeTypeFileExtension.Mp4, MimeTypes.Mp4)]
    [InlineData(MimeTypeFileExtension.Ogv, MimeTypes.Ogv)]
    [InlineData(MimeTypeFileExtension.Webm, MimeTypes.Webm)]
    [InlineData(MimeTypeFileExtension.Csv, MimeTypes.Csv)]
    [InlineData(MimeTypeFileExtension.Rtf, MimeTypes.Rtf)]
    [InlineData(MimeTypeFileExtension.Doc, MimeTypes.Doc)]
    [InlineData(MimeTypeFileExtension.Xls, MimeTypes.Xls)]
    [InlineData(MimeTypeFileExtension.Ppt, MimeTypes.Ppt)]
    [InlineData(MimeTypeFileExtension.Docx, MimeTypes.Docx)]
    [InlineData(MimeTypeFileExtension.Pptx, MimeTypes.Pptx)]
    [InlineData(MimeTypeFileExtension.Xlsx, MimeTypes.Xlsx)]
    [InlineData(MimeTypeFileExtension.Abi, MimeTypes.Abi)]
    [InlineData(MimeTypeFileExtension.Ics, MimeTypes.Ics)]
    [InlineData(MimeTypeFileExtension.Css, MimeTypes.Css)]
    [InlineData(MimeTypeFileExtension.Html, MimeTypes.Html)]
    [InlineData(MimeTypeFileExtension.Php, MimeTypes.Php)]
    [InlineData(MimeTypeFileExtension.Js, MimeTypes.Js)]
    [InlineData(MimeTypeFileExtension.Json, MimeTypes.Json)]
    [InlineData(MimeTypeFileExtension.Jsonld, MimeTypes.Jsonld)]
    [InlineData(MimeTypeFileExtension.Ico, MimeTypes.Ico)]
    [InlineData(MimeTypeFileExtension.Epub, MimeTypes.Epub)]
    [InlineData(MimeTypeFileExtension.Azw, MimeTypes.Azw)]
    [InlineData(MimeTypeFileExtension.Otf, MimeTypes.Otf)]
    [InlineData(MimeTypeFileExtension.Ttf, MimeTypes.Ttf)]
    [InlineData(MimeTypeFileExtension.Woff, MimeTypes.Woff)]
    [InlineData(MimeTypeFileExtension.Woff2, MimeTypes.Woff2)]
    [InlineData(MimeTypeFileExtension.Txt, MimeTypes.Txt)]
    [InlineData(MimeTypeFileExtension.Xml, MimeTypes.Xml)]
    [InlineData(".unknown", MimeTypes.Default)]
    [InlineData("", "")]
    [InlineData("noDot", "")]
    public void CanConvertFileExtensionToMime(string fileExtension, string expectedResult)
    {
        var result = MimeTypeConverter.GetContentTypeForFileExtension(fileExtension);
        Assert.Equal(expectedResult, result);
    }
        
    [Theory]
    [InlineData(MimeTypes.Jpeg, MimeTypeFileExtension.Jpeg)]
    [InlineData(MimeTypes.Png, MimeTypeFileExtension.Png)]
    [InlineData(MimeTypes.Gif, MimeTypeFileExtension.Gif)]
    [InlineData(MimeTypes.Bmp, MimeTypeFileExtension.Bmp)]
    [InlineData(MimeTypes.Svg, MimeTypeFileExtension.Svg)]
    [InlineData(MimeTypes.Tiff, MimeTypeFileExtension.Tiff)]
    [InlineData(MimeTypes.Webp, MimeTypeFileExtension.Webp)]
    [InlineData(MimeTypes.Pdf, MimeTypeFileExtension.Pdf)]
    [InlineData(MimeTypes.Zip, MimeTypeFileExtension.Zip)]
    [InlineData(MimeTypes.SevenZip, MimeTypeFileExtension.SevenZip)]
    [InlineData(MimeTypes.Gz, MimeTypeFileExtension.Gz)]
    [InlineData(MimeTypes.Bz, MimeTypeFileExtension.Bz)]
    [InlineData(MimeTypes.Bz2, MimeTypeFileExtension.Bz2)]
    [InlineData(MimeTypes.Rar, MimeTypeFileExtension.Rar)]
    [InlineData(MimeTypes.Jar, MimeTypeFileExtension.Jar)]
    [InlineData(MimeTypes.Tar, MimeTypeFileExtension.Tar)]
    [InlineData(MimeTypes.Mpkg, MimeTypeFileExtension.Mpkg)]
    [InlineData(MimeTypes.Aac, MimeTypeFileExtension.Aac)]
    [InlineData(MimeTypes.Oga, MimeTypeFileExtension.Oga)]
    [InlineData(MimeTypes.Mid, MimeTypeFileExtension.Mid)]
    [InlineData(MimeTypes.Midi, MimeTypeFileExtension.Midi)]
    [InlineData(MimeTypes.Mp3, MimeTypeFileExtension.Mp3)]
    [InlineData(MimeTypes.Wav, MimeTypeFileExtension.Wav)]
    [InlineData(MimeTypes.Weba, MimeTypeFileExtension.Weba)]
    [InlineData(MimeTypes.Avi, MimeTypeFileExtension.Avi)]
    [InlineData(MimeTypes.Mp4, MimeTypeFileExtension.Mp4)]
    [InlineData(MimeTypes.Ogv, MimeTypeFileExtension.Ogv)]
    [InlineData(MimeTypes.Webm, MimeTypeFileExtension.Webm)]
    [InlineData(MimeTypes.Csv, MimeTypeFileExtension.Csv)]
    [InlineData(MimeTypes.Rtf, MimeTypeFileExtension.Rtf)]
    [InlineData(MimeTypes.Doc, MimeTypeFileExtension.Doc)]
    [InlineData(MimeTypes.Xls, MimeTypeFileExtension.Xls)]
    [InlineData(MimeTypes.Ppt, MimeTypeFileExtension.Ppt)]
    [InlineData(MimeTypes.Docx, MimeTypeFileExtension.Docx)]
    [InlineData(MimeTypes.Pptx, MimeTypeFileExtension.Pptx)]
    [InlineData(MimeTypes.Xlsx, MimeTypeFileExtension.Xlsx)]
    [InlineData(MimeTypes.Abi, MimeTypeFileExtension.Abi)]
    [InlineData(MimeTypes.Ics, MimeTypeFileExtension.Ics)]
    [InlineData(MimeTypes.Css, MimeTypeFileExtension.Css)]
    [InlineData(MimeTypes.Html, MimeTypeFileExtension.Html)]
    [InlineData(MimeTypes.Php, MimeTypeFileExtension.Php)]
    [InlineData(MimeTypes.Js, MimeTypeFileExtension.Js)]
    [InlineData(MimeTypes.Json, MimeTypeFileExtension.Json)]
    [InlineData(MimeTypes.Jsonld, MimeTypeFileExtension.Jsonld)]
    [InlineData(MimeTypes.Ico, MimeTypeFileExtension.Ico)]
    [InlineData(MimeTypes.Epub, MimeTypeFileExtension.Epub)]
    [InlineData(MimeTypes.Azw, MimeTypeFileExtension.Azw)]
    [InlineData(MimeTypes.Otf, MimeTypeFileExtension.Otf)]
    [InlineData(MimeTypes.Ttf, MimeTypeFileExtension.Ttf)]
    [InlineData(MimeTypes.Woff, MimeTypeFileExtension.Woff)]
    [InlineData(MimeTypes.Woff2, MimeTypeFileExtension.Woff2)]
    [InlineData(MimeTypes.Txt, MimeTypeFileExtension.Txt)]
    [InlineData(MimeTypes.Xml, MimeTypeFileExtension.Xml)]
    [InlineData(".unknown", "")]
    [InlineData("", "")]
    [InlineData("noDot", "")]
    public void CanConvertMimeTypeToFileExtension(string mimeType, string expectedResult)
    {
        var result = MimeTypeConverter.GetFileExtensionByContentType(mimeType);
        Assert.Equal(expectedResult, result);
    }
}