using UnityEngine;

public class BossSoundEffect : MonoBehaviour
{
    public void PlayFootstepSound()
    {
        SoundEffectManager.Play("GolemWalk");
    }

    public void PlayAttackSound()
    {
        SoundEffectManager.Play("GolemAttack");
    }
}