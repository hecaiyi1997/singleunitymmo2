
local BagConst = require "game.bag.BagConst"
local GoodsCfg = require "Config.ConfigGoods"
local this = {
	bagLists = {},
	user_info = nil,
	id_service = nil,
	gameDBServer = nil,
}

local function initBagList( cur_role_id,pos)--如reqData.pos=BagConst.Pos.Bag
	--this.gameDBServer = this.gameDBServer or skynet.localname(".GameDBServer")
	local condition = string.format("roleID%sandpos%s", cur_role_id, pos)
	print("initBagList condition=",condition);
	local goodsList={}
	local tmp={}
	local hs=CS.XMLdoc.Instance:select_by_condition(condition,tmp,goodsList)--此刻从服务器还回的goodsList只是一个物品列表{table，table，}
	
	local list={}
		for m,n in pairs (goodsList) do	
			local tb={}	
			tb.roleID=n.roleID;	
			tb.num=n.num;
			tb.pos=n.pos;
			tb.typeID=n.typeID;
			tb.uid=n.uid;
			tb.cell=n.cell;
			table.insert(list,tb)
			--for k,j in pairs (tb) do
				--print(k, "select_by_condition2", j);
			--end
			tb=nil
		end
	goodsList=list;
	for m,n in pairs (list) do
		print(m, "select_by_condition1", n);
		for k,j in pairs (n) do
			print(k, "select_by_condition2", j);
		end
	end
	--local hasBagList, goodsList = skynet.call(this.gameDBServer, "lua", "select_by_condition", "Bag", condition)
	local bagInfo = {cellNum=200, pos=pos}
	if hs then
		bagInfo.goodsList = list  --goodsList={{uid=,cell=,}。。。。，}这样的一个列表而已
	else
		bagInfo.goodsList = {}
	end
	return bagInfo  --={cellNum=200, pos=pos,goodsList = goodsList},而goodsList只是一个列表{table，table，}
end

local findEmptyCellIndex = function ( goodsList )
	local cell = 1
	if goodsList then
		cell = #goodsList + 1
	else
		cell = 1
	end
	return cell
end

--ignoreFullOverlap：是否忽略数量已满重叠数的道具
local findGoodsInList = function ( goodsList, goodsTypeID, ignoreFullOverlap )
	if not goodsList then return end
	local cfg = ignoreFullOverlap and GoodsCfg[goodsTypeID]
	for i,v in ipairs(goodsList) do
		if v.typeID == goodsTypeID then
			if not ignoreFullOverlap or v.num < cfg.overlap then
				return v, i
			end
		end
	end
	return nil
end

local notifyBagChange = function (  )
	if this.cacheChangeList and #this.cacheChangeList > 0 and this.coForGoodsChangeList then
		local co = this.coForGoodsChangeList
		this.coForGoodsChangeList = nil
		skynet.wakeup(co)
	end
end

local addNewGoodsToNotifyCache = function ( goodsInfo, notify )
	this.cacheChangeList = this.cacheChangeList or {}
	local hasFind = false
	for i,v in ipairs(this.cacheChangeList) do
		if v.uid == goodsInfo.uid then
			hasFind = true
			this.cacheChangeList[i] = goodsInfo
			break
		end
	end
	if not hasFind then
		table.insert(this.cacheChangeList, goodsInfo)
	end
	if notify then
		notifyBagChange()
	end
end

