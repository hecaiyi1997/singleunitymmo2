
local openwordsView = BaseClass(UINode)

function openwordsView:Constructor(  )
	self.viewCfg = {
		prefabPath = "Assets/AssetBundleRes/ui/task/TaskDialogView_2_1.prefab",
		canvasName = "Normal",
	}
	self.PlayAudioCompleteflg=0
	self.PlayAudioStartflg=0
	self:Load()
end

function openwordsView:OnLoad( )
	local names = {
		"bottom/npc:raw","bottom/chat:txt",
	}
	UI.GetChildren(self, self.transform, names)
	self.transform.sizeDelta = Vector2.zero
	self:AddEvents()
	
end

function openwordsView:HandleBtnClick( )
	print('Cat:TaskDialogView.lua[26] self.curShowData.clickCallBack', self.curShowData.clickCallBack)
	if self.curShowData.clickCallBack then
		self.curShowData.clickCallBack()
	else
		self:Unload()
	end
end
function openwordsView:PlayAudioComplete()
	print("TalkDialogView:PlayAudioComplete")
	self.PlayAudioCompleteflg=1
    
end
function openwordsView:PlayAudioStart()
	print("TalkDialogView:PlayAudioStart")
	self.PlayAudioStartflg=1
    
end
function openwordsView:AddEvents(  )
	GlobalEventSystem:Bind(NetDispatcher.Event.PlayAudioComplete, openwordsView.PlayAudioComplete, self)
    GlobalEventSystem:Bind(NetDispatcher.Event.PlayAudioStart, openwordsView.PlayAudioStart, self)
	
end

function openwordsView:ShowNextTalk(  )
	--CS.SpeechVoice.Instance:StopSpeechVoice()
    self.curShowTalkNum = self.curShowTalkNum + 1
	self:OnUpdate()
end
function openwordsView:ClickOk(  )
	self:PlayAudioComplete()
	self:Unload()
end

function openwordsView:ProcessBtnNameAndCallBack( flag )
	
	if not self.flagMap then
		self.flagMap = {
			[TaskConst.DialogBtnFlag.Continue] = {name="继续", func=openwordsView.ShowNextTalk},
			[TaskConst.DialogBtnFlag.TakeTask] = {name="接取", func=openwordsView.ReqTakeTask},
			[TaskConst.DialogBtnFlag.DoTask]   = {name="完成", func=openwordsView.ReqDoTask}, 
			[TaskConst.DialogBtnFlag.Ok] 	   = {name="确定", func=openwordsView.ClickOk},
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
			print("lefttime curShowTalkNum=",string.format("%s秒后自动", math.floor((leftTime-22)/1)),math.floor((leftTime-22)/1),leftTime-22)
  			if leftTime-22<0 then leftTime=22 end
			--self.left_time_txt.text = string.format("%s秒后自动", math.floor((leftTime-22)/1))
		elseif self.PlayAudioCompleteflg==1 then
			self.PlayAudioCompleteflg=0
			print("TalkDialogView:ProcessBtnNameAndCallBack curShowTalkNum")
			self:HandleBtnClick()
		end
	end, 0.2)
end

function openwordsView:ProcessTaskInfo(  )
	if not self.data then return end
	self.curShowData = table.deep_copy(self.data)
	local cfg=require("Config/ConfigTalk")
	local subTaskCfg=cfg[self.data.type_id]
    if not subTaskCfg or not subTaskCfg.content then
        	self.curShowData = nil
    end
	if self.curShowData then
    	self.curShowTalkNum = self.curShowTalkNum or 1
		print("self.curShowTalkNum =",self.curShowTalkNum)
    	local talkCfg = subTaskCfg.content[self.curShowTalkNum]
        	if talkCfg then
	       	 	self.curShowData.content = talkCfg.chat
	        	self.curShowData.who = talkCfg.who
				print("CS.SpeechVoice.Instance:StopVoice")

				CS.SpeechVoice.Instance:Speech_Voice(self.curShowData.content)
				print("CS.SpeechVoice.Instance:Speech_Voic")
				self:ProcessBtnNameAndCallBack(talkCfg.flag)
	    	end
		--Cat_Todo : multi task in npc
    else
        --show default conversation
        self.curShowData = {}
        self.curShowData.content = "哈哈,你猜我是谁?"
        self.curShowData.who = self.data.type_id
        self:ProcessBtnNameAndCallBack(TaskConst.DialogBtnFlag.Ok)
		CS.SpeechVoice.Instance:StopVoice();
		CS.SpeechVoice.Instance:Speech_Voice(self.curShowData.content)
    end
end

function openwordsView:OnUpdate(  )
	self:ProcessTaskInfo()
	if not self.curShowData then return end

	self:UpdateContent()
	--self:UpdateLooks()
end

function openwordsView:UpdateContent(  )
	print("self.curShowData.typeID=,uid=",self.curShowData.type_id,self.data.u_id)
	print(ConfigMgr:GetMonsterName(self.curShowData.type_id))
	--self.npc_name_txt.text = ConfigMgr:GetMonsterName(self.curShowData.type_id)
	self.chat_txt.text = self.curShowData.content
	--self.btn_label_txt.text = self.curShowData.btnName
end

return openwordsView