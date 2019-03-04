using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Ins { get; private set; }

    public JsonClient jsonClient { get; private set; }

    public HttpRequest httpRequest { get; private set; }

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
       
        httpRequest = gameObject.AddComponent<HttpRequest>();

        JsonData json = new JsonData();
        json["server"] = "AccountServer";
        json["action"] = "login_account";
        json["username"] = "123456";
        json["password"] = "123";
        //StartCoroutine(testHttpLogin(json));
        //开启日志
        logger = Logger.GetInstance();
        logger.Start();
        //Logger.Error("Hell world1");
        //Logger.Error("Hell world2");
        AppBootstrap.Start(this);
    }
    void Update()
    {

    }
    private void OnApplicationQuit()
    {
        if(logger != null)
            logger.Dispose();
    }
    
    private IEnumerator testHttpLogin(JsonData json)
    {
        yield return new WaitForSeconds(1);
        httpRequest.SendMsg("http://127.0.0.1:8080", json, "OnHttpLogin");
    }

    void OnHttpLogin(JsonData json)
    {
        MyDebug.Log(json["token"]);
        JsonData srvList = json["srvList"];
        JsonData list = srvList["list"];
        JsonData gameSrv = list[0];
        jsonClient = gameObject.AddComponent<JsonClient>();
        //连接游戏网关服务器
        jsonClient.connect((string)gameSrv["host"], (int)gameSrv["port"]);
        //MyDebug.Log(string.Format("正在连接服务器 {0}:{1}", gameSrv["host"], gameSrv["port"]));

        jsonClient.eventDispatcher.addEventListener(JsonSocketEvent.SERVER_SOCKET_CONNECTED,delegate(EventObj evt)
        {
            //登陆游戏网关
            JsonData loginGate = new JsonData();
            loginGate["server"] = "GameServer";
            loginGate["action"] = "login_game_server";
            loginGate["aid"] = json["aid"];
            loginGate["token"] = json["token"];

            jsonClient.sendJson(loginGate);
        });
        
    }
}
