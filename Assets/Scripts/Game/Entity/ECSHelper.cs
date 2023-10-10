using UnityEngine;
using Unity.Entities;
using UnityMMO.Component;
using UnityEngine.Playables;
using UnityEngine.AI;
using System.Security.Cryptography;
using System;


namespace UnityMMO
{
    public class ECSHelper
    {
        public static EntityManager entityMgr;
        public static string[] goodnamearr = new string[] { "Bottle_Endurance", "Bottle_Health", "Bottle_Mana" };
        public static bool IsDead(Entity entity)
        {

            var hpData = entityMgr.GetComponentData<HealthStateData>(entity);
            return hpData.CurHp <= 0;
        }

        public static void ChangeLocoState(Entity entity, LocomotionState newState)
        {
            var hasStateStack = entityMgr.HasComponent<LocomotionStateStack>(entity);
            if (hasStateStack)
            {
                var lastState = entityMgr.GetComponentData<LocomotionState>(entity);
                // Debug.Log("change state entity : "+entity+" state:"+lastState.LocoState+"  "+newState.LocoState+" trace:" + new System.Diagnostics.StackTrace().ToString());
                if (lastState.LocoState != newState.LocoState && LocomotionState.IsStateNeedStack(lastState.LocoState))
                {
                    var stack = entityMgr.GetComponentObject<LocomotionStateStack>(entity);
                    stack.Stack.Push(lastState);
                }
            }
            entityMgr.SetComponentData<LocomotionState>(entity, newState);
        }

