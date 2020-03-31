using System.IO;
using Framework;
/// <summary>
/// <para>App 启动引导</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 0:27:05</para>
/// </summary> 
public static class AppBootstrap
{
    public static void Start(Client client)
    {
        // Init and add managers
        GameManager.AddManager(client.gameObject.AddComponent<GameManager>());
        GameManager.AddManager(client.gameObject.AddComponent<ResLoader>());
        GameManager.AddManager(client.gameObject.AddComponent<AssetsManager>());
        GameManager.AddManager(client.gameObject.AddComponent<SceneManager>());
        GameManager.AddManager(client.gameObject.AddComponent<MonoBehaviourManager>());
        GameManager.AddManager(client.gameObject.AddComponent<NetworkManager>());
       
    }
}

