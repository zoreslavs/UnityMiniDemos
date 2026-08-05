using System.Collections.Generic;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class EmojiTextProcessor
    {
        private static readonly Dictionary<string, string> EmojiMap = new()
        {
            ["{satisfied}"] = "😊",
            ["{intrigued}"] = "🤔",
            ["{neutral}"] = "😐",
            ["{affirmative}"] = "👍",
            ["{laughing}"] = "😂",
            ["{win}"] = "🏆",
            ["{sad}"] = "😞",
            ["{crying}"] = "😢",
            ["{angry}"] = "😠",
            ["{cool}"] = "😎",
            ["{heart}"] = "❤",
            ["{party}"] = "🎉"
        };

        public string Process(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            foreach (var pair in EmojiMap)
            {
                text = text.Replace(pair.Key, pair.Value);
            }

            return text;
        }
    }
}