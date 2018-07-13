---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2018/6/14 0:16
---

---@class Core.Ioc.BaseMediator : Core.LuaMonoBehaviour
local LuaMonoBehaviour = require('Core.LuaMonoBehaviour')
local BaseMediator = class("BaseMediator",LuaMonoBehaviour)

function BaseMediator:Ctor()
    LuaMonoBehaviour.Ctor(self)
end

function BaseMediator:Start()
    self:OnInit()
    self:OnAutoRegisterEvent()
end

function BaseMediator:OnInit()

end

--自动注册事件
function BaseMediator:OnAutoRegisterEvent()
    local buttons = LuaHelper.GetChildrenButtons(self.gameObject)
    for i = 0,buttons.Length - 1 do
        local funName = "On_Click_"..buttons[i].gameObject.name
        if self[funName] then
            print(funName)
            LuaHelper.AddButtonClick(buttons[i].gameObject,handler(self,self[funName]))
        end
    end
end

function BaseMediator:OnDestroy()
    print("OnDestroy view: "..self.viewInfo.name)
    self:OnRemove()
end

function BaseMediator:OnRemove()

end

return BaseMediator