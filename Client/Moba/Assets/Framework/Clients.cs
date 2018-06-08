using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clients : MonoBehaviour
{
    public static Clients Ins { get; private set; }

    public JsonClient jsonClient { get; private set; }

    public HttpRequest httpRequest { get; private set; }

    private void Awake()
    {
        if (Ins == null)
            Ins = this;
        else
            throw new Exception("There is only one Client instance in this app");
    }
    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);
       
        httpRequest = gameObject.AddComponent<HttpRequest>();

        JsonData json = new JsonData();
        json["server"] = "AccountServer";
        json["action"] = "login_account";
        json["username"] = "123456";
        json["password"] = "123";

        StartCoroutine(testHttpLogin(json));
    }
    void Update()
    {

    }

    private IEnumerator testHttpLogin(JsonData json)
    {
        yield return new WaitForSeconds(1);
        httpRequest.SendMsg("http://127.0.0.1:8080", json, "OnHttpLogin");
    }

    void OnHttpLogin(JsonData json)
    {
        MyDebug.Log(json["aid"]);
        JsonData srvList = json["srvList"];
        JsonData list = srvList["list"];
        JsonData gameSrv = list[0];
        jsonClient = gameObject.AddComponent<JsonClient>();
        //连接游戏网关服务器
        jsonClient.connect((string)gameSrv["host"], (int)gameSrv["port"]);
        //MyDebug.Log(string.Format("正在连接服务器 {0}:{1}", gameSrv["host"], gameSrv["port"]));

        jsonClient.eventDispatcher.addEventListener(SocketEvent.SERVER_SOCKET_CONNECTED,delegate(EventObject evt)
        {
            //登陆游戏网关
            JsonData loginGate = new JsonData();
            loginGate["server"] = "GateServer";
            loginGate["action"] = "login_game_gateway";
            loginGate["aid"] = json["aid"];
            loginGate["token"] = json["token"];

            jsonClient.sendJson(loginGate);
        });
        
    }
}
