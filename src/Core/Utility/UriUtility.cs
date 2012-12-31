using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace NuGet
{
    internal static class UriUtility
    {
        /// <summary>
        /// Converts a uri to a path. Only used for local paths.
        /// </summary>
        internal static string GetPath(Uri uri)
        {
            string path = uri.OriginalString;
            if (path.StartsWith("/", StringComparison.Ordinal))
            {
                path = path.Substring(1);
            }

            // Bug 483: We need the unescaped uri string to ensure that all characters are valid for a path.
            // Change the direction of the slashes to match the filesystem.
            return Uri.UnescapeDataString(path.Replace('/', Path.DirectorySeparatorChar));
        }

        internal static Uri CreatePartUri(string path)
        {
            // Only the segments between the path separators should be escaped
            var segments = path.Split( new[] { '/', Path.DirectorySeparatorChar }, StringSplitOptions.None)
                               .Select(Uri.EscapeDataString);
            var escapedPath = String.Join("/", segments);
            return PackUriHelper.CreatePartUri(new Uri(escapedPath, UriKind.Relative));
        }

        // Bug 2379: SettingsCredentialProvider does not work
        private static Uri CreateODataAgnosticUri(string uri)
        {
            if (uri.EndsWith("$metadata", StringComparison.OrdinalIgnoreCase))
            {
                uri = uri.Substring(0, uri.Length - 9).TrimEnd('/');
            }
            return new Uri(uri);
        }

        /// <summary>
        /// Determines if the scheme, server and path of two Uris are identical.
        /// </summary>
        public static bool UriEquals(Uri uri1, Uri uri2)
        {
            uri1 = CreateODataAgnosticUri(uri1.OriginalString.TrimEnd('/'));
            uri2 = CreateODataAgnosticUri(uri2.OriginalString.TrimEnd('/'));

            return Uri.Compare(uri1, uri2, UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }
        
        /// <summary>
        /// Workaround Mono issue with Uri.MakeRelativeUri not returning "../" in relative uri.
        /// 
        /// Taken from latest Mono Uri class.
        /// 
        /// https://github.com/mono/mono/blob/master/mcs/class/System/System/Uri.cs
        /// Commit: 8c7c1ff5f325ad34d38341c93e80b81ce9c553ae
        /// </summary>
        public static Uri MakeRelativeUri (Uri source, Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException ("uri");
            }
            if (source.Host != uri.Host || source.Scheme != uri.Scheme)
                return uri;
            
            string result = String.Empty;
            if (source.AbsolutePath != uri.AbsolutePath){
                string [] segments = source.Segments;
                string [] segments2 = uri.Segments;
                
                int k = 0;
                int max = Math.Min (segments.Length, segments2.Length);
                for (; k < max; k++)
                    if (segments [k] != segments2 [k]) 
                        break;
                
                for (int i = k; i < segments.Length && segments [i].EndsWith ("/"); i++)
                    result += "../";
                for (int i = k; i < segments2.Length; i++)
                    result += segments2 [i];
                
                if (result == string.Empty)
                    result = "./";
            }

            return new Uri (result, UriKind.Relative);
        }
    }
}
