using UnityEngine.SceneManagement;
using UnityEngine;

namespace UnityMiniDemos.Core
{
    public sealed class SceneNavigation : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        private const string AceOfShadowsSceneName = "AceOfShadows";
        private const string MagicWordsSceneName = "MagicWords";
        private const string PhoenixFlameSceneName = "PhoenixFlame";

        public void LoadMainMenu()
        {
            LoadScene(MainMenuSceneName);
        }

        public void LoadAceOfShadows()
        {
            LoadScene(AceOfShadowsSceneName);
        }

        public void LoadMagicWords()
        {
            LoadScene(MagicWordsSceneName);
        }

        public void LoadPhoenixFlame()
        {
            LoadScene(PhoenixFlameSceneName);
        }

        private static void LoadScene(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene '{sceneName}' is not included in the Build Settings.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}