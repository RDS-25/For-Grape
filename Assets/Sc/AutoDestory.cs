using UnityEngine;

public class AutoDestory : MonoBehaviour
{
    public float delay = 1.0f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
