--- fields
---@field public kEpsilon number
---@field public kEpsilonNormalSqrt number
---@field public x number
---@field public y number
---@field public z number
--- properties
---@field public Item number
---@field public back UnityEngine.Vector3
---@field public down UnityEngine.Vector3
---@field public forward UnityEngine.Vector3
---@field public fwd UnityEngine.Vector3
---@field public left UnityEngine.Vector3
---@field public magnitude number
---@field public negativeInfinity UnityEngine.Vector3
---@field public normalized UnityEngine.Vector3
---@field public one UnityEngine.Vector3
---@field public positiveInfinity UnityEngine.Vector3
---@field public right UnityEngine.Vector3
---@field public sqrMagnitude number
---@field public up UnityEngine.Vector3
---@field public zero UnityEngine.Vector3
---@class UnityEngine.Vector3 : System.ValueType
local m = {}

---@param from UnityEngine.Vector3
---@param to UnityEngine.Vector3
---@return number
function m.Angle(from, to)end

---@param from UnityEngine.Vector3
---@param to UnityEngine.Vector3
---@return number
function m.AngleBetween(from, to)end

---@param vector UnityEngine.Vector3
---@param maxLength number
---@return UnityEngine.Vector3
function m.ClampMagnitude(vector, maxLength)end

---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Cross(lhs, rhs)end

---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@return number
function m.Distance(a, b)end

---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
---@return number
function m.Dot(lhs, rhs)end

---@overload fun(other: UnityEngine.Vector3):boolean
---@param other table
---@return boolean
function m:Equals(other)end

---@param excludeThis UnityEngine.Vector3
---@param fromThat UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Exclude(excludeThis, fromThat)end

---@return number
function m:GetHashCode()end

---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
---@return UnityEngine.Vector3
function m.Lerp(a, b, t)end

---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
---@return UnityEngine.Vector3
function m.LerpUnclamped(a, b, t)end

---@param vector UnityEngine.Vector3
---@return number
function m.Magnitude(vector)end

---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Max(lhs, rhs)end

---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Min(lhs, rhs)end

---@param current UnityEngine.Vector3
---@param target UnityEngine.Vector3
---@param maxDistanceDelta number
---@return UnityEngine.Vector3
function m.MoveTowards(current, target, maxDistanceDelta)end

---@overload fun()
---@param value UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Normalize(value)end

---@overload fun(normal: UnityEngine.Vector3, tangent: UnityEngine.Vector3, binormal: UnityEngine.Vector3)
---@param normal UnityEngine.Vector3
---@param tangent UnityEngine.Vector3
function m.OrthoNormalize(normal, tangent)end

---@param vector UnityEngine.Vector3
---@param onNormal UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Project(vector, onNormal)end

---@param vector UnityEngine.Vector3
---@param planeNormal UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.ProjectOnPlane(vector, planeNormal)end

---@param inDirection UnityEngine.Vector3
---@param inNormal UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Reflect(inDirection, inNormal)end

---@param current UnityEngine.Vector3
---@param target UnityEngine.Vector3
---@param maxRadiansDelta number
---@param maxMagnitudeDelta number
---@return UnityEngine.Vector3
function m.RotateTowards(current, target, maxRadiansDelta, maxMagnitudeDelta)end

---@overload fun(scale: UnityEngine.Vector3)
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@return UnityEngine.Vector3
function m.Scale(a, b)end

---@param newX number
---@param newY number
---@param newZ number
function m:Set(newX, newY, newZ)end

---@param from UnityEngine.Vector3
---@param to UnityEngine.Vector3
---@param axis UnityEngine.Vector3
---@return number
function m.SignedAngle(from, to, axis)end

---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
---@return UnityEngine.Vector3
function m.Slerp(a, b, t)end

---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
---@return UnityEngine.Vector3
function m.SlerpUnclamped(a, b, t)end

---@overload fun(current: UnityEngine.Vector3, target: UnityEngine.Vector3, currentVelocity: UnityEngine.Vector3, smoothTime: number):UnityEngine.Vector3
---@overload fun(current: UnityEngine.Vector3, target: UnityEngine.Vector3, currentVelocity: UnityEngine.Vector3, smoothTime: number, maxSpeed: number, deltaTime: number):UnityEngine.Vector3
---@param current UnityEngine.Vector3
---@param target UnityEngine.Vector3
---@param currentVelocity UnityEngine.Vector3
---@param smoothTime number
---@param maxSpeed number
---@return UnityEngine.Vector3
function m.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed)end

---@param vector UnityEngine.Vector3
---@return number
function m.SqrMagnitude(vector)end

---@overload fun(format: string):string
---@return string
function m:ToString()end
UnityEngine.Vector3 = m
return m