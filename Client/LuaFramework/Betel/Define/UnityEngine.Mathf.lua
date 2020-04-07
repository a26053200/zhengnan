--- fields
---@field public Deg2Rad number
---@field public Epsilon number
---@field public Infinity number
---@field public NegativeInfinity number
---@field public PI number
---@field public Rad2Deg number
--- properties
---@class UnityEngine.Mathf : System.ValueType
local m = {}

---@overload fun(value: number):number
---@param f number
---@return number
function m.Abs(f)end

---@param f number
---@return number
function m.Acos(f)end

---@param a number
---@param b number
---@return boolean
function m.Approximately(a, b)end

---@param f number
---@return number
function m.Asin(f)end

---@param f number
---@return number
function m.Atan(f)end

---@param y number
---@param x number
---@return number
function m.Atan2(y, x)end

---@param f number
---@return number
function m.Ceil(f)end

---@param f number
---@return number
function m.CeilToInt(f)end

---@overload fun(value: number, min: number, max: number):number
---@param value number
---@param min number
---@param max number
---@return number
function m.Clamp(value, min, max)end

---@param value number
---@return number
function m.Clamp01(value)end

---@param value number
---@return number
function m.ClosestPowerOfTwo(value)end

---@param kelvin number
---@return UnityEngine.Color
function m.CorrelatedColorTemperatureToRGB(kelvin)end

---@param f number
---@return number
function m.Cos(f)end

---@param current number
---@param target number
---@return number
function m.DeltaAngle(current, target)end

---@param power number
---@return number
function m.Exp(power)end

---@param val number
---@return number
function m.FloatToHalf(val)end

---@param f number
---@return number
function m.Floor(f)end

---@param f number
---@return number
function m.FloorToInt(f)end

---@param value number
---@param absmax number
---@param gamma number
---@return number
function m.Gamma(value, absmax, gamma)end

---@param value number
---@return number
function m.GammaToLinearSpace(value)end

---@param val number
---@return number
function m.HalfToFloat(val)end

---@param a number
---@param b number
---@param value number
---@return number
function m.InverseLerp(a, b, value)end

---@param value number
---@return boolean
function m.IsPowerOfTwo(value)end

---@param a number
---@param b number
---@param t number
---@return number
function m.Lerp(a, b, t)end

---@param a number
---@param b number
---@param t number
---@return number
function m.LerpAngle(a, b, t)end

---@param a number
---@param b number
---@param t number
---@return number
function m.LerpUnclamped(a, b, t)end

---@param value number
---@return number
function m.LinearToGammaSpace(value)end

---@overload fun(f: number):number
---@param f number
---@param p number
---@return number
function m.Log(f, p)end

---@param f number
---@return number
function m.Log10(f)end

---@overload fun(values: number):number
---@overload fun(a: number, b: number):number
---@overload fun(values: number):number
---@param a number
---@param b number
---@return number
function m.Max(a, b)end

---@overload fun(values: number):number
---@overload fun(a: number, b: number):number
---@overload fun(values: number):number
---@param a number
---@param b number
---@return number
function m.Min(a, b)end

---@param current number
---@param target number
---@param maxDelta number
---@return number
function m.MoveTowards(current, target, maxDelta)end

---@param current number
---@param target number
---@param maxDelta number
---@return number
function m.MoveTowardsAngle(current, target, maxDelta)end

---@param value number
---@return number
function m.NextPowerOfTwo(value)end

---@param x number
---@param y number
---@return number
function m.PerlinNoise(x, y)end

---@param t number
---@param length number
---@return number
function m.PingPong(t, length)end

---@param f number
---@param p number
---@return number
function m.Pow(f, p)end

---@param t number
---@param length number
---@return number
function m.Repeat(t, length)end

---@param f number
---@return number
function m.Round(f)end

---@param f number
---@return number
function m.RoundToInt(f)end

---@param f number
---@return number
function m.Sign(f)end

---@param f number
---@return number
function m.Sin(f)end

---@overload fun(current: number, target: number, currentVelocity: number, smoothTime: number):number
---@overload fun(current: number, target: number, currentVelocity: number, smoothTime: number, maxSpeed: number, deltaTime: number):number
---@param current number
---@param target number
---@param currentVelocity number
---@param smoothTime number
---@param maxSpeed number
---@return number
function m.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed)end

---@overload fun(current: number, target: number, currentVelocity: number, smoothTime: number):number
---@overload fun(current: number, target: number, currentVelocity: number, smoothTime: number, maxSpeed: number, deltaTime: number):number
---@param current number
---@param target number
---@param currentVelocity number
---@param smoothTime number
---@param maxSpeed number
---@return number
function m.SmoothDampAngle(current, target, currentVelocity, smoothTime, maxSpeed)end

---@param from number
---@param to number
---@param t number
---@return number
function m.SmoothStep(from, to, t)end

---@param f number
---@return number
function m.Sqrt(f)end

---@param f number
---@return number
function m.Tan(f)end
UnityEngine.Mathf = m
return m