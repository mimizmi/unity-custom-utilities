using System;

namespace Mimizh.UnityUtilities
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string val) => string.IsNullOrEmpty(val);
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);
        
        public static bool IsBlank(this string val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();
        
        public static string OrEmpty(this string val) => val ?? string.Empty;

        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank()) return val;
            return val.Length <= maxLength ? val : val.Substring(0, maxLength - 3) + "...";
        }

        public static string Slice(this string val, int start, int end)
        {
            if (val.IsBlank()) 
            {
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");
            }

            if (start < 0 || start > val.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            } 
            end = end < 0 ? val.Length + end : end;

            if (end < 0 || end < start || end > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(end));
            }
            return val.Substring(start, end - start);
        }
        
        // Rich text formatting, for Unity UI elements that support rich text.
        public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";
        public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";
        public static string RichBold(this string text) => $"<b>{text}</b>";
        public static string RichItalic(this string text) => $"<i>{text}</i>";
        public static string RichUnderline(this string text) => $"<u>{text}</u>";
        public static string RichStrikethrough(this string text) => $"<s>{text}</s>";
        public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";
        public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";
        public static string RichGradient(this string text, string color1, string color2) => $"<gradient={color1},{color2}>{text}</gradient>";
        public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";
        public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";
    }
}