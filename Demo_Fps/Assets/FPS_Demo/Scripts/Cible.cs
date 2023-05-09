using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Cible : NetworkBehaviour
{
    public GameObject ciblePrefab;
    GameObject reference;
    GameObject referenceAimSpeed;


    private void Start()
    {
        reference = GameObject.FindGameObjectWithTag("AimPoints");
        referenceAimSpeed = GameObject.FindGameObjectWithTag("AimSpeed");
        if (gameObject.transform.name == "SpawnCibleReverse(Clone)")
        {
            StartCoroutine(DestroyCible());
        }
    }

    IEnumerator DestroyCible()
    {
        yield return new WaitForSeconds(2f);
        CmdRespawnTarget();
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "bulletBasic")
        {
            if (gameObject.transform.name == "SpawnCible(Clone)" && reference.GetComponent<StartChallenge>().AimPointsAsStart)
            {
                CmdCallOnDestroyAimPoints();
            }
            if (gameObject.transform.name == "SpawnCibleReverse(Clone)" && referenceAimSpeed.GetComponent<StartChallenge>().AimSpeedAsStart)
            {
                CmdCallOnDestroyAimSpeed();
            }
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
    [Command(requiresAuthority = false)]
    void CmdCallOnDestroyAimSpeed()
    {
        referenceAimSpeed.GetComponent<StartChallenge>().IncrementScoreAimSpeed();
        referenceAimSpeed.GetComponent<StartChallenge>().CmdSpawnNewTargetAimSpeed();
    }
    [Command(requiresAuthority = false)]
    void CmdCallOnDestroyAimPoints()
    {
        reference.GetComponent<StartChallenge>().IncrementScoreAimPoints();
        reference.GetComponent<StartChallenge>().CmdSpawnNewTarget();
    }
    [Command(requiresAuthority = false)]
    void CmdRespawnTarget()
    {
        referenceAimSpeed.GetComponent<StartChallenge>().CmdSpawnNewTargetAimSpeed();
    }
}
