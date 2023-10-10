using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using XLua;
using XLuaFramework;


[LuaCallCSharp]
[CSharpCallLua]
public interface queryTabinterface
{
     long roleID
    {
        get; set;
    }
     int taskID
    {
        get; set;
    }
     int status
    {
        get; set;
    }
     int subTaskIndex
    {
        get; set;
    }
     int subType
    {
        get; set;
    }
     int curProgress
    {
        get; set;
    }
     int maxProgress
    {
        get; set;
    }
     long contentID
    {
        get; set;
    }
}

[LuaCallCSharp]
[CSharpCallLua]
public class queryTab: queryTabinterface
{
    public long roleID
    {
        get; set;
    }
    public long _roleID;
    public int taskID
    {
        get;set;
    }
    public int status
    {
        get; set;
    }
    public int subTaskIndex
    {
        get; set;
    }
    public int subType
    {
        get; set;
    }
    public int curProgress
    {
        get; set;
    }
    public int maxProgress
    {
        get; set;
    }
    public long contentID
    {
        get; set;
    }
}





[LuaCallCSharp]
[CSharpCallLua]
public interface goldTabinterface
{
    long uid
    {
        get; set;
    }
    long typeID
    {
        get; set;
    }
    int num
    {
        get; set;
    }
    int pos
    {
        get; set;
    }
    long cell
    {
        get; set;
    }
    long roleID
    {
        get; set;
    }
}

[LuaCallCSharp]
[CSharpCallLua]
public class goldTab : goldTabinterface
{
    public long uid
    {
        get; set;
    }
    public long roleID
    {
        get; set;
    }
    public long typeID
    {
        get; set;
    }
    public int num
    {
        get; set;
    }
    public int pos
    {
        get; set;
    }
    public long cell
    {
        get; set;
    }
}

[Hotfix]
[LuaCallCSharp]
public class XMLdoc : MonoBehaviour
{
    public static XMLdoc Instance = null;
    public string filepath;
    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //filepath = AppConfig.DataPath + "student.xmlv";
        //Debug.Log("filepathfilepathfilepath=" + filepath);//D:/Users/UnityMMO-farmework/Assets/unitymmo/student.xmlv
        //filepath = @"D:/Users/UnityMMO-farmework/ student.xml";
        //filepath = "D:/Users/UnityMMO-farmework/Assets/unitymmo/student.xmlv";

