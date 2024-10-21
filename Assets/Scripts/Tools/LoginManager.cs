using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button saveButton; // Initially disabled
    private string loginUrl = "https://your-login-server.com/login"; // Replace with your actual login URL
    private string authCheckUrl = "https://your-server.com/auth-check"; // Endpoint to check if user is authenticated

    private bool isAuthenticated = false;

    private void Start()
    {
        // Initially, show the login button and hide the save button
        saveButton.gameObject.SetActive(false);
        loginButton.onClick.AddListener(OpenLoginPage);
    }

    private void OpenLoginPage()
    {
        // Redirect the user to the login page
        Application.OpenURL(loginUrl);
    }

    // This method should be called once the user is redirected back to the Unity WebGL app after login
    public void OnLoginCallback(string token)
    {
        StartCoroutine(CheckAuthentication(token));
    }

    // This coroutine checks the authentication token
    private IEnumerator CheckAuthentication(string token)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token);

        using (UnityWebRequest www = UnityWebRequest.Post(authCheckUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text == "authenticated")
            {
                isAuthenticated = true;
                SwitchToSaveButton();
            }
            else
            {
                Debug.LogError("Authentication failed: " + www.error);
            }
        }
    }

    private void SwitchToSaveButton()
    {
        // Hide the login button and show the save button
        loginButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
    }
}
