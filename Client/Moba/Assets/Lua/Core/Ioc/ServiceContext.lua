---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhengnan.
--- DateTime: 2018/6/12 10:37
---

local IocContext = require("Core.Ioc.IocContext")
local ServiceContext = class("ServiceContext",IocContext)

function ServiceContext:Ctor(binder)
    self.binder = binder
end

function ServiceContext:Launch()
    --TODO
    --TODO
end

return ServiceContext