---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2019/1/19 0:04
---

local LuaMonoBehaviour = require("Betel.LuaMonoBehaviour")
---@class Betel.UI.ListItemRenderer : Betel.LuaMonoBehaviour
---@field listView Betel.UI.BaseList
---@field adapter EasyList.LuaListViewAdapter
---@field cell EasyList.LuaListViewCell
local ListItemRenderer = class("Game.UI.ListItemRenderer",LuaMonoBehaviour)

---@param gameObject UnityEngine.GameObject
function ListItemRenderer:Ctor(gameObject)
    ListItemRenderer.super.Ctor(self,gameObject)
end

function ListItemRenderer:UpdateItem(data, index)
    self.data = data
    self:AddLuaMonoBehaviour(self.gameObject,"ListItemRenderer")
end

function ListItemRenderer:Update()

end

function ListItemRenderer:OnDestroy()

end

return ListItemRenderer