using UnityEngine;

public class EnemySoundEvents : MonoBehaviour
{
    public void PlayFootstepSound()
    {
        SoundEffectManager.PlayAtPosition("SkeletonWalk",transform.position, 1f, 8f);
    }

    public void PlaySlimeFootstepSound()
    {
        SoundEffectManager.PlayAtPosition("SlimeWalk", transform.position, 1f, 8f);
    }
}
