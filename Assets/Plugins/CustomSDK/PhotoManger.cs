using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManger : MonoBehaviour {

    public Text msgText;
    public void SaveToAlbum()
    {
#if UNITY_ANDROID
        StartCoroutine(CutImage("Shot4Share.png"));
#elif UNITY_IPHONE
        Application.CaptureScreenshot("Shot4Share.png");
        string path = Application.persistentDataPath + "/Shot4Share.png";
        _SavePhoto(path);
#endif
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void _SavePhoto(string readAddr);

#endif

#if UNITY_ANDROID
    private IEnumerator CutImage(string name)
    {
        //图片大小  
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
        tex.Apply();
        yield return tex;

        byte[] byt = tex.EncodeToPNG();
        string path = "/mnt/sdcard/DCIM/Arphoto/";
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes(path + name, byt);
        }
        catch (Exception e)
        {
            msgText.text = e.Message;
        }
        string[] paths = new string[1];
        paths[0] = path;
        ScanFile(paths);
    }

    /// <summary>
    /// 刷新安卓相册
    /// </summary>
    /// <param name="path"></param>
    private void ScanFile(string[] path)
    {
        using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
            {
                Conn.CallStatic("scanFile", playerActivity, path, null, null);
            }
        }
    }
#endif
}
