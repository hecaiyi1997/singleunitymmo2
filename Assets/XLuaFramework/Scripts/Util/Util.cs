using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using XLua;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XLuaFramework {
    [LuaCallCSharp]
    public class Util {
        private static List<string> luaPaths = new List<string>();

        public static int Int(object o) {
            return Convert.ToInt32(o);
        }

        public static float Float(object o) {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o) {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max) {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max) {
            return UnityEngine.Random.Range(min, max);
        }

        public static string Uid(string uid) {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime() {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 搜索子物体组件-GameObject版
        /// </summary>
        public static T Get<T>(GameObject go, string subnode) where T : Component {
            if (go != null) {
                Transform sub = go.transform.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Transform版
        /// </summary>
        public static T Get<T>(Transform go, string subnode) where T : Component {
            if (go != null) {
                Transform sub = go.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Component版
        /// </summary>
        public static T Get<T>(Component go, string subnode) where T : Component {
            return go.transform.Find(subnode).GetComponent<T>();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component {
            if (go != null) {
                T[] ts = go.GetComponents<T>();
                for (int i = 0; i < ts.Length; i++) {
                    if (ts[i] != null) GameObject.Destroy(ts[i]);
                }
                return go.gameObject.AddComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(Transform go) where T : Component {
            return Add<T>(go.gameObject);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(GameObject go, string subnode) {
            return Child(go.transform, subnode);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(Transform go, string subnode) {
            Transform tran = go.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode) {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(Transform go, string subnode) {
            Transform tran = go.parent.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string md5(string source) {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++) {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file) {
            try {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++) {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            } catch (Exception ex) {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(Transform go) {
            if (go == null) return;
            for (int i = go.childCount - 1; i >= 0; i--) {
                GameObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        // public static void ClearMemory() {
        //     GC.Collect(); Resources.UnloadUnusedAssets();
        //     LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        //     if (mgr != null) mgr.LuaGC();
        // }


        /// <summary>
        /// 取得行文本
        /// </summary>
        public static string GetFileText(string path) {
            return File.ReadAllText(path);
        }

        public static byte[] GetFileBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }
                // File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        public static string GetFileNamesInFolder(string path, string separate=",")
        {
            System.IO.DirectoryInfo direction = new System.IO.DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*.lua", SearchOption.AllDirectories);
            StringBuilder strbuf = new StringBuilder();
            foreach (var file in files)
            {
                strbuf.Append(Path.GetFileNameWithoutExtension(file.Name) + separate);
            }
            strbuf.Remove(strbuf.Length - 1, 1);
            return strbuf.ToString();
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable {
            get {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi {
            get {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }


        /// <summary>
        /// 应用程序内容路径StreamingAssets路径
        /// </summary>
        public static string AppContentPath(){
            string path = string.Empty;
            /*
            string head = string.Empty;
            if (Application.platform == RuntimePlatform.Android)
                head="jar:file://";
            else
                head="";
            */
            
           
            Debug.Log("AppContentPath Application.platform=11?" + Application.platform+ RuntimePlatform.Android);
            string pathh = "";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                pathh = string.Format("{0}/StreamingAssets/", Application.dataPath.Replace("/Assets",""));//window
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                //pathh = string.Format("{0}!/assets/", Application.dataPath);//在网上看到这样写，我觉得应该pathh = string.Format("{0}/!/assets/", Application.dataPath);
                /*                 
                Application.xxxPath对应的路径
                IOS:
                Application.dataPath :                      Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/xxx.app/Data
                Application.streamingAssetsPath :   Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/xxx.app/Data/Raw
                Application.persistentDataPath :      Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Documents
                Application.temporaryCachePath :   Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Library/Caches

                Android:
                Application.dataPath :                         /data/app/xxx.xxx.xxx.apk
                Application.streamingAssetsPath :      jar:file:///data/app/xxx.xxx.xxx.apk/!/assets
                Application.persistentDataPath :         /data/data/xxx.xxx.xxx/files
                Application.temporaryCachePath :      /data/data/xxx.xxx.xxx/cache

                Windows:
                Application.dataPath :                         /Assets
                Application.streamingAssetsPath :      /Assets/StreamingAssets
                Application.persistentDataPath :         C:/Users/xxxx/AppData/LocalLow/CompanyName/ProductName
                Application.temporaryCachePath :      C:/Users/xxxx/AppData/Local/Temp/CompanyName/ProductName

                Mac:
                Application.dataPath :                         /Assets
                Application.streamingAssetsPath :      /Assets/StreamingAssets
                Application.persistentDataPath :         /Users/xxxx/Library/Caches/CompanyName/Product Name
                Application.temporaryCachePath :     /var/folders/57/6b4_9w8113x2fsmzx_yhrhvh0000gn/T/CompanyName/Product Name
                */
                
                pathh = Application.streamingAssetsPath + "/";
                Debug.Log("AppContentPath------pathh="+pathh);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //pathh = string.Format("{0}/Raw/", Application.dataPath.Replace("/Assets", ""));
                pathh = Application.streamingAssetsPath + "/";
            }
            Debug.Log("Application.platform=" + Application.platform);

            //path = head+pathh;
            path =pathh;
            return path;
        }

        public static void Log(string str) {
            Debug.Log(str);
        }

        public static void LogWarning(string str) {
            Debug.LogWarning(str);
        }

        public static void LogError(string str) {
            Debug.LogError(str);
        }

        /// <summary>
        /// 防止初学者不按步骤来操作
        /// </summary>
        /// <returns></returns>
        public static int CheckRuntimeFile() {
            if (!Application.isEditor) return 0;
            string streamDir = Application.dataPath.Replace("/Assets","") + "/StreamingAssets/";
            if (!Directory.Exists(streamDir)) {
                return -1;
            } else {
                string[] files = Directory.GetFiles(streamDir);
                if (files.Length == 0) return -1;

                if (!File.Exists(streamDir + "files.txt")) {
                    return -1;
                }
            }
            string sourceDir = AppConfig.FrameworkRoot + "/ToLua/Source/Generate/";
            if (!Directory.Exists(sourceDir)) {
                return -2;
            } else {
                string[] files = Directory.GetFiles(sourceDir);
                if (files.Length == 0) return -2;
            }
            return 0;
        }

        /// <summary>
        /// 执行Lua方法
        /// </summary>
        // public static object[] CallMethod(string module, string func, params object[] args) {
        //     LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        //     if (luaMgr == null) return null;
        //     return luaMgr.CallFunction(module + "." + func, args);
        // }

                /// <summary>
        /// 检查运行环境
        /// </summary>
        public static bool CheckEnvironment() {
#if UNITY_EDITOR
            // int resultId = Util.CheckRuntimeFile();
            // if (resultId == -1) {
            //     Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
            //     EditorApplication.isPlaying = false;
            //     return false;
            // } else if (resultId == -2) {
            //     Debug.LogError("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
            //     EditorApplication.isPlaying = false;
            //     return false;
            // }
#endif
            return true;
        }

        public static void ThrowLuaException(string error, Exception exception = null, int skip = 1)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                Debug.LogWarning(error);
            }
            else
            {
                Debug.LogError(error);
            }
        }

        public static GameObject InstantiateObject(GameObject prefab)
        {
            try
            {
                return GameObject.Instantiate(prefab);
            }
            catch (Exception e)
            {
                Util.ThrowLuaException(e.Message);
                return null;
            }
        }
        
        public static ushort SwapUInt16(ushort n)
        {
            //Debug.Log("SwapUInt16() BitConverter.IsLittleEndian : " + BitConverter.IsLittleEndian.ToCString());
            if (BitConverter.IsLittleEndian)
                return (ushort)(((n & 0xff) << 8) | ((n >> 8) & 0xff));
            else
                return n;
        }
        public static short SwapInt16(short n)  
        {  
            if (BitConverter.IsLittleEndian)
                return (short)(((n & 0xff) << 8) | ((n >> 8) & 0xff));  
            else
                return n;
        }  
        public static int SwapInt32(int n)  
        {  
            if (BitConverter.IsLittleEndian)
                return (int)(((SwapInt16((short)n) & 0xffff) << 0x10) |  
                    (SwapInt16((short)(n >> 0x10)) & 0xffff));  
            else
                return n;
        }  

        public static uint SwapUInt32(uint n)  
        {  
            return (uint)(((SwapUInt16((ushort)n) & 0xffff) << 0x10) |  
                (SwapUInt16((ushort)(n >> 0x10)) & 0xffff));  
        }  

        public static long SwapInt64(long n)  
        {  
            return (long)(((SwapInt32((int)n) & 0xffffffffL) << 0x20) |  
                (SwapInt32((int)(n >> 0x20)) & 0xffffffffL));  
        }  

        public static ulong SwapUInt64(ulong n)  
        {  
            return (ulong)(((SwapUInt32((uint)n) & 0xffffffffL) << 0x20) |  
                (SwapUInt32((uint)(n >> 0x20)) & 0xffffffffL));  
        }

        public static int FindMaterial(Material[] materials, string materialName)
        {
            // var materials = renderer.materials;
            int materialIndex = -1;
            for (int i = 0; i < materials.Length; i++)
            {
                // Debug.Log("material name :"+materials[i].name+"  "+materials[i].name.IndexOf(materialName));
                if (-1 != materials[i].name.IndexOf(materialName))
                {
                    materialIndex = i;
                    break;
                }
            }
            return materialIndex;
        }
    }
}