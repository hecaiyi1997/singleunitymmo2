using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
using XLuaFramework;
using Unity.Entities;
using UnityMMO.Component;


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
    bool PlayAudioStart=false;

    long talkmonster_uid=0;

    bool PlayAudioComplete=false;

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
            luaEnv.AddLoader(CustomLoader);

            InitExternal();
            LoadScript("BaseRequire");

            luaUpdate = luaEnv.Global.Get<Action<float, float,bool,bool>>("Update");
            luaLateUpdate = luaEnv.Global.Get<Action>("LateUpdate");
            luaFixedUpdate = luaEnv.Global.Get<Action<float>>("FixedUpdate");

            if (NetworkManager.GetInstance())
            {
                SafeDoString("require 'NetDispatcher'");
                NetworkManager.GetInstance().OnConnectCallBack += luaEnv.Global.Get<Action<byte[]>>("OnConnectServer");
                NetworkManager.GetInstance().OnDisConnectCallBack += luaEnv.Global.Get<Action<byte[]>>("OnDisConnectFromServer");
                NetworkManager.GetInstance().OnReceiveLineCallBack += luaEnv.Global.Get<Action<byte[]>>("OnReceiveLineFromServer");
                NetworkManager.GetInstance().OnReceiveMsgCallBack += luaEnv.Global.Get<Action<byte[]>>("OnReceiveMsgFromServer");
                NetworkManager.GetInstance().onApplyTalkExp += luaEnv.Global.Get<Action<int,long>>("onApplyTalkExp");

                //SpeechVoice.Instance.PlayAudioComplete+=luaEnv.Global.Get<EventHandler>("PlayAudioComplete");
                //SpeechVoice.Instance.PlayAudioStart+=luaEnv.Global.Get<EventHandler>("PlayAudioStart");

                SpeechVoice.Instance.PlayAudioComplete+=delegate(object sender, EventArgs e){
                    this.onPlayAudioComplete();
                };
                SpeechVoice.Instance.PlayAudioStart+=delegate(object sender, EventArgs e){
                    this.onPlayAudioStart();
                };
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

    //登录成功后，在lua端的LoginController会调用本方法的
    public void OnLoginOk()
    {
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
                luaEnv.DoString(scriptContent, chunkName);
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
    public static byte[] CustomLoader(ref string filepath)
    {
        string scriptPath = string.Empty;
        filepath = filepath.Replace(".", "/") + ".lua";

        if (!IsUseLuaFileWithoutBundle)
        {
            scriptPath = Path.Combine(AppConfig.LuaAssetsDir, filepath);
            // Debug.Log("Load lua script : " + scriptPath);
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
