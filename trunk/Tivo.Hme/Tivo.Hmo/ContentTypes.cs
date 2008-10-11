using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivo.Hmo
{
    public static class ContentTypes
    {
        public const string VideoContainer = "x-tivo-container/tivo-videos";
        public const string FolderContainer = "x-tivo-container/folder";
        public const string TivoRawVideo = "video/x-tivo-raw-tts";

        public static bool IsContainer(string contentType)
        {
            return contentType.StartsWith("x-tivo-container/");
        }

        public static bool IsVideo(string contentType)
        {
            return contentType.StartsWith("video/");
        }
    }
}
