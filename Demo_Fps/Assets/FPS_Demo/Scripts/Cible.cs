using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Cible : NetworkBehaviour
{
    public GameObject ciblePrefab;
    GameObject reference;
    private void Start()
    {
        reference = GameObject.FindGameObjectWithTag("AimPoints");
        if (gameObject.transform.name == "SpawnCibleReverse(Clone)")
        {
            reference = GameObject.FindGameObjectWithTag("AimSpeed");
            StartCoroutine(DestroyCible());
        }
    }

    IEnumerator DestroyCible()
    {
        yield return new WaitForSeconds(2f);
        reference.GetComponent<StartChallenge>().SpawnNewTargetAimSpeed();
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "bulletBasic")
        {
            if (gameObject.transform.name == "SpawnCible(Clone)" && reference.GetComponent<StartChallenge>().AimPointsAsStart)
            {
                reference.GetComponent<StartChallenge>().IncrementScoreAimPoints();
                reference.GetComponent<StartChallenge>().SpawnNewTarget();
            }
            if (gameObject.transform.name == "SpawnCibleReverse(Clone)" && reference.GetComponent<StartChallenge>().AimSpeedAsStart)
            {
                print("spawn new cible");
                reference.GetComponent<StartChallenge>().IncrementScoreAimSpeed();
                reference.GetComponent<StartChallenge>().SpawnNewTargetAimSpeed();
            }
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}
