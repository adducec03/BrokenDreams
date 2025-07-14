using UnityEngine;

public class EnemySoundEvents : MonoBehaviour
{
    public void PlayFootstepSound()
    {
        SoundEffectManager.Play("SkeletonWalk");
    }

    public void PlaySlimeFootstepSound()
    {
        SoundEffectManager.Play("SlimeWalk");
    }
}
