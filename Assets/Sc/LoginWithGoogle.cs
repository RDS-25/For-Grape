using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Google;
using System.Threading.Tasks;

public class LoginWithGoogle : MonoBehaviour
{
    [Header("Google Client ID")]
    public string GoogleAPI = "674861751602-mi809cken04pegicbjap90jgc1jgp80n.apps.googleusercontent.com";

    [Header("UI Elements")]
    public TextMeshProUGUI Username;
    public TextMeshProUGUI UserEmail;
    public Image UserProfilePic;

    private FirebaseAuth auth;
    private FirebaseUser user;

    private static GoogleSignInConfiguration configuration;

    private void Start()
    {
        InitFirebase();
        InitGoogleSignIn();
    }

    void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    void InitGoogleSignIn()
    {
        if (configuration == null)
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = GoogleAPI,
                RequestIdToken = true,
                RequestEmail = true
            };

            GoogleSignIn.Configuration = configuration;
        }
    }

    public async void Login()
    {
        try
        {
            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            var credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

            var authResult = await auth.SignInWithCredentialAsync(credential);
            user = authResult;

            Debug.Log("로그인 성공: " + user.DisplayName);

            Username.text = user.DisplayName;
            UserEmail.text = user.Email;

            string photoUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : "";
            if (!string.IsNullOrEmpty(photoUrl))
                StartCoroutine(LoadImage(photoUrl));

            SceneManager.LoadScene("GameScene");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("구글 로그인 실패: " + ex.Message);
        }
    }

    IEnumerator LoadImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Debug.LogWarning("프로필 이미지 불러오기 실패: " + www.error);
        }
    }
}
