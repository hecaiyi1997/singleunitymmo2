local user_info
local Task = {
	cfg = require "Config.ConfigTask",
	taskInfos = {},
	ackTaskProgressChanged = {},
	cacheChangedTaskInfos = {},
	monsterTargetIDs = {},
}
local notifyNewChangedTaskInfo = function ( roleID )
	local taskInfo = Task.cacheChangedTaskInfos[roleID] and Task.cacheChangedTaskInfos[roleID][1]
	if taskInfo  then
		--local co = Task.ackTaskProgressChanged[roleID]
		--coroutine.resume(co)
		TaskController:ReqTaskList()
		print("notifyNewChangedTaskInfo ReqTaskList")
	end

end
local addNewChangedTaskInfo = function ( roleID, taskInfo )
	Task.cacheChangedTaskInfos[roleID] = Task.cacheChangedTaskInfos[roleID] or {}
	table.insert(Task.cacheChangedTaskInfos[roleID], taskInfo)

	CS.XMLdoc.Instance:UpdateXML(roleID, taskInfo)
	notifyNewChangedTaskInfo(roleID, taskInfo)
end
local isTaskCanTake = function ( userInfo, taskID )
	return true
end
--[[
.taskInfo {
	taskID 0 : integer
	status 1 : integer
	subTaskIndex 2 : integer
	subType 3 : integer
	curProgress 4 : integer
	maxProgress 5 : integer
	addition 6 : string
}
--]]
local createTaskInfoByID = function ( userInfo, taskID )
	local cfg = Task.cfg[taskID]
	local subTaskCfg = cfg.subTasks[1]
	local isCanTake = isTaskCanTake(userInfo, taskID)
	local taskInfo = {
		taskID = taskID, 
		status = isCanTake and TaskConst.Status.CanTake or TaskConst.Status.Unmet, 
		subTaskIndex = 1, 
		subType = subTaskCfg.subType, 
		curProgress = 0, 
		maxProgress = subTaskCfg.maxProgress,
		contentID = subTaskCfg.contentID,
	}
	return taskInfo, cfg, subTaskCfg
end

local addTargetMonsterIfKillTask = function ( roleID, taskInfo )
	if taskInfo.subType == TaskConst.SubType.KillMonster then
		Task.monsterTargetIDs[roleID] = Task.monsterTargetIDs[roleID] or {}
		Task.monsterTargetIDs[roleID][taskInfo.contentID] = Task.monsterTargetIDs[roleID][taskInfo.contentID] or 0
		Task.monsterTargetIDs[roleID][taskInfo.contentID] = Task.monsterTargetIDs[roleID][taskInfo.contentID] + taskInfo.maxProgress
		print("Task_TakeTask addTargetMonsterIfKillTask",roleID,taskInfo.contentID,Task.monsterTargetIDs[roleID][taskInfo.contentID])
	end 
end
local updateTargetMonsterIfKillTask = function ( roleID, taskInfo )
	print("KillMonster updateTargetMonsterIfKillTask0",taskInfo.subType,TaskConst.SubType.KillMonster)
	if taskInfo.subType == TaskConst.SubType.KillMonster and Task.monsterTargetIDs[roleID] and Task.monsterTargetIDs[roleID][taskInfo.contentID] then
		print("KillMonster updateTargetMonsterIfKillTask",Task.monsterTargetIDs[roleID][taskInfo.contentID])
		Task.monsterTargetIDs[roleID][taskInfo.contentID] = Task.monsterTargetIDs[roleID][taskInfo.contentID] - 1
		print("KillMonster updateTargetMonsterIfKillTask",Task.monsterTargetIDs[roleID][taskInfo.contentID])
		if Task.monsterTargetIDs[roleID][taskInfo.contentID] <= 0 then
			print("KillMonster Task.monsterTargetIDs[roleID][taskInfo.contentID] = nil")
			Task.monsterTargetIDs[roleID][taskInfo.contentID] = nil
		end
	end 
end

--TaskList={
--{roleID=xxx,other=xxx,},{},{},
--}



