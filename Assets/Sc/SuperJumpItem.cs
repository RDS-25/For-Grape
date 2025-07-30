using UnityEngine;

public class SuperJumpItem : MonoBehaviour, IItemEffect
{
    public AudioClip sound;

    public void ApplyEffect(GameObject player)
    {
        player.GetComponent<PlayerController>()?.BoostJump(1.5f);
        SoundEffectManager.Instance?.PlaySound(sound, transform.position);
    }
}