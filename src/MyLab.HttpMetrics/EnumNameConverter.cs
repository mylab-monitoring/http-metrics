using System;
using System.Collections.Generic;
using System.Text;

namespace MyLab.HttpMetrics
{
    /// <summary>
    /// Converts <see cref="Enum"/> for metrics
    /// </summary>
    public static class EnumConverter
    {
        public static string ToLabel(Enum enumVal)
        {
            var items = new List<string>();
            var enumString = enumVal.ToString();

            StringBuilder buff = null;
            char? lastChar = null;

            for (int i = 0; i < enumString.Length; i++)
            {
                char ch = enumString[i];

                if(!char.IsLetterOrDigit(ch)) continue;

                if (char.IsUpper(ch) || (char.IsDigit(ch) && lastChar.HasValue && !char.IsDigit(lastChar.Value)))
                {
                    ApplyBuffer();
                }

                buff ??= new StringBuilder();
                
                buff.Append(char.ToLower(ch).ToString());

                lastChar = ch;
            }

            if (buff != null && buff.Length != 0)
                items.Add(buff.ToString());

            return string.Join('_', items);

            void ApplyBuffer()
            {
                if (buff != null && buff.Length != 0)
                    items.Add(buff.ToString());

                buff = new StringBuilder();
            }

        }
    }
}
