using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
using XLuaFramework;
using Unity.Entities;
using UnityMMO.Component;
using System.Text;

[Hotfix]
[LuaCallCSharp]
public class XLuaManager : MonoBehaviour
{
    Action<float, float,bool,bool> luaUpdate = null;
    Action luaLateUpdate = null;
    Action<float> luaFixedUpdate = null;
    LuaEnv luaEnv = null;
    public static XLuaManager Instance = null;
    Action onLoginOk = null;
    Action onchunkloadOk = null;

    public Action onStartTrgger = null;

    public Action onsceneloadedTrgger = null;

    public Action<long, long, int> onKillMonster = null;

    public LuaTable userinfo;

    bool PlayAudioStart=false;

    long talkmonster_uid=0;

    bool PlayAudioComplete=false;

    /*************************************************/

    private string lua_ab_path;

    private AssetBundleManifest hotFixManifest;
    private Dictionary<string, AssetBundle> bundleMap = new Dictionary<string, AssetBundle>();
    //private StringBuilder sb = new StringBuilder();

    /********* 热更模式相关 *********/
    // 文件名和对应的MD5码
    //private Dictionary<string, string> fileMap = new Dictionary<string, string>();
    //private string filesTxtPath = Application.streamingAssetsPath+"/" + "LuaHotfix/files.txt";


    /******************************************/
    protected void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InitExternal()
    {
        luaEnv.AddBuildin("sproto.core", XLua.LuaDLL.Lua.LoadSproto);
        luaEnv.AddBuildin("crypt", XLua.LuaDLL.Lua.LoadCrypt);
        luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
    }

    public void InitLuaEnv()
    {
        // XLua.LuaEnv.AddIniter(XLua.CSObjectWrap.XLua_Gen_Manual_Register__.Init);
        luaEnv = new LuaEnv();
        if (luaEnv != null)
        {
            //处理AssetBundle
            HandleAB();
            //luaEnv.AddLoader(CustomLoader);
            luaEnv.AddLoader(LoaderByAB);
            
            InitExternal();
            //LoadScript("Common.UnityEngine.Mathf");
            
            LoadScript("BaseRequire");

            luaUpdate = luaEnv.Global.Get<Action<float, float,bool,bool>>("Update");
            luaLateUpdate = luaEnv.Global.Get<Action>("LateUpdate");
            luaFixedUpdate = luaEnv.Global.Get<Action<float>>("FixedUpdate");

            onStartTrgger = luaEnv.Global.Get<Action>("onStartTrgger");
            onsceneloadedTrgger = luaEnv.Global.Get<Action>("onsceneloadedTrgger");
            onKillMonster = luaEnv.Global.Get<Action<long, long, int>>("onKillMonster");



            if (NetworkManager.GetInstance())
            {
                SafeDoString("require 'NetDispatcher'");
                Debug.Log("NetworkManager.GetInstance().OnConnectCallBack before");
                NetworkManager.GetInstance().OnConnectCallBack += luaEnv.Global.Get<Action<byte[]>>("OnConnectServer");
                NetworkManager.GetInstance().OnDisConnectCallBack += luaEnv.Global.Get<Action<byte[]>>("OnDisConnectFromServer");
                NetworkManager.GetInstance().OnReceiveLineCallBack += luaEnv.Global.Get<Action<byte[]>>("OnReceiveLineFromServer");
                NetworkManager.GetInstance().OnReceiveMsgCallBack += luaEnv.Global.Get<Action<byte[]>>("OnReceiveMsgFromServer");
                NetworkManager.GetInstance().onApplyTalkExp += luaEnv.Global.Get<Action<int,long>>("onApplyTalkExp");
                Debug.Log("NetworkManager.GetInstance().OnConnectCallBack mid");
                //SpeechVoice.Instance.PlayAudioComplete+=luaEnv.Global.Get<EventHandler>("PlayAudioComplete");
                //SpeechVoice.Instance.PlayAudioStart+=luaEnv.Global.Get<EventHandler>("PlayAudioStart");

#if UNITY_EDITOR
                SpeechVoice.Instance.PlayAudioComplete+=delegate(object sender, EventArgs e){
                    this.onPlayAudioComplete();
                };
                SpeechVoice.Instance.PlayAudioStart+=delegate(object sender, EventArgs e){
                    this.onPlayAudioStart();
                };
#endif
                Debug.Log("NetworkManager.GetInstance().OnConnectCallBack");
            }
            else
                Debug.LogError("must init network manager before init xlua manager!");
        }
        else
        {
            Debug.LogError("InitLuaEnv failed!");
        }
    }
    
