using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right; // 기본은 좌우
    public float moveDistance = 2f;
    public float moveSpeed = 1f;

    private Vector2 startPos;
    private bool movingForward = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime;
        Vector2 target = startPos + (movingForward ? moveDirection.normalized * moveDistance : -moveDirection.normalized * moveDistance);

        transform.position = Vector2.MoveTowards(transform.position, target, step);

        if (Vector2.Distance(transform.position, target) < 0.01f)
        {
            movingForward = !movingForward; // 방향 반전
        }
    }
}