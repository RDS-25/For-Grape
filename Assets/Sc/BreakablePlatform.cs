using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class BreakablePlatform : MonoBehaviour
{
    public float breakDelay = 0.5f;
    public float respawnDelay = 3f; // 다시 나타날 시간

    private bool isBreaking = false;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreaking) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 위에서 밟았을 때만 반응
                if (collision.relativeVelocity.y <= 0f && contact.point.y > transform.position.y)
                {
                    StartCoroutine(BreakAndRespawn());
                    break;
                }
            }
        }
    }

    IEnumerator BreakAndRespawn()
    {
        isBreaking = true;

        // 깜빡이기 연출
        float blinkTime = breakDelay / 5f;
        for (int i = 0; i < 5; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(blinkTime / 2);
            sr.enabled = true;
            yield return new WaitForSeconds(blinkTime / 2);
        }

        // 사라지기
        sr.enabled = false;
        col.enabled = false;

        // 일정 시간 후 복구 (재사용 위해)
        yield return new WaitForSeconds(respawnDelay);

        sr.enabled = true;
        col.enabled = true;
        isBreaking = false;
    }
}