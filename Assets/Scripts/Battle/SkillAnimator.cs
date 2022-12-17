using UnityEngine;
using GameManager.Hub;

public class SkillAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Play(string anim, Transform location) {
        animator.Play(anim);
        transform.position = location.position;
    }

    private void OnDestroy() => WorldAnimations.Remove(this);

    public void FinishedPlaying() => Destroy(gameObject);
}
