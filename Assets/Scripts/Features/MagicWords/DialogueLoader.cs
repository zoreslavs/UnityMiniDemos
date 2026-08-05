using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class DialogueLoader
    {
        private const string EndpointUrl = "https://private-624120-softgamesassignment.apiary-mock.com/v3/magicwords";
        private const string LoadErrorMessage = "Unable to load messages.";

        public IEnumerator Load(Action<DialogueResponse, string> onCompleted)
        {
            using var request = UnityWebRequest.Get(EndpointUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Magic Words request failed: {request.error}");
                onCompleted?.Invoke(null, LoadErrorMessage);
                yield break;
            }

            DialogueResponse response;

            try
            {
                response = JsonUtility.FromJson<DialogueResponse>(request.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Magic Words response parsing failed: {exception.Message}");
                onCompleted?.Invoke(null, LoadErrorMessage);
                yield break;
            }

            if (response == null)
            {
                Debug.LogError("Magic Words response is empty.");
                onCompleted?.Invoke(null, LoadErrorMessage);
                yield break;
            }

            onCompleted?.Invoke(response, null);
        }
    }
}