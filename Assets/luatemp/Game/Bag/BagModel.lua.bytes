BagModel = BaseClass(EventDispatcher)

function BagModel:Constructor(  )
	self:Reset()
end

function BagModel:GetInstance(  )
	if not BagModel.Instance then
		BagModel.Instance = BagModel.New()
	end
	return BagModel.Instance
end

function BagModel:Reset(  )
	self.bagInfo = {}
	self.fullGoodsList = {}--包含空格子的全道具列表，主要用于背包界面显示之用
end

function BagModel:GetFullGoodsList( pos )
	local dd=self.fullGoodsList[pos]   --self.fullGoodsList[bagInfo.pos] = fullGoodsList  --这是背包里面的所有物品，一个物品是一个table
	--for i,v in ipairs(dd) do
		--print(i,"BagModel:GetFullGoodsList goodsList typeID",v.typeID)
		--for n,m in pairs(v) do --每一个物品一个table
			--print(n,"vvvvvvvvBagModel:GetFullGoodsListvvvvv",m)
		--end
	--end
	return dd 
end

function BagModel:GetBagInfo( pos )
	return self.bagInfo[pos]
end

function BagModel:SetBagInfo( bagInfo ) ---bagInfo={cellNum=200, pos=pos,goodsList = goodsList},而goodsList只是一个列表{table，table，}
	if not bagInfo or not bagInfo.pos then return end	
	self.bagInfo[bagInfo.pos] = bagInfo
	self.fullGoodsList[bagInfo.pos] =bagInfo.goodsList   --bagInfo.pos此乃背包仓库
	self:GetFullGoodsList( bagInfo.pos )
	
	GlobalEventSystem:Fire(BagConst.Event.BagChange,bagInfo)
end

function BagModel:RemoveGoods( cell, pos )
	local goodsInfo, index = self:FindGoods(cell, pos)
	print("Cat:BagModel [start:51] goodsInfo: , index", goodsInfo, index, uid, pos)
	PrintTable(goodsInfo)
	print("Cat:BagModel [end]")
	pos = goodsInfo and goodsInfo.pos--可以不传pos的，不传pos则查所有的背包
	if goodsInfo then
		local fullGoodsList = self.fullGoodsList and self.fullGoodsList[pos] 
		print('Cat:BagModel.lua[57] fullGoodsList', fullGoodsList)
		if fullGoodsList then
			fullGoodsList[goodsInfo.cell] = false
		end
		local goodsList = self.bagInfo and self.bagInfo[pos] and self.bagInfo[pos].goodsList 
		table.remove(goodsList, index)
	end
end

function BagModel:FindGoods2( uid, pos )
	if pos then
		local goodsList = self.bagInfo and self.bagInfo[pos] and self.bagInfo[pos].goodsList 
		print('Cat:BagModel.lua[69] goodsList, uid, pos', goodsList, uid, pos)
		if goodsList then
			for i,goodsInfo in ipairs(goodsList) do
				print('Cat:BagModel.lua[72] goodsInfo.uid', goodsInfo.uid)
				if goodsInfo.uid == uid then
					return goodsInfo, i
				end
			end
		end
	elseif self.bagInfo then
		for k,v in pairs(self.bagInfo) do
			if v.goodsList then
				for i,goodsInfo in ipairs(v.goodsList) do
					if goodsInfo.uid == uid then
						return goodsInfo, i
					end
				end
			end
		end
	end
end

function BagModel:FindEmptyCell2(  )
	local cell = 1
	if bagInfo and bagInfo.goodsList then
		cell = #bagInfo.goodsList + 1
		for i,v in ipairs(bagInfo.goodsList) do
			if v.cell > i then
				return i
			end
		end
	end
	return cell
end

function BagModel:AddGoods2( newGoodsInfo )
	if not newGoodsInfo then return end
	
	local pos = newGoodsInfo.pos
	local goodsList = self.bagInfo and self.bagInfo[pos] and self.bagInfo[pos].goodsList 
	if goodsList then
		local hasInserted = false
		for i,v in ipairs(goodsList) do
			if v.cell > newGoodsInfo.cell then
				hasInserted = true
				table.insert(goodsList, i, newGoodsInfo)
				break
			end
		end	
		if not hasInserted then
			table.insert(goodsList, newGoodsInfo)
		end
	end
	local fullGoodsList = self.fullGoodsList and self.fullGoodsList[pos] 
	if fullGoodsList then
		fullGoodsList[newGoodsInfo.cell] = newGoodsInfo
	end
end
function BagModel:UpdateBagInfos( bagInfos )--BagView监听BagConst.Event.BagChange，从FullGoodsList取数据更新ui
--所以这里要更新数据

end
function BagModel:UpdateBagInfos2( bagInfos )
	if not bagInfos or not bagInfos.goodsList then return end

	for i,changeGoodsInfo in ipairs(bagInfos.goodsList) do
		if changeGoodsInfo.num <= 0 then
			self:RemoveGoods(changeGoodsInfo.cell, changeGoodsInfo.pos)
		else
			local isInBag = false
			local goodsInfo, index = self:FindGoods(changeGoodsInfo.cell, changeGoodsInfo.pos)
			if goodsInfo then
				--已在背包里的道具
				isInBag = true
				for key,value in pairs(changeGoodsInfo) do
					goodsInfo[key] = value
				end
			end
			print('Cat:BagModel.lua[132] isInBag', isInBag)
			if not isInBag then
				--新加入的道具
				self:AddGoods(changeGoodsInfo)
			end
		end
	end
	self:Fire(BagConst.Event.BagChange)
end

function BagModel:GetGoodsName( typeID, needColor )
	local cfg = ConfigMgr:GetGoodsCfg(typeID)
	if needColor then
		return string.format("<color=%s>%s</color>", ColorUtil:GetStr(cfg.color), cfg.name)
	else
		return cfg.name
	end
end

return BagModel
