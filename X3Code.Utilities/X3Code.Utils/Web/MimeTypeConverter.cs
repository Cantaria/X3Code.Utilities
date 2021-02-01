using System.IO;

namespace X3Code.Utils.Web
{
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
                case ".jpg":
                case ".jpeg":
                    return MimeTypes.Jpeg;
                case ".png": return MimeTypes.Png;
                case ".gif": return MimeTypes.Gif;
                case ".bmp": return MimeTypes.Bmp;
                case ".svg": return MimeTypes.Svg;
                case ".tiff": 
                case ".tif": 
                    return MimeTypes.Tiff;
                case ".webp": return MimeTypes.Webp;
                //Binary files
                case ".pdf": return MimeTypes.Pdf;
                case ".zip": return MimeTypes.Zip;
                case ".7z": return MimeTypes.SevenZip;
                case ".gz": return MimeTypes.Gz;
                case ".bz": return MimeTypes.Bz;
                case ".bz2": return MimeTypes.Bz2;
                case ".rar": return MimeTypes.Rar;
                case ".jar": return MimeTypes.Jar;
                case ".tar": return MimeTypes.Tar;
                case ".mpkg": return MimeTypes.Mpkg;
                //Audio
                case ".aac": return MimeTypes.Aac;
                case ".oga": return MimeTypes.Oga;
                case ".mid": return MimeTypes.Mid;
                case ".midi": return MimeTypes.Midi;
                case ".mp3": return MimeTypes.Mp3;
                case ".wav": return MimeTypes.Wav;
                case ".weba": return MimeTypes.Weba;
                //Video
                case ".avi": return MimeTypes.Avi;
                case ".mp4": return MimeTypes.Mp4;
                case ".ogv": return MimeTypes.Ogv;
                case ".webm": return MimeTypes.Webm;
                //Office
                case ".csv": return MimeTypes.Csv;
                case ".rtf": return MimeTypes.Rtf;
                case ".doc":  return MimeTypes.Doc;
                case ".xls":  return MimeTypes.Xls;
                case ".ppt":  return MimeTypes.Ppt;
                case ".docx": return MimeTypes.Docx;
                case ".pptx": return MimeTypes.Pptx;
                case ".xlsx": return MimeTypes.Xlsx;
                case ".abi":  return MimeTypes.Abi;
                case ".ics":  return MimeTypes.Ics;
                //Web
                case ".css": return MimeTypes.Css;
                case ".html": return MimeTypes.Html;
                case ".php": return MimeTypes.Php;
                case ".js": return MimeTypes.Js;
                case ".json": return MimeTypes.Json;
                case ".jsonld": return MimeTypes.Jsonld;
                case ".ico": return MimeTypes.Ico;
                case ".epub": return MimeTypes.Epub;
                //Other
                case ".azw": return MimeTypes.Azw;
                case ".otf": return MimeTypes.Otf;
                case ".ttf": return MimeTypes.Ttf;
                case ".woff": return MimeTypes.Woff;
                case ".woff2": return MimeTypes.Woff2;
                case ".txt": return MimeTypes.Txt;
                case ".xml": return MimeTypes.Xml;
                
                default: return MimeTypes.Default; //default for every else
            }
        }
    }
}