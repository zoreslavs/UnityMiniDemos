using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class DialogueController : MonoBehaviour
    {
        private const string LoadingMessage = "Loading dialogue...";
        private const string EmptyMessage = "No dialogue available.";
        private const string FinishMessage = "The dialogue is finished.";

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;
        [SerializeField] private DialogueItemView _dialogueItemPrefab;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private float _messageMinInterval = 1f;
        [SerializeField] private float _messageMaxInterval = 3f;
        [SerializeField] private float _scrollDuration = 0.35f;

        private readonly DialogueLoader _loader = new DialogueLoader();
        private readonly AvatarLoader _avatarLoader = new AvatarLoader();
        private readonly EmojiTextProcessor _emojiTextProcessor = new EmojiTextProcessor();
        private Coroutine _loadRoutine;

        private void OnEnable()
        {
            _retryButton?.onClick.AddListener(LoadDialogue);
        }

        private void Start()
        {
            LoadDialogue();
        }

        private void OnDisable()
        {
            _retryButton?.onClick.RemoveListener(LoadDialogue);

            if (_loadRoutine != null)
            {
                StopCoroutine(_loadRoutine);
                _loadRoutine = null;
            }
        }

        private void LoadDialogue()
        {
            if (!HasValidSetup())
                return;

            if (_loadRoutine != null)
                StopCoroutine(_loadRoutine);

            ClearMessages();
            SetStatus(LoadingMessage);
            _loadRoutine = StartCoroutine(LoadDialogueRoutine());
        }

        private IEnumerator LoadDialogueRoutine()
        {
            DialogueResponse response = null;
            string errorMessage = null;

            yield return _loader.Load((loadedResponse, loadError) =>
            {
                response = loadedResponse;
                errorMessage = loadError;
            });

            if (!isActiveAndEnabled || !TryHandleResponse(response, errorMessage))
                yield break;

            Dictionary<string, Texture2D> avatarTextures = null;

            yield return _avatarLoader.Load(response.avatars, loadedTextures => avatarTextures = loadedTextures);

            if (!isActiveAndEnabled)
                yield break;

            var dialogueEntries = GetDialogueEntries(response.dialogue);

            if (dialogueEntries.Count == 0)
            {
                _loadRoutine = null;
                SetStatus(EmptyMessage);
                yield break;
            }

            yield return ShowDialogueRoutine(response, avatarTextures, dialogueEntries);

            _loadRoutine = null;
            SetStatus(FinishMessage);
        }

        private IEnumerator ShowDialogueRoutine(DialogueResponse response, Dictionary<string, Texture2D> avatarTextures, List<DialogueEntry> dialogueEntries)
        {
            SetStatus(null);

            for (var index = 0; index < dialogueEntries.Count; index++)
            {
                CreateMessage(response, avatarTextures, dialogueEntries[index]);

                yield return null;
                yield return ScrollToLatestMessage();

                if (index < dialogueEntries.Count - 1)
                    yield return new WaitForSeconds(UnityEngine.Random.Range(_messageMinInterval, _messageMaxInterval));
            }
        }

        private static List<DialogueEntry> GetDialogueEntries(DialogueEntry[] dialogue)
        {
            var dialogueEntries = new List<DialogueEntry>();

            foreach (var dialogueEntry in dialogue)
            {
                if (dialogueEntry != null)
                    dialogueEntries.Add(dialogueEntry);
            }

            return dialogueEntries;
        }

        private bool TryHandleResponse(DialogueResponse response, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _loadRoutine = null;
                SetStatus(errorMessage, true);
                return false;
            }

            if (response == null || response.dialogue == null || response.dialogue.Length == 0)
            {
                _loadRoutine = null;
                SetStatus(EmptyMessage);
                return false;
            }

            return true;
        }

        private void CreateMessage(DialogueResponse response, Dictionary<string, Texture2D> avatarTextures, DialogueEntry dialogueEntry)
        {
            var avatarData = FindAvatar(response.avatars, dialogueEntry.name);
            var avatarTexture = FindTexture(avatarTextures, dialogueEntry.name);
            var dialogueItem = Instantiate(_dialogueItemPrefab, _content);
            var message = _emojiTextProcessor.Process(dialogueEntry.text);
            dialogueItem.Configure(dialogueEntry.name, message, avatarTexture, IsLeftPosition(avatarData));
        }

        private static Texture2D FindTexture(Dictionary<string, Texture2D> textures, string characterName)
        {
            if (textures == null || string.IsNullOrWhiteSpace(characterName))
                return null;

            textures.TryGetValue(characterName, out var texture);
            return texture;
        }

        private static AvatarData FindAvatar(AvatarData[] avatars, string characterName)
        {
            if (avatars == null || string.IsNullOrWhiteSpace(characterName))
                return null;

            foreach (var avatar in avatars)
            {
                if (avatar == null || !string.Equals(avatar.name, characterName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (string.Equals(avatar.position, "left", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(avatar.position, "right", StringComparison.OrdinalIgnoreCase))
                    return avatar;
            }

            return null;
        }

        private void ClearMessages()
        {
            for (var index = _content.childCount - 1; index >= 0; index--)
            {
                Destroy(_content.GetChild(index).gameObject);
            }
        }

        private IEnumerator ScrollToLatestMessage()
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.velocity = Vector2.zero;

            var startPosition = _scrollRect.verticalNormalizedPosition;
            var elapsed = 0f;

            while (elapsed < _scrollDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var progress = Mathf.Clamp01(elapsed / _scrollDuration);
                _scrollRect.verticalNormalizedPosition = Mathf.SmoothStep(startPosition, 0f, progress);
                yield return null;
            }

            _scrollRect.verticalNormalizedPosition = 0f;
        }

        private void SetStatus(string message, bool showRetry = false)
        {
            _statusText.gameObject.SetActive(message != null);
            _retryButton.gameObject.SetActive(showRetry);

            if (message != null)
                _statusText.SetText(message);
        }

        private static bool IsLeftPosition(AvatarData avatar)
        {
            return avatar != null && string.Equals(avatar.position, "left", StringComparison.OrdinalIgnoreCase);
        }

        private bool HasValidSetup()
        {
            if (_scrollRect == null || _content == null || _dialogueItemPrefab == null)
            {
                Debug.LogError("DialogueController requires scroll rect, content, and dialogue item prefab.", this);
                return false;
            }

            if (_statusText == null || _retryButton == null)
            {
                Debug.LogError("DialogueController requires status text and retry button.", this);
                return false;
            }

            return true;
        }
    }
}