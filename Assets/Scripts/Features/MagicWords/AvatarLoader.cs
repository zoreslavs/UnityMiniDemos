using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class AvatarLoader
    {
        public IEnumerator Load(AvatarData[] avatars, Action<Dictionary<string, Texture2D>> onCompleted)
        {
            var textures = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
            var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (avatars == null)
            {
                onCompleted?.Invoke(textures);
                yield break;
            }

            foreach (var avatar in avatars)
            {
                if (!IsValidAvatar(avatar) || !processedNames.Add(avatar.name))
                    continue;

                yield return LoadTexture(avatar, textures);
            }

            onCompleted?.Invoke(textures);
        }

        private static IEnumerator LoadTexture(AvatarData avatar, Dictionary<string, Texture2D> textures)
        {
            using var request = UnityWebRequestTexture.GetTexture(avatar.url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Avatar request failed for {avatar.name}: {request.error}");
                yield break;
            }

            var texture = DownloadHandlerTexture.GetContent(request);

            if (texture != null)
                textures[avatar.name] = texture;
        }

        private static bool IsValidAvatar(AvatarData avatar)
        {
            if (avatar == null || string.IsNullOrWhiteSpace(avatar.name) || string.IsNullOrWhiteSpace(avatar.url))
                return false;

            return string.Equals(avatar.position, "left", StringComparison.OrdinalIgnoreCase) || 
                   string.Equals(avatar.position, "right", StringComparison.OrdinalIgnoreCase);
        }
    }
}