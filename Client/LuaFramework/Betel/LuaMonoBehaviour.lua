---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2018/6/14 0:22
--- LuaMonoBehaviour
---

local BehaviourFun = {"Awake","Start","OnEnable","OnDisable","OnDestroy","Update","LateUpdate","FixedUpdate"}
local LuaObject = require("Betel.LuaObject")

---@class Betel.LuaMonoBehaviour : Betel.LuaObject
---@field gameObject UnityEngine.GameObject
---@field transform UnityEngine.Transform
local LuaMonoBehaviour = class("LuaMonoBehaviour",LuaObject)

---@param gameObject UnityEngine.GameObject
function LuaMonoBehaviour:Ctor(gameObject)
    self.gameObject = gameObject
    if not isNull(gameObject) then
        self.transform = gameObject.transform
    end
    self.coMap = {}
    self.eventMap = {}
    self.delayList = {}
end

function LuaMonoBehaviour:AddLuaMonoBehaviour(go,name)
    self.luaBehaviour = nil
    for k, v in pairs(BehaviourFun) do
        if self[v] and isFunction(self[v]) then
            self.luaBehaviour = LuaHelper.AddLuaMonoBehaviour(go,name,v,handler(self,self[v]))
        end
    end
    return self.luaBehaviour
end

function LuaMonoBehaviour:AddGlobalEventListener(type, listener)
    if self.eventMap[listener] == nil then
        self.eventMap[listener] = {type = type, handler = handler(self,listener)}
        edp:AddEventListener(type, self.eventMap[listener].handler)
    end
end

function LuaMonoBehaviour:RemoveGlobalEventListener(type, handler)
    if self.eventMap[handler] then
        edp:RemoveEventListener(type, self.eventMap[handler].handler)
    end
end

function LuaMonoBehaviour:StartCoroutine(coFun)
    if self.coMap[coFun] == nil then
        self.coMap[coFun] = coroutine.start(function ()
            coFun()
        end)
    else
        logError("StartCoroutine has already exits" .. tostring(coFun))
    end
    return self.coMap[coFun]
end

---@param delay number
---@param callback Handler
function LuaMonoBehaviour:CreateDelay(delay, callback)
    local d = DelayCallback(delay,callback)
    table.insert(self.delayList, d)
    return d
end

function LuaMonoBehaviour:Dispose()
    if self.coMap then
        for _, co in pairs(self.coMap) do
            coroutine.stop(co)
        end
    end
    if self.eventMap then
        for _, event in pairs(self.eventMap) do
            edp:RemoveEventListener(event.type, event.handler)
        end
    end
    if self.delayList then
        for _, d in pairs(self.delayList) do
            CancelDelayCallback(d)
        end
    end
    self.coMap = {}
    self.eventMap = {}
    self.delayList = {}
end

function LuaMonoBehaviour:Destroy()
    destroy(self.gameObject)
    self:Dispose()
end

function LuaMonoBehaviour:OnDestroy()

end

return LuaMonoBehaviour