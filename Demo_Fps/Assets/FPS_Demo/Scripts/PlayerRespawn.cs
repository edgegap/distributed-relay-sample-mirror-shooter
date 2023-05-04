using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;
using QuickStart;
using System.Collections;

public class PlayerRespawn : NetworkBehaviour
{
    [SerializeField] private List<GameObject> respawnPoints;
    [SerializeField] public Image fadeToBlack;

    public AudioClip ExplosionSfx;
    public GameObject ExplosionVfx;
    private void Start()
    {
        GameObject[] spawnPointsArray = GameObject.FindGameObjectsWithTag("respawnPoint");

        // Create a new List to hold our spawn points
        List<GameObject> spawnPointList = new();

        // Add all of the spawn points to our List
        foreach (GameObject spawnPoint in spawnPointsArray)
        {
            spawnPointList.Add(spawnPoint);
        }

        respawnPoints = spawnPointList;
    }
    [ClientRpc]
    public void RpcOnPlayerDeath()
    {
        gameObject.GetComponent<PlayerScript>().isDead = true;
        fadeToBlack.gameObject.SetActive(true);
        fadeToBlack.gameObject.GetComponent<Animator>().SetBool("IsDead", true);
        CmdExplosion();
        StartCoroutine(RespawnCoroutine());
    }
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(3f);
        CmdRespawn();
    }
    [Command]
    void CmdRespawn()
    {
        RpcRespawn();
    }

    [Command]
    void CmdExplosion()
    {
        RpcExplosion();
    }
    [ClientRpc]
    void RpcExplosion()
    {
        print("j'entend ceci");
        gameObject.GetComponent<AudioSource>().PlayOneShot(ExplosionSfx, 0.15f);
        Instantiate(ExplosionVfx,new Vector3(transform.position.x, 2f, transform.position.z),Quaternion.identity);
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        gameObject.GetComponent<PlayerScript>().isDead = false;
        if (isLocalPlayer)
        {
            gameObject.GetComponent<PlayerScript>().isDead = false;
            gameObject.GetComponent<PlayerScript>().ResetPlayer();
            fadeToBlack.gameObject.GetComponent<Animator>().SetBool("IsDead", false);
            fadeToBlack.gameObject.SetActive(false);

            // Move the player to the respawn point
            int random = Random.Range(0, respawnPoints.Count);
            transform.position = respawnPoints[random].transform.position;
            transform.rotation = respawnPoints[random].transform.rotation;

            // Reset the player's health to default value
            gameObject.GetComponent<PlayerHealth>().CmdResetHealth();
        }
    }

}//Cree un fonction qui reinitialise tout lorsqu'il respawn
