using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLScreenshot : MonoBehaviour {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void DownloadDataUrl(string filename, string dataUrl);
#endif

    public string defaultFilename = "screenshot.png";

    public void CaptureAndDownload() => StartCoroutine(Capture());

    private System.Collections.IEnumerator Capture() {
        yield return new WaitForEndOfFrame();

        var w = Screen.width;
        var h = Screen.height;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        var png = tex.EncodeToPNG();
        Destroy(tex);

        var b64 = Convert.ToBase64String(png);
        var dataUrl = $"data:image/png;base64,{b64}";

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadDataUrl(defaultFilename, dataUrl);
#else
        // Editor fallback: write to project folder
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.dataPath, defaultFilename), png);
        Debug.Log("Saved screenshot to Assets/" + defaultFilename);
#endif
    }
}
