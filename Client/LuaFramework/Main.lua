--主入口函数。从这里开始lua逻辑

function Main()
    print("toLua logic start" .. Time.timeSinceLevelLoad)
    local LuaClient = require ("Game.LuaClient")
    if LuaClient then
        LuaClient.Start()
    end
end

--场景切换通知
function OnLevelWasLoaded(level)
    collectgarbage("collect")
    Time.timeSinceLevelLoad = 0

    print(level .. " was loaded")
end

function OnApplicationQuit()

end