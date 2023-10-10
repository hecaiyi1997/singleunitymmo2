using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System.IO;
namespace XLuaFramework {
    [LuaCallCSharp]
    public class AppConfig {
        //调试模式-开启后直接加载本地资源,不需要打包,如果是手机平台的话将自动设为false
        //public static bool DebugMode = true;
        public static bool DebugMode = false;//false表示为需要加载assetbundle
        /// <summary>
        /// 如果开启更新模式，前提必须启动框架自带服务器端。
        /// 否则就需要自己将StreamingAssets里面的所有内容
        /// 复制到自己的Webserver上面，并修改下面的WebUrl。
        /// 更新模式-默认关闭,如果是手机平台的话将自动设为true
        /// </summary>
        public static bool UpdateMode = false;                       
         //Lua字节码模式-默认关闭 
        //public const bool LuaByteMode = false;
        public const bool LuaByteMode = true;
        //Lua代码AssetBundle模式
        //public static bool LuaBundleMode = false;
        public static bool LuaBundleMode = true;//true lua代码也要打包assetbundle
        //把sproto协议文件编译成二进制码
        public static bool SprotoBinMode = false;                    
        //应用程序名称
        public const string AppName = "UnityMMO";        
        //临时目录       
        public const string LuaTempDir = "Assets/luatemp/";    
        //应用程序前缀
        public const string AppPrefix = AppName + "_";          
        //Asset Bundle的目录 
        public const string AssetDir = "StreamingAssets";          

        public static void Init()
        {
            //从配置文件里读取
            if (Application.isMobilePlatform)
            {
                DebugMode = false;
                UpdateMode = false;//移动端不 更新
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                DebugMode = false;
                UpdateMode = true;
            }
        }                

        
        /// <summary>
        /// 取得数据存放目录,我认为这个目录必须是可读可写
        /// </summary>
        private static string data_path = null;
        public static string DataPath {
            get {
                string game = AppConfig.AppName.ToLower();
                if (Application.isMobilePlatform) {//移动端，绝对路径
                    if (data_path == null)
                        data_path = Application.persistentDataPath + "/" + game + "/";
                        Debug.Log("CustomLoader3 Application.isMobilePlatform : " + Application.isMobilePlatform);
                }
                else if (AppConfig.DebugMode) {//如果是调试即非ab模式（AppConfig.AppDataPath==Assets的父目录）
                    if (data_path == null)
                        data_path = Application.dataPath + "/AssetBundleRes/";
                }
                else
                    if (data_path == null)
                    data_path = Application.dataPath + "/" + game + "/";//区别于StreamingAssets目录的存储

                Debug.Log("CustomLoader4 DataPath : " + data_path);
                return data_path;
            }
        }
        private static string relative_Path = null;
        //
        //AppConfig.DebugMode=false表示要打ab包
        //Load AssetBundleManifest调用到，
        public static string GetRelativePath() { 
            if (relative_Path == null)
            {
                relative_Path = "file://" + DataPath.Replace("\\", "/");
                
                Debug.Log("relative_Path=" + relative_Path);
            }
            //return relative_Path;
            if (Application.isMobilePlatform)
            {//移动端，绝对路径
                string game = AppConfig.AppName.ToLower();
                return "file://" + Application.persistentDataPath + "/" + game + "/";//我觉得www前面才加file://吧
            }
            else
            {
                relative_Path = "file://" + DataPath.Replace("\\", "/");
                return relative_Path;
            }

        }

        static string luaAssetsDir = string.Empty;
        public static string LuaAssetsDir  //lua脚本路径
        {
            get
            {
                if (luaAssetsDir != string.Empty)
                {
                    Debug.Log("CustomLoader2 LuaAssetsDir= : " + luaAssetsDir);
                    return luaAssetsDir;
                    
                }

                if (DebugMode)//非ab包
                {
                    luaAssetsDir = AppDataPath+"lua/";//AppDataPath为assets父目录
                }
                //添加一个
                else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    luaAssetsDir = Application.dataPath + "/lua/";
                }
                else
                {
                    Debug.Log("CustomLoader LuaAssetsDir2 : " + AppConfig.DataPath);
                    luaAssetsDir = AppConfig.DataPath + "lua/";
                    Debug.Log("CustomLoader LuaAssetsDir5 : " + AppConfig.DataPath+":"+ luaAssetsDir);
                }
                return luaAssetsDir.Replace("//", "/") ;
                /*
                 Assets/Lua/BaseRequire.lua.bytes 则应该对应 Application.streamingAssetsPath + @"\Lua\" + filePath + ".lua.btyes";
                 此处Assets/LuaTemp/BaseRequire.lua.bytes 应该AppConfig.DataPath+ @"\LuaTemp\" + filePath + ".lua.btyes";
                 * */
            }
        }

        public static string FrameworkRoot {
            get {
                return Application.dataPath + "/XLuaFramework";
            }
        }

        //打包后的资源路径,注意不是运行时使用的!
        private static string streamingAssetsTargetPath = string.Empty;
        public static string StreamingAssetsTargetPath
        {
            get
            {
                if (streamingAssetsTargetPath != string.Empty)
                    return streamingAssetsTargetPath;
                streamingAssetsTargetPath = GetStreamingAssetsTargetPathByPlatform(Application.platform);
                if (streamingAssetsTargetPath == string.Empty)
                    Debug.Log("Unspport System!");
                return streamingAssetsTargetPath;
            }
        }
        
        public static string GetStreamingAssetsTargetPathByPlatform(RuntimePlatform platform)//打包出来的资源路径
        {
            string dataPath = Application.dataPath.Replace("/Assets", "");
            if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WebGLPlayer)
                return dataPath + "/" + AppConfig.AssetDir;
            else if (platform == RuntimePlatform.Android)
                return dataPath + "/StreamingAssetsAndroid/" + AppConfig.AssetDir;
            else if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
                return dataPath + "/StreamingAssetsIOS/" + AppConfig.AssetDir;
            else
                Debug.Log("Unspport System!");

            return string.Empty;
        }

        //热更新用的文件服务器地址,可以自己用IIS或Apache搭建
        private static string webUrl = string.Empty;
        public static string WebUrl
        {
            get
            {
                if (webUrl != string.Empty)
                    return webUrl;
                var fileUrl = UnityMMO.ConfigGame.GetInstance().Data.FileServerURL;//fileUrl=192.168.1.7上存放的各种平台的ab资源
                Debug.Log("fileUrl : "+fileUrl);
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
                    webUrl = fileUrl+ "/StreamingAssets/";
                else if (Application.platform == RuntimePlatform.Android)
                    webUrl = fileUrl+"/AndroidStreamingAssets/";
                else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                    webUrl = fileUrl+"/IOSStreamingAssets/";
                else
                    Debug.Log("Unspport System!");
                return webUrl;
            }
        }

        public static string AppDataPath  //Assets的父目录
        {
            get
            {
                string dataPath = Application.dataPath;//乃assets的根目录D:\Users\UnityMMO-farmework\Assets也
                bool isWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
                Debug.Log("Application.platform=" + Application.platform);//Application.platform=WindowsEditor
                if (AppConfig.DebugMode || isWindows)
                    dataPath = dataPath.Replace("/Assets", "");
                return isWindows ? dataPath.Replace(AppName+"/App/PC/"+AppName+"_Data", AppName) : dataPath;
            }
        }
    }
}