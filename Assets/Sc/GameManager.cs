using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI countdownText;
    public Transform player;
    public Camera mainCamera;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public CameraFollow cameraFollow;
    public GameObject deathEffectPrefab;

    public GameObject description;
    
    public int firebaseScroe = 0;//파이어베이스에 있던 기존 최고 기록

    //기본점수 
    public float HighestScore = 0f;
    //아이템으로 얻는 추가점수 
    private float itemBonusScore = 0f;
    //게임 중임을 알리는 함수 
    public bool isPlaying =false;
    public bool IsDead { get; private set; } = false;

    public static bool IsDescriptionOnce;
    
    private bool hasStarted = false;  // 터치 한 번만 허용하기 위한 변수

    // Firebase 관련 정보
    public FirebaseUser FirebaseUser { get; private set; }
    public FirebaseFirestore Firestore { get; private set; }
    public string UserId { get; private set; }
    
    public GameObject pausePanel;  // Inspector에 할당

    public bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        RefreshFirebaseUser();

        if (IsDescriptionOnce)
        {
            description.SetActive(false);
            StartCoroutine(CountdownAndStart());
        }
    }
    

    void Update()
    {
   
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("실행중");
            //모든 행동을 일시 정지 시키고 "일시정지 패널 생성하기"
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }

        if (player.position.y > HighestScore)
        {
            HighestScore = player.position.y;
        }
        scoreText.text = $"{Mathf.FloorToInt(HighestScore+itemBonusScore)}";

        if (player.position.y < mainCamera.transform.position.y - 6f)
        {
            Die();
        }
        
        
    }

    private void LateUpdate()
    {
        if (IsDead || isPlaying || hasStarted||IsDescriptionOnce) return;
        
     
        //최초 실행할때 딱한번만 설명 보여주기 그다음 재실행 할때는 resetgame 할때는 StartCoroutine(CountdownAndStart()); 바로 실행 
        if (Touchscreen.current?.primaryTouch.press.isPressed == true || Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Debug.Log("실행?");
            description.SetActive(false);
            IsDescriptionOnce = true;
            hasStarted = true; // 이후 터치 무시
            StartCoroutine(CountdownAndStart());
        }

    }

    IEnumerator CountdownAndStart()
    {
        countdownText.gameObject.SetActive(true);
        string[] countdowns = { "3", "2", "1", "Start!" };

        foreach (string count in countdowns)
        {
            countdownText.text = count;
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);
        player.GetComponent<PlayerController>().enabled = true;
        isPlaying = true;
    }

    public void Die()
    {
        IsDead = true;
        if (player != null && player.TryGetComponent(out PlayerController controller))
        {
            controller.StartDeathFall();
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SpawnDeathEffect(Vector3 position)
    {
        Instantiate(deathEffectPrefab, position, Quaternion.identity);
    }

    public void HandleGameOver()
    {
        IsDead = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        SaveScoreToFirebase();
    }

    public void AddScore(float amount)
    {
        itemBonusScore += amount;
        // scoreText.text = $"{Mathf.FloorToInt(HighestScore)}";
    }
    
    public void OnExitPressed()
    {
        Debug.Log("게임 종료 요청됨");
        Application.Quit();
    }

    #region 일지정지 함수

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    #endregion
    

    #region 파이어베이스용 함수 
    public void RefreshFirebaseUser()
    {
        FirebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
        Firestore = FirebaseFirestore.DefaultInstance;

        if (FirebaseUser != null)
        {
            UserId = FirebaseUser.UserId;
            Debug.Log("Firebase 유저 초기화 완료: " + UserId);
        }
        else
        {
            Debug.LogWarning("Firebase 유저 정보가 없습니다.");
        }
    }

    public void SaveScoreToFirebase()
    {
        if (FirebaseUser == null || Firestore == null) return;

        DocumentReference docRef = Firestore.Collection("rankings").Document(FirebaseUser.UserId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.Result.Exists)
            {
                // 기존 문서가 없거나 오류 발생 시, 새로 저장
                UploadNewScore(docRef);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            int existingScore = snapshot.ContainsField("score") ? snapshot.GetValue<int>("score") : 0;
            int currentScore = Mathf.FloorToInt(HighestScore);

            if (currentScore > existingScore)
            {
                firebaseScroe = existingScore;
                UploadNewScore(docRef);
            }
            else
            {
                Debug.Log("기존 점수가 더 높거나 동일하므로 갱신하지 않음");
            }
        });
    }

    private void UploadNewScore(DocumentReference docRef)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "uid", FirebaseUser.UserId },
            { "name", FirebaseUser.DisplayName ?? "익명" },
            { "score", Mathf.FloorToInt(HighestScore) },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        };

        docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
                Debug.Log("파이어베이스 점수 저장 완료 (신기록)");
            else
                Debug.LogError("점수 저장 실패: " + task.Exception);
        });
    }
    #endregion
}