    public void onPlayAudioComplete(){
        //Debug.LogError("xluamanager.onPlayAudioComplete!");
        //luaEnv.Global.Get<Action>("PlayAudioComplete").Invoke();
        this.PlayAudioComplete=true;

    }
    public void ApplyTalkExp(int typeid,long uid){
        this.talkmonster_uid=uid;
    }

    public void FinishTalkExp(){
        this.talkmonster_uid=0;
    }

    public  void onPlayAudioStart(){
        //Debug.LogError("xluamanager.onPlayAudioStart!");
        //luaEnv.Global.Get<Action>("PlayAudioStart").Invoke();
        this.PlayAudioStart=true;

    }
    public void StartLogin(Action login_ok)
    {
        onLoginOk = login_ok;
        LoadScript("LuaMain");
        SafeDoString("LuaMain()");
    }
    public void Startloadpersistentchunk(Action login_ok)
    {
        onchunkloadOk = login_ok;

    }
    public void OnchunkloadedOk()
    {

        onchunkloadOk();
        onchunkloadOk = null;
    }
    //登录成功后，在lua端的LoginController会调用本方法的
    public void OnLoginOk(LuaTable userinfo)
    {
        this.userinfo = userinfo;
        XLuaManager.Instance.userinfo.Get("pos_x", out long pos_x);
        XLuaManager.Instance.userinfo.Get("pos_y", out long pos_y);
        XLuaManager.Instance.userinfo.Get("pos_z", out long pos_z);
        Debug.Log("onlognoook" + pos_x + ":" + pos_y + ":" + pos_z);
        Debug.Log("XLuaManager onLoginOk");
        onLoginOk();
        onLoginOk = null;
    }

    public LuaEnv GetLuaEnv()
    {
        return luaEnv;
    }

