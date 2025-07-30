using UnityEngine;
using UnityEngine.InputSystem;

public class AppManager : MonoBehaviour
{
    void Update()
    {
        // Android에서 뒤로가기 버튼 처리
   
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            QuitGame();
        }
        
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 요청됨");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // 에디터용 종료
#endif
    }
}