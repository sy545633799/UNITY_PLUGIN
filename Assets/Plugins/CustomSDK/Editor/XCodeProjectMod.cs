using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

public class XCodeProjectMod
{

    static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        if (Directory.Exists(dstPath))
        {
            Directory.Delete(dstPath);
        }
        if (Directory.Exists(srcPath))
        {
            Directory.Delete(srcPath);
        }
        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
        {
            File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
        }

        foreach (var dir in Directory.GetDirectories(srcPath))
        {
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        //Debug.Log ("path: " + path);
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        #region 修改工程
//        string projPath = PBXProject.GetPBXProjectPath(path);
//        //Debug.Log ("projPath: " + projPath);
//        PBXProject proj = new PBXProject();
//        string fileText = File.ReadAllText(projPath);
//        proj.ReadFromString(fileText);
//        //Debug.Log ("fileText: " + fileText);

//        string targetName = PBXProject.GetUnityTargetName();//Unity-iPhone
//        string targetGuid = proj.TargetGuidByName(targetName);
//        //Debug.Log ("targetName: " + targetName);
//        //Debug.Log ("targetGuid: " + targetGuid);

//        // common
//        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
//        //proj.SetBuildProperty(targetGuid, "DEBUG_INFORMATION_FORMAT", "dwarf");
//        proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
//        //proj.AddBuildProperty (targetGuid, "OTHER_LDFLAGS", "-all_load ");
//        //proj.SetBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
//        //proj.AddBuildProperty (targetGuid, "FRAMEWORK_SEARCH_PATHS","xxxxxxx");
//        //proj.AddBuildProperty (targetGuid, "HEADER_SEARCH_PATHS","xxxxxxx");
//        //proj.AddBuildProperty (targetGuid, "LIBRARY_SEARCH_PATHS","xxxxxxx");

//        // umeng
//        proj.AddFrameworkToProject(targetGuid, "libz.tbd", false);
//        // bugly
//        proj.AddFrameworkToProject(targetGuid, "libz.tbd", false);
//        proj.AddFrameworkToProject(targetGuid, "libc++.tbd", false);
//        proj.AddFrameworkToProject(targetGuid, "Security.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "JavaScriptCore.framework", true);
//        // wechat
//        proj.AddFrameworkToProject(targetGuid, "libz.tbd", false);
//        proj.AddFrameworkToProject(targetGuid, "libsqlite3.0.tbd", false);
//        proj.AddFrameworkToProject(targetGuid, "libc++.tbd", false);
//        proj.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "Security.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "CFNetwork.framework", false);
//        // ios定位
//        proj.AddFrameworkToProject(targetGuid, "CoreLocation.framework", false);

//        // 为魔窗添加Associated Domains域名
//        string fileName = "my.entitlements";    // 这个名字任意
//        string filePath = Path.Combine(path, fileName);
//        //Debug.Log ("filePath: " + filePath);
//        string fileContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
//<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd 

//"">
//<plist version=""1.0"">
//<dict>
//    <key>com.apple.developer.associated-domains</key>
//    <array>
//        <string>applinks:sjsbna.mlinks.cc 

//</string>
//    </array>
//</dict>
//</plist>";
//        File.WriteAllText(filePath, fileContent);
//        proj.AddFile(filePath, fileName);
//        proj.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", fileName);
//        // save changed
//        File.WriteAllText(projPath, proj.WriteToString());

        #endregion


        #region 修改plist
        string plistPath = Path.Combine(path, "Info.plist");
        Debug.Log("plistPath: " + plistPath);
        PlistDocument plist = new PlistDocument();
        string plistFileText = File.ReadAllText(plistPath);
        plist.ReadFromString(plistFileText);
        PlistElementDict rootDict = plist.root;

        // 设置语言语言环境
        //rootDict.SetString("CFBundleDevelopmentRegion", "zh_CN");
        //      rootDict.SetString("CFBundleDevelopmentRegion", "en");

        // 一些权限声明
        rootDict.SetString("NSContactsUsageDescription", "App需要您的同意,才能访问通讯录");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "App需要您的同意,才能访问相册");
        rootDict.SetString("NSCameraUsageDescription", "App需要您的同意,才能访问相机");
        rootDict.SetString("NSLocationUsageDescription", "App需要您的同意,才能访问位置");
        rootDict.SetString("NSLocationAlwaysUsageDescription", "App需要您的同意,才能始终访问位置");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "App需要您的同意,才能在使用期间访问位置");
        rootDict.SetString("NSCalendarsUsageDescription", "App需要您的同意,才能访问日历");
        rootDict.SetString("NSRemindersUsageDescription", "App需要您的同意,才能访问提醒事项");
        rootDict.SetString("NSMotionUsageDescription", "App需要您的同意,才能访问运动与健身");
        rootDict.SetString("NSHealthUpdateUsageDescription", "App需要您的同意,才能访问健康更新");
        rootDict.SetString("NSHealthShareUsageDescription", "App需要您的同意,才能访问健康分享");
        rootDict.SetString("NSBluetoothPeripheralUsageDescription", "App需要您的同意,才能访问蓝牙");
        rootDict.SetString("NSAppleMusicUsageDescription", "App需要您的同意,才能访问媒体资料库");

        //PlistElementArray urlTypes = rootDict.CreateArray("CFBundleURLTypes");
        //// add url scheme for wechat
        //string wechat_appid = "wx8888888888888";
        //PlistElementDict wxUrl = urlTypes.AddDict();
        //wxUrl.SetString("CFBundleTypeRole", "Editor");
        //wxUrl.SetString("CFBundleURLName", "weixin");
        //wxUrl.SetString("CFBundleURLSchemes", wechat_appid);
        //PlistElementArray wxUrlScheme = wxUrl.CreateArray("CFBundleURLSchemes");
        //wxUrlScheme.AddString(wechat_appid);
        //// 白名单 for wechat
        //PlistElementArray queriesSchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");
        //queriesSchemes.AddString("wechat");
        //queriesSchemes.AddString("weixin");
        //queriesSchemes.AddString(wechat_appid);
        //// add url scheme magic window
        //string mw_sheme = "xxxx"; // xxxx://
        //PlistElementDict mwUrl = urlTypes.AddDict();
        //mwUrl.SetString("CFBundleTypeRole", "Editor");
        //mwUrl.SetString("CFBundleURLName", "magicwindow");
        ////mwUrl.SetString("CFBundleURLSchemes", mw_sheme );
        //PlistElementArray mwUrlScheme = mwUrl.CreateArray("CFBundleURLSchemes");
        //mwUrlScheme.AddString(mw_sheme);

        //// for wechat
        //PlistElementArray backModes = rootDict.CreateArray("UIBackgroundModes");
        //backModes.AddString("remote-notification");

        // 保存修改
        File.WriteAllText(plistPath, plist.WriteToString());

        #endregion
    }
}