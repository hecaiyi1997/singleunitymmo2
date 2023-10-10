local scene_const = require("Game.Scene.scene_const")
local monster_cfg = require "config.config_monster"
local npc_cfg = require "config.config_npc"
local mt = {}

	

function mt:init()
	print('Cat:SceneController.lua[Init]')
    self.scene_cfg = require("config.config_scene_"..1001)
    self.nest_cfg=self.scene_cfg.monster_list
    self.npcCfgList=self.scene_cfg.npc_list
    self.scene_uid_counter = {0, 0, 0};
    self.sceneNode = GameObject.Find("UICanvas/Scene")
    self.sceneNode:SetActive(false)
	self.mainCamera = Camera.main
	self:init_events()
    
end
function mt:addmonsterTerrain()
    local posarry={}
    for i,v in ipairs(self.nest_cfg) do
		local patrolInfo = {x=v.pos_x, y=v.pos_y, z=v.pos_z, radius=v.radius}
			--self:CreateMonster(v.monster_type_id, patrolInfo, v)
            local type_id=v.monster_type_id
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
            table.insert(posarry,pos)
            SceneMgr.Instance:RecordSceneObjectsingleTerrain(uid,type_id,pos,pos,cur,max)
    end

	for i,v in ipairs(self.nest_cfg) do
		local patrolInfo = {x=v.pos_x, y=v.pos_y, z=v.pos_z, radius=v.radius}
			--self:CreateMonster(v.monster_type_id, patrolInfo, v)
            local type_id=v.monster_type_id
            local cfg = monster_cfg[type_id]
            if not cfg then return end
            
            local radius = patrolInfo.radius/2
            local pos_x = patrolInfo.x + math.random(-radius, radius)
            local pos_y = patrolInfo.y + math.random(-radius, radius)
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
            --print("AddSceneObjectsingle",pos.x,pos.y,pos.z)
            --{x=pos_x,y=pos_y,z=pos_z},{x=pos_x,y=pos_y,z=pos_z}
            SceneMgr.Instance:AddSceneObjectsingleTerrain(uid,type_id,posarry[i],posarry[i],cur,max)
            
	end
end
function mt:InitMonster(  )
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
function mt:InitNpc(  )
	for i,v in ipairs(self.npcCfgList) do
		self:CreateNPC(v.npc_id, v.pos_x, v.pos_y, v.pos_z)
	end
end
function mt:CreateNPC( type_id, pos_x, pos_y, pos_z )
    print("CreateNPC",type_id)
	local cfg = npc_cfg[type_id]
	if not cfg then return end

    local uid=self:NewSceneUID(scene_const.ObjectType.NPC)
    local pos=CS.UnityEngine.Vector3()
    pos.x=pos_x/100;
    pos.y=pos_y/100
    pos.z=pos_z/100
    print("AddSceneObjectsingle CreateNPC",uid,pos.x,pos.y,pos.z)
    --{x=pos_x,y=pos_y,z=pos_z},{x=pos_x,y=pos_y,z=pos_z}
    SceneMgr.Instance:AddSceneObjectsingle(uid,type_id,pos,pos,0,0)

end

function mt:CreateMonster( type_id, patrolInfo )
    local cfg = monster_cfg[type_id]
	if not cfg then return end
	
	local radius = patrolInfo.radius/2
	local pos_x = patrolInfo.x + math.random(-radius, radius)
	local pos_y = patrolInfo.y + math.random(-radius, radius)
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
    --print("AddSceneObjectsingle",pos.x,pos.y,pos.z)
    --{x=pos_x,y=pos_y,z=pos_z},{x=pos_x,y=pos_y,z=pos_z}
    SceneMgr.Instance:AddSceneObjectsingle(uid,type_id,pos,pos,cur,max)
    
end
function mt:NewSceneUID( scene_obj_type )
	self.scene_uid_counter[scene_obj_type] = self.scene_uid_counter[scene_obj_type] + 1
	return scene_obj_type*10000000000 + self.scene_uid_counter[scene_obj_type]
end
function mt:onsceneloaded()
    print("scene_mgr.onsceneloaded")
    self:InitMonster()
    --self:InitNpc()
end

