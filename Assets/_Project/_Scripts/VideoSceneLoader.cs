using UnityEngine;
using UnityEngine.Video;

namespace _Project._Scripts
{
    public class VideoSceneLoader : MonoBehaviour
    {
        public VideoPlayer videoPlayer;
        public string nextSceneName;

        private void Start()
        {
            if (videoPlayer == null)
            {
                videoPlayer = GetComponent<VideoPlayer>();
            }

            videoPlayer.loopPointReached += OnVideoFinished;
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            LevelManager.LoadScene(nextSceneName);
        }
    }
}