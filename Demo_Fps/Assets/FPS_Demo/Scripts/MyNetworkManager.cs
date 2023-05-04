using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public GameObject HUD;
    public GameObject[] spawnablePrefabs;
    // Paste your API token here
    //private string apiToken = "de5ff38d-de97-4233-ac3a-7b670ce2be31";

    public override void OnStartServer()
    {
        Debug.Log("Server Started");
    }
    public override void OnStartClient()
    {
        foreach (GameObject prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
            
        }
    }
    public override void Start()
    {
        HUD.SetActive(true);
        //base.Start();
        // Set the API token in the NetworkManager singleton
        //NetworkManager.singleton.networkAddress = apiToken;
    }
    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);

        // Do something when the scene changes on the server
        Debug.Log("Server changed scene to: " + newSceneName);
    }

}

