using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using ECS;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Ins { get; private set; }

    public Logger logger { get; private set; }
    
    // Use this for initialization
    void Start ()
    {
        if (Ins == null)
            Ins = this;
        else
        {
            return;
            //throw new Exception("There is only one Client instance in this app");
        }
        
        DontDestroyOnLoad(this);
        // 初始变量赋值
        Application.targetFrameRate = 60;
        GlobalConsts.FrameTime = 1f / Application.targetFrameRate;
        Application.runInBackground = true;
        Debug.Log("Application TargetFrameRate:" + Application.targetFrameRate);
        Debug.Log("Application RunInBackground:" + Application.runInBackground);
        Debug.Log("Application InstallMode:" + Application.installMode);
        Debug.Log("Application Unity Version:" + Application.unityVersion);
        
        //开启日志
        logger = Logger.GetInstance();
        logger.Start();
        Logger.Info("Game Start");
        
        if (GlobalConsts.isRunningInMobileDevice || GlobalConsts.isResBundleMode)
        {
            if (!Directory.Exists("Assets/Res") || !Directory.Exists("Assets/Lua"))
            {
                Logger.LogError("There is no 'Assets/Res' or 'Assets/Lua' Directory in the project!");
                return;
            }
        }
        AppBootstrap.Start(this);
    }
   
    private void OnApplicationQuit()
    {
        if(logger != null)
            logger.Dispose();
    }
}
