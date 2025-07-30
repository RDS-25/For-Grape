using UnityEngine;

public class ScoreBoostItem : MonoBehaviour, IItemEffect
{
    public AudioClip sound;

    public void ApplyEffect(GameObject player)
    {
        GameManager.Instance?.AddScore(10.0f);
        SoundEffectManager.Instance?.PlaySound(sound, transform.position);
    }
}