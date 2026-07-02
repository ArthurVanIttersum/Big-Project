using UnityEngine;

public class AnimationSpeedBall : MonoBehaviour
{
    public Animator animator;
    public float multiplier = 0.1f;

    public void SetAnimationSpeed(float speed)
    {
        animator.SetFloat("Speed", speed * multiplier);
    }

    private void OnEnable()
    {
        TestMovement script = FindAnyObjectByType<TestMovement>();
        script.animationSpeed += SetAnimationSpeed;
    }
}