local changeGoodsNum = nil
changeGoodsNum = function( goodsTypeID, diffNum, pos, notify )
	-- print('Cat:BagMgr.lua[81] goodsTypeID, diffNum, pos, notify', goodsTypeID, diffNum, pos, notify)
	--this.gameDBServer = this.gameDBServer or skynet.localname(".GameDBServer")
	local goe = RoleMgr.GetInstance():GetMainRole().Entity;
    local cur_role_id = ECS:GetComponentData(goe, CS.UnityMMO.UID)
	--print("changeGoodsNum roleid=",cur_role_id);
	--local condition = string.format("roleID=%s and pos=%s",cur_role_id, pos)--我觉得必须加上cur_role_id才能指定某个人
	local condition = string.format("roleID%sandpos%s", cur_role_id.Value, pos)
	print("changeGoodsNum condition=",condition);
	local bagInfo = this.bagLists[condition]

	if bagInfo and bagInfo.goodsList then
		local goodsInfo, goodsIndex = findGoodsInList(bagInfo.goodsList, goodsTypeID, true)
		local cfg = GoodsCfg[goodsTypeID]
		local overlapNum = cfg and cfg.overlap or 1
		-- print('Cat:BagMgr.lua[90] overlapNum', goodsInfo, goodsIndex, overlapNum)
		local newGoods
		if goodsInfo and goodsInfo.num < overlapNum then
			local goodsLastNum = goodsInfo.num
			goodsInfo.num = math.min(overlapNum, goodsInfo.num + diffNum)
			-- print('Cat:BagMgr.lua[90] goodsInfo.num, diffNum', goodsInfo.num, diffNum)
			if goodsInfo.num <= 0 then
				--diffNum可以为负数，道具数为0，该清除掉该道具了
				table.remove(bagInfo.goodsList, goodsIndex)
				if goodsInfo.num < 0 then
					print("bag change goods num less than 0")
				end
				goodsInfo.num = 0
				--skynet.call(this.gameDBServer, "lua", "delete", "Bag", "uid", goodsInfo.uid)

				local goe = RoleMgr.GetInstance():GetMainRole().Entity;
				local cur_role_id = ECS:GetComponentData(goe, CS.UnityMMO.UID)
				--print("changeGoodsNum roleid=",cur_role_id);
				local condition = string.format("roleID%sandpos%s", cur_role_id.Value, 1)
				print("delete condition=",condition);
				local hs=CS.XMLdoc.Instance:delete(condition, goodsInfo.typeID)
			else
				--更新道具数量
				--skynet.call(this.gameDBServer, "lua", "update", "Bag", "uid", goodsInfo.uid, goodsInfo)
				
				
				
				local goe = RoleMgr.GetInstance():GetMainRole().Entity;
				local cur_role_id = ECS:GetComponentData(goe, CS.UnityMMO.UID)
				--print("changeGoodsNum roleid=",cur_role_id);
				local condition = string.format("roleID%sandpos%s", cur_role_id.Value, 1)
				print("update condition=",condition);
				local hs=CS.XMLdoc.Instance:update(condition, goodsInfo)
				
				diffNum = diffNum-(goodsInfo.num-goodsLastNum)
				-- print('Cat:BagMgr.lua[113] diffNum', diffNum)
				if diffNum > 0 then
					changeGoodsNum(goodsTypeID, diffNum, pos, false)
				end
			end
			newGoods = goodsInfo
		else
			--已达到该道具的最大重叠数，所以在占另外的背包格子
			local emptyCell = findEmptyCellIndex(bagInfo and bagInfo.goodsList)
			if emptyCell > BagConst.MaxCell then
				--Cat_Todo : handle full cell
				return
			end
			-- print('Cat:BagMgr.lua[81] emptyCell', emptyCell)
			--this.id_service = this.id_service or skynet.localname(".id_service")
			--local uid = skynet.call(this.id_service, "lua", "gen_uid", "goods")
			local uid=1;
			local addNum = math.min(overlapNum, diffNum)
			newGoods = {
				uid = uid,
				typeID = goodsTypeID,
				num = addNum,
				pos = pos,
				cell = emptyCell,
				roleID = cur_role_id.Value,
			}
			table.insert(bagInfo.goodsList, emptyCell, newGoods)
			--skynet.call(this.gameDBServer, "lua", "insert", "Bag", newGoods)
			local goe = RoleMgr.GetInstance():GetMainRole().Entity;
			local cur_role_id = ECS:GetComponentData(goe, CS.UnityMMO.UID)
			--print("changeGoodsNum roleid=",cur_role_id);
			local condition = string.format("roleID%sandpos%s", cur_role_id.Value, 1)
			print("insert condition=",condition);
			local hs=CS.XMLdoc.Instance:insert(condition, newGoods)

			diffNum = diffNum-addNum
			if diffNum > 0 then
				changeGoodsNum(goodsTypeID, diffNum, pos, false)
			end
		end
		--addNewGoodsToNotifyCache(newGoods, notify)
	else
		--uninit?
		print("bag:add goods uninit bag info")
	end
