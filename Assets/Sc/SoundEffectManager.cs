using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        GameObject temp = new GameObject("TempSound");
        temp.transform.position = position;
        Debug.Log("소리나옴");

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        Destroy(temp, clip.length);
    }
}
