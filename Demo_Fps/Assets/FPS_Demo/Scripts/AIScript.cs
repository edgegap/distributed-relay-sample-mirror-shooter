using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AIScript : NetworkBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.tag == "bulletBasic")
        {
            if(gameObject.transform.parent.name == "SpawnPoints")
            {
                GameObject reference = GameObject.FindGameObjectWithTag("ChallPoints");
                reference.GetComponent<StartChallenge>().IncrementScore();
            }
            print("destroyRobot");
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}
