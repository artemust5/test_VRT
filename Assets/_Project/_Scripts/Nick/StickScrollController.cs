using UnityEngine;
using System.Collections;

namespace _Project._Scripts.Nick
{
    public class StickScrollController : MonoBehaviour
    {
        public RectTransform viewport;
        public RectTransform content;
        public int amountOfVisibleButtons = 3;
        public float scrollDuration = 0.5f;
        private int scrollIndex = 1;
        public UINavigator uiNavigator;


        private void Start()
        {
            uiNavigator.OnButtonSelected += OnButtonSelected;
        }

        private void OnButtonSelected(int currentButtonIndex)
        {
            currentButtonIndex++;
            var newScrollIndex = Mathf.CeilToInt((float)currentButtonIndex / amountOfVisibleButtons);

            if (newScrollIndex == scrollIndex) return;
            scrollIndex = newScrollIndex;
            var newPositionY = viewport.rect.height * (scrollIndex - 1);
            StartCoroutine(SmoothScroll(newPositionY));
        }

        private IEnumerator SmoothScroll(float targetY)
        {
            var startPosition = content.anchoredPosition;
            var targetPosition = new Vector2(startPosition.x, targetY);
            var elapsedTime = 0f;

            while (elapsedTime < scrollDuration)
            {
                content.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / scrollDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            content.anchoredPosition = targetPosition;
        }
    }
}