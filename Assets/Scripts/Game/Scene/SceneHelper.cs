using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityMMO.Component;

namespace UnityMMO
{
public struct RaycastSceneObjHit 
{
    public Entity entity;
    public Vector3 point;
    public bool hit;
    public long typeID;
    }
public class SceneHelper
{
    private static EntityManager EntityMgr;
    public static Entity[] UIDTempCacheList = new Entity[10];

    public static void Init(EntityManager entityMgr)
    {
        EntityMgr = entityMgr;
    }

        public static void changehp(float num)
        {
            GameObjectEntity goe = UnityMMO.RoleMgr.GetInstance().GetMainRole();
            if (goe.Entity == Entity.Null)
            {
                Debug.LogError("changehp goe.Entity == Entity.Null");
                return;

            }
            Entity e = goe.Entity;
            var healthData = EntityMgr.GetComponentData<HealthStateData>(e);
            healthData.CurHp = healthData.CurHp + num;
            if (healthData.CurHp > 100) healthData.CurHp = 100.0f;
            EntityMgr.SetComponentData(e, healthData);
            XLuaFramework.CSLuaBridge.GetInstance().CallLuaFunc2Num(GlobalEvents.MainRoleHPChanged, (long)(healthData.CurHp), (long)healthData.MaxHp);

            var nameboardData = EntityMgr.GetComponentObject<NameboardData>(e);
            if (nameboardData.UIResState == NameboardData.ResState.Loaded)
            {
                var nameboardNode = nameboardData.LooksNode.GetComponent<Nameboard>();
                if (nameboardNode != null)
                {
                    nameboardNode.CurHp = healthData.CurHp;

                }
            }
        }
    public static RaycastSceneObjHit GetClickSceneObject()
    {
        RaycastSceneObjHit result = new RaycastSceneObjHit();
        result.hit = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Debug.Log("Input.mousePosition : "+Input.mousePosition.x+" "+Input.mousePosition.y+" z:"+Input.mousePosition.z);
        RaycastHit hit=new RaycastHit();
        if(Physics.Raycast(ray,out hit))
        {
            Debug.Log("get click scene object : "+hit.collider.name);
            result.point = hit.point;
            result.hit = true;
            var uid = hit.collider.GetComponentInParent<UIDProxy>();
            if (uid != null)
            {
                result.entity = SceneMgr.Instance.GetSceneObject(uid.Value.Value);
            }
            if(hit.collider.name== "bottle_green")
            {
                    result.typeID = 100000;
                    GameObject.Destroy(hit.collider.transform.parent.parent.gameObject);

            }
           else if (hit.collider.name == "bottle_red") 
           {
                    result.typeID = 100002;
                    GameObject.Destroy(hit.collider.transform.parent.parent.gameObject);

                }
           else if (hit.collider.name == "bottle_blue") 
           {
                    result.typeID = 100001;
                    GameObject.Destroy(hit.collider.transform.parent.parent.gameObject);
                }
        }
        return result;
    }

    //unfinished method
    public static long GetSceneObjectByPos(Vector3 absPos, Dictionary<long, Entity> entityDic)
    {
        int inAreaNum = 0; 
        foreach (var item in entityDic)
        {
            var isIn = IsPosInEntityBound(absPos, item.Value);
            if (isIn)
            {
                UIDTempCacheList[inAreaNum] = item.Value;
                inAreaNum++;
            }
        }
        return 0;
    }

    public static bool IsPosInEntityBound(Vector3 absPos, Entity entity)
    {
        var hasMoveQuery = EntityMgr.HasComponent<MoveQuery>(entity);
        if (hasMoveQuery)
        {
            var moveQuery = EntityMgr.GetComponentObject<MoveQuery>(entity);
            var entityPos = moveQuery.transform.position;
            var bottomY = entityPos.y;
            var topY = entityPos.y + moveQuery.height;
            var leftX = entityPos.x - moveQuery.radius;
            var rightX = entityPos.x + moveQuery.radius;
        }
        return false;
    }

}
}