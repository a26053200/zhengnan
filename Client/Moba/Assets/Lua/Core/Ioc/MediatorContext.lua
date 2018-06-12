---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhengnan.
--- DateTime: 2018/6/12 10:37
---

local IocContext = require("Core.Ioc.IocContext")
local MediatorContext = class("MediatorContext",IocContext)

function MediatorContext:Ctor(binder)
    self.binder = binder
end

function MediatorContext:GetMediator(viewName)
    local mdrClass = self.binder.typeDict[viewName]
    if mdrClass == nil then
        logError("View:{0} mediator has not register",viewName)
    end
    return mdrClass
end

function MediatorContext:Launch()

end

return MediatorContext