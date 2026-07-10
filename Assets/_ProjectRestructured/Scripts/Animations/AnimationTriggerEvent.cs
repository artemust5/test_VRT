using UnityEngine;
using UnityEngine.Events;

namespace _ProjectRestructured.Scripts.Animations
{
    public class AnimationTriggerEvent : MonoBehaviour
    {
        [Tooltip("Animator с анимацией")]
        public Animator animator;

        [Tooltip("Название клипа, за которым нужно следить")]
        public string animationClipName;

        [Tooltip("Порог срабатывания (от 0 до 1). По умолчанию 1.0 = 100%")]
        [Range(0f, 1f)]
        public float triggerThreshold = 1.0f;

        [Tooltip("Событие, которое сработает")]
        public UnityEvent onAnimationReachedThreshold;

        private bool hasTriggered = false;

        void Update()
        {
            if (hasTriggered || animator == null || string.IsNullOrEmpty(animationClipName))
                return;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(animationClipName))
            {
                float normalizedTime = stateInfo.normalizedTime;

                if (normalizedTime >= triggerThreshold)
                {
                    hasTriggered = true;
                    onAnimationReachedThreshold.Invoke();
                }
            }
        }
    }
}