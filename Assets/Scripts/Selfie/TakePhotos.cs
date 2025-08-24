using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class TakePhotos : MonoBehaviour {
    public enum CaptureMode { CameraScreenshot, Webcam }

    [Header("Settings")]
    public CaptureMode captureMode = CaptureMode.CameraScreenshot;
    public bool showPreview = true;

    [Header("Webcam Settings")]
    public UnityEngine.UI.RawImage webcamPreview; // Assign in inspector

    // Private variables
    private WebCamTexture webCamTexture;
    private Camera mainCamera;
    private bool isWebcamInitialized = false;

    void Start() {
        mainCamera = Camera.main;

        if (captureMode == CaptureMode.Webcam) {
            InitializeWebcam();
        }
    }

    void InitializeWebcam() {
        // Check for webcam availability
        if (WebCamTexture.devices.Length == 0) {
            Debug.LogError("No webcam found!");
            return;
        }

        // Use first available webcam
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, 1920, 1080, 30);

        if (showPreview && webcamPreview != null) {
            webcamPreview.texture = webCamTexture;
        }

        webCamTexture.Play();
        isWebcamInitialized = true;
    }

    public void CapturePhoto() {
        if (captureMode == CaptureMode.CameraScreenshot) {
            StartCoroutine(CaptureScreenshot());
        }
        else if (captureMode == CaptureMode.Webcam && isWebcamInitialized) {
            StartCoroutine(CaptureWebcamPhoto());
        }
        else {
            Debug.LogError("Webcam not initialized!");
        }
    }

    private IEnumerator CaptureScreenshot() {
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Read screen contents
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Process the captured texture
        ProcessCapturedImage(texture, "screenshot");
    }

    private IEnumerator CaptureWebcamPhoto() {
        yield return new WaitForEndOfFrame();

        // Create texture with webcam dimensions
        Texture2D texture = new Texture2D(
            webCamTexture.width,
            webCamTexture.height,
            TextureFormat.RGB24,
            false
        );

        // Get pixels from webcam
        texture.SetPixels(webCamTexture.GetPixels());
        texture.Apply();

        // Process the captured texture
        ProcessCapturedImage(texture, "webcam");
    }

    private void ProcessCapturedImage(Texture2D texture, string type) {
        byte[] bytes = texture.EncodeToPNG();
        string fileName = $"{type}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

        // Platform-specific handling
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            // WebGL: Use browser download without plugins
            DownloadFileWebGL(bytes, fileName);
        }
        else {
            // Other platforms: Save to persistent data path
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Saved {type} to: {filePath}");

            // On mobile, refresh gallery
#if UNITY_ANDROID || UNITY_IOS
            RefreshGallery(filePath);
#endif
        }

        // Clean up
        Destroy(texture);
    }

    private void DownloadFileWebGL(byte[] bytes, string fileName) {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Create a Blob URL and trigger download without plugins
        string base64 = Convert.ToBase64String(bytes);
        string dataUrl = $"data:application/octet-stream;base64,{base64}";
        
        // Execute JavaScript to download the file
        string jsCode = $@"
            var a = document.createElement('a');
            a.href = '{dataUrl}';
            a.download = '{fileName}';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        ";
        
        Application.ExternalEval(jsCode);
#else
        // Fallback for editor testing
        string filePath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"Saved to: {filePath} (WebGL download simulated in editor)");
#endif
    }

#if UNITY_ANDROID
    private void RefreshGallery(string filePath) {
        // Refresh Android gallery
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection"))
        using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext")) {
            mediaScanner.CallStatic("scanFile", context, new string[] { filePath }, null, null);
        }
    }
#elif UNITY_IOS
    // For iOS, you would need a native plugin to refresh the gallery
    // This is a placeholder that would need implementation
    private void RefreshGallery(string filePath)
    {
        Debug.Log("iOS gallery refresh would need a native plugin implementation");
    }
#else
    private void RefreshGallery(string filePath) { /* Not needed on this platform */ }
#endif

    void OnDestroy() {
        // Clean up webcam
        if (webCamTexture != null && webCamTexture.isPlaying) {
            webCamTexture.Stop();
            Destroy(webCamTexture);
        }
    }
}