using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManger : MonoBehaviour {

    public static PhotoManger Instance { get; private set; }
    /// <summary>
    /// 打开相册中的照片回调
    /// </summary>
    public System.Action<string> CallBack_PickImage_With_Base64;
    /// <summary>
    /// 保照片到相册的回掉
    /// </summary>
    public System.Action<string> CallBack_ImageSavedToAlbum;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 保存图片到相册
    /// </summary>
    public void SaveToAlbum(string readPath, Texture2D texture)
    {
#if UNITY_ANDROID
        _saveImageToAndroidAlbum(texture);
#elif UNITY_IPHONE
        _iosSaveImageToPhotosAlbum(readPath);
#endif
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void _iosOpenPhotoLibrary();
    [DllImport("__Internal")]
    private static extern void _iosOpenPhotoAlbums();
    [DllImport("__Internal")]
    private static extern void _iosOpenCamera();
    [DllImport("__Internal")]
    private static extern void _iosOpenPhotoLibrary_allowsEditing();
    [DllImport("__Internal")]
    private static extern void _iosOpenPhotoAlbums_allowsEditing();
    [DllImport("__Internal")]
    private static extern void _iosOpenCamera_allowsEditing();
    [DllImport("__Internal")]
    private static extern void _iosSaveImageToPhotosAlbum(string readAddr);

    /// <summary>
    /// 打开照片
    /// </summary>
    /// <param name="allowsEditing"></param>
	public static void iosOpenPhotoLibrary(bool allowsEditing = false)
    {
        if (allowsEditing)
            _iosOpenPhotoLibrary_allowsEditing();
        else
            _iosOpenPhotoLibrary();
    }

    /// <summary>
    /// 打开相册
    /// </summary>
    /// <param name="allowsEditing"></param>
	public static void iosOpenPhotoAlbums(bool allowsEditing = false)
    {
        if (allowsEditing)
            _iosOpenPhotoAlbums_allowsEditing();
        else
            _iosOpenPhotoAlbums();
    }

    /// <summary>
    /// 打开相机
    /// </summary>
    /// <param name="allowsEditing"></param>
	public static void iosOpenCamera(bool allowsEditing = false)
    {
        if (allowsEditing)
            _iosOpenCamera_allowsEditing();
        else
            _iosOpenCamera();
    }


    /// <summary>
    /// 将ios传过的string转成u3d中的texture
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
	public static Texture2D Base64StringToTexture2D(string base64)
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
        try
        {
            byte[] bytes = System.Convert.FromBase64String(base64);
            tex.LoadImage(bytes);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return tex;
    }

    /// <summary>
    /// 打开相册相机后的从ios回调到unity的方法(传图)
    /// </summary>
    /// <param name="base64">Base64.</param>
    void PickImageCallBack_Base64(string base64)
    {
        if (CallBack_PickImage_With_Base64 != null)
        {
            CallBack_PickImage_With_Base64(base64);
        }
    }

    /// <summary>
    /// 示例
    /// </summary>
    /// <param name="base64"></param>
    void callback_PickImage_With_Base64(string base64)
    {
        Texture2D tex = Base64StringToTexture2D(base64);
        //rawImage.texture = tex;
    }

    /// <summary>
    /// 保存图片到相册后，从ios回调到unity的方法
    /// </summary>
    /// <param name="msg">Message.</param>
    void SaveImageToPhotosAlbumCallBack(string msg)
    {
        if (CallBack_ImageSavedToAlbum != null)
        {
            CallBack_ImageSavedToAlbum(msg);
        }
    }

#elif UNITY_ANDROID
    /// <summary>
    /// 是否可以尝试直接拷贝
    /// </summary>
    /// <param name="texture"></param>
    private void _saveImageToAndroidAlbum(Texture2D texture)
    {
        byte[] byt = texture.EncodeToPNG();
        string path = "/mnt/sdcard/DCIM/Arphoto/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        File.WriteAllBytes(path + name, byt);
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
