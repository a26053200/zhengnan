using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clients : MonoBehaviour {

    public JsonClient jsonClient { get; private set; }
	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
        jsonClient = gameObject.AddComponent<JsonClient>();

        //test 
        jsonClient.connect("127.0.0.1", 8080);

        StartCoroutine(testSend());
    }

    private IEnumerator testSend()
    {
        yield return new WaitForSeconds(1);
        JsonData json = new JsonData();
        json["server"] = "LoginServer";
        json["action"] = "login";
        json["username"] = "123456";
        json["password"] = "123";
        jsonClient.sendJson(json);
    }


    // Update is called once per frame
    void Update () {
		
	}
}
