using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class NavigateToSelfie : MonoBehaviour {
    [SerializeField] private Button selfieButton;
    [SerializeField] private string sceneName = "Test Selfie";

    private void Awake() {
        if (!selfieButton) selfieButton = GetComponent<Button>();
    }

    private void OnEnable() {
        selfieButton.onClick.AddListener(OnClick);
    }

    private void OnDisable() {
        selfieButton.onClick.RemoveListener(OnClick);
    }

    private void OnClick() {
        // Make sure "Test Selfie" is in Build Settings (File → Build Settings → Scenes In Build)
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
