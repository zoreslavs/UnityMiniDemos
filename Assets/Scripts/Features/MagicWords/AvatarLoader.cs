using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class AvatarLoader
    {
        private const int RequestTimeoutSeconds = 10;

        private readonly Dictionary<string, Texture2D> _textures = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _pendingNames = new(StringComparer.OrdinalIgnoreCase);

        public bool TryGetTexture(string characterName, out Texture2D texture)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                texture = null;
                return false;
            }

            return _textures.TryGetValue(characterName, out texture);
        }

        public IEnumerator LoadAll(AvatarData[] avatars)
        {
            if (avatars == null)
                yield break;

            var pending = new List<(string Name, UnityWebRequest Request)>();

            foreach (var avatar in avatars)
            {
                if (!IsValidAvatar(avatar) || !_pendingNames.Add(avatar.name))
                    continue;

                var request = UnityWebRequestTexture.GetTexture(avatar.url);
                request.timeout = RequestTimeoutSeconds;
                request.SendWebRequest();
                pending.Add((avatar.name, request));
            }

            foreach (var (name, request) in pending)
            {
                while (!request.isDone)
                {
                    yield return null;
                }

                TakeTexture(name, request);
                request.Dispose();
                _pendingNames.Remove(name);
            }
        }

        public IEnumerator WaitForAvatar(string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
                yield break;

            while (_pendingNames.Contains(characterName))
            {
                yield return null;
            }
        }

        public void Clear()
        {
            _pendingNames.Clear();

            foreach (var texture in _textures.Values)
            {
                if (texture != null)
                    UnityEngine.Object.Destroy(texture);
            }

            _textures.Clear();
        }

        private void TakeTexture(string characterName, UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Avatar request failed for {characterName}: {request.error}");
                return;
            }

            var texture = DownloadHandlerTexture.GetContent(request);

            if (texture != null)
                _textures[characterName] = texture;
        }

        private static bool IsValidAvatar(AvatarData avatar)
        {
            if (avatar == null || string.IsNullOrWhiteSpace(avatar.name) || string.IsNullOrWhiteSpace(avatar.url))
                return false;

            return AvatarPosition.IsKnown(avatar.position);
        }
    }
}