local InitTaskInfos = function ( userInfo )
	print("InitTaskInfos began")
	local taskInfos = {}
	local tb={}
	local tasklist={}
	--Task.gameDBServer = Task.gameDBServer or skynet.localname(".GameDBServer")
	--local hasTaskList, taskList = skynet.call(Task.gameDBServer, "lua", "select_by_key", "TaskList", "roleID", userInfo.cur_role_id)
	CS.XMLdoc.Instance:select_by_key(userInfo.cur_role_id,tb) 
	tasklist=tb
	print("xmldoc.insert call wwwwww-1",tasklist["roleID"],tb["roleID"])
	--for i,v in pairs(tasklist) do
		--print("tasklist===",i,v)
	--end
	local isDataOk =tasklist["roleID"]~=nil
	if not isDataOk then
		
		--init the first task
		local firstMainTaskID = 1
		local taskInfo, cfg, subTaskCfg = createTaskInfoByID(userInfo, firstMainTaskID)
		taskInfos.taskList = {taskInfo}
		taskInfo.roleID = userInfo.cur_role_id
		print("not isDataOk CS.XMLdoc.Instance:insert",taskInfo)
		--skynet.call(Task.gameDBServer, "lua", "insert", "TaskList", taskInfo)
		CS.XMLdoc.Instance:insert(taskInfo)
		--need to use the 'id' value in the database, so query again
		tb={}
		tasklist=CS.XMLdoc.Instance:select_by_key(userInfo.cur_role_id,tb)
		print("xmldoc.insert call wwwwww1",tasklist["roleID"],tb["roleID"])
		--local isDataOk =tasklist and #tasklist > 0
		isDataOk =tasklist["roleID"]~=nil
		
	end
	
	if isDataOk then
		taskInfos.taskList = tasklist
		--taskInfos.taskList=taskInfos.taskList
		
	end
	if taskInfos and taskInfos.taskList then
		for i,v in ipairs(taskInfos.taskList) do
			addTargetMonsterIfKillTask(userInfo.cur_role_id, v)
		end
	end
	taskInfos.taskList={taskInfos.taskList}
	return taskInfos
end

function Task_GetInfoList(userInfo, reqData)
	local taskInfos = Task.taskInfos[userInfo.cur_role_id]
	if not taskInfos then
		taskInfos = InitTaskInfos(userInfo)
		Task.taskInfos[userInfo.cur_role_id] = taskInfos
	end
	return taskInfos
end
local getTaskListInNPC = function ( roleID, npcID )
	--Cat_Todo : check if it is close to npc 
	print("dddssggggetTaskListInNPC",roleID, npcID )
	
	local taskInfos = Task.taskInfos[roleID]
	
	for i,v in pairs(taskInfos.taskList) do
		print(i,"dddssggg",v)
	end
	
 --发现本单机taskInfos.taskList={taskID=xx00,npcid=xx00}
 --而服务器上taskInfos.taskList = {taskInfo}
 --所以如果我Taskserver.lua上copy服务器端Task.lua代码，我需要在使用taskInfos把他先变成一个taskInfos.taskList={taskInfos.taskList}
 --taskInfos.taskList={taskInfos.taskList}
 local taskIDs = {}
	if taskInfos and taskInfos.taskList then
		for i,v in pairs(taskInfos.taskList) do
			local cfg = Task.cfg[v.taskID]
			local subCfg = cfg and cfg.subTasks[v.subTaskIndex]
			if v.subType == TaskConst.SubType.Talk and subCfg and subCfg.contentID == npcID then
				table.insert(taskIDs, v.taskID)
			end
		end
	end
	return taskIDs
	--Cat_Todo : dynamic task
end

function Task_GetInfoListInNPC( userInfo, reqData )
	local taskIDs = getTaskListInNPC(userInfo.cur_role_id, reqData.npcID)

	return {npcID=reqData.npcID, taskIDList=taskIDs}
end

