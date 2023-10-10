local BagView = BaseClass(UINode)

function BagView:Constructor( parent )
	self.viewCfg = {
		prefabPath = "Assets/AssetBundleRes/ui/bag/BagView.prefab",
		parentTrans = parent,
	}
	self.model = BagModel:GetInstance()
end

function BagView:OnLoad(  )
	local names = {
		--"item_scroll","sort:obj","role_look:raw","item_scroll/Viewport/item_con","swallow:obj","name:txt","equip_con",
		"item_scroll","role_look:raw","item_scroll/Viewport/item_con","name:txt","equip_con",
	}
	UI.GetChildren(self, self.transform, names)

	self:InitEquips()
	self:AddEvents()
	self:OnUpdate()
end

function BagView:AddEvents(  )

	local onBagChanged = function ( ackData )
		print('Cat:BagView.lua[37] onBagChanged', ackData.pos)
		if ackData.pos == BagConst.Pos.Bag then
			self:UpdateGoodsItems()
		end
    end
	--GlobalEventSystem:Bind(BagConst.Event.BagChange, onBagChanged)
	self:BindEvent(GlobalEventSystem,BagConst.Event.BagChange,onBagChanged)--注意以便在他关闭后能取消bind，否则自己成员self.item_scroll空,因为已注销
end

function BagView:OnUpdate(  )
	self:UpdateRoleLooks()
	self:UpdateGoodsItems()
	self:UpdateEquips()
end

function BagView:UpdateGoodsItems(  )
	local goodsList = self.model:GetFullGoodsList(BagConst.Pos.Bag)--self.fullGoodsList[bagInfo.pos] = fullGoodsList 这是背包里面的所有物品，一个物品是一个table
	--local goe = RoleMgr.GetInstance():GetMainRole().Entity;
    --local roleid = ECS:GetComponentData(goe, CS.UnityMMO.UID)
    --local bagInfo=Bag_GetInfo({cur_role_id=roleid.Value,pos=BagConst.Pos.Bag})
	--local goodsList = bagInfo.goodsList
	
	for i,v in pairs(goodsList) do
		--print(i,"BagView:UpdateGoodsItems goodsList typeID",v.typeID)
		for n,m in pairs(v) do --每一个物品一个table
			print(n,"BagView:goodsListgoodsListvvvvv222",m)
		end
	end
	 
	--print("not self.goods_item_com=".. self.goods_item_com)
	self.goods_item_com = self.goods_item_com or self:AddUIComponent(UI.ItemListCreator)
	self.infoViewShowData = self.infoViewShowData or {
		comeFrom = "BagView"
	}
	print("self.item_scroll.sizeDelta="..self.item_scroll.sizeDelta.x..self.item_scroll.sizeDelta.y);
	local info = {
		data_list = goodsList, --我需要他是一个个
		item_con = self.item_con, 
		scroll_view = self.item_scroll,
		create_frequency = 0.1,
		create_num_per_time = 5,
		item_class = require("Game.Bag.BagGoodsItem"),
		item_width = 86,
		item_height = 86,
		space_x = 4,
		space_y = 4,
		on_update_item = function(item, i, v)
			item:SetShowData(self.infoViewShowData)
			item:SetData(v)
		end,
	}
	self.goods_item_com:UpdateItems(info)
end

function BagView:UpdateRoleLooks(  )
	local mainRoleLooksInfo = LRoleMgr:GetMainRoleLooksInfo()
	if not mainRoleLooksInfo then return end
	
	local show_data = {
		showType = UILooksNode.ShowType.Role,
		showRawImg = self.role_look_raw,
		body = mainRoleLooksInfo.body,
		hair = mainRoleLooksInfo.hair,
		career = mainRoleLooksInfo.career,
		canRotate = true,
		position = Vector3(0, 0, 0),
	}
	self.roleUILooksNode = self.roleUILooksNode or UILooksNode.New(self.role_look)
	self.roleUILooksNode:SetData(show_data)
	self.name_txt.text = mainRoleLooksInfo.name
end

function BagView:InitEquips(  )
	
end

function BagView:UpdateEquips(  )
	
end

return BagView