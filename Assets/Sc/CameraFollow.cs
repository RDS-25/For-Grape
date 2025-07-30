using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    private float highestY;

    public GameObject startPlatform;
    public GameObject deadZone;
    public float activationHeight = 5f;  // DeadZone 활성화 높이

    private bool activated = false;
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
        highestY = transform.position.y;

        if (deadZone != null)
            deadZone.SetActive(false); // DeadZone 시작 시 비활성화
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetY = target.position.y;

        // 살아있을 땐, 올라가는 경우에만 따라감
        if (!gm.IsDead)
        {
            if (targetY > highestY)
            {
                highestY = targetY;
            }
        }
        else
        {
            // 죽은 이후엔 아래도 따라감
            highestY = targetY;
        }

        // DeadZone 활성화 조건 체크
        if (!activated && highestY > activationHeight)
        {
            if (deadZone != null) deadZone.SetActive(true);
            if (startPlatform != null) startPlatform.SetActive(false);
            activated = true;
        }

        Vector3 newPos = new Vector3(transform.position.x, highestY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
    }
    
}