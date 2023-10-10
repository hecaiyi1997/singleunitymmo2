
using UnityEngine;

public class PackRule
{
    static string UIPath = "Assets/AssetBundleRes/ui/";
    static string ScenePath = "Assets/AssetBundleRes/scene/";
    static string RolePath = "Assets/AssetBundleRes/role/";
    static string EffectPath = "Assets/AssetBundleRes/effect/";
    static string NPCPath = "Assets/AssetBundleRes/npc/";
    static string MonsterPath = "Assets/AssetBundleRes/monster/";

    static string SplitPrefabsPath = "Assets/AssetBundleRes/SplitPrefabs/";
    static string SplitTerrainPath = "Assets/AssetBundleRes/SplitTerrain/";

    static string LuaPath = "assets/luatemp/";//

    static string LuabundlePath = "lua/";

    static string GoodsPath = "Assets/AssetBundleRes/Goods/";
    public static string PathToAssetBundleName(string path)
    {
        path = path.Replace('\\', '/');
        Debug.Log("PathToAssetBundleName path : " + path);
        // string ab_name = "";
        if (path.StartsWith(UIPath))
        {
            string sub_path = path.Substring(UIPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length>0)
                return  "ui_"+path_parts[0];
        }
        else if (path.StartsWith(ScenePath))
        {
            string sub_path = path.Substring(ScenePath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length > 0)
            {
                Debug.Log("PathToAssetBundleName path_parts[0] : " +sub_path +":"+ path_parts[0]);
                return "scene_" + path_parts[0];
            }
                //return  "scene_"+path_parts[0];
        }
        else if (path.StartsWith(RolePath))
        {
            string sub_path = path.Substring(RolePath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length>0)
                return "role_"+path_parts[0];
        }
        else if (path.StartsWith(EffectPath))
        {
            string sub_path = path.Substring(EffectPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length>0)
                return "effect_"+path_parts[0];
        }
        else if (path.StartsWith(NPCPath))
        {
            string sub_path = path.Substring(NPCPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length>0)
                return "npc_"+path_parts[0];
        }
        else if (path.StartsWith(MonsterPath))
        {
            string sub_path = path.Substring(MonsterPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length>0)
                return "monster_"+path_parts[0];
        }
        else if (path.StartsWith(SplitPrefabsPath))
        {
            string sub_path = path.Substring(SplitPrefabsPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length > 0)
                return "SplitPrefabs_" + path_parts[0];
        }
        else if (path.StartsWith(SplitTerrainPath))
        {
            string sub_path = path.Substring(SplitTerrainPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length > 0)
                return "SplitTerrain_" + path_parts[0];
        }
        else if (path.StartsWith(LuaPath))
        {
            Debug.Log("LuaPath1=" + path);
            string sub_path = path.Substring(LuaPath.Length);
            Debug.Log("LuaPath2=" + sub_path);
            string sub_path2 = sub_path.Substring(0, sub_path.Length-11);//.lua.bytes
            Debug.Log("LuaPath3=" + sub_path2);
            string[] path_parts = sub_path2.Split('/');
            string sum = "";
            if (path_parts.Length > 1)
            {
                for (int i = 0; i < path_parts.Length-1; i++)
                {
                    Debug.Log("LuaPathi=" + path_parts[i]);
                    sum = sum + "_" + path_parts[i];

                }
                Debug.Log("LuaPath4=" + "lua/lua" + sum);
                return "lua/lua" + sum;
            }

            else
            {
                Debug.Log("LuaPath5=" + "lua/lua");
                return "lua/lua ";
            }
        }
        else if (path.StartsWith(GoodsPath))
        {
            string sub_path = path.Substring(GoodsPath.Length);
            string[] path_parts = sub_path.Split('/');
            if (path_parts.Length > 0)
                return "Goods_" + path_parts[0];
        }
        Debug.LogError("PackRule:PathToAssetBundleName : cannot find ab name : " + path);
        return "";
    } 
}