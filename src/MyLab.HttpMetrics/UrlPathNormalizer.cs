using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLab.HttpMetrics
{
    static class UrlPathNormalizer
    {
        public static string Normalize(string origin)
        {
            if (string.IsNullOrEmpty(origin))
                return "~";

            var norm = origin.TrimEnd('/');

            if (!norm.StartsWith('~') && !norm.StartsWith('/'))
                norm = "/" + norm;

            norm = MaskVariableParts(norm);

            return norm;
        }

        private static string MaskVariableParts(string path)
        {
            var items = path.ToLower().Split('/').ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var itm = items[i];

                if (itm.Count(char.IsDigit) > itm.Length / 2 ||
                    Guid.TryParse(itm, out _) ||
                    Regex.IsMatch(itm, "[\\d]{3,}"))
                {
                    items[i] = "xxx";
                }
            }

            return string.Join('/', items);
        }
    }
}