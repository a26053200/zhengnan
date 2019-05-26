--- fields
---@field public kEpsilon number
---@field public w number
---@field public x number
---@field public y number
---@field public z number
--- properties
---@field public Item number
---@field public magnitude number
---@field public negativeInfinity UnityEngine.Vector4
---@field public normalized UnityEngine.Vector4
---@field public one UnityEngine.Vector4
---@field public positiveInfinity UnityEngine.Vector4
---@field public sqrMagnitude number
---@field public zero UnityEngine.Vector4
---@class UnityEngine.Vector4 : System.ValueType
local m = {}

---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@return number
function m.Distance(a, b)end

---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@return number
function m.Dot(a, b)end

---@overload fun(other: UnityEngine.Vector4):boolean
---@param other table
---@return boolean
function m:Equals(other)end

---@return number
function m:GetHashCode()end

---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@param t number
---@return UnityEngine.Vector4
function m.Lerp(a, b, t)end

---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@param t number
---@return UnityEngine.Vector4
function m.LerpUnclamped(a, b, t)end

---@param a UnityEngine.Vector4
---@return number
function m.Magnitude(a)end

---@param lhs UnityEngine.Vector4
---@param rhs UnityEngine.Vector4
---@return UnityEngine.Vector4
function m.Max(lhs, rhs)end

---@param lhs UnityEngine.Vector4
---@param rhs UnityEngine.Vector4
---@return UnityEngine.Vector4
function m.Min(lhs, rhs)end

---@param current UnityEngine.Vector4
---@param target UnityEngine.Vector4
---@param maxDistanceDelta number
---@return UnityEngine.Vector4
function m.MoveTowards(current, target, maxDistanceDelta)end

---@overload fun()
---@param a UnityEngine.Vector4
---@return UnityEngine.Vector4
function m.Normalize(a)end

---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@return UnityEngine.Vector4
function m.Project(a, b)end

---@overload fun(scale: UnityEngine.Vector4)
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@return UnityEngine.Vector4
function m.Scale(a, b)end

---@param newX number
---@param newY number
---@param newZ number
---@param newW number
function m:Set(newX, newY, newZ, newW)end

---@overload fun():number
---@param a UnityEngine.Vector4
---@return number
function m.SqrMagnitude(a)end

---@overload fun(format: string):string
---@return string
function m:ToString()end
UnityEngine.Vector4 = m
return m