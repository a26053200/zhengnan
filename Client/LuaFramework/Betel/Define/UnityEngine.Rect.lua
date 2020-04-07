--- fields
--- properties
---@field public bottom number
---@field public center UnityEngine.Vector2
---@field public height number
---@field public left number
---@field public max UnityEngine.Vector2
---@field public min UnityEngine.Vector2
---@field public position UnityEngine.Vector2
---@field public right number
---@field public size UnityEngine.Vector2
---@field public top number
---@field public width number
---@field public x number
---@field public xMax number
---@field public xMin number
---@field public y number
---@field public yMax number
---@field public yMin number
---@field public zero UnityEngine.Rect
---@class UnityEngine.Rect : System.ValueType
local m = {}

---@overload fun(point: UnityEngine.Vector3):boolean
---@overload fun(point: UnityEngine.Vector3, allowInverse: boolean):boolean
---@param point UnityEngine.Vector2
---@return boolean
function m:Contains(point)end

---@overload fun(other: UnityEngine.Rect):boolean
---@param other table
---@return boolean
function m:Equals(other)end

---@return number
function m:GetHashCode()end

---@param xmin number
---@param ymin number
---@param xmax number
---@param ymax number
---@return UnityEngine.Rect
function m.MinMaxRect(xmin, ymin, xmax, ymax)end

---@param rectangle UnityEngine.Rect
---@param normalizedRectCoordinates UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.NormalizedToPoint(rectangle, normalizedRectCoordinates)end

---@overload fun(other: UnityEngine.Rect, allowInverse: boolean):boolean
---@param other UnityEngine.Rect
---@return boolean
function m:Overlaps(other)end

---@param rectangle UnityEngine.Rect
---@param point UnityEngine.Vector2
---@return UnityEngine.Vector2
function m.PointToNormalized(rectangle, point)end

---@param x number
---@param y number
---@param width number
---@param height number
function m:Set(x, y, width, height)end

---@overload fun(format: string):string
---@return string
function m:ToString()end
UnityEngine.Rect = m
return m