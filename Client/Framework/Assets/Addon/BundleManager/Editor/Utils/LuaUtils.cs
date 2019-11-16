using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BM
{
    public class LuaUtils
    {
        
        public static bool EncodeLuaFile(string args, bool isWin, string luajitDir, string luajitExecuteFile)
        {
            string currDir  = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(luajitDir);
                System.Diagnostics.ProcessStartInfo info    = new System.Diagnostics.ProcessStartInfo();
                info.FileName                               = luajitExecuteFile;
                info.Arguments                              = args;
                info.WindowStyle                            = System.Diagnostics.ProcessWindowStyle.Hidden;
                info.UseShellExecute                        = isWin;
                info.ErrorDialog                            = true;
                System.Diagnostics.Process pro              = System.Diagnostics.Process.Start(info);
                pro.WaitForExit();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                Directory.SetCurrentDirectory(currDir);
                EditorUtility.ClearProgressBar();
                throw;
            }
            Directory.SetCurrentDirectory(currDir);
            return true;
        }
        /// <summary>
        /// 判断当前运行平台是 x86 还是 x86_x64
        /// </summary>
        public static bool isX64
        {
            get
            {
                if (System.IntPtr.Size == 4)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}