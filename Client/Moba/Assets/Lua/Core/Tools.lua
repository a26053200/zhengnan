---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2018/7/12 21:39
---

Tools = {}

function Tools.GetArgs(...)
    local args = {}
    for i = 1, select("#",...) do
        args[i] = select(i,...)
    end
    return args
end

function Tools.FindObj(go, path)
    if isUnityNullObj(child) then
        return nil
    end
    local nodeNames = string.split(path, "/")
    local child = go.transform
    local i = 1
    while not isUnityNullObj(child) do
        child = child:Find(nodeNames[i])
    end
    if i == #nodeNames then
        return child.gameObject
    else
        return nil
    end
end