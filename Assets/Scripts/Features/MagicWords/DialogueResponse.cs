using System;

namespace UnityMiniDemos.Features.MagicWords
{
    [Serializable]
    public sealed class DialogueResponse
    {
        public DialogueEntry[] dialogue;
        public AvatarData[] avatars;
    }

    [Serializable]
    public sealed class DialogueEntry
    {
        public string name;
        public string text;
    }

    [Serializable]
    public sealed class AvatarData
    {
        public string name;
        public string url;
        public string position;
    }
}