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
    private static string logDir = Application.dataPath + "/../Logs/";
#elif UNITY_STANDALONE_WIN
    private static string logDir = Application.dataPath + "/../Logs/";
#elif UNITY_STANDALONE_OSX
    private static string logDir = Application.dataPath + "/Logs/";
#else
    private static string logDir = Application.persistentDataPath + "/Logs/";
#endif

    static string TimeFormat = "yyyy-MM-dd-HH:mm:ss";
    static string FileTimeFormat = "yyyy-MM-dd-HH.mm.ss";
    //private static bool writeDown;
    static Logger instance;

    //一般日志是否输出到控制台
    public bool LogInfoToConsole = false;
    //是否输出日志堆栈
    public bool LogInfoTraceStack = true;
    //是否输出到文本文件
    public bool WriteToFile = true;

    private List<string> contentList = new List<string>();
    private List<string> writeList = new List<string>();
    private List<string> logers;
    private Action logged;
    private object locker = new object();
    private bool isRunning = false;
    private StreamWriter currSW;
#if UNITY_EDITOR
    private StreamWriter editorSW;
#endif

    public static Logger GetInstance()
    {
        if (instance == null)
            instance = new Logger();
        return instance;
    }
        /// <summary>
        /// Application.dataPath and Application.persistentDataPath can only be accessed in Unity Thread.
        /// Do remember to call DebugKit in Unity thread first, not in the work thread you create.
        /// </summary>
    public Logger()
    {
        if (instance != null)
            throw new Exception("no more instance");
        if (!Directory.Exists(logDir))
            Directory.CreateDirectory(logDir);
        string currLogFilePath = Path.Combine(logDir, "game.log");
        if (File.Exists(currLogFilePath))
            File.Delete(currLogFilePath);// fileStream = new FileStream(logFilePath, FileMode.Append);
        currSW = new StreamWriter(new FileStream(currLogFilePath, FileMode.Create), Encoding.UTF8);
#if UNITY_EDITOR
        string edirlogDir = logDir + "Editor/";
        if (!Directory.Exists(edirlogDir))
            Directory.CreateDirectory(edirlogDir);
        string editlogFilePath = Path.Combine(logDir + "Editor/", DateTime.Now.ToString(FileTimeFormat) + ".log");
        editorSW = new StreamWriter(new FileStream(editlogFilePath, FileMode.Create), Encoding.UTF8);
#endif
        //Application.logMessageReceived += OnApplicationLogMessageReceived;
    }

    public void Start(bool write = true)
    {
        logers = new List<string>() { "Log", "Error", "Exception", "TODO" };

        isRunning = true;
        Thread thread = new Thread(ASyncWrite);
        thread.Start();
    }
    public void Dispose()
    {
        try
        {
            isRunning = false;
            //Application.logMessageReceived -= OnApplicationLogMessageReceived;
            currSW.Close();
#if UNITY_EDITOR
            editorSW.Close();
#endif
            instance = null;
        }catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            isRunning = false;
            instance = null;
        }
    }
    public void ASyncWrite()
    {
        while (isRunning && WriteToFile)
        {
            lock (locker)
            {
                if (contentList.Count == 0)
                    continue;
                writeList.AddRange(contentList);
                contentList.Clear();
            }
            for (int i = 0; i < writeList.Count; i++)
            {
                currSW.WriteLine(writeList[i]);
#if UNITY_EDITOR
                editorSW.WriteLine(writeList[i]);
#endif
            }
            if (writeList.Count > 0)
            {
                currSW.Flush();
#if UNITY_EDITOR
                editorSW.Flush();
#endif
            }
            writeList.Clear();
        }
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
                //Application.logMessageReceived -= OnApplicationLogMessageReceived;
                Debug.LogError(logContent);
                //Application.logMessageReceived += OnApplicationLogMessageReceived;
                break;
        }
    }
    private string AddLog(string tag, string message)
    {
        string logContent = "";
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
                trace += "  " + mb.DeclaringType.FullName + ":" + mb.Name + "() (at " + mb.DeclaringType.FullName.Replace(".", "/") + ".cs: " + sfs[u].GetFileLineNumber() + ")\r\n";
            }
            logContent = string.Format("{0} [{1}] {2}\r\n{3}", time, tag, message, (showStack ? trace : ""));
        }
        else
        {
            logContent = string.Format("{0} [{1}] {2}", time, tag, message);
        }
        
        lock (locker)
        {
            contentList.Add(logContent);
        }
        return logContent;
    }
    public static void Log(string type,string format, params object[] args)
    {
        string logContent = instance.AddLog(type, string.Format(format, args));
        if (instance.LogInfoToConsole)
            Debug.Log(logContent);
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
        if (instance.LogInfoToConsole)
            Debug.Log(logContent);
    }

    public static void LogError(string format, params object[] args)
    {
        string logContent = instance.AddLog("ErrorLog", string.Format(format, args));
        if (instance.LogInfoToConsole)
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Debug.LogError(logContent);
    }

    private void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
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