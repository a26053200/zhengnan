--- fields
---@field public kEpsilon number
---@field public kEpsilonNormalSqrt number
---@field public x number
---@field public y number
--- properties
---@field public Item number
---@field public down UnityEngine.Vector2
---@field public left UnityEngine.Vector2
---@field public magnitude number
---@field public negativeInfinity UnityEngine.Vector2
---@field public normalized UnityEngine.Vector2
---@field public one UnityEngine.Vector2
---@field public positiveInfinity UnityEngine.Vector2
---@field public right UnityEngine.Vector2
---@field public sqrMagnitude number
---@field public up UnityEngine.Vector2
---@field public zero UnityEngine.Vector2
---@class UnityEngine.Vector2 : System.ValueType
local m = {}

---@param from UnityEngine.Vector2
---@param to UnityEngine.Vector2
---@return number
function m.Angle(from, to)end

---@param vector UnityEngine.Vector2
---@param maxLength number
---@return UnityEngine.Vector2
function m.ClampMagnitude(vector, maxLength)end

---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
---@return number
function m.Distance(a, b)end

---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
---@return number
function m.Dot(lhs, rhs)end

---@overload fun(other: UnityEngine.Vector2):boolean
---@param other table
---@return boolean
function m:Equals(other)end

---@return number
function m:GetHashCode()end

---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
---@param t number
---@return UnityEngine.Vector2
function m.Lerp(a, b, t)end

---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
---@param t number
---@return UnityEngine.Vector2
function m.LerpUnclamped(a, b, t)end

---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.Max(lhs, rhs)end

---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.Min(lhs, rhs)end

---@param current UnityEngine.Vector2
---@param target UnityEngine.Vector2
---@param maxDistanceDelta number
---@return UnityEngine.Vector2
function m.MoveTowards(current, target, maxDistanceDelta)end

function m:Normalize()end

---@param inDirection UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.Perpendicular(inDirection)end

---@param inDirection UnityEngine.Vector2
---@param inNormal UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.Reflect(inDirection, inNormal)end

---@overload fun(scale: UnityEngine.Vector2)
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.Scale(a, b)end

---@param newX number
---@param newY number
function m:Set(newX, newY)end

---@param from UnityEngine.Vector2
---@param to UnityEngine.Vector2
---@return number
function m.SignedAngle(from, to)end

---@overload fun(current: UnityEngine.Vector2, target: UnityEngine.Vector2, currentVelocity: UnityEngine.Vector2, smoothTime: number):UnityEngine.Vector2
---@overload fun(current: UnityEngine.Vector2, target: UnityEngine.Vector2, currentVelocity: UnityEngine.Vector2, smoothTime: number, maxSpeed: number, deltaTime: number):UnityEngine.Vector2
---@param current UnityEngine.Vector2
---@param target UnityEngine.Vector2
---@param currentVelocity UnityEngine.Vector2
---@param smoothTime number
---@param maxSpeed number
---@return UnityEngine.Vector2
function m.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed)end

---@overload fun():number
---@param a UnityEngine.Vector2
---@return number
function m.SqrMagnitude(a)end

---@overload fun(format: string):string
---@return string
function m:ToString()end
UnityEngine.Vector2 = m
return m