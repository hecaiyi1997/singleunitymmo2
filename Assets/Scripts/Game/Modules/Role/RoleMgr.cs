using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityMMO.Component;

namespace UnityMMO
{
public class RoleMgr
{
	static RoleMgr Instance;
    GameWorld m_GameWorld;
    private Transform container;
    GameObjectEntity mainRoleGOE;
    Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();
    Dictionary<long, string> names = new Dictionary<long, string>();
    Dictionary<long, RoleLooksInfo> looksInfos = new Dictionary<long, RoleLooksInfo>();
    public EntityManager EntityManager { get => m_GameWorld.GetEntityManager();}
    public Transform RoleContainer { get => container; set => container = value; }

    public static RoleMgr GetInstance()
    {
        if (Instance!=null)
            return Instance;
        Instance = new RoleMgr();
        return Instance;
    }
    private RoleMgr(){}

    public void Init(GameWorld world)
	{
        m_GameWorld = world;
        container = GameObject.Find("SceneObjContainer/RoleContainer").transform;
	}

    public void OnDestroy()
	{
		Instance = null;
	}

    public Entity AddMainRole(long uid, long typeID, string name, int career, Vector3 pos, float curHp, float maxHp)
	{
        GameObjectEntity roleGameOE = m_GameWorld.Spawn<GameObjectEntity>(ResMgr.GetInstance().GetPrefab("MainRole"));
        // Debug.Log("add main role : "+uid+" "+new System.Diagnostics.StackTrace().ToString());
        roleGameOE.name = "MainRole_"+uid;
        roleGameOE.transform.SetParent(container);
        
        Vector3 ps = new Vector3 { x = 965, y = 250, z = 1005 };
        if (pos != Vector3.zero) ps = pos;
        roleGameOE.transform.localPosition = ps;
        Entity role = roleGameOE.Entity;
        SetName(uid, name);
        Debug.Log("setName namenamename" + name+GetName(uid)+ roleGameOE.transform.localPosition.x+":"+roleGameOE.transform.localPosition.y+ ":" + roleGameOE.transform.localPosition.z);
        var roleInfo = roleGameOE.GetComponent<RoleInfo>();
        roleInfo.Name = name;
        roleInfo.Career = career;
        Debug.Log("init info.carrer=" + roleInfo.Career);

        InitRole(roleGameOE, uid, typeID, ps, ps, curHp, maxHp, false);

        EntityManager.AddComponentData(role, new PosSynchInfo {LastUploadPos = float3.zero});
        EntityManager.AddComponent(role, ComponentType.ReadWrite<UserCommand>());
        var nameboardData = EntityManager.GetComponentObject<NameboardData>(role);
        nameboardData.SetName(name);
        mainRoleGOE = roleGameOE;
        SceneMgr.Instance.ApplyMainRole(roleGameOE);
        
        UpdateLooksInfo(uid, new RoleLooksInfo
            {
                uid = uid,
                career = (int)career,
                body = 1,
                hair = 1,
                weapon = 1,
                wing = 1,
                horse = 1,
                hp = (int)curHp,
                maxHp = (int)maxHp,
                name = name,
            });
            return role;
	}

    public void UpdateMainRoleNavAgent()
    {
        if (mainRoleGOE != null)
        {
            Debug.Log("role mgr reset nav agent");
            var moveQuery = mainRoleGOE.GetComponent<MoveQuery>();
            moveQuery.UpdateNavAgent();
        }
    }

    public GameObjectEntity GetMainRole()
    {
        return mainRoleGOE;
    }

        public long GetMainRoleUID()
    {
        if (mainRoleGOE != null)
        {
            return EntityManager.GetComponentData<UID>(mainRoleGOE.Entity).Value;
        }
        return 0;
    }

    public bool IsMainRoleEntity(Entity entity)
    {
        if (mainRoleGOE==null || entity==Entity.Null)
            return false;
        return mainRoleGOE.Entity == entity;
    }

    public bool IsRoleEntity(Entity entity)
    {
        var sceneObjectTypeData = EntityManager.GetComponentData<SceneObjectTypeData>(entity);
        return sceneObjectTypeData.Value == SceneObjectType.Role;
    }

    public Entity AddRole(long uid, long typeID, Vector3 pos, Vector3 targetPos, float curHp, float maxHp)
	{
        GameObjectEntity roleGameOE = m_GameWorld.Spawn<GameObjectEntity>(ResMgr.GetInstance().GetPrefab("Role"));
        roleGameOE.name = "Role_"+uid;
        roleGameOE.transform.SetParent(container);
        roleGameOE.transform.localPosition = pos;
        Entity role = roleGameOE.Entity;
        InitRole(roleGameOE, uid, typeID, pos, targetPos, curHp, maxHp);
        return role;
	}

