---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zheng.
--- DateTime: 2018/6/30 0:38
---

--首字母小写
function string.startLower(s)
    local first = string.sub(s, 1, 1)
    return string.lower(first)..string.sub(s, 2)
end