        public static void ChangeHP(Entity entity, long hp, long flag, long attackerUID)
        {
            float curHp = (float)hp;
            var healthData = entityMgr.GetComponentData<HealthStateData>(entity);
            healthData.CurHp = curHp;
            entityMgr.SetComponentData(entity, healthData);

            if (hp == 0)
            {
                var isMainrole = RoleMgr.GetInstance().IsMainRoleEntity(entity);
                if (!isMainrole)
                {
                    GameObjectEntity goe = UnityMMO.RoleMgr.GetInstance().GetMainRole();
                    long roleid = entityMgr.GetComponentData<UID>(goe.Entity).Value;
                    //long uid = entityMgr.GetComponentData<UID>(goe.Entity).Value;

                    Transform p = entityMgr.GetComponentObject<Transform>(entity).parent;
                    Transform monpos = entityMgr.GetComponentObject<Transform>(entity);

                    byte[] randomBytes = new byte[4];
                    RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
                    rngServiceProvider.GetBytes(randomBytes);
                    Int32 result = BitConverter.ToInt32(randomBytes, 0);
                    
                    int idx = Mathf.Abs(result) % 3;
                    Debug.Log("random="+idx+":"+ result);


                    var looksNode = ResMgr.GetInstance().GetGameObject(goodnamearr[idx]);
                    looksNode.transform.SetParent(p);

                    NavMeshHit closestHit;
                    // Debug.Log("transform.position : "+transform.position.x+" "+transform.position.y+" "+transform.position.z);
                   // if (NavMesh.Raycast(monpos.position, monpos.position+ new Vector3(0,-500f,0), out closestHit, NavMesh.AllAreas))
                    if (NavMesh.SamplePosition(monpos.position, out closestHit, 1000f, NavMesh.AllAreas))
                            //if (Physics.Raycast(monpos.position,Vector3.down, out hit, 1000))
                     {
                        Debug.Log("NavMesh.Raycast sample pos" + closestHit.position.x + ":" + closestHit.position.y + ":" + closestHit.position.z);
                        //transform.position = closestHit.position;
                        Debug.DrawLine(monpos.position, closestHit.position+new Vector3(0,20f,0), Color.red);

                        looksNode.transform.position = new Vector3(closestHit.position.x, closestHit.position.y-1.05f, closestHit.position.z);
                    }    

                    long monsterid = entityMgr.GetComponentData<TypeID>(entity).Value;
                    XLuaManager.Instance.onKillMonster.Invoke(roleid, monsterid, 1); ;





                }
                else//玩家死去保存复活的位置
                {
                    Vector3 pos = UnityMMO.RoleMgr.GetInstance().GetMainRole().transform.position;
                    GameObjectEntity goe = UnityMMO.RoleMgr.GetInstance().GetMainRole();
                    long roleid = entityMgr.GetComponentData<UID>(goe.Entity).Value;

                    XLuaManager.Instance.userinfo.Set("role_id", roleid);
                    //XLuaManager.Instance.userinfo.Set("career", out int career);用原来的
                    XLuaManager.Instance.userinfo.Set("pos_x", pos.x);
                    XLuaManager.Instance.userinfo.Set("pos_y", pos.y);
                    XLuaManager.Instance.userinfo.Set("pos_z", pos.z);
                    XLuaManager.Instance.userinfo.Set("cur_hp",100);
                    XLuaManager.Instance.userinfo.Set("max_hp",100);
                    //XLuaManager.Instance.userinfo.Set("name", out string name);用原来的不用更新
                    Debug.Log("setName namenamename role dead" + pos.x + ":" + pos.y + ":" + pos.z);

                }
            }

            var isMainRole = RoleMgr.GetInstance().IsMainRoleEntity(entity);
            if (isMainRole)
            {
                XLuaFramework.CSLuaBridge.GetInstance().CallLuaFunc2Num(GlobalEvents.MainRoleHPChanged, hp, (long)healthData.MaxHp);
                Debug.Log("isMainRole ChangeHP"+ curHp);

            }
                
            bool hasNameboardData = entityMgr.HasComponent<NameboardData>(entity);
            var isRelive = flag==5;//复活
            var isDead = hp==0;//死亡
            if (hasNameboardData)
            {
                var nameboardData = entityMgr.GetComponentObject<NameboardData>(entity);
                if (nameboardData.UIResState==NameboardData.ResState.Loaded)
                {
                    var nameboardNode = nameboardData.LooksNode.GetComponent<Nameboard>();
                    if (nameboardNode != null)
                    {
                        nameboardNode.CurHp = curHp;
                        //remove nameboard when dead
                        if (isDead)
                        {
                            nameboardData.UnuseLooks();
                            // SceneMgr.Instance.World.RequestDespawn(nameboardNode.gameObject);
                            nameboardData.UIResState = NameboardData.ResState.DontLoad;

                            // nameboardData.LooksNode = null;
                            //entityMgr.GetComponentObject<NameboardData>(entity, nameboardData);
                        }
                    }
                }
                else if (nameboardData.UIResState==NameboardData.ResState.DontLoad)
                {
                    if (isRelive)
                    {
                        nameboardData.UIResState = NameboardData.ResState.WaitLoad;
                        //entityMgr.SetComponentData(entity, nameboardData);
                    }
                }
            }
            if (isDead || isRelive)
            {
                // var isRelive = strs[1]=="relive";
                var locoState = entityMgr.GetComponentData<LocomotionState>(entity);
                locoState.LocoState = isRelive?LocomotionState.State.Idle:LocomotionState.State.Dead;
                // Debug.Log("Time : "+TimeEx.ServerTime.ToString()+" isRelive:"+isRelive+" state:"+locoState.LocoState.ToString());
                // locoState.StartTime = Time.time - (TimeEx.ServerTime-change_info.time)/1000.0f;//CAT_TODO:dead time
                entityMgr.SetComponentData(entity, locoState);
                if (isDead && isMainRole)
                {
                    Debug.Log("isDead && isMainRole StopMainRoleRunning");
                    RoleMgr.GetInstance().StopMainRoleRunning();//停止攻击和寻址

                    var playableDir = entityMgr.GetComponentObject<PlayableDirector>(entity);
                    if (playableDir != null)
                    {
                        playableDir.Stop();
                    }

                    //等5秒钟倒下了再转lua出现复活对话框
                    Timer.Register(4.0f, () => {
                        XLuaFramework.CSLuaBridge.GetInstance().CallLuaFuncNum(GlobalEvents.MainRoleDie, attackerUID);//转到scene_mgr.lua
                    });
                    /*
                    var playableDir = entityMgr.GetComponentObject<PlayableDirector>(entity);
                    if (playableDir != null)
                    {
                        playableDir.Stop();
                    }
                    */
                    // var attackerName = SceneMgr.Instance.GetNameByUID(attackerUID);
                    

                   
                }
            }
            long uid =entityMgr.GetComponentData<UID>(entity).Value;
            if (hp == 0)
            {
                Timer.Register(3.0f, () => {
                    SceneMgr.Instance.RemoveSceneObject(uid);
                });
            }
        }

        public static void UpdateNameboardHeight(Entity entity, Transform looksTrans)
        {
            if (entityMgr.HasComponent<NameboardData>(entity))
            {
                var meshRender = looksTrans.GetComponentInChildren<SkinnedMeshRenderer>();
                // Debug.Log(string.Format("RoleLooksModule[260] (meshRender!=null):{0}", (meshRender!=null)));
                if (meshRender != null)
                {
                    var nameboardData = entityMgr.GetComponentObject<NameboardData>(entity);
                    nameboardData.Height = meshRender.bounds.size.y;
                    // Debug.Log(string.Format("RoleLooksModule[264] nameboardData.Height:{0}", nameboardData.Height));
                }
            }
        }

    }

}