local completeSubTask = function ( roleID, taskInfo )
	if not taskInfo then return end
	local cfg = Task.cfg[taskInfo.taskID]
	if not cfg then return end
	
	updateTargetMonsterIfKillTask(roleID, taskInfo)
	taskInfo.subTaskIndex = taskInfo.subTaskIndex + 1
	local valll=#cfg.subTasks
	print("Task_DoTask completeSubTask"..taskInfo.subTaskIndex..":"..valll)
	local isFinishTask = taskInfo.subTaskIndex > #cfg.subTasks
	if isFinishTask then
		taskInfo.subTaskIndex = #cfg.subTasks
		taskInfo.status = TaskConst.Status.Finished
		taskInfo.curProgress = taskInfo.maxProgress
	else
		local nextSubTask = cfg.subTasks[taskInfo.subTaskIndex]
		taskInfo.subType = nextSubTask.subType
		taskInfo.curProgress = 0
		taskInfo.maxProgress = nextSubTask.maxProgress or 1
		taskInfo.contentID = nextSubTask.contentID or 0
		print("Task_TakeTask --taskInfo.subType == TaskConst.SubType.KillMonster",taskInfo.subType == TaskConst.SubType.KillMonster)
		addTargetMonsterIfKillTask(roleID, taskInfo)
		if taskInfo.roleID then
			--skynet.send(Task.gameDBServer, "lua", "update", "TaskList", "id", taskInfo.id, taskInfo)
			CS.XMLdoc.Instance:UpdateXML(taskInfo.roleID, taskInfo)
		end
	end	
	return isFinishTask
end
function Task_TakeTask( userInfo, reqData )
	print("Task_TakeTask")
	local taskInfos = Task.taskInfos[userInfo.cur_role_id]
	--发现本单机taskInfos.taskList={taskID=xx00,npcid=xx00}
 	--而服务器上taskInfos.taskList = {taskInfo}
 	--所以如果我Taskserver.lua上copy服务器端Task.lua代码，我需要在使用taskInfos把他先变成一个taskInfos.taskList={taskInfos.taskList}
 	--taskInfos.taskList={taskInfos.taskList}
	--[[
	print("yyyyy",reqData.taskID)
	 for i,v in pairs(taskInfos.taskList) do
		if type(v)=="table" then
			for m,n in pairs(v) do
				print(m,"yyyyy",n)
			end
		end
	end
	--]]
	local taskInfo = table.get_value_in_array(taskInfos and taskInfos.taskList, "taskID", reqData.taskID)
	
	local result = TaskConst.ErrorCode.Unknow
	if taskInfo then
		print("Task_TakeTask",taskInfo.status ,TaskConst.Status.CanTake )
		if taskInfo.status == TaskConst.Status.CanTake then
			completeSubTask(userInfo.cur_role_id, taskInfo)
			taskInfo.status = TaskConst.Status.Doing
			result = TaskConst.ErrorCode.NoError
			--skynet.timeout(1,function()
				--addNewChangedTaskInfo(userInfo.cur_role_id, taskInfo)
			--end)
			addNewChangedTaskInfo(userInfo.cur_role_id, taskInfo)

		elseif taskInfo.status == TaskConst.Status.CanTake then
			result = TaskConst.ErrorCode.NoError
		end
	end
	return {result=result}
end