    private void InitRole(GameObjectEntity roleGOE, long uid, long typeID, Vector3 pos, Vector3 targetPos, float curHp, float maxHp, bool isNeedNavAgent=false)
    {
        Entity role = roleGOE.Entity;
        roleGOE.GetComponent<UIDProxy>().Value = new UID{Value=uid};
        EntityManager.AddComponentData(role, new TargetPosition {Value = targetPos});
        var locoStateData = new LocomotionState {LocoState = curHp>0?LocomotionState.State.Idle:LocomotionState.State.Dead};
        //It should have been dead for a long time
        if (curHp==0)
            locoStateData.StartTime = 0;
        EntityManager.AddComponentData(role, locoStateData);
        EntityManager.AddComponentData(role, new LooksInfo {CurState=LooksInfo.State.None, LooksEntity=Entity.Null});
        EntityManager.AddComponentData(role, new SceneObjectTypeData {Value=SceneObjectType.Role});
        EntityManager.AddComponentData(role, new TypeID {Value=typeID});
        //EntityManager.AddComponentData(role, new UID { Value = uid });
        EntityManager.AddComponentData(role, new GroundInfo {GroundNormal=Vector3.zero, Altitude=0});
        EntityManager.AddComponentData(role, new JumpData{JumpCount=0});
        EntityManager.AddComponentData(role, ActionData.Empty);
        EntityManager.AddComponentData(role, new PosOffset {Value = float3.zero});
        EntityManager.AddComponentData(role, new HealthStateData {CurHp=curHp, MaxHp=maxHp});
        EntityManager.AddComponentData(role, new TimelineState {NewStatus=TimelineState.NewState.Allow, InterruptStatus=TimelineState.InterruptState.Allow});

        roleGOE.gameObject.AddComponent<SpeedData>();
        var speedData = roleGOE.gameObject.GetComponent<SpeedData>();
        speedData.InitSpeed(12);
        EntityManager.AddComponentObject(role, speedData);

        roleGOE.gameObject.AddComponent<ParticleEffects>();
        var particleEffects = roleGOE.gameObject.GetComponent<ParticleEffects>();
        EntityManager.AddComponentObject(role, particleEffects);
        
        MoveQuery rmq = EntityManager.GetComponentObject<MoveQuery>(role);
        rmq.Initialize(isNeedNavAgent);
        Debug.Log("GameVariable.IsSingleMode=" + GameVariable.IsSingleMode);
        if (GameVariable.IsSingleMode)
        {
                this.CreateLooks(role);

        }
        var dynamicBuffer = SceneMgr.Instance.EntityManager.AddBuffer<DamageEvent>(role);
        dynamicBuffer.Add(new DamageEvent { instigator = role, damage = 0, direction = Vector3.zero, impulse = 0 });
        }

    private void CreateLooks(Entity ownerEntity)
    {
           
            RoleInfo info = EntityManager.GetComponentObject<RoleInfo>(ownerEntity);
            Debug.Log("info.carrer=" + info.Career);
            var looksInfo = EntityManager.GetComponentData<LooksInfo>(ownerEntity);
            int career = info.Career;
            int body = 1;
            int hair = 1;
            // Debug.Log("body : "+body+" hair:"+hair);
            string bodyPath = ResPath.GetRoleBodyResPath(career, body);
            string hairPath = ResPath.GetRoleHairResPath(career, hair);
            Debug.Log("SpawnRoleLooks bodyPath : "+bodyPath);
            XLuaFramework.ResourceManager.GetInstance().LoadAsset<GameObject>(bodyPath, delegate (UnityEngine.Object[] objs) {
                if (objs != null && objs.Length > 0)
                {
                    GameObject bodyObj = objs[0] as GameObject;
                    GameObjectEntity bodyOE = m_GameWorld.Spawn<GameObjectEntity>(bodyObj);
                    var parentTrans = EntityManager.GetComponentObject<Transform>(ownerEntity);
                    //parentTrans.localPosition = new Vector3 { x = 965, y = 550, z = 1005 } ;
                    bodyOE.transform.SetParent(parentTrans);
                    bodyOE.transform.localPosition = Vector3.zero;
                    bodyOE.transform.localRotation = Quaternion.identity;
                    ECSHelper.UpdateNameboardHeight(ownerEntity, bodyOE.transform);
                    LoadHair(hairPath, bodyOE.transform.Find("head"));
                    // Debug.Log("load ok role model");
                    looksInfo.CurState = LooksInfo.State.Loaded;
                    looksInfo.LooksEntity = bodyOE.Entity;
                    EntityManager.SetComponentData<LooksInfo>(ownerEntity, looksInfo);
                }
                else
                {
                    Debug.LogError("cannot fine file " + bodyPath);
                }
            });
        }
    void LoadHair(string hairPath, Transform parentNode)
     {
            XLuaFramework.ResourceManager.GetInstance().LoadPrefabGameObjectWithAction(hairPath, delegate (UnityEngine.Object obj) {
                var hairObj = obj as GameObject;
                hairObj.transform.SetParent(parentNode);
                hairObj.transform.localPosition = Vector3.zero;
                hairObj.transform.localRotation = Quaternion.identity;
            });
     }
    public string GetName(long uid)
    {
        string name = "";
        names.TryGetValue(uid, out name);
        return name;
    }

    public void SetName(long uid, string name)
    {
        names[uid] = name;
    }

    public void UpdateLooksInfo(long uid, RoleLooksInfo info)
    {
        //Cat_TODO:增加时间字段，如果取值时超过一定时间就向服务器拿最新的
        looksInfos[uid] = info;
    }

    public RoleLooksInfo GetLooksInfo(long uid)
    {
        RoleLooksInfo info = new RoleLooksInfo();
        looksInfos.TryGetValue(uid, out info);
        return info;
    }

    public RoleLooksInfo GetMainRoleLooksInfo()
    {
        long uid = GetMainRoleUID();
        return GetLooksInfo(uid);
    }

    public void StopMainRoleRunning()
    {
        if (mainRoleGOE == null)
            return;
        var query = mainRoleGOE.GetComponent<MoveQuery>();
        query.StopFindWay();
        var auto = mainRoleGOE.GetComponent<AutoFight>();
        auto.enabled = false;
    }
}

    public struct RoleLooksInfo
    {
        public Int64 uid;
        public int career;
        public int body;
        public int hair;
        public int weapon;
        public int wing;
        public int horse;
        public Int64 hp;
        public Int64 maxHp;
        public string name;
        // public RoleLooksInfo() => this.name = "NoOne";
    }
}