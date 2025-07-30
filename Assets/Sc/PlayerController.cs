using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;

    private float NotOutScreen;
    private bool isGrounded = false;
    private bool isFallingToDeath = false;

    private float moveDir = 0f;
    int originalLayer;

    // Jump/Fall Sprites
    public Sprite jumpSprite;
    public Sprite fallSprite;
    private Sprite defaultSprite;
    
    public AudioClip jumpClip;
    public AudioClip fallClip;
    public AudioClip DieClip;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // 추가

        defaultSprite = sr.sprite;
        anim.enabled = false;

        float halfPlayerWidth = sr.bounds.extents.x;
        NotOutScreen = Camera.main.aspect * Camera.main.orthographicSize - halfPlayerWidth;
    }

    void Update()
    {
        if (isFallingToDeath)
        {
            rb.linearVelocity = new Vector2(0f, -10f); // 원하는 낙하 속도 설정
            return;
        }

        HandleInput();
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        if (isGrounded)
        {
            Jump();
            isGrounded = false;
        }

        UpdateSpriteByVelocity();
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -NotOutScreen, NotOutScreen);
        transform.position = pos;
    }

    void HandleInput()
    {
#if UNITY_EDITOR
        if (Keyboard.current.leftArrowKey.isPressed)
            moveDir = -1f;
        else if (Keyboard.current.rightArrowKey.isPressed)
            moveDir = 1f;
        else
            moveDir = 0f;
#endif
    }

    public void MoveLeftDown() => moveDir = -1f;
    public void MoveRightDown() => moveDir = 1f;
    public void StopMove() => moveDir = 0f;

    void Jump()
    {
        Debug.Log("Jump");  // 점프는 작동하므로 호출 확인 가능

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
            return;
        }
        if (jumpClip == null)
        {
            Debug.LogError("JumpClip is null!");
            return;
        }

        Debug.Log($"Playing jump clip. Volume: {audioSource.volume}, Pitch: {audioSource.pitch}");
        audioSource.PlayOneShot(jumpClip, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFallingToDeath) return;

        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y > 0.5f)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    //낙하 애니메이션 
    public void StartDeathFall()
    {
        isFallingToDeath = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f; // 중력 제거
        moveDir = 0f;

        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("IgnorePlatform");

        // 낙사 애니메이션 재생 및 소리 재생
        if (anim != null)
        {
            anim.enabled = true;
            audioSource.clip = fallClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFallingToDeath) return;

        if (other.CompareTag("DeathZone"))
        {
            GameManager.Instance.SpawnDeathEffect(transform.position);
            gameObject.SetActive(false);
            GameManager.Instance.HandleGameOver();
        }
    }
  

    public void BoostJump(float multiplier)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * multiplier);
    }

    void UpdateSpriteByVelocity()
    {
        if (rb.linearVelocity.y > 0.1f)
        {
            sr.sprite = jumpSprite;
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            sr.sprite = fallSprite;
        }
        else
        {
            sr.sprite = defaultSprite;
        }
    }
}
