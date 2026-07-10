using System.Collections;
using UnityEngine;

namespace _Project._Scripts
{
    public abstract class SceneTransition : MonoBehaviour
    {
        public abstract IEnumerator AnimateTransitionIn();
        public abstract IEnumerator AnimateTransitionOut();
    }
}