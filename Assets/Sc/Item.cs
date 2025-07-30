using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { ScoreBoost, SuperJump }

    public ItemType type;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
            Destroy(gameObject); // 아이템은 1회용
        }
    }

    void ApplyEffect(GameObject player)
    {
        switch (type)
        {
            case ItemType.ScoreBoost:
                GameManager.Instance?.AddScore(10.0f);
                break;

            case ItemType.SuperJump:
                player.GetComponent<PlayerController>().BoostJump(1.5f); // 점프력 1.5배
                break;
        }
    }
}