---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhengnan.
--- DateTime: 2018/6/12 10:37
---

local IocContext = require("Core.Ioc.IocContext")
local ModelContext = class("ModelContext",IocContext)

function ModelContext:Ctor(binder)
    self.binder = binder
end

function ModelContext:Launch()

end

return ModelContext