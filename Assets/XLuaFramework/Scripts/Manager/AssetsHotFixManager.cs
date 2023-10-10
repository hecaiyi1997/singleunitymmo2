//extract resource on game start and hot fix assets from cdn
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

namespace XLuaFramework
{
public class AssetsHotFixManager : MonoBehaviour
{
    private List<string> downloadFiles = new List<string>();
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

    public static AssetsHotFixManager Instance;
    private void Awake() 
    {
        Instance = this;
    }

    public void CheckExtractResource(Action<float,string> on_update, Action on_ok) 
    {
        //考虑到如果是编辑器模式，不释放，数据目录一直都是旧的，所以注销，以后数据都去数据目录取
        
        
        bool isExists = Directory.Exists(AppConfig.DataPath) &&
        Directory.Exists(AppConfig.DataPath + "lua/") && File.Exists(AppConfig.DataPath + "files.txt");
        Debug.Log("CheckExtraceResource AppConfig.DataPath:" + AppConfig.DataPath+ " isExists:"+ isExists.ToString()+" debugmode : "+ AppConfig.DebugMode.ToString());
            //CheckExtraceResource AppConfig.DataPath:D:/Users/UnityMMO-farmework/StreamingAssets/ isExists:True debugmode : True
            
             if( AppConfig.DebugMode|| isExists)
             {
                 Debug.Log("AssetsHotFixManager:CheckExtractResource ok");
                 on_ok();
                 return;   //文件已经解压过了，自己可添加检查文件列表逻辑
             }
             
        on_update(0.1f,"开始解压文件");//预先执行到0.03,目标是执行到0.3
        StartCoroutine(OnExtractResource(on_update,on_ok));    //启动释放协成，从 StreamingAssets文件夹copy到DataPath
        }

    public IEnumerator OnExtractResource(Action<float,string> on_update,Action on_ok) {//因为以后得程序中我取资源都是从数据目录取
        string dataPath = AppConfig.DataPath;  //数据目录
        string resPath = Util.AppContentPath(); //游戏包资源目录

        //string resPath = Application.streamingAssetsPath + "/";
        Debug.Log("dataPath=" + dataPath +":"+ resPath);//dataPath=/storage/emulated/0/Android/data/com.test.test/files/unitymmo/
        Debug.Log("resPath=" + resPath);//应用程序内容路径resPath=D:/Users/UnityMMO-farmework/StreamingAssets/
                                        //resPath=jar:file:///data/app/~~pLAZSGzPVHw4GIhB3rRJRg==/com.test.test-07fJyzPxpHI8BHwWnFxUgQ==/base.apk!/assets/

            while (Directory.Exists(dataPath))
            {
                Thread.Sleep(1);
                //StreamWriter savefile = new StreamWriter(dataPath, true);
                //savefile.Dispose();
                Directory.Delete(dataPath, true);
            }
            Directory.CreateDirectory(dataPath);


        string infile = resPath + "files.txt";
        string outfile = dataPath + "files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);
        
        string message = "正在解包文件:>files.txt";
        on_update(0.2f, message);//预先执行到0.06,目标是执行到0.3
        if (Application.platform == RuntimePlatform.Android) {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone) {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        } else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        int i = 0;
        foreach (var file in files) {
            i=i+1;
            string[] fs = file.Split('|');
            string str = fs[0].Substring(1, fs[0].Length - 1); //截取1位
            infile = resPath + str;  //fs[0]改成fs[1]
            outfile = dataPath + str;

            message = "正在解包文件:>" + fs[0];
            on_update(0.2f + 0.8f * ((i) / files.Length), message);//预先执行到0.06,目标是执行到0.3
            Debug.Log("正在解包文件:>" + outfile);
            //正在解包文件:>jar:file:///data/app/~~mkpQIquVGMrmZaowdtH6TA==/com.test.test-D1erBiJoN8dhkeVsrMRFzQ==/base.apk!/assets//base_world_1001多了一个/
                // facade.SendMessageCommand(NotiData.UPDATE_MESSAGE, message);

                string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android) {
                    /*
                    UnityWebRequest uwr = UnityWebRequest.Get(infile); //创建UnityWebRequest对象，将Url传入
                    yield return uwr.SendWebRequest();//开始请求
                    if (uwr.isNetworkError || uwr.isHttpError)                                                             //如果出错
                    {
                        Debug.Log(uwr.error); //输出 错误信息
                    }
                    else
                    {
                        while (!uwr.isDone) //只要下载没有完成，一直执行此循环
                        {
                            message = Math.Floor(uwr.downloadProgress * 100) + "%";
                            yield return 0;
                        }

                        if (uwr.isDone) //如果下载完成了
                        {
                            message = 100 + "%";
                        }

                        byte[] results = uwr.downloadHandler.data;
                        // 注意真机上要用Application.persistentDataPath
                        CreateFile(outfile, results, uwr.downloadHandler.data.Length);
                        //AssetDatabase.Refresh(); //刷新一下
                    }
                    */
                    ///storage/emulated/0/Android/data/com.test.test/files/unitymmo/luatemp/:
                    ///storage/emulated/0/Android/data/com.test.test/files/unitymmo/luatemp/NetDispatcher.lua
                    ///storage/emulated/0/Android/data/com.test.test/files/unitymmo/lua/lua_game_scene.manifest
                    WWW www = new WWW(infile);
                    yield return www;

                    if (www.isDone) {
                        File.WriteAllBytes(outfile, www.bytes);
                    }
                    yield return 0;
                    
                } 
                else 
                {
                if (File.Exists(outfile)){
                    File.Delete(outfile);
                    }
                File.Copy(infile, outfile, true);
                }
            yield return new WaitForEndOfFrame();
        }
        message = "解包完成!!!";
        // facade.SendMessageCommand(NotiData.UPDATE_MESSAGE, message);
        yield return waitForSeconds;
        on_ok();
    }
        /// <summary>
        /// 这是一个创建文件的方法
        /// </summary>
        /// <param name="path">保存文件的路径</param>
        /// <param name="bytes">文件的字节数组</param>
        /// <param name="length">数据长度</param>
        void CreateFile(string path, byte[] bytes, int length)
        {
            Stream sw;
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                sw = file.Create();
            }
            else
            {
                return;
            }

