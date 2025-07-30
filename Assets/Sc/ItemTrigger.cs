using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && TryGetComponent(out IItemEffect effect))
        {
            effect.ApplyEffect(other.gameObject);
            Destroy(gameObject); // 또는 SetActive(false) 등
        }
    }
}