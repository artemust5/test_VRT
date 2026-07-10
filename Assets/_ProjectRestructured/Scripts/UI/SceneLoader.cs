using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _ProjectRestructured.Scripts.UI
{
    public class SceneLoader : MonoBehaviour
    {
        public string menuScene;
        public Canvas canvas;

        public Image fadeImage;
        public float fadeDuration = 2f;

        public bool turnAudioVolumeOnAtStart;

        private static SceneLoader instance;

        public static SceneLoader Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindObjectOfType<SceneLoader>();

                if (instance != null) return instance;
                var singletonObject = new GameObject(nameof(SceneLoader));
                instance = singletonObject.AddComponent<SceneLoader>();
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

            var color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;

            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(Scene sceneName, LoadSceneMode loadMode)
        {
            if (sceneName.name == menuScene || turnAudioVolumeOnAtStart)
            {
                AudioListener.volume = 1f;
            }
            canvas.worldCamera = Camera.main;
            StartCoroutine(FadeIn());
            canvas.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
            }
        }

        public void LoadNextScene()
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);

            if (string.IsNullOrEmpty(scenePath)) return;
            var sceneName = Path.GetFileNameWithoutExtension(scenePath);
            LoadScene(sceneName);
        }

        public void LoadMenuScene()
        {
            LoadScene(menuScene);
        }

        public void RestartScene()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            LoadScene(currentSceneName);
        }

        public void LoadScene(string sceneName)
        {
            canvas.gameObject.SetActive(true);
            canvas.worldCamera = Camera.main;
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Fade out
            var elapsedTime = 0f;
            var color = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            // Load scene
            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene!.allowSceneActivation = false;

            while (scene.progress < 0.9f) yield return null;
            yield return new WaitForSeconds(1f);

            scene.allowSceneActivation = true;
            AudioListener.volume = 0f;
        }

        private IEnumerator FadeIn()
        {
            var elapsedTime = 0f;
            var color = fadeImage.color;
            yield return new WaitForSeconds(fadeDuration);

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
        }

        public void EnableBlackScreen(bool status)
        {
            var color = fadeImage.color;
            color.a = status ? 1f : 0f;
            fadeImage.color = color;
        }
    }
}