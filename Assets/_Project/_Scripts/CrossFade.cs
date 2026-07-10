using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _Project._Scripts
{
    public class CrossFade : SceneTransition
    {
        public CanvasGroup crossFade;

        public override IEnumerator AnimateTransitionIn()
        {
            var tween = crossFade.DOFade(1f, 1f);
            yield return tween.WaitForCompletion();
        }

        public override IEnumerator AnimateTransitionOut()
        {
            var tween = crossFade.DOFade(0f, 1f);
            yield return tween.WaitForCompletion();
        }
    }
}