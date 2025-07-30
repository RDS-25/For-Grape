using System;
using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI myScoreText;
    public TextMeshProUGUI[] topScoresText;

    void OnEnable()
    {
        if (string.IsNullOrEmpty(GameManager.Instance?.UserId))
        {
            Debug.LogWarning("UserId가 존재하지 않습니다.");
            return;
        }

        LoadMyScore();
        LoadTopScores();
    }

    //점수 보여주기
    public void LoadMyScore()
    {
       
        
        int currentScore = Mathf.FloorToInt(GameManager.Instance.HighestScore);
        if (currentScore > GameManager.Instance.firebaseScroe)
        {
            Debug.Log("뉴레코드");
            myScoreText.text = $"NewRecord!! \n My Best: {GameManager.Instance.firebaseScroe}\n Now: {currentScore}";
        }
        else
        {
            Debug.Log("뉴레코드아님");
            myScoreText.text = $"My Best: {GameManager.Instance.firebaseScroe}\n Now: {currentScore}";
        }
      
    }

    //전체 점수 보여주기 
    public async void LoadTopScores()
    {
        var db = GameManager.Instance.Firestore;
        QuerySnapshot snapshot = await db.Collection("rankings")
            .OrderByDescending("score")
            .Limit(topScoresText.Length)
            .GetSnapshotAsync();

        var docs = snapshot.Documents.ToList();

        for (int i = 0; i < topScoresText.Length; i++)
        {
            if (i < docs.Count)
            {
                var doc = docs[i];
                // string name = doc.Id.Substring(0, 5);
                int score = doc.GetValue<int>("score");
                topScoresText[i].text = $" {score}";
            }
            else
            {
                topScoresText[i].text = $"None";
            }
        }
    }
}