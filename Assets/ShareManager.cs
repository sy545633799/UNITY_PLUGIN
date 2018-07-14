using UnityEngine;
using System.Collections;
//using cn.sharesdk.unity3d;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class ShareManager : MonoBehaviour
{
    //private ShareSDK shareSdk;
    //显示回调信息的文本框
    public Text msgText;
    void Start()
    {
        //shareSdk = GetComponent<ShareSDK>();
        ////分享回调事件 绑定
        //shareSdk.shareHandler += ShareResultHandle;
        ////授权回调事件
        //shareSdk.authHandler += AuthResultHandle;
        ////用户信息事件
        //shareSdk.showUserHandler += GetUserInfoResultHandle;
    }


    #region 保存到相册

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
    IEnumerator CutImage(string name)
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
    void ScanFile(string[] path)
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
    #endregion

    //#region 分享

    //public void ShareTexture()
    //{
    //    ShareContent content = new ShareContent();
    //    content.SetText("分享text");
    //    content.SetTitle("分享title");
    //    content.SetTitleUrl("http://www.baidu.com");
    //    content.SetSite("分享site");
    //    content.SetSiteUrl("http://www.sohu.com");
    //    content.SetUrl("http://www.sina.com");
    //    content.SetComment("分享comment");
    //    content.SetShareType(ContentType.Image);

    //    //截屏
    //    Application.CaptureScreenshot("Shot4Share.png");
    //    //设置图片路径
    //    content.SetImagePath(Application.persistentDataPath + "/Shot4Share.png");
    //    shareSdk.ShowPlatformList(null, content, 100, 100);

    //    //shareSdk.ShowShareContentEditor(PlatformType.QQ, content);
    //}



    ////分享结果回调
    //void ShareResultHandle(int reqID, ResponseState state, PlatformType type, Hashtable data)
    //{
    //    if (state == ResponseState.Success)
    //    {

    //        msgText.text = MiniJSON.jsonEncode(data);

    //    }
    //    else if (state == ResponseState.Fail)
    //    {

    //        print("fail! throwable stack = " +
    //        data["stack"] + "; error msg = "
    //        + data["msg"]);

    //    }
    //    else if (state == ResponseState.Cancel)
    //    {

    //        msgText.text = "Cancel !~!~~~~~~";

    //    }
    //}

    //#endregion


    //#region qq授权登录

    ////点击登录按钮调用的方法
    //public void OnClickedToLogin()
    //{
    //    //请求授权来获用户信息从而实现第三方登录
    //    shareSdk.Authorize(PlatformType.QQ);
    //}

    ////授权登录的回调
    //void AuthResultHandle(int reqID, ResponseState state, PlatformType type, Hashtable data)
    //{
    //    if (state == ResponseState.Success)
    //    {
    //        msgText.text = "授权登录成功";

    //        //授权成功的话, 获取用户的资料
    //        shareSdk.GetUserInfo(type);

    //    }
    //    else if (state == ResponseState.Fail)
    //    {

    //        print("fail! throwable stack = " +
    //        data["stack"] + "; error msg = "
    //        + data["msg"]);

    //    }
    //    else if (state == ResponseState.Cancel)
    //    {

    //        msgText.text = "Cancel !~!~~~~~~";

    //    }
    //}

    //#endregion


    //#region 获取用户信息的回调

    //void GetUserInfoResultHandle(int reqID, ResponseState state, PlatformType type, Hashtable data)
    //{
    //    if (state == ResponseState.Success)
    //    {
    //        //利用PlatformType来判断不同的平台获取用户信息的回调
    //        //将返回数据编码成Json格式的数据, 进行json解析展示到界面即可
    //        switch (type)
    //        {
    //            case PlatformType.QQ:
    //                msgText.text = MiniJSON.jsonEncode(data);
    //                break;
    //        }

    //    }
    //    else if (state == ResponseState.Fail)
    //    {

    //        print("fail! throwable stack = " +
    //        data["stack"] + "; error msg = "
    //        + data["msg"]);

    //    }
    //    else if (state == ResponseState.Cancel)
    //    {

    //        msgText.text = "Cancel !~!~~~~~~";

    //    }
    //}

    //#endregion

}