--DoTask flag=3 也就是点击完成会执行他
function Task_DoTask( userInfo, reqData )
	print("Task_DoTask"..reqData.taskID..":"..userInfo.cur_role_id)
	local taskInfos = Task.taskInfos[userInfo.cur_role_id]
	--发现本单机taskInfos.taskList={taskID=xx00,npcid=xx00}
 	--而服务器上taskInfos.taskList = {taskInfo}
 	--所以如果我Taskserver.lua上copy服务器端Task.lua代码，我需要在使用taskInfos把他先变成一个taskInfos.taskList={taskInfos.taskList}
 	--taskInfos.taskList={taskInfos.taskList}
	local taskInfo = table.get_value_in_array(taskInfos and taskInfos.taskList, "taskID", reqData.taskID)
	--local taskInfo=null
	--for i,A in pairs(taskInfos.taskList) do
		--if A["taskID"]==reqData.taskID then
			--taskInfo=A
		--end
	--end
	if taskInfo then
		--if TaskConst.ClientDoTask[taskInfo.subType] then
			local cfg = Task.cfg[reqData.taskID]
			if cfg and cfg.subTasks then
				taskInfo.subTaskIndex = taskInfo.subTaskIndex + 1
				local isFinishTask = completeSubTask(userInfo.cur_role_id, taskInfo)
				if isFinishTask then
					table.remove_value_in_array(taskInfos.taskList, "taskID", reqData.taskID)
					local nextTaskInfo = createTaskInfoByID(userInfo, cfg.nextTaskID)
					--nextTaskInfo.id = taskInfo.id
					nextTaskInfo.roleID = taskInfo.roleID
					
					addTargetMonsterIfKillTask(userInfo.cur_role_id, nextTaskInfo)
					table.insert(taskInfos.taskList, nextTaskInfo)
					--skynet.send(Task.gameDBServer, "lua", "update", "TaskList", "id", taskInfo.id, nextTaskInfo)
					print("Task_DoTask"..nextTaskInfo.taskID..":"..taskInfo.roleID)
					for k,v in pairs(nextTaskInfo) do
						print(k..":Task_DoTask"..v)
					end
					CS.XMLdoc.Instance:UpdateXML(taskInfo.roleID, nextTaskInfo)
				else
				--skynet.timeout(1,function()
					print("Task_DoTask".."not")
					addNewChangedTaskInfo(userInfo.cur_role_id, taskInfo)
				--end)
				end
				return {result=TaskConst.ErrorCode.NoError}
			else
				return {result=TaskConst.ErrorCode.HadNotTaskCfg}
			end
		--else
			--return {result=TaskConst.ErrorCode.CannotDoByClient}
		--end
	else
		return {result=TaskConst.ErrorCode.HadNotTaskInfoInRole}
	end
end

function KillMonster( roleID, monsterID, killNum )
	print('Cat:Task.lua[203] roleID, monsterID, killNum', roleID, monsterID, killNum, Task.monsterTargetIDs[roleID], Task.monsterTargetIDs[roleID] and Task.monsterTargetIDs[roleID][monsterID] or "nil", Task.taskInfos[roleID])
	--if  Task.monsterTargetIDs[roleID] or Task.monsterTargetIDs[roleID][monsterID] then
		--return
	--end
	local found_roleID=false;
	local found_monsterID=false;
	for m,n in pairs(Task.monsterTargetIDs) do
		if m==roleID then
			found_roleID=true;
		end
	end
	if found_roleID==true then
		for k,v in pairs(Task.monsterTargetIDs[roleID]) do
			if k==monsterID then
				found_monsterID=true;
			end
		end
	end
	if found_monsterID==false then return end
	print("KillMonster1",Task.monsterTargetIDs[roleID][monsterID])
	if Task.monsterTargetIDs[roleID] and Task.monsterTargetIDs[roleID][monsterID] then
		print("KillMonster2")
		local taskInfos = Task.taskInfos[roleID]
		if taskInfos and taskInfos.taskList then
			print("KillMonster3")
			for i,v in ipairs(taskInfos.taskList) do
				print('Cat:Task.lua[216] v.curProgress', v.curProgress, v.maxProgress, v.contentID, v.subType)
				if v.subType == TaskConst.SubType.KillMonster and v.contentID == monsterID then
					print("KillMonster4")
					v.curProgress = v.curProgress + killNum
					if v.curProgress >= v.maxProgress then
						print("KillMonster5")
						completeSubTask(roleID, v)
						addNewChangedTaskInfo(roleID, v)
						CS.XMLdoc.Instance:UpdateXML(v.roleID, v)

					else
						print("KillMonster6")
						addNewChangedTaskInfo(roleID, v)
						--avoid frequent update database
						--if v.curProgress%5==0 then
							--skynet.send(Task.gameDBServer, "lua", "update", "TaskList", "id", v.id, v)
						--end
						CS.XMLdoc.Instance:UpdateXML(v.roleID, v)
					end
				end
			end
		end
	end
end
