using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button toggleMenuButton; // Button to toggle the model specification popup
    [SerializeField] private InputField tokenInputField; // Input field for user to paste token
    [SerializeField] private Button submitTokenButton; // Button to submit the token from input field
    [SerializeField] private Text debugText; // Text element for displaying messages on the screen
    private string loginUrl = "https://3ddelft01.bk.tudelft.nl:80/login"; // Real login URL
    private string authCheckUrl = "https://3ddelft01.bk.tudelft.nl:80/validate"; // Real auth-check URL
    public static string token;
    private bool isAuthenticated = false;

    private void Start()
    {
        // Initially, show the login button and hide the toggle button, input field, and submit button
        toggleMenuButton.gameObject.SetActive(false); // Hide the toggle menu button
        tokenInputField.gameObject.SetActive(false); // Hide input field initially
        submitTokenButton.gameObject.SetActive(false); // Hide submit button initially
        debugText.text = ""; // Clear the debug message initially

        loginButton.onClick.AddListener(OnLoginButtonPressed);

        // Add a listener to the submit button to trigger authentication when clicked
        submitTokenButton.onClick.AddListener(OnTokenSubmit);
    }

    private void OnLoginButtonPressed()
    {
        // Redirect the user to the login page
        Application.OpenURL(loginUrl);

        // After opening the login page, show the token input field and submit button
        tokenInputField.gameObject.SetActive(true);
        submitTokenButton.gameObject.SetActive(true);
    }

    // Called when the user submits a token via the input field
    private void OnTokenSubmit()
    {
        // Trim and remove non-visible characters like zero-width spaces, non-breaking spaces, etc.
        string token = Regex.Replace(tokenInputField.text, @"\s+", "").Trim();

        Debug.Log("Original Token Length: " + tokenInputField.text.Length);
        Debug.Log("Cleaned Token Length: " + token.Length);

        if (!string.IsNullOrEmpty(token))
        {
            Debug.Log("Token submitted: " + token);
            debugText.text = "Token submitted, checking authentication..."; // Update the on-screen message

            // Hide the input field and submit button after submitting the token
            tokenInputField.gameObject.SetActive(false);
            submitTokenButton.gameObject.SetActive(false);

            OnLoginCallback(token);  // Pass the cleaned token to the login callback
        }
        else
        {
            Debug.LogError("No token provided!");
            debugText.text = "No token provided, please paste a valid token."; // Update the on-screen message
            StartCoroutine(ClearDebugTextAfterDelay(5)); // Clear debug message after 5 seconds
        }
    }

    // This method should be called once the user is redirected back to the Unity WebGL app after login
    public void OnLoginCallback(string token)
    {
        Debug.Log("Login callback triggered with token: " + token);
        StartCoroutine(CheckAuthentication(token));
    }

    // This coroutine checks the authentication token by sending it to the server for verification
    private IEnumerator CheckAuthentication(string token)
    {
        Debug.Log("Checking authentication for token: " + token);

        using (UnityWebRequest www = UnityWebRequest.Post(authCheckUrl, new WWWForm()))
        {
            // Set the bearer token in the request header
            www.SetRequestHeader("Authorization", $"Bearer {token}");

            // Add the certificate handler to bypass SSL certificate validation
            www.certificateHandler = new BypassCertificateHandler();

            // Send the request and wait for the response
            Debug.Log("Request sent to server, waiting for response...");
            debugText.text = "Authenticating..."; // Show a message while waiting for the server response
            yield return www.SendWebRequest();

            // Check if the request was successful
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server response: " + www.downloadHandler.text);

                // Check if the response contains "authenticated"
                if (www.downloadHandler.text == "authenticated")
                {
                    Debug.Log("Token validated successfully, switching buttons.");
                    debugText.text = "Authentication successful!"; // Show success message
                    isAuthenticated = true;
                    LoginManager.token = token;
                    ShowToggleMenuButton(); // Show the toggle button after successful authentication
                }
                else
                {
                    Debug.LogError("Authentication failed: Invalid token. Response: " + www.downloadHandler.text);
                    debugText.text = "Authentication failed: Invalid token."; // Show error message
                }
            }
            else
            {
                // Log the error if the request failed
                Debug.LogError("Authentication failed: " + www.error);
                debugText.text = "Authentication failed: " + www.error; // Show error message
            }

            StartCoroutine(ClearDebugTextAfterDelay(5)); // Clear debug message after 5 seconds
        }
    }

    private void ShowToggleMenuButton()
    {
        // Hide the login button and show the toggle menu button
        Debug.Log("Switching from login button to toggle menu button.");
        loginButton.gameObject.SetActive(false);
        toggleMenuButton.gameObject.SetActive(true); // Show the toggle button after authentication
    }

    // Custom certificate handler to bypass SSL certificate validation
    private class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Always return true to bypass SSL certificate validation
            return true;
        }
    }

    // Coroutine to clear the debug text after a delay
    private IEnumerator ClearDebugTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        debugText.text = ""; // Clear the text after the delay
    }
}
