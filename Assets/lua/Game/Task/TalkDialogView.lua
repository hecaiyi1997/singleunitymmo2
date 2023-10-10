
local TalkDialogView = BaseClass(UINode)

function TalkDialogView:Constructor(  )
	self.viewCfg = {
		prefabPath = "Assets/AssetBundleRes/ui/task/TaskDialogView_1.prefab",
		canvasName = "Normal",
	}
	self.model = TaskModel:GetInstance()
	self.PlayAudioCompleteflg=0
	self.PlayAudioStartflg=0
	self.timer=nil
	self:Load()
end

function TalkDialogView:OnLoad(  )
	print("TalkDialogView:OnLoad");
	local names = {
		"bottom/npc:raw","bottom/chat:txt",
	}
	UI.GetChildren(self, self.transform, names)
	self.transform.sizeDelta = Vector2.zero

	self:AddEvents()
	
end

function TalkDialogView:HandleBtnClick(  )
	if self.curShowData.clickCallBack then
		self.curShowData.clickCallBack()
	else
		CS.XLuaManager.Instance:FinishTalkExp()
		self:Unload()
	end
end
function TalkDialogView:PlayAudioComplete()
	print("TalkDialogView:PlayAudioComplete")
	self.PlayAudioCompleteflg=1
    
end
function TalkDialogView:PlayAudioStart()
	print("TalkDialogView:PlayAudioStart")
	self.PlayAudioStartflg=1
    
end
function TalkDialogView:AddEvents(  )--如果是安卓系统不注册这些事件
	print("TalkDialogView:AddEvents=")
	if CS.UnityEngine.Application.platform==CS.UnityEngine.RuntimePlatform.WindowsEditor then
		print("alkDialogView:AddEvents=",CS.UnityEngine.RuntimePlatform.WindowsEditor)
		GlobalEventSystem:Bind(NetDispatcher.Event.PlayAudioComplete, TalkDialogView.PlayAudioComplete, self)
    	GlobalEventSystem:Bind(NetDispatcher.Event.PlayAudioStart, TalkDialogView.PlayAudioStart, self)
	end
	
end

function TalkDialogView:ShowNextTalk(  )
	--CS.SpeechVoice.Instance:StopSpeechVoice()
    self.curShowTalkNum = self.curShowTalkNum + 1
	self:OnUpdate()
end

function TalkDialogView:ReqTakeTask(  )

end

function TalkDialogView:ReqDoTask(  )
	
end

function TalkDialogView:ClickOk(  )
	print("TalkDialogView:ClickOk",self.curShowData.type_id)
	local ackTakeTask = function ( ackData )
		if ackData.result == NoError then
			CS.XLuaManager.Instance:FinishTalkExp()
        	self:Unload()
        else
        	Message:Show(ConfigMgr:GetErrorDesc(ackData.result))
        end
    end
    NetDispatcher:SendMessage("Task_FinishTalk", {typeID=self.curShowData.type_id}, ackTakeTask)
end

function TalkDialogView:ProcessBtnNameAndCallBack( flag )
	
	if not self.flagMap then
		self.flagMap = {
			[TaskConst.DialogBtnFlag.Continue] = {name="继续", func=TalkDialogView.ShowNextTalk},
			[TaskConst.DialogBtnFlag.TakeTask] = {name="接取", func=TalkDialogView.ReqTakeTask},
			[TaskConst.DialogBtnFlag.DoTask]   = {name="完成", func=TalkDialogView.ReqDoTask}, 
			[TaskConst.DialogBtnFlag.Ok] 	   = {name="确定", func=TalkDialogView.ClickOk},
		}
		-- 3 DoTask表示该子任务系列已经做完了；2 TakeTask表示继续开始做下一子任务吗
	end
	local flagInfo = self.flagMap[flag]
	if not flagInfo then return end
	
	self.curShowData.btnName = flagInfo.name
    self.curShowData.clickCallBack = function()
    	flagInfo.func(self)
	end
	self.countdown = self.countdown or self:AddUIComponent(UI.Countdown)
  	self.countdown:CountdownByLeftTime(30, function(leftTime)
  		if leftTime > 0 and self.PlayAudioCompleteflg==0 then
			--print("lefttime curShowTalkNum=",string.format("%s秒后自动", math.floor((leftTime-22)/1)),math.floor((leftTime-22)/1),leftTime-22)
  			if leftTime-22<0 then leftTime=22 end
			--self.left_time_txt.text = string.format("%s秒后自动", math.floor((leftTime-22)/1))
		elseif self.PlayAudioCompleteflg==1 then
			self.PlayAudioCompleteflg=0
			--print("TalkDialogView:ProcessBtnNameAndCallBack curShowTalkNum")
			self:HandleBtnClick()
		end
	end, 0.2)
end

