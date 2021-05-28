using System.IO;

namespace X3Code.Utils.Web
{
    /// <summary>
    /// Mime Type Converter, helps converting file extension to their specific mime-type
    /// </summary>
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
    public static class MimeTypeConverter
    {
        /// <summary>
        /// Determines for a given filename with extension a fitting mime-type
        /// </summary>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
        /// <param name="fileName">The filename with extension</param>
        /// <returns>"application/octet-stream" if unknown, else the mime type</returns>
        public static string GetContentTypeForFileExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension)) return string.Empty;

            switch (extension.ToLowerInvariant())
            {
                //Images
                case MimeTypeFileExtension.Jpg:
                case MimeTypeFileExtension.Jpeg:
                    return MimeTypes.Jpeg;
                case MimeTypeFileExtension.Png: return MimeTypes.Png;
                case MimeTypeFileExtension.Gif: return MimeTypes.Gif;
                case MimeTypeFileExtension.Bmp: return MimeTypes.Bmp;
                case MimeTypeFileExtension.Svg: return MimeTypes.Svg;
                case MimeTypeFileExtension.Tiff: 
                case MimeTypeFileExtension.Tif: 
                    return MimeTypes.Tiff;
                case MimeTypeFileExtension.Webp: return MimeTypes.Webp;
                //Binary files
                case MimeTypeFileExtension.Pdf: return MimeTypes.Pdf;
                case MimeTypeFileExtension.Zip: return MimeTypes.Zip;
                case MimeTypeFileExtension.SevenZip: return MimeTypes.SevenZip;
                case MimeTypeFileExtension.Gz: return MimeTypes.Gz;
                case MimeTypeFileExtension.Bz: return MimeTypes.Bz;
                case MimeTypeFileExtension.Bz2: return MimeTypes.Bz2;
                case MimeTypeFileExtension.Rar: return MimeTypes.Rar;
                case MimeTypeFileExtension.Jar: return MimeTypes.Jar;
                case MimeTypeFileExtension.Tar: return MimeTypes.Tar;
                case MimeTypeFileExtension.Mpkg: return MimeTypes.Mpkg;
                //Audio
                case MimeTypeFileExtension.Aac: return MimeTypes.Aac;
                case MimeTypeFileExtension.Oga: return MimeTypes.Oga;
                case MimeTypeFileExtension.Mid: return MimeTypes.Mid;
                case MimeTypeFileExtension.Midi: return MimeTypes.Midi;
                case MimeTypeFileExtension.Mp3: return MimeTypes.Mp3;
                case MimeTypeFileExtension.Wav: return MimeTypes.Wav;
                case MimeTypeFileExtension.Weba: return MimeTypes.Weba;
                //Video
                case MimeTypeFileExtension.Avi: return MimeTypes.Avi;
                case MimeTypeFileExtension.Mp4: return MimeTypes.Mp4;
                case MimeTypeFileExtension.Ogv: return MimeTypes.Ogv;
                case MimeTypeFileExtension.Webm: return MimeTypes.Webm;
                //Office
                case MimeTypeFileExtension.Csv: return MimeTypes.Csv;
                case MimeTypeFileExtension.Rtf: return MimeTypes.Rtf;
                case MimeTypeFileExtension.Doc:  return MimeTypes.Doc;
                case MimeTypeFileExtension.Xls:  return MimeTypes.Xls;
                case MimeTypeFileExtension.Ppt:  return MimeTypes.Ppt;
                case MimeTypeFileExtension.Docx: return MimeTypes.Docx;
                case MimeTypeFileExtension.Pptx: return MimeTypes.Pptx;
                case MimeTypeFileExtension.Xlsx: return MimeTypes.Xlsx;
                case MimeTypeFileExtension.Abi:  return MimeTypes.Abi;
                case MimeTypeFileExtension.Ics:  return MimeTypes.Ics;
                //Web
                case MimeTypeFileExtension.Css: return MimeTypes.Css;
                case MimeTypeFileExtension.Html: return MimeTypes.Html;
                case MimeTypeFileExtension.Php: return MimeTypes.Php;
                case MimeTypeFileExtension.Js: return MimeTypes.Js;
                case MimeTypeFileExtension.Json: return MimeTypes.Json;
                case MimeTypeFileExtension.Jsonld: return MimeTypes.Jsonld;
                case MimeTypeFileExtension.Ico: return MimeTypes.Ico;
                case MimeTypeFileExtension.Epub: return MimeTypes.Epub;
                //Other
                case MimeTypeFileExtension.Azw: return MimeTypes.Azw;
                case MimeTypeFileExtension.Otf: return MimeTypes.Otf;
                case MimeTypeFileExtension.Ttf: return MimeTypes.Ttf;
                case MimeTypeFileExtension.Woff: return MimeTypes.Woff;
                case MimeTypeFileExtension.Woff2: return MimeTypes.Woff2;
                case MimeTypeFileExtension.Txt: return MimeTypes.Txt;
                case MimeTypeFileExtension.Xml: return MimeTypes.Xml;
                
                default: return MimeTypes.Default; //default for every else
            }
        }
        
        /// <summary>
        /// Determines for a given contenttype a fitting file extension
        /// </summary>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types"/>
        /// <param name="contentType">The content type</param>
        /// <returns>The file extension is known, else string.Empty</returns>
        public static string GetFileExtensionByContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return string.Empty;

            switch (contentType.ToLowerInvariant())
            {
                //Images
                 case MimeTypes.Jpeg: return MimeTypeFileExtension.Jpeg;
                 case MimeTypes.Png:  return MimeTypeFileExtension.Png;
                 case MimeTypes.Gif:  return MimeTypeFileExtension.Gif;
                 case MimeTypes.Bmp:  return MimeTypeFileExtension.Bmp;
                 case MimeTypes.Svg:  return MimeTypeFileExtension.Svg;
                 case MimeTypes.Tiff: return MimeTypeFileExtension.Tiff;
                 case MimeTypes.Webp:  return MimeTypeFileExtension.Webp;
                //Binary files
                 case MimeTypes.Pdf:  return MimeTypeFileExtension.Pdf;
                 case MimeTypes.Zip:  return MimeTypeFileExtension.Zip;
                 case MimeTypes.SevenZip:  return MimeTypeFileExtension.SevenZip;
                 case MimeTypes.Gz:  return MimeTypeFileExtension.Gz;
                 case MimeTypes.Bz:  return MimeTypeFileExtension.Bz;
                 case MimeTypes.Bz2:  return MimeTypeFileExtension.Bz2;
                 case MimeTypes.Rar:  return MimeTypeFileExtension.Rar;
                 case MimeTypes.Jar:  return MimeTypeFileExtension.Jar;
                 case MimeTypes.Tar:  return MimeTypeFileExtension.Tar;
                 case MimeTypes.Mpkg:  return MimeTypeFileExtension.Mpkg;
                //Audio
                 case MimeTypes.Aac:  return MimeTypeFileExtension.Aac;
                 case MimeTypes.Oga:  return MimeTypeFileExtension.Oga;
                 case MimeTypes.Mid:  return MimeTypeFileExtension.Mid;
                 case MimeTypes.Midi:  return MimeTypeFileExtension.Midi;
                 case MimeTypes.Mp3:  return MimeTypeFileExtension.Mp3;
                 case MimeTypes.Wav:  return MimeTypeFileExtension.Wav;
                 case MimeTypes.Weba:  return MimeTypeFileExtension.Weba;
                //Video
                 case MimeTypes.Avi:  return MimeTypeFileExtension.Avi;
                 case MimeTypes.Mp4:  return MimeTypeFileExtension.Mp4;
                 case MimeTypes.Ogv:  return MimeTypeFileExtension.Ogv;
                 case MimeTypes.Webm:  return MimeTypeFileExtension.Webm;
                //Office
                 case MimeTypes.Csv:  return MimeTypeFileExtension.Csv;
                 case MimeTypes.Rtf:  return MimeTypeFileExtension.Rtf;
                 case MimeTypes.Doc:   return MimeTypeFileExtension.Doc;
                 case MimeTypes.Xls:   return MimeTypeFileExtension.Xls;
                 case MimeTypes.Ppt:   return MimeTypeFileExtension.Ppt;
                 case MimeTypes.Docx:  return MimeTypeFileExtension.Docx;
                 case MimeTypes.Pptx:  return MimeTypeFileExtension.Pptx;
                 case MimeTypes.Xlsx:  return MimeTypeFileExtension.Xlsx;
                 case MimeTypes.Abi:   return MimeTypeFileExtension.Abi;
                 case MimeTypes.Ics:   return MimeTypeFileExtension.Ics;
                //Web
                 case MimeTypes.Css:  return MimeTypeFileExtension.Css;
                 case MimeTypes.Html:  return MimeTypeFileExtension.Html;
                 case MimeTypes.Php:  return MimeTypeFileExtension.Php;
                 case MimeTypes.Js:  return MimeTypeFileExtension.Js;
                 case MimeTypes.Json:  return MimeTypeFileExtension.Json;
                 case MimeTypes.Jsonld:  return MimeTypeFileExtension.Jsonld;
                 case MimeTypes.Ico:  return MimeTypeFileExtension.Ico;
                 case MimeTypes.Epub:  return MimeTypeFileExtension.Epub;
                //Other
                 case MimeTypes.Azw:  return MimeTypeFileExtension.Azw;
                 case MimeTypes.Otf:  return MimeTypeFileExtension.Otf;
                 case MimeTypes.Ttf:  return MimeTypeFileExtension.Ttf;
                 case MimeTypes.Woff:  return MimeTypeFileExtension.Woff;
                 case MimeTypes.Woff2:  return MimeTypeFileExtension.Woff2;
                 case MimeTypes.Txt:  return MimeTypeFileExtension.Txt;
                 case MimeTypes.Xml:  return MimeTypeFileExtension.Xml;
                
                default: return string.Empty;
            }
        }
    }
}