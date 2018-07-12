---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2018/6/10 21:15
---

function class(className, super)
    local superType = type(super)
    local cls

    if superType ~= "function" and superType ~= "table" then
        superType = nil
        super = nil
    end

    if superType == "function" or (super and super.__ctype == 1) then
        -- inherited from native C++ Object
        cls = {}

        if superType == "table" then
            -- copy fields from super
            for k,v in pairs(super) do cls[k] = v end
            cls.__create = super.__create
            cls.super    = super
        else
            cls.__create = super
            cls.ctor = function() end
        end

        cls.__cname = className
        cls.__ctype = 1

        function cls.New(...)
            local instance = cls.__create(...)
            -- copy fields from class to native object
            for k,v in pairs(cls) do instance[k] = v end
            instance.class = cls
            instance:Ctor(...)
            return instance
        end

    else
        -- inherited from Lua Object
        if super then
            cls = {}
            setmetatable(cls, {__index = super})
            cls.super = super
        else
            cls = {ctor = function() end}
        end

        cls.__cname = className
        cls.__ctype = 2 -- lua
        cls.__index = cls

        function cls.New(...)
            local instance = setmetatable({}, cls)
            for k,v in pairs(cls) do instance[k] = v end
            instance.class = cls
            instance:Ctor(...)
            return instance
        end
    end

    return cls
end

---回调
function handler(obj,method)
    if method == nil then
        logError("method is nil")
    end
    return function (...)
        return method(obj, ...)
    end
end

---是否为Unity空对象
function isUnityNullObj(obj)
    if obj == nil then
        return true
    else
        return LuaHelper.isNullObj(obj)
    end
end

---异步销毁
function destroy(obj,delay)
    delay = delay or 0
    GameObject.Destroy(obj,delay)
end

---同步销毁
function destroyImmediate(obj)
    GameObject.DestroyImmediate(obj)
end

---标志不销毁对象
function dontDestroyOnLoad(obj)
    GameObject.DontDestroyOnLoad(obj)
end