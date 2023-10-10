using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;
using UnityMMO.Component;
using System.Security.Cryptography;
using System;
using UnityMMO;
using System;
public class monsterapi : MonoBehaviour
{
    public long uid;
    public long typeID;
    public float radius;

    public GameObject target=null;

    public Entity ent= Entity.Null;

    public float elasped = 0;
    public Vector3 origpos;

    float attackInterval = 0.8f;
    float lastAttackTime=0;

    public UnityMMO.MoveQuery movequery;



    public enum state
    {
        idle,
        patrol,
        fight,
    }
    public state curstate=state.idle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ent != Entity.Null)
        {
            var locoState = SceneMgr.Instance.EntityManager.GetComponentData<LocomotionState>(ent);
            if (locoState.LocoState != LocomotionState.State.Dead) this.think();
        }

    }
    public void think()
    {
        elasped += Time.deltaTime;
       // Debug.Log("curstate+elasped" + curstate+":"+elasped+";"+ Time.deltaTime);
        if (curstate== state.idle)
        {
            if (elasped > 3.0f)
            {
                patrolpath();//巡逻并且查找target
                curstate = state.patrol;
                elasped = 0;
            }
        }
        if (curstate == state.patrol)
        {
            if (elasped > 5.0f)
            {
                idlepath();//休息
                curstate = state.idle;
                elasped = 0;
            }

        }

        if (curstate == state.fight)
        {
            if (target == null)
            {
                idlepath();//休闲，
                curstate = state.idle;
                elasped = 0;
            }
            else
            {
                if (target != null)
                {
                    fight();//攻击，
                    //curstate = state.fight;
                    //elasped = 0;
                }
            }
        }
        Findtarget();
    }

    void idlepath()//休息
    {
        var newTargetPos = new TargetPosition();
        newTargetPos.Value = gameObject.transform.position;
        UnityMMO.SceneMgr.Instance.EntityManager.SetComponentData(ent, newTargetPos);
    }

    void fight()//攻击，
    {
        //Debug.Log("enter fight function");
        //if (movequery.IsAutoFinding || movequery.navAgent.pathPending || !movequery.navAgent.isStopped)
            //return;
        if (Time.time - lastAttackTime < attackInterval)
            return;
        if (target == null) return;
        var isExist = UnityMMO.SceneMgr.Instance.EntityManager.Exists(target.GetComponent<GameObjectEntity>().Entity);
        if (!isExist)
        {
            target = null;
            return;
        }
        var goe = target.GetComponent<GameObjectEntity>();
        var State = SceneMgr.Instance.EntityManager.GetComponentData<LocomotionState>(goe.Entity);
        if (State.LocoState == LocomotionState.State.Dead) return;//敌人已亡
        var monsTrans = target.transform;
        var dis = Vector3.Distance(gameObject.transform.position, monsTrans.position);
        //Debug.Log("fight dis=" + dis);
        if (dis <= 1.2)
        {
            var UIDData=UnityMMO.SceneMgr.Instance.EntityManager.GetComponentData<UnityMMO.UID>(ent);
            //Debug.Log("fight cast skill" + UIDData.Value);
            lastAttackTime = Time.time;
            //UnityMMO.SkillManager.GetInstance().CastSkillByIndex();
            CastSkillByIndex();
            //var isNormalAttack = UnityMMO.SkillManager.GetInstance().IsNormalAttack(skillID);
            //attackInterval = isNormalAttack ? 0.8f : 1.5f;
        }
        else
        {
            var findInfo = new UnityMMO.FindWayInfo
            {
                destination = monsTrans.position,
                stoppingDistance = 1.2f,
                onStop = null,
            };
            //Debug.Log("fight StartFindWay");
            gameObject.GetComponent<UnityMMO.MoveQuery>().StartFindWay(findInfo);
        }
    }
    public Vector3 getrandompos()
    {
        //Debug.Log("monster random po ");
        Vector3 curpos = origpos;

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
        rngServiceProvider.GetBytes(randomBytes);
        Int32 result = BitConverter.ToInt32(randomBytes, 0);
        int idx = Mathf.Abs(result) % 20;
        float randx = idx;

        byte[] randomBytes2 = new byte[4];
        rngServiceProvider.GetBytes(randomBytes2);
        result = BitConverter.ToInt32(randomBytes2, 0);
        idx = Mathf.Abs(result) % 20;
        float randy = idx;

        byte[] randomBytes3 = new byte[4];
        rngServiceProvider.GetBytes(randomBytes3);
        result = BitConverter.ToInt32(randomBytes3, 0);
        idx = Mathf.Abs(result) % 20;
        float randz = idx;

        //Debug.Log("monster random pos="+randx + ":" + randy + ":" + randz);

        Vector3 target = new Vector3(curpos.x + randx, curpos.y + randy, curpos.z + randz);
        NavMeshHit closestHit;
        // Debug.Log("transform.position : "+transform.position.x+" "+transform.position.y+" "+transform.position.z);
        if (NavMesh.SamplePosition(target, out closestHit, 1000f, NavMesh.AllAreas))
        {
            //Debug.Log("main role pos update nav agent in sample pos" + closestHit.position.x + ":" + closestHit.position.y + ":" + closestHit.position.z);
            //transform.position = closestHit.position;

            return new Vector3(closestHit.position.x, closestHit.position.y, closestHit.position.z);
        }
        return curpos;
    }
    public void Findtarget()//巡逻并且查找target
    {
        GameObjectEntity goe = UnityMMO.RoleMgr.GetInstance().GetMainRole();
        if (!goe) return;
        Vector3 pos = UnityMMO.RoleMgr.GetInstance().GetMainRole().transform.position;
        Vector3 curpos = gameObject.transform.position;
        var dis = Vector3.Distance(pos, curpos);
        //Debug.Log("fight Findtarget"+dis);
        if (dis < 10f)
        {
            //Debug.Log("fight Findtarget UID DIS" + uid + ":" + dis);
            target = UnityMMO.RoleMgr.GetInstance().GetMainRole().gameObject;
            curstate = state.fight;
            elasped = 0;
        }
        /*
        else
        {
            target = null;
            curstate = state.idle;
            elasped = 0;
        }
        */
    }
    public void patrolpath()//巡逻并且查找target
    {
        Vector3 targetpos = getrandompos();
        var newTargetPos = new TargetPosition();
        newTargetPos.Value = targetpos;
        //EntityManager.SetComponentData<TargetPosition>(ent, newTargetPos);
        UnityMMO.SceneMgr.Instance.EntityManager.SetComponentData(ent, newTargetPos);
    }
    public void init(long uuid, long typeid, float radiuss, Entity monster,Vector3 pos)
    {
        uid = uuid;
        typeID = typeid;
        radius = radiuss;
        ent = monster;
        origpos = pos;
        movequery = gameObject.GetComponent<UnityMMO.MoveQuery>();
        movequery.UpdateNavAgent();
    }

    public int CastSkillByIndex(int skillIndex = -1)
    {
        var skillID = GetSkillIDByIndex(skillIndex);
        CastSkill(skillID);
        return skillID;
    }
    public int GetSkillIDByIndex(int skillIndex)
    {
        if (skillIndex == -1)
            return GetCurAttackID();
        else
            return 0;
    }
    public int GetCurAttackID()
    {
        return GetAttackID(2,1);
    }
    private int GetAttackID(int career, int comboIndex)
    {
        //等于typeID00=typeID*100
        //技能id：十万位是类型1角色，2怪物，3NPC，万位为职业，个十百位随便用
        return (int)(typeID * 100);
    }
    public void CastSkill(int skillID)
    {
        // var skillID = GetSkillIDByIndex(skillIndex);

        
        string assetPath = UnityMMO.ResPath.GetMonsterSkillResPath(skillID);
        //bool isNormalAttack = IsNormalAttack(skillID);//普通攻击
        //Debug.Log("OnBehaviourPlayMMMMMMMM PATH : " + assetPath);
        //if (!isNormalAttack)
            //ResetCombo();//使用非普攻技能时就重置连击索引
        var uid = UnityMMO.SceneMgr.Instance.EntityManager.GetComponentData<UnityMMO.UID>(ent);
      
        Action<TimelineInfo.Event> afterAdd = null;

        var timelineInfo = new TimelineInfo { ResPath = assetPath, Owner = ent, StateChange = afterAdd };
        TimelineManager.GetInstance().AddTimeline(uid.Value, timelineInfo, SceneMgr.Instance.EntityManager);
    }
}
