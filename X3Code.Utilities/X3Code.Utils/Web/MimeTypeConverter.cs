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
                    return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".bmp": return "image/bmp";
                case ".svg": return "image/svg+xml";
                case ".tiff": return "image/tiff";
                case ".tif": return "image/tiff";
                case ".webp": return "image/webp";
                //Binary files
                case ".pdf": return "application/pdf";
                case ".zip": return "application/zip";
                case ".7z": return "application/x-7z-compressed";
                case ".gz": return "application/gzip";
                case ".bz": return "application/x-bzip";
                case ".bz2": return "application/x-bzip2";
                case ".rar": return "application/vnd.rar";
                case ".jar": return "application/java-archive";
                case ".tar": return "application/x-tar";
                case ".mpkg": return "application/vnd.apple.installer+xml";
                //Audio
                case ".aac": return "audio/aac";
                case ".oga": return "audio/ogg";
                case ".mid": return "audio/midi";
                case ".midi": return "audio/x-midi";
                case ".mp3": return "audio/mpeg";
                case ".wav": return "audio/wav";
                case ".weba": return "audio/weba";
                //Video
                case ".avi": return "video/x-msvideo";
                case ".mp4": return "video/mpeg";
                case ".ogv": return "video/ogg";
                case ".webm": return "video/webm";
                //Office
                case ".csv": return "text/csv";
                case ".rtf": return "application/rtf";
                case ".doc":  return "application/msword";
                case ".xls":  return "application/vnd.ms-excel";
                case ".ppt":  return "application/vnd.ms-powerpoint";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".abi":  return "application/x-abiword";
                case ".ics":  return "text/calendar";
                //Web
                case ".css": return "text/css";
                case ".html": return "text/html";
                case ".php": return "text/php";
                case ".js": return "text/javascript";
                case ".json": return "text/json";
                case ".jsonld": return "text/ld+json";
                case ".ico": return "image/vnd.microsoft.icon";
                case ".epub": return "application/epub+zip";
                //Other
                case ".azw": return "application/vnd.amazon.ebook";
                case ".otf": return "font/otf";
                case ".ttf": return "font/ttf";
                case ".woff": return "font/woff";
                case ".woff2": return "font/woff2";
                case ".txt": return "text/plain";
                case ".xml": return "text/xml";
                
                default: return "application/octet-stream"; //default for every else
            }
        }
    }
}