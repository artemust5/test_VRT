using UnityEngine;

namespace _ProjectRestructured.Scripts.UI
{
    public class SceneHandler : MonoBehaviour
    {
        public void LoadNextScene()
        {
            SceneLoader.Instance.LoadNextScene();
        }

        public void LoadScene(string sceneName)
        {
            SceneLoader.Instance.LoadScene(sceneName);
        }

        public void RestartScene()
        {
            SceneLoader.Instance.RestartScene();   
        }

        public void LoadMenuScene()
        {
            SceneLoader.Instance.LoadMenuScene();
        }

        public void EnableBlackScreen()
        {
            SceneLoader.Instance.EnableBlackScreen(true);
        }

        public void DisableBlackScreen()
        {
            SceneLoader.Instance.EnableBlackScreen(false);
        }
    }
}