    public void SafeDoString(string scriptContent, string chunkName="chunk")
    {
        if (luaEnv != null)
        {
            try
            {
                //luaEnv.DoString(scriptContent, chunkName);
                luaEnv.DoString(scriptContent);
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg, null);
            }
        }
    }
    
    
    void LoadScript(string scriptName)
    {
        SafeDoString(string.Format("require('{0}')", scriptName));
    }

    public void LoadOutsideFile(string file_path)
    {
        SafeDoString(File.ReadAllText(file_path));
    }

    static bool IsUseLuaFileWithoutBundle = false;//just for debug 
    public byte[] CustomLoader(ref string filepath)
    {
        Debug.Log("CustomLoader:" + filepath);
        /*
        string scriptPath = string.Empty;
        //filepath = filepath.Replace(".", "/") + ".lua.bytes";
        filepath = filepath.Replace(".", "/") + ".lua.bytes";
        if (filepath.StartsWith("/"))
        {
            filepath = filepath.Substring(1, filepath.Length - 1);
        }
        if (!IsUseLuaFileWithoutBundle)
        {

            Debug.Log("CustomLoader0 AppConfig.LuaAssetsDir : ");
            string str = AppConfig.LuaAssetsDir;
            Debug.Log("CustomLoader1 AppConfig.LuaAssetsDir : " + str);
            scriptPath = str+filepath;//我们不需要考虑arr_pa里面的字符串是不是以”\” 结尾，这的确提供了方便，而且这也是很多人喜欢使用Path.Combine的一个原因，但是仅此而已。
            Debug.Log("CustomLoader6 Load lua script : " +AppConfig.LuaAssetsDir +":"+ scriptPath);
            return Util.GetFileBytes(scriptPath);
        }
        else
        {
            string dataPath = Application.dataPath;
            dataPath = dataPath.Replace("/Assets", "");
            dataPath = dataPath.Replace(AppConfig.AppName+"/App/PC/"+AppConfig.AppName+"_Data", AppConfig.AppName);
            scriptPath = Path.Combine(dataPath + "/Lua/", filepath);
            // Debug.Log("Load lua script : " + scriptPath);
            return Util.GetFileBytes(scriptPath);
        }
        */
        //string scriptPath = string.Empty;
        //filepath = filepath.Replace(".", "/").ToLower() + ".lua";
        /*
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
        string str = AppConfig.LuaAssetsDir;
        scriptPath = Path.Combine(str, filepath);
        Debug.Log("Load lua script : " + scriptPath);
        return Util.GetFileBytes(scriptPath);
        }
        */
        /*
        string scriptPath2 = string.Format("{0}/{1}.bytes", "assets/luatemp", filepath);//luaAssetbundleAssetName="Assets/luatemp/"
        Debug.Log("scriptPath2=" + scriptPath2);//assets/luatemp/baserequire.lua.bytes
        byte[] bys=null;
        ResourceManager.GetInstance().LoadAsset<TextAsset>(scriptPath2, delegate (UnityEngine.Object[] objs)
      {
          TextAsset txt = objs[0] as TextAsset;
          bys=txt.bytes;
          Debug.Log("wwwwwwwwwww" + txt.bytes+txt.text);
      });
        //Debug.LogError("Load lua script failed : " + scriptPath + ", You should preload lua assetbundle first!!!");

        return bys;
        
        */


        if (filepath == null)
        {
            return null;
        }
        byte[] bys = null;
        string filename = "assets/luatemp/" + filepath.Replace('.', '/').ToLower() + ".lua.bytes";
        string resname = "";
        int pos = filename.LastIndexOf('/');
        if (pos > 0)
        {
            resname = filename.Substring(pos + 1);
        }
        
        string abName = PackRule.PathToAssetBundleName(filename);
        Debug.Log("wwwwwwwwwwwfilename" + filename + ":" +abName +":"+ AppConfig.DataPath + "lua/lua");
        AssetBundle bundle;
        bundleMap.TryGetValue(abName, out bundle);
        if (bundle != null)
        {
            Debug.Log("wwwwwwwwwwwresname=" + resname);
            TextAsset textAsset = bundle.LoadAsset<TextAsset>(resname);
            Debug.Log("wwwwwwwwwww" + textAsset.bytes);
            bys=textAsset.bytes;
        }
        //AssetBundle ab = AssetBundle.LoadFromFile(AppConfig.DataPath + "lua/lua");
        return bys;
        
    }
    // AB模式下的loader，使用打包成AB的lua文件
    private byte[] LoaderByAB(ref string fileName)
    {
        Debug.Log("LoaderByAB1 : "+ fileName);
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Debug.Log("LoaderByAB1 : + Application.platform == RuntimePlatform.WindowsEditor");
            string scriptPath2 = string.Empty;
            string filepath = fileName.Replace(".", "/").ToLower() + ".lua";
            string str = AppConfig.LuaAssetsDir;
            scriptPath2 = Path.Combine(str, filepath);
            Debug.Log("Load lua script : " + scriptPath2);
            return Util.GetFileBytes(scriptPath2);
        }
        /*//这是ok的
        fileName = fileName.Replace('.', '/').ToLower();//LUA 是区别大小写的，浪费了多多时间
        byte[] buffer = null;
        sb.Append("lua");
        int pos = fileName.LastIndexOf('/');
        Debug.Log("LoaderByAB2 pos: " + pos);
        if (pos > 0)
        {
            sb.Append("_");
            sb.Append(fileName, 0, pos).Replace('/', '_');
            fileName = fileName.Substring(pos + 1);
        }

        if (!fileName.EndsWith(".lua"))
        {
            fileName += ".lua";
        }

        fileName += ".bytes";
        AssetBundle bundle;
        Debug.Log("LoaderByAB3 : " + sb.ToString()+": "+fileName);//lua_game_gm: gmmodel.lua.bytes
        bundleMap.TryGetValue(sb.ToString(), out bundle);
        if (bundle != null)
        {
            Debug.Log("LoaderByAB4 : bundle != null" );//
            TextAsset luaCode = bundle.LoadAsset<TextAsset>(fileName);//提供BaseRequire.lua.bytes，不需要路径
            if (luaCode != null)
            {
                Debug.Log("LoaderByAB5 :luaCode != null ");
                buffer = luaCode.bytes;
                Resources.UnloadAsset(luaCode);
            }
        }

        sb.Length = 0;
        sb.Capacity = 0;

        return buffer;
        */
        string filename = "assets/luatemp" + "/" + fileName.Replace('.', '/') + ".lua.bytes"; ;//"assets/luatemp"没有意义,只用于比较
        string abName = PackRule.PathToAssetBundleName(filename).ToLower();//一定要小写，因为HandAB都是小写
        string resName="";
        int pos = filename.LastIndexOf('/');
        Debug.Log("LoaderByAB2 pos=" + pos+":"+ filename);
        if (pos > 0)
        {
            resName = filename.Substring(pos + 1);
        }
        Debug.Log("LoaderByAB2.5=" + resName);
        string resname = resName.ToLower();//common.unityengine.mathf.lua.bytes应该是mathf.lua.bytes
        byte[] buffer = null;


         Debug.Log("LoaderByAB2 abName=" + abName);
        string bundleName = abName.Replace("lua/", string.Empty).Trim();
        Debug.Log("LoaderByAB2 bundleName=" + bundleName);
        AssetBundle bundle=null;
        foreach(var item in bundleMap)
        {
            string ky= item.Key.Trim();
            Debug.Log("foreach" + item.Key+":"+ bundleName+ String.Equals(bundleName, ky));//foreach lua_common_unityengine:lua_Common_UnityEngine False
            if (String.Equals(bundleName, ky))//在比较之前尝试清除字符串中的空格
            {
                bundle = item.Value;
                Debug.Log("LoaderByAB find assetbundle:"+ ky);
                break;
            }
        }
         //bundleMap.TryGetValue(bundleName, out bundle);
         if (bundle != null)
         {
             Debug.Log("LoaderByAB=" + filename+": "+abName + ": " + resname);
             TextAsset luaCode = bundle.LoadAsset<TextAsset>(resname);
             if (luaCode != null)
             {
                Debug.Log("LoaderByAB=luaCode != null"+ resname+ luaCode.bytes);
                buffer = luaCode.bytes;
                Resources.UnloadAsset(luaCode);
             }
         }
         return buffer;
         
        /*
        byte[] buffer = null;
        sb.Append("lua");
        int pos = fileName.LastIndexOf('/');
        if (pos > 0)
        {
            sb.Append("_");
            sb.Append(fileName, 0, pos).Replace('/', '_');
            fileName = fileName.Substring(pos + 1);
        }

        if (!fileName.EndsWith(".lua"))
        {
            fileName += ".lua";
        }

        fileName += ".bytes";
        */
        /*
        Debug.Log("LoaderByAB2" + abName + ":" + resName);
        string bundleName = abName.Replace("lua/", string.Empty).Replace(".unity3d", "");
        Debug.Log("LoaderByAB3" + bundleName + ":" + resName);
        AssetBundle bundle;
        bundleMap.TryGetValue(bundleName, out bundle);
        if (bundle != null)
        {
            TextAsset luaCode = bundle.LoadAsset<TextAsset>(resName);
            if (luaCode != null)
            {
                buffer = luaCode.bytes;
                Debug.Log("luaCode != null" + buffer);
                Resources.UnloadAsset(luaCode);
            }
        }

        sb.Length = 0;
        sb.Capacity = 0;

        return buffer;
        */
    }
    // 处理AssetBundle
    private void HandleAB()
    {
        
        // 正确热更新模式下读这个路径
        
        foreach (string luaBundleName in ResourceManager.GetInstance().AllManifest)
        {
            if (luaBundleName.StartsWith("lua/"))
            {
                string luaBundleNametemp = luaBundleName.Substring(4);
                Debug.Log("luaBundleName: " + luaBundleNametemp + ": "+ AppConfig.DataPath + luaBundleName);
                
                AssetBundle bundle = AssetBundle.LoadFromFile(AppConfig.DataPath + luaBundleName);
                if (bundle != null)
                {
                    string bundleName = luaBundleNametemp.Replace(".unity3d", string.Empty);
                    //ResourceManager.GetInstance().Addassetbundles(luaBundleName, bundle);
                    Debug.Log("luaBundleName undle != null: "+ bundleName);
                    bundleMap[bundleName] = bundle;
                }

            }

        }
        
        
        //
        //
        /*
        
        // 正确热更新模式下读这个路径
        lua_ab_path = Application.streamingAssetsPath+"/"+"LuaHotfix/";
        Debug.Log("lua_ab_pathlua_ab_path=" + lua_ab_path);
        //lua_ab_path = Application.streamingAssetsPath.Replace("/Assets", "") + "/LuaHotfix/";
        AssetBundle hotFixAB = AssetBundle.LoadFromFile(lua_ab_path + "LuaHotfix");//D:/Users/UnityMMO-farmework/Assets/StreamingAssets/LuaHotfix/LuaHotfix提供完全ab包路径
        hotFixManifest = hotFixAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        hotFixAB.Unload(false);

        foreach (string luaBundleName in hotFixManifest.GetAllAssetBundles())
        {
            Debug.Log("HandleAB luaBundleName: " + luaBundleName); //
            AssetBundle bundle = AssetBundle.LoadFromFile(lua_ab_path + luaBundleName);//D:/Users/UnityMMO-farmework/Assets/StreamingAssets/LuaHotfix/lua/lua_bp.unity3d提供完全ab包路径，有后缀的要写上
            if (bundle != null)
            {
                string bundleName = luaBundleName.Replace("lua/", string.Empty).Replace(".unity3d", "");
                //bundleMap[bundleName] = bundle;
                bundleMap.Add(bundleName, bundle);
                Debug.Log("HandleAB bundleName: " + bundleName); //
            }

        }
        */
        
        
    }
    private void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();
            if (luaUpdate != null)
            {
                try
                {
                    luaUpdate(Time.deltaTime, Time.unscaledDeltaTime,this.PlayAudioComplete,this.PlayAudioStart);
                    this.PlayAudioComplete=false;
                    this.PlayAudioStart=false;
                }
                catch (Exception ex)
                {
                    Debug.LogError("luaUpdate err : " + ex.Message + "\n" + ex.StackTrace);
                }
            }
            // if (Time.frameCount % 6000 == 0)
            // {
            //     luaEnv.FullGc();
            // }
        }
        if(this.talkmonster_uid!=0){
            GameObjectEntity goe=UnityMMO.RoleMgr.GetInstance().GetMainRole();
	        Vector3 mainrolepos = goe.GetComponent<Transform>().localPosition;
	        Entity mon=UnityMMO.SceneMgr.Instance.GetSceneObject(this.talkmonster_uid);
            Vector3 v=UnityMMO.SceneMgr.Instance.EntityManager.GetComponentData<TargetPosition>(goe.Entity).Value;
	        
            if (mon!=Entity.Null){
                Transform curTrans=goe.GetComponent<Transform>();
                Vector3 TargetPosition=UnityMMO.RoleMgr.GetInstance().EntityManager.GetComponentObject<Transform>(mon).localPosition;
                Vector3 groundDir=TargetPosition-mainrolepos;
                groundDir.y=0;
                Vector3 grddir=v-mainrolepos;
                grddir.y=0;
                float moveDistance = Vector3.Magnitude(grddir);
                UnityMMO.MoveQuery query=goe.GetComponent<UnityMMO.MoveQuery>();
                var isAutoFinding = query.IsAutoFinding;
                bool isMoveWanted = moveDistance>0.01f || isAutoFinding;

                if (isMoveWanted) return;
                Vector3 targetDirection = new Vector3(groundDir.x, groundDir.y, groundDir.z);
                Vector3 lookDirection = targetDirection.normalized;
                Quaternion freeRotation = Quaternion.LookRotation(lookDirection, curTrans.up);
                var diferenceRotation = freeRotation.eulerAngles.y - curTrans.eulerAngles.y;
                var eulerY = curTrans.eulerAngles.y;
                if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                var euler = new Vector3(0, eulerY, 0);
                curTrans.rotation = Quaternion.Slerp(curTrans.rotation, Quaternion.Euler(euler), Time.deltaTime*50); 

                Debug.Log("not monster .rotation="+curTrans.rotation);


                Vector3 mongroundDir=mainrolepos-TargetPosition;//怪物指向玩家的
                mongroundDir.y=0;
                Vector3 minpos=UnityMMO.SceneMgr.Instance.EntityManager.GetComponentObject<Transform>(mon).localPosition;
                Vector3 monv=UnityMMO.SceneMgr.Instance.EntityManager.GetComponentData<TargetPosition>(mon).Value;
                grddir=monv-minpos;
                grddir.y=0;
                moveDistance = Vector3.Magnitude(grddir);
                query=UnityMMO.SceneMgr.Instance.EntityManager.GetComponentObject<UnityMMO.MoveQuery>(mon);
                isAutoFinding = query.IsAutoFinding;
                isMoveWanted = moveDistance>0.01f || isAutoFinding;
                Debug.Log("moveDistance monster .rotation="+moveDistance);
                if (isMoveWanted) return;
                curTrans=UnityMMO.MonsterMgr.GetInstance().EntityManager.GetComponentObject<Transform>(mon);
                targetDirection = new Vector3(mongroundDir.x, mongroundDir.y, mongroundDir.z);
                lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, curTrans.up);
                diferenceRotation = freeRotation.eulerAngles.y - curTrans.eulerAngles.y;
                eulerY = curTrans.eulerAngles.y;
                if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                euler = new Vector3(0, eulerY, 0);
                curTrans.rotation = Quaternion.Slerp(curTrans.rotation, Quaternion.Euler(euler), Time.deltaTime*50); 
                Debug.Log("monster .rotation="+curTrans.rotation);
            }
        }
    }

    private void LateUpdate() {
        if (luaLateUpdate != null)
        {
            try
            {
                luaLateUpdate();
            }
            catch (Exception ex)
            {
                Debug.LogError("luaLateUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
    
    private void FixedUpdate() {
        if (luaFixedUpdate != null)
        {
            try
            {
                luaFixedUpdate(Time.fixedDeltaTime);
            }
            catch (Exception ex)
            {
                Debug.LogError("luaFixedUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
    private void OnDestroy()
    {
        luaUpdate = null;
        luaLateUpdate = null;
        luaFixedUpdate = null;


        NetworkManager.GetInstance().OnConnectCallBack = null;
        NetworkManager.GetInstance().OnDisConnectCallBack = null;
        NetworkManager.GetInstance().OnReceiveLineCallBack = null;
        NetworkManager.GetInstance().OnReceiveMsgCallBack = null;
        CSLuaBridge.GetInstance().ClearDelegate();
        LoadScript("LuaMain");
        SafeDoString("ExitGame()");
        if (luaEnv != null)
        {
            try
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg, null);
            }
        }
    }
    
#if UNITY_EDITOR
public static class LuaUpdaterExporter
{
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Action),
        typeof(Action<byte[]>),
        typeof(Action<float>),
        typeof(Action<float, float>),
    };
}
#endif

    
}
