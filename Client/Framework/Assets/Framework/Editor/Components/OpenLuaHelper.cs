using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class OpenLuaHelper : Editor
    {
        private const string EXTERNAL_EDITOR_PATH_KEY = "mTv8";
        private const string LUA_PROJECT_ROOT_FOLDER_PATH_KEY = "obUd";

        //[UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        //static bool OnOpenAsset(int instanceID, int line)
        //{
        // string name = EditorUtility.InstanceIDToObject(instanceID).name;
        // Debug.Log("Open Asset Step1,asset name=>" + name);
        // return true;
        //}

        [UnityEditor.Callbacks.OnOpenAssetAttribute(2)]
        public static bool OnOpenAsset2(int instanceID, int line)
        {
            string logText = GetLogText();
            // get filePath
            Regex regex = new Regex(@"<filePath>.*<\/filePath>");
            Match match = regex.Match(logText);
            string filePath = match.Groups[0].Value.Trim();
            int length = filePath.Length - 10 - 12;
            filePath = filePath.Substring(10, length);
            filePath = filePath.Replace(".", "/");

            string luaFolderRoot = EditorUserSettings.GetConfigValue(LUA_PROJECT_ROOT_FOLDER_PATH_KEY);
            if (string.IsNullOrEmpty(luaFolderRoot))
            {
                SetLuaProjectRoot();
                luaFolderRoot = EditorUserSettings.GetConfigValue(LUA_PROJECT_ROOT_FOLDER_PATH_KEY);
            }

            filePath = luaFolderRoot.Trim() + "/" + filePath.Trim() + ".lua";

            // get line number
            Regex lineRegex = new Regex(@"<line>.*<\/line>");
            match = lineRegex.Match(logText);
            string luaLineString = match.Groups[0].Value;
            luaLineString.Trim();
            length = luaLineString.Length - 6 - 11;
            luaLineString = luaLineString.Substring(10, length);
            int luaLine = int.Parse(luaLineString.Trim());

            return OpenFileAtLineExternal(filePath, luaLine);
        }


        static bool OpenFileAtLineExternal(string fileName, int line)
        {
            string editorPath = EditorUserSettings.GetConfigValue(EXTERNAL_EDITOR_PATH_KEY);
            if (string.IsNullOrEmpty(editorPath) || !File.Exists(editorPath))
            {
                // 没有path就弹出面板设置
                SetExternalEditorPath();
            }

            OpenFileWith(fileName, line);
            return true;
        }

        static void OpenFileWith(string fileName, int line)
        {
            string editorPath = EditorUserSettings.GetConfigValue(EXTERNAL_EDITOR_PATH_KEY);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = editorPath;
            proc.StartInfo.Arguments = string.Format("{0}:{1}:0", fileName, line);
            proc.Start();
        }

        [MenuItem("Tools/Lua/SetExternalEditorPath",false,49)]
        static void SetExternalEditorPath()
        {
            string path = EditorUserSettings.GetConfigValue(EXTERNAL_EDITOR_PATH_KEY);
            path = EditorUtility.OpenFilePanel(
                "Select Editor",
                path,
                "exe");

            if (path != "")
            {
                EditorUserSettings.SetConfigValue(EXTERNAL_EDITOR_PATH_KEY, path);
                Debug.Log("Set Editor Path: " + path);
            }
        }

        [MenuItem("Tools/Lua/SetLuaProjectRoot",false,49)]
        static void SetLuaProjectRoot()
        {
            string path = EditorUserSettings.GetConfigValue(LUA_PROJECT_ROOT_FOLDER_PATH_KEY);
            path = EditorUtility.OpenFolderPanel(
                "Select Editor",
                path,
                "");

            if (path != "")
            {
                EditorUserSettings.SetConfigValue(LUA_PROJECT_ROOT_FOLDER_PATH_KEY, path);
                Debug.Log("Set Editor Path: " + path);
            }
        }

        static string GetLogText()
        {
            // 找到UnityEditor.EditorWindow的assembly
            var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
            if (assembly_unity_editor == null) return null;

            // 找到类UnityEditor.ConsoleWindow
            var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
            if (type_console_window == null) return null;
            // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
            var field_console_window = type_console_window.GetField("ms_ConsoleWindow",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field_console_window == null) return null;
            // 获取ms_ConsoleWindow的值
            var instance_console_window = field_console_window.GetValue(null);
            if (instance_console_window == null) return null;

            // 如果console窗口时焦点窗口的话，获取stacktrace
            if ((object) UnityEditor.EditorWindow.focusedWindow == instance_console_window)
            {
                // 通过assembly获取类ListViewState
                var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
                if (type_list_view_state == null) return null;

                // 找到类UnityEditor.ConsoleWindow中的成员m_ListView
                var field_list_view = type_console_window.GetField("m_ListView",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_list_view == null) return null;

                // 获取m_ListView的值
                var value_list_view = field_list_view.GetValue(instance_console_window);
                if (value_list_view == null) return null;

                // 下面是stacktrace中一些可能有用的数据、函数和使用方法，这里就不一一说明了，我们这里暂时还用不到
                /* var field_row = type_list_view_state.GetField("row", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public); if (field_row == null) return null; var field_total_rows = type_list_view_state.GetField("totalRows", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public); if (field_total_rows == null) return null; var type_log_entries = assembly_unity_editor.GetType("UnityEditorInternal.LogEntries"); if (type_log_entries == null) return null; var method_get_entry = type_log_entries.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public); if (method_get_entry == null) return null; var type_log_entry = assembly_unity_editor.GetType("UnityEditorInternal.LogEntry"); if (type_log_entry == null) return null; var field_instance_id = type_log_entry.GetField("instanceID", BindingFlags.Instance | BindingFlags.Public); if (field_instance_id == null) return null; var field_line = type_log_entry.GetField("line", BindingFlags.Instance | BindingFlags.Public); if (field_line == null) return null; var field_condition = type_log_entry.GetField("condition", BindingFlags.Instance | BindingFlags.Public); if (field_condition == null) return null; object instance_log_entry = Activator.CreateInstance(type_log_entry); int value_row = (int)field_row.GetValue(value_list_view); int value_total_rows = (int)field_total_rows.GetValue(value_list_view); int log_by_this_count = 0; for (int i = value_total_rows – 1; i > value_row; i–) { method_get_entry.Invoke(null, new object[] { i, instance_log_entry }); string value_condition = field_condition.GetValue(instance_log_entry) as string; if (value_condition.Contains("[SDebug]")) { log_by_this_count++; } } */

                // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
                var field_active_text = type_console_window.GetField("m_ActiveText",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_active_text == null) return null;

                // 获得m_ActiveText的值，就是我们需要的stacktrace
                string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
                return value_active_text;
            }

            return null;
        }
    }
}