function mt:init_events()
    print("scene_mgr.init_events......")
    --GlobalEventSystem:Bind(NetDispatcher.Event.OnReceiveLine, mt.onsceneloaded,self)
	local on_start_game = function (  )
        print("scene_mgr.on_start_game",CS.UnityEngine.Application.platform==CS.UnityEngine.RuntimePlatform.WindowsEditor)
        self.sceneNode:SetActive(true)
		ECS:Init(SceneMgr.Instance.EntityManager)
        if CS.UnityEngine.RuntimePlatform.WindowsEditor==CS.UnityEngine.Application.platform then--安卓的不显示
            local view = require("Game/Task/openwordsView").New()
            view:SetData({type_id=2005,u_id=uid})
        end
        self:InitNpc()
        self:addmonsterTerrain()
        CS.XLuaManager.Instance:OnchunkloadedOk()
	end
    GlobalEventSystem:Bind(GlobalEvents.GameStart, on_start_game)

	local on_up = function ( target, x, y )
    	print('Cat:SceneController.lua[up] x, y', target, x, y, self.is_dragged)
        if self.is_dragged then
            self.is_dragged = false
            return
        end
        local hit = SceneHelper.GetClickSceneObject()
        print('Cat:SceneController.lua[34] hit.hit', hit.hit)
        if hit.hit then
            print('Cat:SceneController.lua[35] hit.entity, hit.point.x, hit.point.y', hit.entity, hit.point.x, hit.point.y, hit.point.z,hit.typeID)
            -- print('Cat:SceneController.lua[30] ECS:HasComponent(hit.entity, CS.UnityMMO.Component.SceneObjectTypeData)', ECS:HasComponent(hit.entity, CS.UnityMMO.Component.SceneObjectTypeData))
            if hit.typeID==100000 then
                print("lua get click scene object"..hit.typeID)
                BagController:GetInstance():addGood(hit.typeID,1)
            elseif hit.typeID==100001 then
                print("lua get click scene object"..hit.typeID)
                BagController:GetInstance():addGood(hit.typeID,1)

            elseif hit.typeID==100002 then 
                print("lua get click scene object"..hit.typeID)
                BagController:GetInstance():addGood(hit.typeID,1)

            elseif hit.entity == Entity.Null then
                local goe = RoleMgr.GetInstance():GetMainRole()
                local moveQuery = goe:GetComponent(typeof(CS.UnityMMO.MoveQuery))
                local findInfo = {
                    destination = hit.point,
                    stoppingDistance = 0,
                }
                moveQuery:StartFindWay(findInfo)
                local fightAi = goe:GetComponent(typeof(CS.UnityMMO.AutoFight))
                if fightAi then
                    fightAi.enabled = false
                end
            elseif ECS:HasComponent(hit.entity, CS.UnityMMO.Component.SceneObjectTypeData) then
                local sceneObjType = ECS:GetComponentData(hit.entity, CS.UnityMMO.Component.SceneObjectTypeData)
                print('Cat:SceneController.lua[41] sceneObjType', sceneObjType.Value)
                if sceneObjType.Value == CS.UnityMMO.SceneObjectType.NPC then
                    local typeID = ECS:GetComponentData(hit.entity, CS.UnityMMO.Component.TypeID)
                    print('Cat:SceneController.lua[43] typeID.Value', typeID.Value, SceneMgr.Instance.CurSceneID)
                    TaskController:GetInstance():DoTalk({sceneID=SceneMgr.Instance.CurSceneID, npcID=typeID.Value})
                end
            end
        end
    end
	UI.BindClickEvent(self.sceneNode, on_up)
 
    local on_drag_begin = function ( obj, x, y )
        print('Cat:SceneController.lua[85]on_drag_begin obj, x, y', obj, x, y)
        self.is_dragged = true
        if not self.lastMousePos then
            self.lastMousePos = {x=x, y=y}
            self.freeLookCamera = SceneMgr.Instance.FreeLookCamera
            self.cameraRotateSpeed = {x=1280/3, y=720/250}
            self.screenSize = {x=CS.UnityEngine.Screen.width, y=CS.UnityEngine.Screen.height}
            -- self.cameraCtrl = CS.UnityMMO.CameraCtrl.Instance
            self.cameraCtrl = self.freeLookCamera:GetComponent("CameraCtrl")
            -- print('Cat:SceneController.lua[65] self.cameraCtrl', self.cameraCtrl, self.cameraCtrl)
        end
        self.lastMousePos.x = x
        self.lastMousePos.y = y
    end
    UI.BindDragBeginEvent(self.sceneNode, on_drag_begin)

    local on_drag = function ( obj, x, y )
        -- print('Cat:SceneController.lua[68] obj, x, y', obj, x, y)
        self.cameraCtrl:ApplyMove((x-self.lastMousePos.x)/self.screenSize.x*self.cameraRotateSpeed.x, 
            (y-self.lastMousePos.y)/self.screenSize.y*self.cameraRotateSpeed.y)
        self.lastMousePos.x = x
        self.lastMousePos.y = y
    end
    UI.BindDragEvent(self.sceneNode, on_drag)

    local MainRoleDie = function ( killerUID )
        print('Cat:SceneController.lua[58] self.reliveView', self.reliveView, killerUID)
        if not self.reliveView then
            self.reliveView = require("Game.Scene.ReliveView").New()
            self.reliveView:Load()
            self.reliveView:SetUnloadCallBack(function()
                self.reliveView = nil
            end)
        end
        self.reliveView:SetData(killerUID)
    end
    print('Cat:SceneController.lua[66] GlobalEvents.MainRoleDie', GlobalEvents.MainRoleDie, CSLuaBridge.GetInstance())
    print('Cat:SceneController.lua[67] CSLuaBridge.GetInstance:SetLuaFuncNum', CSLuaBridge.GetInstance().SetLuaFuncNum)
    CSLuaBridge.GetInstance():SetLuaFuncNum(GlobalEvents.MainRoleDie, MainRoleDie)
end

function mt:on_actor_enter()
    global.actor_mgr:add_actor(info)
end

return mt