function TalkDialogView:ProcessTaskInfo(  )
	if not self.data then return end
	self.curShowData = table.deep_copy(self.data)
	local cfg=require("Config/ConfigTalk")
	local subTaskCfg=cfg[self.data.type_id]
    if not subTaskCfg or not subTaskCfg.content then
        	self.curShowData = nil
    end
	if self.curShowData then
    	self.curShowTalkNum = self.curShowTalkNum or 1
		--print("self.curShowTalkNum =",self.curShowTalkNum)
    	local talkCfg = subTaskCfg.content[self.curShowTalkNum]
        	if talkCfg then
	       	 	self.curShowData.content = talkCfg.chat
	        	self.curShowData.who = talkCfg.who
				--print("CS.SpeechVoice.Instance:StopVoice")
				--CS.SpeechVoice.Instance:StopSpeechVoice();
				CS.SpeechVoice.Instance:Speech_Voice(self.curShowData.content)
				--print("CS.SpeechVoice.Instance:Speech_Voic")
				self:ProcessBtnNameAndCallBack(talkCfg.flag)
	    	end
		--Cat_Todo : multi task in npc
    else
        --show default conversation
        self.curShowData = {}
        self.curShowData.content = "哈哈,你猜我是谁?"
        self.curShowData.who = self.data.type_id
        self:ProcessBtnNameAndCallBack(TaskConst.DialogBtnFlag.Ok)
		--CS.SpeechVoice.Instance:StopVoice();
		CS.SpeechVoice.Instance:Speech_Voice(self.curShowData.content)
    end
end

function TalkDialogView:OnUpdate(  )
	local typeid=self.data.type_id;
	local goe = RoleMgr.GetInstance():GetMainRole()
	local mainrolepos = goe:GetComponent(typeof(CS.UnityEngine.Transform)).localPosition
	--print("AddMonster TalkDialogView:OnUpdate self.data.u_id=",self.data.u_id)
	local mon=SceneMgr.Instance:GetSceneObject(self.data.u_id);
	if mon~=Entity.Null then
		--local TargetPosition=ECS:GetComponentData(mon, CS.UnityMMO.Component.TargetPosition).Value
		local TargetPosition=ECS:GetComponentObject(mon, typeof(CS.UnityEngine.Transform)).localPosition
		local a=Vector3.New(mainrolepos.x,mainrolepos.y,mainrolepos.z)
		local b=Vector3.New(TargetPosition.x,TargetPosition.y,TargetPosition.z)
		local pos=ECS:GetComponentData(goe.Entity,CS.UnityMMO.Component.TargetPosition)

		--print("origin pos monster",b.x,b.y,b.z)
		--print("origin pos",pos.Value.x,pos.Value.y,pos.Value.z)
		--ECS:SetComponentData(goe.Entity,CS.UnityMMO.Component.TargetPosition, {Value ={x=b.x,y=b.y,z=b.z}})
		--pos=ECS:GetComponentData(goe.Entity,CS.UnityMMO.Component.TargetPosition)
		--print("origin pos dest pos",pos.Value.x,pos.Value.y,pos.Value.z)


		--[[
		local curTrans=goe:GetComponent(typeof(CS.UnityEngine.Transform))
		local dir=Vector3(TargetPosition.x-mainrolepos.x,0,TargetPosition.z-mainrolepos.z)
		local lookDirection = Vector3.Normalize(dir)
		local  freeRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
		local diferenceRotation = freeRotation.eulerAngles.y - curTrans.eulerAngles.y;
        local eulerY = curTrans.eulerAngles.y;
		
		if diferenceRotation < 0 or diferenceRotation > 0 then
			 eulerY = freeRotation.eulerAngles.y;
		end
		local euler = Vector3(0, eulerY, 0);
		curTrans.rotation =Quaternion.Slerp(curTrans.rotation, Quaternion.Euler(euler.x,euler.y,euler.z), Time.deltaTime*50);
		print("eulerY=",eulerY,curTrans.rotation)

		--]]
		print("Vector3.Distance(a,b)",Vector3.Distance(a,b));
		if Vector3.Distance(a,b)>10 then
			CS.SpeechVoice.Instance:StopSpeechVoice();
			if self.timer then
				self.timer:Stop()
			end
			CS.XLuaManager.Instance:FinishTalkExp()
			self:Unload()
			return
		end

	end
	self:ProcessTaskInfo()
	if not self.curShowData then return end

	self:UpdateContent()
	self:UpdateLooks()
end

function TalkDialogView:UpdateContent(  )
	print("self.curShowData.typeID=,uid=",self.curShowData.type_id,self.data.u_id)
	print(ConfigMgr:GetMonsterName(self.curShowData.type_id))
	--self.npc_name_txt.text = ConfigMgr:GetMonsterName(self.curShowData.type_id)
	self.chat_txt.text = self.curShowData.content
	--self.btn_label_txt.text = self.curShowData.btnName
end

function TalkDialogView:UpdateLooks( )
	local show_data
	if self.curShowData.who == 0 then
		local mainRoleLooksInfo = LRoleMgr:GetMainRoleLooksInfo()
		if not mainRoleLooksInfo then return end
		show_data = {
			showType = UILooksNode.ShowType.Role,
			showRawImg = self.npc_raw,
			body = mainRoleLooksInfo.body,
			hair = mainRoleLooksInfo.hair,
			career = mainRoleLooksInfo.career,
			canRotate = true,
		}
	else
		show_data = {
			showType = UILooksNode.ShowType.Monster,
			showRawImg = self.npc_raw,
			npcID = self.curShowData.who,
			canRotate = true,
		}
	end
	self.looksNode = self.looksNode or UILooksNode.New(self.npc)
	self.looksNode:SetData(show_data)
end

return TalkDialogView