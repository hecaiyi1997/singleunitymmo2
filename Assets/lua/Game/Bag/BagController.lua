BagConst = require("Game/Bag/BagConst")
BagModel = require("Game/Bag/BagModel")
local GoodsInfoView = require("Game.Common.UI.GoodsInfoView")

require("Game/Bag/BagServer")

BagController = {}

function BagController:Init(  )
	self.model = BagModel:GetInstance()
	self:AddEvents()
end

function BagController:GetInstance()
	return BagController
end

function BagController:AddEvents(  )
	local onGameStart = function (  )

        print("BagControlleronGameStartdddddffggghhhhh")
        self.model:Reset()
        self:ReqAllBags()
        --self:ListenBagChange()
    end
    GlobalEventSystem:Bind(GlobalEvents.GameStart, onGameStart)

    local onBagChanged = function ( ackData )
        print("BagController:ListenBagChange");
        print("Cat:BagController [start:38] onBagChanged ackData:", ackData)
        PrintTable(ackData)
        print("Cat:BagController [end]")
        self.model:UpdateBagInfos(ackData)
    end

    --GlobalEventSystem:Bind(BagConst.Event.BagChange, onBagChanged)

end
function BagController:useGood(typeID,num)
    print("BagController:useGood",typeID,num)
    if 100000==typeID then
        SceneHelper.changehp(20.0);
        Bag_ChangeBagGoods(typeID,num)
        self:ReqBagList(BagConst.Pos.Bag)
        return{result=0}
    end
    if 100001==typeID then
        SceneHelper.changehp(40.0);
        Bag_ChangeBagGoods(typeID,num)
        self:ReqBagList(BagConst.Pos.Bag)
        return{result=0}
    end
    if 100002==typeID then
        SceneHelper.changehp(60.0);
        Bag_ChangeBagGoods(typeID,num)
        self:ReqBagList(BagConst.Pos.Bag)
        return{result=0}
    end
    return{result=1}
end
function BagController:addGood(typeID,num)
    --print("BagController:addGood",typeID,num)
    --if 100001==typeID then
    Bag_ChangeBagGoods(typeID,num)
    self:ReqBagList(BagConst.Pos.Bag)
    return{result=0}
    --end
    --return{result=0}
end
function BagController:ReqAllBags(  )
    print("BagController:ReqAllBags")
    self:ReqBagList(BagConst.Pos.Bag)
    --self:ReqBagList(BagConst.Pos.Warehouse)
    --self:ReqBagList(BagConst.Pos.Equip)
end

function BagController:ReqBagList( pos )

    local goe = RoleMgr.GetInstance():GetMainRole().Entity;
    local roleid = ECS:GetComponentData(goe, CS.UnityMMO.UID)
    print("BagController roleid=",roleid);
    local bagList=Bag_GetInfo({cur_role_id=roleid.Value,pos=pos})
    for i,v in pairs(bagList.goodsList) do
        print("BagController:ReqBagList"..v.typeID)
    end
    -----bagList={cellNum=200, pos=pos,goodsList = goodsList},而goodsList只是一个列表{table，table，}
    self.model:SetBagInfo(bagList)

    --得到的这个bagList是啥格式？答：一个列表，每个元素一个gold详情

end

function BagController:ReqDropGoods( uid )
    local on_ack = function ( ackData )
        print("Cat:BagController [start:29] ackData: ", ackData)
        PrintTable(ackData)
        print("Cat:BagController [end]")
        self.model:SetBagInfo(ackData)
    end
    NetDispatcher:SendMessage("Bag_DropGoods", {uid=uid}, on_ack)
end

function BagController:ListenBagChange(  )
    
	local onBagChanged = function ( ackData )
        print("BagController:ListenBagChange");
        print("Cat:BagController [start:38] onBagChanged ackData:", ackData)
        PrintTable(ackData)
        print("Cat:BagController [end]")
        self.model:UpdateBagInfos(ackData)
    end
    --NetDispatcher:Listen("Bag_GetChangeList", nil, onBagChanged)
    GlobalEventSystem:Bind(BagConst.Event.BagChange, onBagChanged)
end

function BagController:ShowGoodsTips( goodsInfo, showData )
    if not goodsInfo then return end
    
    if not goodsInfo.cfg then
        goodsInfo.cfg = ConfigMgr:GetGoodsCfg(goodsInfo.typeID)
    end
    if not goodsInfo.cfg then return end

    local infoView = GoodsInfoView.Create()
    print('Cat:BagController.lua[ShowGoodsTips] infoView', infoView)
    infoView:SetData(goodsInfo, showData)
    infoView:Load()
    -- if goodsInfo.cfg.type == 
end

return BagController