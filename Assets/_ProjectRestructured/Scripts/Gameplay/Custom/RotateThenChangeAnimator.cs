using UnityEngine;

public class RotateThenChangeAnimator : MonoBehaviour
{
    public Transform target;
    public Animator animator;
    public RuntimeAnimatorController newController;
    [Range(0f, 1f)] public float animationStartNormalizedTime = 0f;
    public float rotationSpeed = 180f; // градусов в секунду
    public float angleThreshold = 1f; // когда считать поворот завершённым

    private bool hasRotated = false;

    private void Update()
    {
        if (hasRotated || target == null) return;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0f; // игнорируем вертикальный наклон

        if (directionToTarget == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        if (angle <= angleThreshold)
        {
            hasRotated = true;
            ApplyNewAnimatorController();
        }
    }

    private void ApplyNewAnimatorController()
    {
        if (animator == null || newController == null) return;

        animator.runtimeAnimatorController = newController;
        animator.Play(0, 0, animationStartNormalizedTime);
    }
}
