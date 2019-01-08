using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using LuaInterface;
using System.IO;
using UnityEngine;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/1/9 2:00:52</para>
/// </summary> 
/// 

namespace Framework
{
    public static class EmmyLuaApiExporter
    {
        [MenuItem("Lua/Export EmmyLuaApi", false, 14)]
        static void Gen()
        {
            string path = "./EmmyApi/";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            //UnityEngine
            GenAssembly("UnityEngine", path);
            //GenAssembly("UnityEngine.UI", path);
            GenCustom(path);
        }

        public static void GenAssembly(string name, string path)
        {
            List<string> excludeList;
            List<string> includeList;
            CustomExport.OnGetNoUseList(out excludeList);
            CustomExport.OnGetUseList(out includeList);
            Type[] types = Assembly.Load(name).GetTypes();
            foreach (Type t in types)
            {
                if (LuaCodeGen.filterType(t, excludeList, includeList))
                {
                    GenType(t, false, path);
                }
            }
        }

        public static void GenCustom(string path)
        {
            Type[] types = Assembly.Load("Assembly-CSharp-firstpass").GetTypes();
            foreach (Type t in types)
            {
                if (t.IsDefined(typeof(CustomLuaClassAttribute), false))
                {
                    GenType(t, true, path);
                }
            }

            types = Assembly.Load("Assembly-CSharp").GetTypes();
            foreach (Type t in types)
            {
                if (t.IsDefined(typeof(CustomLuaClassAttribute), false))
                {
                    GenType(t, true, path);
                }
            }
        }
        public static void GenType(Type t, bool custom, string path)
        {
            if (!CheckType(t, custom))
                return;
            //TODO System.MulticastDelegate
            var sb = new StringBuilder();
            if (!CheckType(t.BaseType, custom))
                sb.AppendFormat("---@class {0}\n", t.Name);
            else
                sb.AppendFormat("---@class {0} : {1}\n", t.Name, t.BaseType.Name);
            GenTypeField(t, sb);
            sb.AppendFormat("local {0}={{ }}\n", t.Name);

            GenTypeMehod(t, sb);

            sb.AppendFormat("{0}.{1} = {2}", t.Namespace, t.Name, t.Name);

            File.WriteAllText(path + t.FullName + ".lua", sb.ToString(), Encoding.UTF8);
        }

        static bool CheckType(Type t, bool custom)
        {
            if (t == null)
                return false;
            if (t == typeof(System.Object))
                return false;
            if (t.IsGenericTypeDefinition)
                return false;
            if (t.IsDefined(typeof(ObsoleteAttribute), false))
                return false;
            if (t == typeof(YieldInstruction))
                return false;
            if (t == typeof(Coroutine))
                return false;
            if (t.IsNested)
                return false;
            if (custom && !t.IsDefined(typeof(CustomLuaClassAttribute), false))
                return false;
            return true;
        }

        public static void GenTypeField(Type t, StringBuilder sb)
        {
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(DoNotToLuaAttribute), false))
                    continue;
                sb.AppendFormat("---@field public {0} {1}\n", field.Name, GetLuaType(field.FieldType));
            }
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var pro in properties)
            {
                if (pro.IsDefined(typeof(DoNotToLuaAttribute), false))
                    continue;
                sb.AppendFormat("---@field public {0} {1}\n", pro.Name, GetLuaType(pro.PropertyType));
            }
        }

        public static void GenTypeMehod(Type t, StringBuilder sb)
        {
            MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                if (method.IsGenericMethod)
                    continue;
                if (method.IsDefined(typeof(DoNotToLuaAttribute), false))
                    continue;
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                    continue;
                sb.AppendLine("---@public");
                var paramstr = new StringBuilder();
                foreach (var param in method.GetParameters())
                {
                    sb.AppendFormat("---@param {0} {1}\n", param.Name, GetLuaType(param.ParameterType));
                    if (paramstr.Length != 0)
                    {
                        paramstr.Append(", ");
                    }
                    paramstr.Append(param.Name);
                }
                sb.AppendFormat("---@return {0}\n", method.ReturnType == null ? "void" : GetLuaType(method.ReturnType));
                if (method.IsStatic)
                {
                    sb.AppendFormat("function {0}.{1}({2}) end\n", t.Name, method.Name, paramstr);
                }
                else
                {
                    sb.AppendFormat("function {0}:{1}({2}) end\n", t.Name, method.Name, paramstr);
                }
            }
        }

        static string GetLuaType(Type t)
        {
            if (t.IsEnum
                //|| t == typeof(ulong)
                //|| t == typeof(long)
                //|| t == typeof(int)
                //|| t == typeof(uint)
                //|| t == typeof(float)
                || t == typeof(double)
                //|| t == typeof(byte)
                //|| t == typeof(ushort)
                //|| t == typeof(short)
                )
                return "number";
            if (t == typeof(bool))
                return "bool";
            if (t == typeof(string))
                return "string";
            if (t == typeof(void))
                return "void";

            return t.Name;
        }
    }
}


