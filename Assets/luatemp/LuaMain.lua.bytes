require "Game.Common.UIManager"
PrefabPool = require "Game.Common.PrefabPool"
require "Common.UI.UIComponent"
require "Common.UI.Countdown"
require("Common.UI.ItemListCreator")
require("Game.Common.Message")

local monster_cfg = require "config.config_monster"
local scene_const = require("Game.Scene.scene_const")
--管理器--
local Game = {}
local Ctrls = {}

function LuaMain()
    print("logic start 2023008030211111")     
    UpdateManager:GetInstance():Startup()
    Game:OnInitOK()
end

function ExitGame()
    print('Cat:LuaMain.lua[ExitGame]')
    local util = require 'Tools.print_delegate'
    util.print_func_ref_by_csharp()
end
function Game:InitMonster(  )
	local create_num = 0
	for i,v in ipairs(self.nest_cfg) do
		local mons_type_ok = true
		if mons_type_ok then
			local patrolInfo = {x=v.pos_x, y=v.pos_y, z=v.pos_z, radius=v.radius}
			for ii=1,v.monster_num do
				self:CreateMonster(v.monster_type_id, patrolInfo, v)
				create_num = create_num + 1
			end
		end
	end
end
function Game:CreateMonster( type_id, patrolInfo )
    local cfg = monster_cfg[type_id]
	if not cfg then return end
	
	local radius = patrolInfo.radius/2
	local pos_x = patrolInfo.x + math.random(-radius, radius)
	local pos_y = patrolInfo.y 
	local pos_z = patrolInfo.z + math.random(-radius, radius)
    local maxHp = cfg.attr_list[scene_const.Attr.HP] or 0
    local cur=maxHp 
    local max=maxHp
    local uid=self:NewSceneUID(scene_const.ObjectType.Monster)
    local mng=CS.UnityMMO.MonsterMgr.GetInstance()
    local pos=CS.UnityEngine.Vector3()
    pos.x=pos_x/100;
    pos.y=pos_y/100
    pos.z=pos_z/100
    print("AddSceneObjectsingle POS",pos.x,pos.y,pos.z)
    --{x=pos_x,y=pos_y,z=pos_z},{x=pos_x,y=pos_y,z=pos_z}
    SceneMgr.Instance:AddSceneObjectsingle(uid,type_id,pos,pos,cur,max)
    
end
function Game:NewSceneUID( scene_obj_type )
	self.scene_uid_counter[scene_obj_type] = self.scene_uid_counter[scene_obj_type] + 1
	return scene_obj_type*10000000000 + self.scene_uid_counter[scene_obj_type]
end
function Game:onsceneloaded()
    print("scene_mgr.onsceneloaded")
    self.scene_cfg = require("config.config_scene_"..1001)
    self.nest_cfg=self.scene_cfg.monster_list
    self.npcCfgList=self.scene_cfg.npc_list
    self.scene_uid_counter = {0, 0, 0};
    self:InitMonster()
    --self:InitNpc()
end
--初始化完成
function Game:OnInitOK()
    print('Cat:Game.lua[Game.OnInitOK()]')
    GlobalEventSystem.Init()
    Game:InitLuaPool()
    Game:InitUI()
    Game:InitControllers()
    GlobalEventSystem:Bind(GlobalEvents.onsceneloaded, Game.onsceneloaded,self)
end

function Game.InitUI()
    local msg_panel = GameObject.Find("UICanvas/Dynamic/MessagePanel")
    assert(msg_panel, "cannot fine message panel!")
    Message:Init(msg_panel.transform)

    UIMgr:Init({"UICanvas/Normal","UICanvas/MainUI", "UICanvas/Dynamic"}, "Normal")
    
    local pre_load_prefab = {
        "Assets/AssetBundleRes/ui/common/Background.prefab",
        "Assets/AssetBundleRes/ui/common/GoodsItem.prefab",
        "Assets/AssetBundleRes/ui/common/WindowBig.prefab",
        "Assets/AssetBundleRes/ui/common/WindowNoTab.prefab",
        "Assets/AssetBundleRes/ui/common/EmptyContainer.prefab",
        "Assets/AssetBundleRes/ui/common/Button1.prefab",
        "Assets/AssetBundleRes/ui/common/Button2.prefab",
        "Assets/AssetBundleRes/ui/common/Button3.prefab",
    }
    PrefabPool:Init("UICanvas/HideUI")
    PrefabPool:Register(pre_load_prefab)
end

function Game:InitControllers()
    local ctrl_paths = {
        "Game/Test/TestController",
        "Game/Login/LoginController", 
        "Game/MainUI/MainUIController", 
        "Game/Scene/main_world", 
        "Game/Scene/actor_mgr", 
        "Game/Scene/scene_mgr", --一定要让他先init，这样BagController中的ECS下的函数才能不是nil
        "Game/Task/TaskController", 
        "Game/Bag/BagController", 
        "Game/GM/GMController", 
        "Game/Chat/ChatController", 
    }
    local m="Game/Scene/scene_mgr"
    local ctrll = require(m)
    if type(ctrll) ~= "boolean" then
        --调用每个Controller的Init函数
        if ctrll.init then
            ctrll:init()
        elseif ctrll.Init then
            ctrll:Init()
        end
        table.insert(Ctrls, ctrll)
    else
        --Controller类忘记了在最后return
        assert(false, 'Cat:Main.lua error : you must forgot write a return in you controller file :'..v)
    end
    for i,v in ipairs(ctrl_paths) do
        if i~=6 then 
        local ctrl = require(v)
        if type(ctrl) ~= "boolean" then
            --调用每个Controller的Init函数
            if ctrl.init then
                ctrl:init()
            elseif ctrl.Init then
                ctrl:Init()
            end
            table.insert(Ctrls, ctrl)
        else
            --Controller类忘记了在最后return
            assert(false, 'Cat:Main.lua error : you must forgot write a return in you controller file :'..v)
        end
        end
    end
    --CS.XLuaManager.Instance:OnLoginOk()   --这句话要放在longincontroller 选角色之后
end

function Game:InitLuaPool(  )
    LuaPool = require "Game.Common.LuaPool"
    local info = {
        ["Window"] = {
            name="Window", maxNum = 5, prototype = require("Game.Common.UI.Window")
        },
        ["GoodsItem"] = {
            name="GoodsItem", maxNum = 50, prototype = require("Game.Common.UI.GoodsItem")
        },
        ["GoodsInfoView"] = {
            name="GoodsInfoView", maxNum = 50, prototype = require("Game.Common.UI.GoodsInfoView")
        },
        
        ["TabBar"] = {
            name="TabBar", maxNum = 5, createFunc = function()
                return require("Game.Common.UI.TabBar").New()
            end
        },
    }
    LuaPool:Init(info)
end

--销毁--
function Game:OnDestroy()

end
