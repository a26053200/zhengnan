//
// Class Introduce
// Author: zhengnan
// Create: 2018/6/11 17:53:50
// 

using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Logger
{
#if UNITY_EDITOR
    private static string path = Application.dataPath + "/../Log/";
#elif UNITY_STANDALONE_WIN
    private static string path = Application.dataPath + "/../Log/";
#elif UNITY_STANDALONE_OSX
    private static string path = Application.dataPath + "/Log/";
#else
    private static string path = Application.persistentDataPath + "/Log/";
#endif
    //一般日志是否输出到控制台
    public static bool LogInfoToConsole = false;
    //是否输出日志堆栈
    public static bool LogInfoTraceStack = true;

    static string TimeFormat = "yyyy-MM-dd-HH:mm:ss";
    //private static bool writeDown;
    private static List<string> logers;
    private static object locker = new object();
    private static Action logged;
    private static Logger instance;

    private List<string> contentList = new List<string>();
    private List<string> writeList = new List<string>();
    private bool isRunning = false;
    private FileStream fileStream;
    private StreamWriter sw;
    private string logFilePath;
    private List<string> ContentList
    {
        get
        {
            List<string> tempList = null;
            lock (locker)
            {
                tempList = new List<string>(tempList);
            }
            return tempList;
        }
    }

    /// <summary>
    /// Application.dataPath and Application.persistentDataPath can only be accessed in Unity Thread.
    /// Do remember to call DebugKit in Unity thread first, not in the work thread you create.
    /// </summary>
    public Logger()
    {
        instance = this;
        logFilePath = Path.Combine(path,"game.log");
        if (File.Exists(logFilePath))
            File.Delete(logFilePath);
        fileStream = new FileStream(logFilePath,FileMode.Create);
        sw = new StreamWriter(fileStream, Encoding.UTF8);
        Application.logMessageReceived += OnApplicationLogMessageReceived;
    }

    public void Start(bool write = true)
    {
        logers = new List<string>() { "Log", "Error", "Exception", "TODO" };

        isRunning = true;
        Thread thread = new Thread(ASyncWrite);
        thread.Start();
    }

    public void ASyncWrite()
    {
        Log();
    }
    /// <summary>
    /// 添加日志输出
    /// </summary>
    public void AddTagger(string tagger)
    {
        lock (locker)
        {
            if (!logers.Contains(tagger))
            {
                logers.Add(tagger);
            }
        }
    }

    /// <summary>
    /// 移除日志输出
    /// </summary>
    public void RemoveTagger(string tagger)
    {
        lock (locker)
        {
            if (logers.Contains(tagger))
            {
                logers.Remove(tagger);
            }
        }
    }

    public void RegisterLogEvent(Action callback)
    {
        lock (locker)
        {
            logged += callback;
        }
    }

    public void UnregisterLogEvent(Action callback)
    {
        lock (locker)
        {
            logged -= callback;
        }
    }

    /// <summary>
    /// 移除所有日志输出
    /// </summary>
    public void RemoveAll()
    {
        lock (locker)
        {
            logers.Clear();
        }
    }

    public List<string> TakeContentList()
    {
        List<string> tempList = null;
        lock (locker)
        {
            tempList = contentList;
            contentList = new List<string>();
        }
        return tempList;
    }

    private void AddLog(LogType tag, string message)
    {
        string logContent = AddLog(tag.ToString(), message);
        switch(tag)
        {
            case LogType.Log:
                if (LogInfoToConsole)
                    Debug.Log(logContent);
                break;
            case LogType.Warning:
                if (LogInfoToConsole)
                    Debug.LogWarning(logContent);
                break;
            case LogType.Error:
            case LogType.Exception:
                if (LogInfoToConsole)
                    Debug.LogError(logContent);
                break;
        }
    }
    private string AddLog(string tag, string message)
    {
        string time = DateTime.Now.ToString(TimeFormat);
        string trace = "";
        bool showStack = LogInfoTraceStack || tag == LogType.Error.ToString() || tag == LogType.Exception.ToString();
        if (showStack)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            for (int u = 2; u < sfs.Length; ++u)
            {
                System.Reflection.MethodBase mb = sfs[u].GetMethod();
                trace += "\t" + mb.DeclaringType.FullName + ":" + mb.Name + "() (at " + mb.DeclaringType.FullName.Replace(".", "/") + ".cs: " + sfs[u].GetFileLineNumber() + ")\r\n";
            }
        }
        string logContent = string.Format("{0} {1} {2}\r\n{3}", time, tag, message, (showStack ? trace : ""));
        contentList.Add(logContent);
        return logContent;
    }
    private void Log(string tag, string message)
    {
        while (isRunning)
        {
            lock (locker)
            {
                if (contentList.Count == 0)
                    Monitor.Wait(locker);
                writeList.AddRange(contentList);
                contentList.Clear();
            }
            for(int i = 0;i<contentList.Count;i++)
            {
                sw.WriteLine(contentList[i]);
            }
        }
        lock (locker)
        {
            //#if UNITY_EDITOR
            if (logers.Contains(tag))
            {
                string time = DateTime.Now.ToString(TimeFormat);
                string trace = "";
                bool showStack = LogInfoTraceStack || tag == LogType.Error.ToString() || tag == LogType.Exception.ToString();
                if (showStack)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                    System.Diagnostics.StackFrame[] sfs = st.GetFrames();
                    for (int u = 2; u < sfs.Length; ++u)
                    {
                        System.Reflection.MethodBase mb = sfs[u].GetMethod();
                        trace += "\t" + mb.DeclaringType.FullName + ":" + mb.Name + "() (at " + mb.DeclaringType.FullName.Replace(".", "/") + ".cs: " + sfs[u].GetFileLineNumber() + ")\r\n";
                    }
                }
                string logContent = time + "#(" + tag + ") => " + message + "\r\n" + (showStack ? trace : "");
                contentList.Add(logContent);
                if (LogInfoToConsole)
                    Debug.Log(logContent);
                if (logged != null)
                    logged();
                //Write to file
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                try
                {
                    using (StreamWriter w = new StreamWriter(path + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".txt", true, Encoding.UTF8))
                    {
                        w.WriteLine(logContent);
                        w.Flush();
                        w.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Write log file exception, message = " + ex.Message);
                }
            }
            //#endif
        }
    }
    public static void Log(string type,string format, params object[] args)
    {
        string logContent = instance.AddLog(type, string.Format(format, args));
        if (LogInfoToConsole)
            Debug.Log(logContent);
        //Log(type, string.Format(format, args));
    }
    public static void Info(string format, params object[] args)
    {
        instance.AddLog(LogType.Log, string.Format(format, args));
    }

    public static void Error(string format, params object[] args)
    {
        instance.AddLog(LogType.Error, string.Format(format, args));
    }

    public static void Exception(string format, params object[] args)
    {
        instance.AddLog(LogType.Exception, string.Format(format, args));
    }

    public static void Warning(string format, params object[] args)
    {
        instance.AddLog(LogType.Warning, string.Format(format, args));
    }

    public static void Todo(string format, params object[] args)
    {
        string logContent = instance.AddLog("TODO", string.Format(format, args));
        if (LogInfoToConsole)
            Debug.Log(logContent);
    }

    private static void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        lock (locker)
        {
            if (type == LogType.Exception)
            {
                instance.AddLog(LogType.Exception, condition + "\r\n" + stackTrace);
            }
            else if (type == LogType.Error)
            {
                instance.AddLog(LogType.Error, condition + "\r\n" + stackTrace);
            }
        }
    }
}