end

local getGoodsByUID = function ( uid )
	if not this.bagLists then
		return
	end
	for pos,bagList in pairs(this.bagLists) do
		if bagList.goodsList then
			for i,goodsInfo in ipairs(bagList.goodsList) do
				if goodsInfo.uid == uid then
					return goodsInfo, i
				end
			end
		end
	end
	return nil
end

local clearAllGoods = function()
	if not this.bagLists then return end
	
	this.gameDBServer = this.gameDBServer or skynet.localname(".GameDBServer")
	skynet.call(this.gameDBServer, "lua", "delete", "Bag", "roleID", this.user_info.cur_role_id)
	for pos,bagList in pairs(this.bagLists) do
		bagList.goodsList = {}
		print('Cat:BagMgr.lua[172] pos', pos)
		-- for k,goodsInfo in pairs(bagList.goodsList) do
		-- 	print('Cat:BagMgr.lua[171] goodsInfo.uid', goodsInfo.uid)
		-- end
	end
end

local SprotoHandlers = {}
function Bag_GetInfo( reqData ) --如cur_role_id=roleid.Value,pos=BagConst.Pos.Bag
	--local condition = string.format("roleID=%s and pos=%s", reqData.cur_role_id, reqData.pos)--我觉得必须加上cur_role_id才能指定某个人
	local condition = string.format("roleID%sandpos%s", reqData.cur_role_id, reqData.pos)
	local bagList = this.bagLists[condition]
	if not bagList then
		bagList = initBagList(reqData.cur_role_id,reqData.pos)
		this.bagLists[condition] = bagList --把reqData.pos改成condition才合理
		print("Bag_GetInfo condition=",condition);
	end
	return bagList  ----bagList={cellNum=200, pos=pos,goodsList = goodsList},而goodsList只是一个列表{table，table，}
end

function SprotoHandlers.Bag_DropGoods( reqData )
	local goodsInfo, index = getGoodsByUID(reqData.uid)
	-- print('Cat:BagMgr.lua[146] goodsInfo, pos, index', goodsInfo, index, reqData.uid)
	if goodsInfo then
		goodsInfo.num = 0
		addNewGoodsToNotifyCache(goodsInfo, true)
		skynet.call(this.gameDBServer, "lua", "delete", "Bag", "uid", goodsInfo.uid)
		table.remove(this.bagLists[goodsInfo.pos].goodsList, index)
		return {result = ErrorCode.Succeed}
	else
		return {result = ErrorCode.CannotFindGoods}
	end
end

function SprotoHandlers.Bag_GetChangeList( reqData )
	-- print('Cat:BagMgr.lua[131] req get change list')
	if not this.coForGoodsChangeList then
		if not this.cacheChangeList or #this.cacheChangeList <= 0 then
			this.coForGoodsChangeList = coroutine.running()
			-- print('Cat:BagMgr.lua[134] this.coForGoodsChangeList', this.coForGoodsChangeList)
			skynet.wait(this.coForGoodsChangeList)
		end
		local changeList = this.cacheChangeList
		if changeList then
			this.cacheChangeList = nil
			return {goodsList=changeList}
		else
		end
	else
		--shouldn't be here,the client requested it again before replying
	end
	return {}
end

local PublicFuncs = {}
function PublicFuncs.Init( user_info )
	this.user_info = user_info
end
function Bag_ChangeBagGoods( goodsTypeID, diffNum )
	print('Cat:BagServer.lua[137] goodsTypeID, diffNum', goodsTypeID, diffNum)
	changeGoodsNum(goodsTypeID, diffNum, BagConst.Pos.Bag, true)
end
function PublicFuncs.ClearAllGoods()
	print('Cat:BagMgr.lua[222] ClearAllGoods')
	clearAllGoods()
end
SprotoHandlers.PublicClassName = "Bag"
SprotoHandlers.PublicFuncs = PublicFuncs
return SprotoHandlers