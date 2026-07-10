using System.Collections;
using System.Linq;
using _ProjectRestructured.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project._Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        public Slider progressBar;
        public GameObject transitionsContainer;

        private SceneTransition[] _transitions;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
        }

        public void LoadScene(string sceneName, string transitionName)
        {
            StartCoroutine(LoadSceneAsync(sceneName, transitionName));
            //PauseManager.ResumeGame();
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            //PauseManager.ResumeGame();
        }

        private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
        {
            var transition = _transitions.First(t => t.name == transitionName);

            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            yield return transition.AnimateTransitionIn();

            progressBar.gameObject.SetActive(true);

            do
            {
                progressBar.value = scene.progress;
                yield return null;
            } while (scene.progress < 0.9f);

            yield return new WaitForSeconds(1f);

            scene.allowSceneActivation = true;

            progressBar.gameObject.SetActive(false);

            yield return transition.AnimateTransitionOut();
        }
    }
}