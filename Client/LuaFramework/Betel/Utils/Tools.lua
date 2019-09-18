---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2018/7/12 21:39
---

local Tools = {}

--获取...的参数数组
function Tools.GetArgs(...)
    local args = {}
    for i = 1, select("#",...) do
        args[i] = select(i,...)
    end
    return args
end


--是否是空对象
function Tools.isNullObj(obj)
    if obj == nil or isNull(obj.gameObject) then
        return true
    end
    return false
end

--c# 数组转化为Lua表
---@param array any
function Tools.ToLuaArray(array)
    local list = {}
    for i = 0, array.Length - 1 do
        table.insert(list, array[i])
    end
    return list
end

function Tools.FindObj(go, path)
    if isNull(go) then
        return nil
    end
    local nodeNames = string.split(path, "/")
    local child = go.transform
    local i = 1
    while not isNull(child) do
        child = child:Find(nodeNames[i])
    end
    if i == #nodeNames then
        return child.gameObject
    else
        return nil
    end
end

-- 删除table中的元素
function Tools.removeElementByKey(tbl,key)
    --新建一个临时的table
    local tmp ={}

    --把每个key做一个下标，保存到临时的table中，转换成{1=a,2=c,3=b}
    --组成一个有顺序的table，才能在while循环准备时使用#table
    for i in pairs(tbl) do
        table.insert(tmp,i)
    end

    local newTbl = {}
    --使用while循环剔除不需要的元素
    local i = 1
    while i <= #tmp do
        local val = tmp [i]
        if val == key then
            --如果是需要剔除则remove
            table.remove(tmp,i)
        else
            --如果不是剔除，放入新的tabl中
            newTbl[val] = tbl[val]
            i = i + 1
        end
    end
    return newTbl
end
--获取一个随机数数组
function Tools.GetRandomArray(n)
    local map = {}
    local list = {}
    for i = 1, n do
        local r = math.random(i, n)
        local a = r
        if map[r] then
            a = map[r]
        end
        table.insert(list, a)
        map[r] = map[i] or i
    end
    return list
end


---@param camera UnityEngine.Camera
function Tools.WorldToUILocalPosition(camera, worldPosition)
    local pos = UnityEngine.Camera.main:WorldToViewportPoint(worldPosition);
    return camera:ViewportToWorldPoint(pos);
end
return Tools