            sw.Write(bytes, 0, length);
            sw.Close();
            sw.Dispose();
        }
    public void UpdateResource(Action<float,string> on_update, Action<string> on_ok) 
    {
        if (AppConfig.UpdateMode) //默认是关闭的，因为没有服务器
        {
            StartCoroutine(OnUpdateResource(on_update, on_ok));
        }
        else
        {
            on_ok("");
        }
    }

    IEnumerator OnUpdateResource(Action<float,string> on_update, Action<string> on_ok) 
    {
        Debug.Log("OnUpdateResource() AppConfig.UpdateMode:"+ AppConfig.UpdateMode.ToString());
        string dataPath = AppConfig.DataPath;  //数据目录，可读可写
        Debug.Log("dataPath=" + dataPath);
        
            
        string url = AppConfig.WebUrl;
        Debug.Log("AppConfig.WebUrl=" + url);
        string message = string.Empty;
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string listUrl = url + "files.txt?v=" + random;
        Debug.LogWarning("OnUpdateResource() LoadUpdate---->>>" + listUrl);
        //TODO:不要直接返回中文，否则处理不了多语言版本，应该改成读取外部配置文件的
        on_update(0.05f, "下载最新资源列表文件...");
        WWW www = new WWW(listUrl); yield return www;
        if (www.error != null) {
            on_ok("下载最新资源列表文件失败!");
            yield break;
        }
        Debug.Log("is data path exist : "+Directory.Exists(dataPath)+" small:"+Directory.Exists(dataPath.Trim()));
        if (!Directory.Exists(dataPath)) {
            Directory.CreateDirectory(dataPath);
        }
        on_update(0.1f, "更新资源列表文件...");
        File.WriteAllBytes(dataPath + "files.txt", www.bytes);
        string filesText = www.text;
        string[] files = filesText.Split('\n');
        on_update(0.15f, "开始下载最新的资源文件...");
        float percent = 0.15f;
        for (int i = 0; i < files.Length; i++) {
            if (string.IsNullOrEmpty(files[i])) continue;
            string[] keyValue = files[i].Split('|');
            string f = keyValue[0];
            on_update(0.15f+0.85f*((i+1)/files.Length), "下载文件:"+f);
            string localfile = (dataPath + f).Trim();
            string path = Path.GetDirectoryName(localfile);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            string fileUrl = url + f + "?v=" + random;
            fileUrl.Replace("\\", "/");
            bool canUpdate = !File.Exists(localfile);
            if (!canUpdate) {
                string remoteMd5 = keyValue[1].Trim();
                string localMd5 = Util.md5file(localfile);
                canUpdate = !remoteMd5.Equals(localMd5);
                if (canUpdate) File.Delete(localfile);
            }
            if (canUpdate) {   //本地缺少文件
                Debug.Log(fileUrl);
                //这里都是资源文件，用线程下载
                BeginDownload(fileUrl, localfile);
                while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }
            }
        }
        yield return new WaitForEndOfFrame();

        message = "更新完成!!";
        Debug.Log(message);
        on_ok("");
    }

    // void OnUpdateFailed(string file) 
    // {
    //     string message = "更新失败!>" + file;
    //     Debug.Log(message);
    //     // facade.SendMessageCommand(NotiData.UPDATE_MESSAGE, message);
    // }

    bool IsDownOK(string file) 
    {
        return downloadFiles.Contains(file);
    }

    void BeginDownload(string url, string file) 
    {     //线程下载
        object[] param = new object[2] { url, file };

        ThreadEvent ev = new ThreadEvent();
        ev.Key = NotiData.UPDATE_DOWNLOAD;
        ev.evParams.AddRange(param);
        ThreadManager.Instance.AddEvent(ev, OnThreadCompleted);   //线程下载
    }

    void OnThreadCompleted(NotiData data) 
    {
        Debug.Log("OnThreadCompleted "+data.evName + " data.evParam.ToString():"+data.evParam.ToString());
        switch (data.evName) 
        {
            case NotiData.UPDATE_EXTRACT:  //解压一个完成
            //
            break;
            case NotiData.UPDATE_DOWNLOAD: //下载一个完成
            downloadFiles.Add(data.evParam.ToString());
            break;
        }
    }

}
}