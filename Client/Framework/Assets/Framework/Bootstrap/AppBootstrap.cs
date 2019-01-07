using System;
using UnityEngine;
using System.Collections.Generic;
using Framework;
/// <summary>
/// <para>App 启动引导</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 0:27:05</para>
/// </summary> 
public static class AppBootstrap
{
    static Client s_client = null;
    public static void Start(Client client)
    {
        s_client = client;

        // Init and add managers
        GameManager.AddManager(client.gameObject.AddComponent<GameManager>());
        GameManager.AddManager(client.gameObject.AddComponent<AssetsManager>());
        GameManager.AddManager(client.gameObject.AddComponent<SceneManager>());
        GameManager.AddManager(client.gameObject.AddComponent<MonoBehaviourManager>());
        GameManager.AddManager(client.gameObject.AddComponent<NetworkManager>());
        // Other
        client.gameObject.AddComponent<LuaBootstrap>();
    }
}