        //Debug.Log("filepathfilepathfilepath=" + filepath);
        //this.createXML();
       // this.createXMLAccount();
        //Debug.Log("finish create xml");
    }

    // Update is called once per frame
    void Update()
    {

    }
           //创建XML
    public void createXML()
    {
        filepath = AppConfig.DataPath + "student.xmlv";
        //新建XmlDocument对象
        XmlDocument doc = new XmlDocument();
        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", ""));
        //载入文件
        if (!File.Exists(filepath))
        {
            XmlElement role_num = doc.CreateElement("role_num");//记录已经分配的roleid
            role_num.InnerText = "10000000000";
            //根节点
            XmlElement root = doc.CreateElement("Task");//总根节点，下辖Account根节点/Baseinfo跟节点/Taskinfo跟节点/bag根节点
                                                        //playerID节点
            XmlElement Taskinfo = doc.CreateElement("Taskinfo");//某一个任务，一个节点

            XmlElement roleID = doc.CreateElement("roleID");
            roleID.InnerText = "";
            //name
            XmlElement taskID = doc.CreateElement("taskID");
            taskID.InnerText = "";
            //age
            XmlElement status = doc.CreateElement("status");
            status.InnerText = "";
            //strenth
            XmlElement subTaskIndex = doc.CreateElement("subTaskIndex");
            subTaskIndex.InnerText = "";
            //speed
            XmlElement subType = doc.CreateElement("subType");
            subType.InnerText = "";

            XmlElement curProgress = doc.CreateElement("curProgress");
            curProgress.InnerText = "";

            XmlElement maxProgress = doc.CreateElement("maxProgress");
            maxProgress.InnerText = "";

            XmlElement contentID = doc.CreateElement("contentID");
            contentID.InnerText = "";

            Taskinfo.AppendChild(roleID);
            Taskinfo.AppendChild(taskID);
            Taskinfo.AppendChild(status);
            Taskinfo.AppendChild(subTaskIndex);
            Taskinfo.AppendChild(subType);
            Taskinfo.AppendChild(curProgress);
            Taskinfo.AppendChild(maxProgress);
            Taskinfo.AppendChild(contentID);

            Taskinfo.AppendChild(role_num);

            root.AppendChild(Taskinfo);
            



            XmlElement Account = doc.CreateElement("Account");//根节点account
            XmlElement Baseinfo = doc.CreateElement("Baseinfo");//根节点Baseinfo
            XmlElement Bag = doc.CreateElement("Bag");//根节点Bag,每个condition=rolid+bag值对应一个goodsList，每个goodsList列表的成员是一个
            /*
			newGoods = {
				uid = uid,
				typeID = goodsTypeID,
				num = addNum,
				pos = pos,
				cell = emptyCell,
				roleID = this.user_info.cur_role_id,
			}
            */

            XmlElement name = doc.CreateElement("name");//某一个玩家节点，下面可以有3个roleid
            name.InnerText = "";
            XmlElement role1 = doc.CreateElement("role1");
            role1.InnerText = "";
            XmlElement role2 = doc.CreateElement("role2");
            role2.InnerText = "";
            XmlElement role3 = doc.CreateElement("role3");
            role3.InnerText = "";

            name.AppendChild(role1);
            name.AppendChild(role2);
            name.AppendChild(role3);

            Account.AppendChild(name);

            root.AppendChild(Account);
            root.AppendChild(Baseinfo);


            
            XmlElement name2 = doc.CreateElement("name");
            name2.InnerText = "";
            Bag.AppendChild(name2);
            root.AppendChild(Bag);

            doc.AppendChild(root);
            doc.Save(filepath);
            Debug.Log("doc create success");
        }

    }
    //添加一个节点
    public void insert(queryTabinterface taskinf)
    {
        
        //新建XmlDocument对象
        XmlDocument doc = new XmlDocument();
        //载入文件
        try
        {
            doc.Load(filepath);
        }
        catch (System.Xml.XmlException)
        {
            throw new UnityException("load fail");
        }
        //查找要插入数据的节点
        XmlNode newxmlnode = doc.SelectSingleNode("Task");//根节点
        //创建新的节点
        XmlElement Taskinfo = doc.CreateElement("Taskinfo");//某一个任务

        XmlElement roleID = doc.CreateElement("roleID");
        roleID.InnerText = "" + taskinf.roleID;
        //name
        XmlElement taskID = doc.CreateElement("taskID");
        taskID.InnerText = "" + taskinf.taskID;
        //age
        XmlElement status = doc.CreateElement("status");
        status.InnerText = "" + taskinf.status;
        //strenth
        XmlElement subTaskIndex = doc.CreateElement("subTaskIndex");
        subTaskIndex.InnerText = "" + taskinf.subTaskIndex;
        //speed
        XmlElement subType = doc.CreateElement("subType");
        subType.InnerText = "" + taskinf.subType;

        XmlElement curProgress = doc.CreateElement("curProgress");
        curProgress.InnerText = "" + taskinf.curProgress;

        XmlElement maxProgress = doc.CreateElement("maxProgress");
        maxProgress.InnerText = "" + taskinf.maxProgress;

        XmlElement contentID = doc.CreateElement("contentID");
        contentID.InnerText = "" + taskinf.contentID;


        Taskinfo.AppendChild(roleID);
        Taskinfo.AppendChild(taskID);
        Taskinfo.AppendChild(status);
        Taskinfo.AppendChild(subTaskIndex);
        Taskinfo.AppendChild(subType);
        Taskinfo.AppendChild(curProgress);
        Taskinfo.AppendChild(maxProgress);
        Taskinfo.AppendChild(contentID);
        newxmlnode.AppendChild(Taskinfo);

        doc.Save(filepath);
        Debug.Log("xmldoc.insert call" + taskinf.taskID + taskinf.subTaskIndex + taskinf.roleID);
    }
    //更新一个节点
    public void UpdateXML(long id, queryTabinterface taskinf)
    {
        Debug.Log("Task_DoTask UpdateXML call");
        XmlDocument doc = new XmlDocument();
        //载入文件
        try
        {
            doc.Load(filepath);
        }
        catch (System.Xml.XmlException)
        {
            throw new UnityException("load fail");
        }
        XmlNode player = doc.SelectSingleNode("Task");//根节点
        XmlNodeList charactors = player.ChildNodes;
        foreach (XmlElement charactor in charactors)//每一个节点
        {
            if (charactor.Name.Equals("Taskinfo"))
            {
                XmlNode roleID = charactor.SelectSingleNode("roleID");
                Debug.Log("Task_DoTask UpdateXML call"+ roleID.InnerText + ":"+taskinf.roleID);
                if (roleID.InnerText == ""+taskinf.roleID)
                {

                    roleID.InnerText = "" + taskinf.roleID;
                    //name
                    XmlNode taskID = charactor.SelectSingleNode("taskID");
                    taskID.InnerText = "" + taskinf.taskID;
                    //age
                    XmlNode status = charactor.SelectSingleNode("status");
                    status.InnerText = "" + taskinf.status;
                    //strenth
                    XmlNode subTaskIndex = charactor.SelectSingleNode("subTaskIndex");
                    subTaskIndex.InnerText = "" + taskinf.subTaskIndex;
                    //speed
                    XmlNode subType = charactor.SelectSingleNode("subType");
                    subType.InnerText = "" + taskinf.subType;

                    XmlNode curProgress = charactor.SelectSingleNode("curProgress");
                    curProgress.InnerText = "" + taskinf.curProgress;

                    XmlNode maxProgress = charactor.SelectSingleNode("maxProgress");
                    maxProgress.InnerText = "" + taskinf.maxProgress;

                    XmlNode contentID = charactor.SelectSingleNode("contentID");
                    contentID.InnerText = "" + taskinf.contentID;


                    doc.Save(filepath);
                    Debug.Log("xmldoc.UpdateXML call" + taskinf.taskID + taskinf.subTaskIndex + taskinf.roleID);
                    return;
                }

            }
        }
        //写入文档
        doc.Save(filepath);
    }
    //读取一个节点
    public void ReadXML()
    {
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)
        {
            XmlElement roleid = personEle.ChildNodes[0] as XmlElement;
            XmlElement taskID = personEle.ChildNodes[1] as XmlElement;
            XmlElement status = personEle.ChildNodes[2] as XmlElement;
            XmlElement subTaskIndex = personEle.ChildNodes[3] as XmlElement;
            XmlElement subType = personEle.ChildNodes[4] as XmlElement;
            XmlElement curProgress = personEle.ChildNodes[5] as XmlElement;
            XmlElement maxProgress = personEle.ChildNodes[6] as XmlElement;
            XmlElement contentID = personEle.ChildNodes[7] as XmlElement;

        }
    }
    //删除一个节点
    public void DeleteXML()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filepath);
        XmlNode player = doc.SelectSingleNode("Task");
        XmlNodeList personsEle = player.ChildNodes;

        foreach (XmlElement personEle in personsEle)
        {
            if (personEle.Name.Equals("Taskinfo"))
            {
                player.RemoveChild(personEle);
            }
        }
        doc.Save(filepath);
    }
    public bool select_by_condition(string cond,LuaTable tmp, LuaTable tablist)
    {
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");//总根节点
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)//遍历每一个节点
        {
            if (personEle.Name.Equals("Bag"))
            {
                XmlNodeList Ele = personEle.ChildNodes;
                foreach (XmlElement e in Ele)//遍历每一个cond节点
                {
                    Debug.Log("select_by_condition" + e.Name+":"+cond);
                    if (e.Name == cond)   //每个cond下元素是一个newGoods = { uid = uid,typeID = goodsTypeID,num = addNum,pos = pos,cell = emptyCell,roleID = this.user_info.cur_role_id,}
                    {
                        //每一个element节点
                        XmlNodeList ee = e.ChildNodes;
                        int i = 0;
                        foreach (XmlElement eee in ee)//遍历每一个cond节点
                        {
                            
                            XmlElement uid = eee.ChildNodes[0] as XmlElement;
                            XmlElement typeID = eee.ChildNodes[1] as XmlElement;
                            XmlElement num = eee.ChildNodes[2] as XmlElement;
                            XmlElement pos = eee.ChildNodes[3] as XmlElement;
                            XmlElement cell = eee.ChildNodes[4] as XmlElement;
                            XmlElement roleID = eee.ChildNodes[5] as XmlElement;

                            Debug.Log("select_by_condition long.Parse(uid.InnerText)" + long.Parse(uid.InnerText));
                            Debug.Log("select_by_condition int.Parse(typeID.InnerText)" + long.Parse(typeID.InnerText));
                            Debug.Log("select_by_condition long.Parse(cell.InnerText)" + long.Parse(cell.InnerText));
                            Debug.Log("select_by_condition long.Parse(roleID.InnerText)" + long.Parse(roleID.InnerText));
                            
                            long ret = long.Parse(uid.InnerText);
                            tmp.Set("uid", ret);

                            long ret1 = long.Parse(typeID.InnerText);
                            tmp.Set("typeID", ret1);

                            tmp.Set("num", int.Parse(num.InnerText));
                            tmp.Set("pos", int.Parse(pos.InnerText));
                            tmp.Set("cell", long.Parse(cell.InnerText));
                            tmp.Set("roleID", long.Parse(roleID.InnerText));
                            //tablist.Set(i, tmp);
                            goldTab tb = new goldTab();
                            tb.num = int.Parse(num.InnerText);
                            tb.pos = int.Parse(pos.InnerText);
                            tb.roleID = long.Parse(roleID.InnerText);
                            tb.typeID= long.Parse(typeID.InnerText);
                            tb.uid= long.Parse(uid.InnerText);
                            tb.cell = long.Parse(cell.InnerText);
                            tablist.Set(i, tb);

                            i =i+1;
                        }
                        return true;
                    }

                }

            }
        }

        return false;
    }

    public bool delete(string cond,long typeiid)
    {
        Debug.Log("delete" + cond + typeiid);
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");//总根节点
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)//遍历每一个节点
        {
            if (personEle.Name.Equals("Bag"))
            {
                XmlNodeList Ele = personEle.ChildNodes;
                foreach (XmlElement e in Ele)//遍历每一个cond节点
                {
                    Debug.Log("select_by_condition" + e.Name + ":" + cond);
                    if (e.Name == cond)   //每个cond下元素是一个newGoods = { uid = uid,typeID = goodsTypeID,num = addNum,pos = pos,cell = emptyCell,roleID = this.user_info.cur_role_id,}
                    {
                        //每一个element节点
                        XmlNodeList ee = e.ChildNodes;
                        int i = 0;
                        foreach (XmlElement eee in ee)//遍历每一个cond节点
                        {

                            XmlElement typeid = eee.ChildNodes[1] as XmlElement;
                            if(typeiid == long.Parse(typeid.InnerText))//要删除eee节点
                            {
                                Debug.Log("delete finish" + cond + typeiid);
                                e.RemoveChild(eee);
                            }
                        }
                        doc.Save(filepath);
                        return true;
                    }

                }

            }
        }

        return false;

    }
    public bool insert(string cond, LuaTable good)
    {
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");//总根节点
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)//遍历每一个节点
        {
            if (personEle.Name.Equals("Bag"))
            {
                XmlNodeList Ele = personEle.ChildNodes;
                foreach (XmlElement e in Ele)//遍历每一个cond节点
                {
                    Debug.Log("select_by_condition" + e.Name + ":" + cond);
                    if (e.Name == cond)   //每个cond下元素是一个newGoods = { uid = uid,typeID = goodsTypeID,num = addNum,pos = pos,cell = emptyCell,roleID = this.user_info.cur_role_id,}
                    {
                        good.Get("uid", out long ret);
                        //创建一个element节点
                        /////一个element是一种物品
                        XmlElement element = doc.CreateElement("element");

                        XmlElement uid = doc.CreateElement("uid");
                        uid.InnerText = "" + ret;

                        good.Get("typeID", out long type_ID);
                        XmlElement typeID = doc.CreateElement("typeID");
                        typeID.InnerText = "" + type_ID;

                        good.Get("num", out int numm);
                        good.Get("pos", out int poss);
                        good.Get("cell", out long celll);
                        good.Get("roleID", out long role_id);

                        XmlElement num = doc.CreateElement("num");
                        num.InnerText = "" + numm;

                        XmlElement pos = doc.CreateElement("pos");
                        pos.InnerText = "" + poss;

                        XmlElement cell = doc.CreateElement("cell");
                        cell.InnerText = "" + celll;

                        XmlElement roleID = doc.CreateElement("roleID");
                        roleID.InnerText = "" + role_id;

                        element.AppendChild(uid);
                        element.AppendChild(typeID);
                        element.AppendChild(num);
                        element.AppendChild(pos);
                        element.AppendChild(cell);
                        element.AppendChild(roleID);

                        e.AppendChild(element);
                        doc.Save(filepath);
                        return true;
                    }

                }

            }
        }

        return false;

    }

    public bool update(string cond, LuaTable good)
    {
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");//总根节点
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)//遍历每一个节点
        {
            if (personEle.Name.Equals("Bag"))
            {
                XmlNodeList Ele = personEle.ChildNodes;
                foreach (XmlElement e in Ele)//遍历每一个cond节点
                {
                    Debug.Log("select_by_condition" + e.Name + ":" + cond);
                    if (e.Name == cond)   //每个cond下元素是一个newGoods = { uid = uid,typeID = goodsTypeID,num = addNum,pos = pos,cell = emptyCell,roleID = this.user_info.cur_role_id,}
                    {
                        //每一个element节点
                        XmlNodeList ee = e.ChildNodes;
                        good.Get("typeID", out long ret);
                        foreach (XmlElement eee in ee)//遍历每一个cond节点
                        {

                            XmlElement uuid = eee.ChildNodes[0] as XmlElement;
                            XmlElement typeID = eee.ChildNodes[1] as XmlElement;
                            XmlElement num = eee.ChildNodes[2] as XmlElement;
                            XmlElement pos = eee.ChildNodes[3] as XmlElement;
                            XmlElement cell = eee.ChildNodes[4] as XmlElement;
                            XmlElement roleID = eee.ChildNodes[5] as XmlElement;
                            if (ret == long.Parse(typeID.InnerText))//要更新的eee节点
                            {
                                good.Get("num", out int numm);
                                num.InnerText = "" + numm;
                                doc.Save(filepath);
                                return true;

                            }
                        }
                    }

                }

            }
        }

        return false;

    }


    public void createBaginfo(long rolid)
    {
        
        string cond = string.Format("roleID{0}andpos{1}", rolid, 1);
        Debug.Log("createBaginfo==" + cond);
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");//总根节点
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)//遍历每一个节点
        {
            if (personEle.Name.Equals("Bag"))
            {
                XmlElement conds = doc.CreateElement(cond);//

                /////一个element是一种物品
                XmlElement element = doc.CreateElement("element");

                XmlElement uid = doc.CreateElement("uid");
                uid.InnerText = "" + 1;

                XmlElement typeID = doc.CreateElement("typeID");
                typeID.InnerText = "" + 100000;

                XmlElement num = doc.CreateElement("num");
                num.InnerText = "" + 1;

                XmlElement pos = doc.CreateElement("pos");
                pos.InnerText = "" + 1;

                XmlElement cell = doc.CreateElement("cell");
                cell.InnerText = "" + 1;

                XmlElement roleID = doc.CreateElement("roleID");
                roleID.InnerText = "" + rolid;



                XmlElement element2 = doc.CreateElement("element1");
                XmlElement uid2 = doc.CreateElement("uid");
                uid2.InnerText = "" + 1;

                XmlElement typeID2 = doc.CreateElement("typeID");
                typeID2.InnerText = "" + 100001;

                XmlElement num2 = doc.CreateElement("num");
                num2.InnerText = "" + 1;

                XmlElement pos2 = doc.CreateElement("pos");
                pos2.InnerText = "" + 1;

                XmlElement cell2 = doc.CreateElement("cell");
                cell2.InnerText = "" + 2;

                XmlElement roleID2 = doc.CreateElement("roleID");
                roleID2.InnerText = "" + rolid;




                XmlElement element3 = doc.CreateElement("element2");
                XmlElement uid3 = doc.CreateElement("uid");
                uid3.InnerText = "" + 1;

                XmlElement typeID3 = doc.CreateElement("typeID");
                typeID3.InnerText = "" + 100002;

                XmlElement num3 = doc.CreateElement("num");
                num3.InnerText = "" + 1;

                XmlElement pos3 = doc.CreateElement("pos");
                pos3.InnerText = "" + 1;

                XmlElement cell3 = doc.CreateElement("cell");
                cell3.InnerText = "" + 3;

                XmlElement roleID3 = doc.CreateElement("roleID");
                roleID3.InnerText = "" + rolid;


                element.AppendChild(uid);
                element.AppendChild(typeID);
                element.AppendChild(num);
                element.AppendChild(pos);
                element.AppendChild(cell);
                element.AppendChild(roleID);


                element2.AppendChild(uid2);
                element2.AppendChild(typeID2);
                element2.AppendChild(num2);
                element2.AppendChild(pos2);
                element2.AppendChild(cell2);
                element2.AppendChild(roleID2);

                element3.AppendChild(uid3);
                element3.AppendChild(typeID3);
                element3.AppendChild(num3);
                element3.AppendChild(pos3);
                element3.AppendChild(cell3);
                element3.AppendChild(roleID3);
                //////

                conds.AppendChild(element3);
                conds.AppendChild(element2);
                conds.AppendChild(element);

                personEle.AppendChild(conds);
                doc.Save(filepath);

            }
        }
    }

    public LuaTable select_by_key(long role_id,LuaTable ttb)
    {
        
        List<queryTab> tasklist = new List<queryTab>();
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement personEle in personsEle)//遍历每一个节点
        {
            XmlElement roleid = personEle.ChildNodes[0] as XmlElement;
            Debug.Log("xmldoc.insert call select_by_key" + roleid.InnerText+"role_id="+ role_id);
            if (roleid.InnerText == "" + role_id)
            {
                Debug.Log("xmldoc.insert call ok");
                XmlElement taskID = personEle.ChildNodes[1] as XmlElement;
                XmlElement status = personEle.ChildNodes[2] as XmlElement;
                XmlElement subTaskIndex = personEle.ChildNodes[3] as XmlElement;
                XmlElement subType = personEle.ChildNodes[4] as XmlElement;
                XmlElement curProgress = personEle.ChildNodes[5] as XmlElement;
                XmlElement maxProgress = personEle.ChildNodes[6] as XmlElement;
                XmlElement contentID = personEle.ChildNodes[7] as XmlElement;
                queryTab qt = new queryTab();
                qt.roleID = long.Parse(roleid.InnerText);
                qt.taskID = int.Parse(taskID.InnerText);
                qt.status = int.Parse(status.InnerText);
                qt.subTaskIndex = int.Parse(subTaskIndex.InnerText);
                qt.subType = int.Parse(subType.InnerText);
                qt.curProgress = int.Parse(curProgress.InnerText);
                qt.maxProgress = int.Parse(maxProgress.InnerText);
                qt.contentID = long.Parse(contentID.InnerText);
                tasklist.Add(qt);

                ttb.Set("roleID", qt.roleID);
                ttb.Set("taskID", qt.taskID);
                ttb.Set("status", qt.status);
                ttb.Set("subTaskIndex", qt.subTaskIndex);
                ttb.Set("subType", qt.subType);
                ttb.Set("curProgress", qt.curProgress);
                ttb.Set("maxProgress", qt.maxProgress);
                ttb.Set("contentID", qt.contentID);
                return ttb;
                //return tasklist;
            }
        }
        return null;
    }
    public void Update(long id, queryTab taskinf)
    {
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        //获取根节点XmlElement表示节点（元素）
        XmlNode player = doc.SelectSingleNode("Task");
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement Ele in personsEle)//遍历每一个节点
        {
            XmlElement roleid = Ele.ChildNodes[0] as XmlElement;
            if (roleid.InnerText == "" + id)
            {
                XmlElement taskID = Ele.ChildNodes[1] as XmlElement;
                XmlElement status = Ele.ChildNodes[2] as XmlElement;
                XmlElement subTaskIndex = Ele.ChildNodes[3] as XmlElement;
                XmlElement subType = Ele.ChildNodes[4] as XmlElement;
                XmlElement curProgress = Ele.ChildNodes[5] as XmlElement;
                XmlElement maxProgress = Ele.ChildNodes[6] as XmlElement;
                XmlElement contentID = Ele.ChildNodes[7] as XmlElement;
                taskID.InnerText = ""+taskinf.taskID;
                status.InnerText = "" + taskinf.status;
                subTaskIndex.InnerText = "" + taskinf.subTaskIndex;
                subType.InnerText = "" + taskinf.subType;
                curProgress.InnerText = "" + taskinf.curProgress;
                maxProgress.InnerText = "" + taskinf.maxProgress;
                contentID.InnerText = "" + taskinf.contentID;

            }
        }
        doc.Save(filepath);

    }

    public bool account_select_role(long roleid, LuaTable res)
    {
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);

        /*
        1Taskinfo  -9role_num
        2Account
        3Baseinfo
        */
        XmlNode Taskroot = doc.SelectSingleNode("Task");//根节点


        XmlNode player = Taskroot.ChildNodes[2];   //XmlNode player = doc.SelectSingleNode("Baseinfo");
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement Ele in personsEle)//遍历每一个name节点
        {
            XmlElement role_id = Ele.ChildNodes[8] as XmlElement;
            Debug.Log("account_select_rolevvv " + role_id.InnerText);
            if (role_id.InnerText == ""+ roleid)//找到自己选择的角色
            {
                Debug.Log("account_select_role2 " + Ele.InnerText);
                XmlElement body = Ele.ChildNodes[0] as XmlElement;
                XmlElement hair = Ele.ChildNodes[1] as XmlElement;
                XmlElement career = Ele.ChildNodes[2] as XmlElement;

                XmlElement x = Ele.ChildNodes[3] as XmlElement;
                XmlElement y = Ele.ChildNodes[4] as XmlElement;
                XmlElement z = Ele.ChildNodes[5] as XmlElement;

                XmlElement curhp = Ele.ChildNodes[6] as XmlElement;
                XmlElement maxhp = Ele.ChildNodes[7] as XmlElement;

                XmlElement name = Ele.ChildNodes[9] as XmlElement;


                res.Set("body", long.Parse(body.InnerText));
                res.Set("hair", long.Parse(hair.InnerText));
                res.Set("career", long.Parse(career.InnerText));
                res.Set("role_id", roleid);

                res.Set("pos_x", long.Parse(x.InnerText));
                res.Set("pos_y", long.Parse(y.InnerText));
                res.Set("pos_z", long.Parse(z.InnerText));
                res.Set("cur_hp", long.Parse(curhp.InnerText));
                res.Set("max_hp", long.Parse(maxhp.InnerText));
                res.Set("name", ""+name.InnerText);
                Debug.Log("NAME=" + name.InnerText);

                return true;
            }

        }

                return false;
    }

    public bool account_create_role(string username,string playname,long career,LuaTable res)//提供角色名字和玩家职业类型
    {
        bool isFirst = true;
        XmlDocument doc = new XmlDocument();
        string xmlfile = filepath;
        doc.Load(xmlfile);
        /*
            1Taskinfo  -9role_num
            2Account
            3Baseinfo
        */
        XmlNode Taskroot = doc.SelectSingleNode("Task");//根节点
        XmlNode player = Taskroot.ChildNodes[1];   //XmlNode player = doc.SelectSingleNode("Account");


        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement Ele in personsEle)//遍历每一个玩家名字节点，注意当前默认只有balance名字的玩家
        {
            if (Ele.Name== username)//找到自己
            {
                isFirst = false;
                XmlElement role1 = Ele.ChildNodes[0] as XmlElement;
                XmlElement role2 = Ele.ChildNodes[1] as XmlElement;
                XmlElement role3 = Ele.ChildNodes[2] as XmlElement;  //最多3个角色


                XmlNode Taskinfo = Taskroot.ChildNodes[0];
                XmlNode role_num = Taskinfo.ChildNodes[8];
               // XmlElement role_num = doc.SelectSingleNode("role_num") as XmlElement;

                if (role1.InnerText == "")//在这里创建一个角色
                {
                    res.Set("roleID1", long.Parse(role_num.InnerText) + 1);
                    role_num.InnerText = "" + (long.Parse(role_num.InnerText) + 1);//记录宗数

                    role1.InnerText = "" + long.Parse(role_num.InnerText);
                    res.Set("role_id", long.Parse(role_num.InnerText));

                    doc.Save(filepath);
                    createXMLbaseinfo(long.Parse(role_num.InnerText), career, playname, ref res);
                    return true;
                }
                if (role2.InnerText == "")
                {
                    res.Set("roleID2", long.Parse(role_num.InnerText) + 1);
                    role_num.InnerText = "" + (long.Parse(role_num.InnerText) + 1);
                    role2.InnerText = "" + long.Parse(role_num.InnerText);
                    doc.Save(filepath);
                    res.Set("role_id", long.Parse(role_num.InnerText));
                    createXMLbaseinfo(long.Parse(role_num.InnerText) , career, playname, ref res);
                   
                    return true;
                }
                if (role3.InnerText == "")
                {
                    res.Set("roleID3", long.Parse(role_num.InnerText) + 1);
                    role_num.InnerText = "" + (long.Parse(role_num.InnerText) + 1);
                    role3.InnerText = "" + long.Parse(role_num.InnerText);
                    res.Set("role_id", long.Parse(role_num.InnerText));
                    doc.Save(filepath);
                    createXMLbaseinfo(long.Parse(role_num.InnerText), career, playname, ref res);
                    
                    return true;
                } 
            }
        }
        if (isFirst)//还是第一次来则创建
        {
            //strenth
            XmlElement person = doc.CreateElement(username);//balance这个名字的玩家
            XmlNode Taskinfo = Taskroot.ChildNodes[0];
            XmlNode role_num = Taskinfo.ChildNodes[8];
            role_num.InnerText = "" + (long.Parse(role_num.InnerText) + 1);


            XmlElement role1 = doc.CreateElement("role1");
            role1.InnerText = role_num.InnerText;

            XmlElement role2 = doc.CreateElement("role2");
            role2.InnerText = "";
            XmlElement role3 = doc.CreateElement("role3");
            role3.InnerText = "";

            person.AppendChild(role1);
            person.AppendChild(role2);
            person.AppendChild(role3);

            player.AppendChild(person);
            doc.Save(filepath);
            res.Set("role_id", long.Parse(role_num.InnerText));
            createXMLbaseinfo(long.Parse(role_num.InnerText), career, playname, ref res);
            return true;


        }

            return false;
    }
    public bool account_get_role_list(string playname, LuaTable res)//提供玩家名字
    {
        bool first = true;
        //新建XmlDocument对象
        XmlDocument doc = new XmlDocument();
        //载入文件
        try
        {
            Debug.Log("Application.dataPath----" + filepath);
            doc.Load(filepath);
        }
        catch (System.Xml.XmlException)
        {
            throw new UnityException("load fail");
        }
        /*
        1Taskinfo  -9role_num
        2Account
        3Baseinfo
         */
        XmlNode Taskroot = doc.SelectSingleNode("Task");//根节点
        XmlNode player = Taskroot.ChildNodes[1];
        XmlNode Baseinfo = Taskroot.ChildNodes[2]; //doc.SelectSingleNode("Baseinfo");
        //获取根节点XmlElement表示节点（元素）
        //XmlNode player = doc.SelectSingleNode("Account");
        XmlNodeList personsEle = player.ChildNodes;
        foreach (XmlElement Ele in personsEle)//遍历每一个name节点
        {
            Debug.Log("balance=" + Ele.Name);
            if (Ele.Name == playname)
            {
                first = false;
                XmlElement role1 = Ele.ChildNodes[0] as XmlElement;
                XmlElement role2 = Ele.ChildNodes[1] as XmlElement;
                XmlElement role3 = Ele.ChildNodes[2] as XmlElement;
                if (role1.InnerText != "")
                {
                    res.Set("roleID1", long.Parse(role1.InnerText));
                    
                    XmlNodeList Eles = Baseinfo.ChildNodes;
                    foreach (XmlElement ele in Eles)//遍历每一个name节点,name节点下辖10个节点
                    {
                        XmlElement roleid = ele.ChildNodes[8] as XmlElement;
                        if (roleid.InnerText == role1.InnerText)
                        {
                            XmlElement body = ele.ChildNodes[0] as XmlElement;
                            XmlElement hair = ele.ChildNodes[1] as XmlElement;
                            XmlElement career = ele.ChildNodes[2] as XmlElement;
                            res.Set("body1", long.Parse(body.InnerText));
                            res.Set("hair1", long.Parse(hair.InnerText));
                            res.Set("career1", long.Parse(career.InnerText));
                        }
                    }
                   
                 }
                if (role2.InnerText != "")
                {
                    res.Set("roleID2", long.Parse(role2.InnerText));
                    XmlNodeList Eles = Baseinfo.ChildNodes;
                    foreach (XmlElement ele in Eles)//遍历每一个role_id节点
                    {
                        XmlElement roleid = ele.ChildNodes[8] as XmlElement;
                        if (roleid.InnerText == role2.InnerText)
                        {
                            XmlElement body = ele.ChildNodes[0] as XmlElement;
                            XmlElement hair = ele.ChildNodes[1] as XmlElement;
                            XmlElement career = ele.ChildNodes[2] as XmlElement;
                            res.Set("body2", long.Parse(body.InnerText));
                            res.Set("hair2", long.Parse(hair.InnerText));
                            res.Set("career2", long.Parse(career.InnerText));
                        }
                    }
                }
                if (role3.InnerText != "")
                {
                    res.Set("roleID3", long.Parse(role3.InnerText));
                    XmlNodeList Eles = Baseinfo.ChildNodes;
                    foreach (XmlElement ele in Eles)//遍历每一个role_id节点
                    {
                        XmlElement roleid = ele.ChildNodes[8] as XmlElement;
                        if (roleid.InnerText == role3.InnerText)
                        {
                            XmlElement body = ele.ChildNodes[0] as XmlElement;
                            XmlElement hair = ele.ChildNodes[1] as XmlElement;
                            XmlElement career = ele.ChildNodes[2] as XmlElement;
                            res.Set("body3", long.Parse(body.InnerText));
                            res.Set("hair3", long.Parse(hair.InnerText));
                            res.Set("career3", long.Parse(career.InnerText));
                        }
                    }
                }
                return true;
            }

        }
        if (first == true)
        {
            //strenth
            XmlElement person = doc.CreateElement(playname);//balance这个名字的玩家
            XmlNode Taskinfo = Taskroot.ChildNodes[0];
            //XmlNode role_num = Taskinfo.ChildNodes[8];
            //role_num.InnerText = "" + (long.Parse(role_num.InnerText) + 1);


            XmlElement role1 = doc.CreateElement("role1");
            role1.InnerText = "";

            XmlElement role2 = doc.CreateElement("role2");
            role2.InnerText = "";
            XmlElement role3 = doc.CreateElement("role3");
            role3.InnerText = "";

            person.AppendChild(role1);
            person.AppendChild(role2);
            person.AppendChild(role3);

            player.AppendChild(person);
            doc.Save(filepath);
            //res.Set("role_id", long.Parse(role_num.InnerText));
            //createXMLbaseinfo(long.Parse(role_num.InnerText), career, playname, ref res);

        }
        res = null;
        return false;
    }
    //创建Account
    public void createXMLAccount()
    {
        //新建XmlDocument对象
        XmlDocument doc = new XmlDocument();
        //载入文件
        try
        {
            doc.Load(filepath);
        }
        catch (System.Xml.XmlException)
        {
            throw new UnityException("load fail");
        }
        //查找要插入数据的节点
        XmlNode node = doc.SelectSingleNode("Task");//根节点

        XmlElement root = doc.CreateElement("Account");//根节点account,旗下每一个玩家一个person节点下辖3个roleid
        XmlElement root2 = doc.CreateElement("Baseinfo");//根节点account


        node.AppendChild(root);
        node.AppendChild(root2);
        doc.Save(filepath);

    }

    //创建baseinfo表
    public void createXMLbaseinfo(long roleid,long career, string playname, ref LuaTable res)
    {
        //新建XmlDocument对象
        XmlDocument doc = new XmlDocument();
        //载入文件
        try
        {
            doc.Load(filepath);
        }
        catch (System.Xml.XmlException)
        {
            throw new UnityException("load fail");
        }
       

        /*
        1Taskinfo  -9role_num
        2Account
        3Baseinfo
        */
        XmlNode Taskroot = doc.SelectSingleNode("Task");//根节点
        XmlNode root = Taskroot.ChildNodes[2]; //XmlNode root = doc.SelectSingleNode("Baseinfo");//根节点Baseinfo

        XmlElement name = doc.CreateElement("name");//某一个玩家节点，下面可以有3个roleid

        XmlElement role_id = doc.CreateElement("role_id");//某一个玩家节点，下面可以有3个roleid
        role_id.InnerText = ""+ roleid;

        XmlElement body = doc.CreateElement("body");
        body.InnerText = ""+1;
        res.Set("body", long.Parse(body.InnerText));
        XmlElement hair = doc.CreateElement("hair");
        hair.InnerText = ""+1;
        res.Set("hair", long.Parse(hair.InnerText));
        XmlElement typeid = doc.CreateElement("career");
        typeid.InnerText = ""+ career;
        res.Set("career", long.Parse(typeid.InnerText));

        //pos_x/pos_y/pos_z/name/cur_hp/max_hp

        XmlElement posx = doc.CreateElement("posx");
        posx.InnerText = "" + 0;
        res.Set("pos_x", long.Parse(posx.InnerText));
        XmlElement posy = doc.CreateElement("posy");
        posy.InnerText = "" + 0;
        res.Set("pos_y", long.Parse(posy.InnerText));
        XmlElement posz = doc.CreateElement("posz");
        posz.InnerText = "" + 0;
        res.Set("pos_z", long.Parse(posz.InnerText));
        XmlElement curhp = doc.CreateElement("curhp");
        curhp.InnerText = "" + 100;
        res.Set("cur_hp", long.Parse(curhp.InnerText));
        XmlElement maxhp = doc.CreateElement("maxhp");
        maxhp.InnerText = "" + 100;
        res.Set("max_hp", long.Parse(maxhp.InnerText));

        XmlElement names = doc.CreateElement("name");
        names.InnerText = playname;
        res.Set("name", playname);

        name.AppendChild(body);
        name.AppendChild(hair);
        name.AppendChild(typeid);
        name.AppendChild(posx);
        name.AppendChild(posy);
        name.AppendChild(posz);
        name.AppendChild(curhp);
        name.AppendChild(maxhp);
        name.AppendChild(role_id);
        name.AppendChild(names);

        root.AppendChild(name);
        //doc.AppendChild(root);
        doc.Save(filepath);

        createBaginfo(roleid);

    }
}
