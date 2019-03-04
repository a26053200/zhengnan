---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2019/1/18 23:21
---

local LuaMonoBehaviour = require("Betel.LuaMonoBehaviour")
---@class Betel.UI.BaseList : Betel.LuaMonoBehaviour
---@field listView ListView
---@field adapter LuaListViewAdapter
---@field cell LuaListViewCell
---@field dataList Betel.List
local BaseList = class("Betel.UI.BaseList",LuaMonoBehaviour)

---@param gameObject UnityEngine.GameObject
---@param itemRendererClass table
function BaseList:Ctor(gameObject, itemRendererClass)
    BaseList.super.Ctor(self,gameObject)
    self.itemRendererClass = itemRendererClass
    self.listView = GetListView(gameObject:FindChild("Content"))
    self.adapter = self.listView.Adapter
    self.eventDispatcher = EventDispatcher.New()
    self.itemList = {}
    --self.cell = self.adapter.gameObject:GetCom("LuaListViewCell")
    self:AddLuaMonoBehaviour(gameObject,"BaseList")
end

---@param dataList Betel.List
function BaseList:SetData(dataList)
    self.dataList = dataList
    self.adapter:Init(self.dataList:Size(),handler(self,self.OnItemCreate))
    self.listView:RefreshData()
end

---@param cell LuaListViewCell
---@param index number
function BaseList:OnItemCreate(cell, index)
    local item = self.itemRendererClass.New(cell.gameObject)
    self.itemList[index + 1] = item
    item:Update(self.dataList[index + 1],index + 1)
    LuaHelper.AddButtonClick(cell.gameObject,handler(self,function ()
        self.eventDispatcher:Dispatcher(ListViewEvent.ItemClick,self.dataList[index + 1])
    end))
end

--单独更新一个
---@param index number
function BaseList:UpdateItem(index)
    local item = self.itemList[index]
    item:Update(self.dataList[index],index)
end

function BaseList:OnDestroy()
    self.eventDispatcher:RemoveAllEventListeners(ListViewEvent.ItemClick)
end

return BaseList