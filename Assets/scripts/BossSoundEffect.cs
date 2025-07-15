using UnityEngine;

public class BossSoundEffect : MonoBehaviour
{
    public void PlayFootstepSound()
    {
        SoundEffectManager.PlayAtPosition("GolemWalk",transform.position, 1f, 8f);
    }

    public void PlayAttackSound()
    {
        SoundEffectManager.Play("GolemAttack");
    }
}