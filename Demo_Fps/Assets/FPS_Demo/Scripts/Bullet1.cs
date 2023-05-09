using Mirror;
using UnityEngine;

public class Bullet1 : NetworkBehaviour
{
    public GameObject hitEffectPrefab;
    public AudioClip hitSound;
    public GameObject player;
    public int bulletDmg;
    bool asHit;


    private void OnTriggerEnter(Collider other)
    {
        
        // Check if the collision was with a player
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null && !asHit)
        {
            asHit = true;
            // Apply damage to the player
            playerHealth.TakeDamage(bulletDmg);

            // Spawn the hit effect on all clients
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
            Destroy(hitEffect, 4f);
           

            // Play the hit sound effect on the client that shot the bullet
            if (isOwned)
            {
                CmdPlayHitSound();
            }
        }
        switch (other.gameObject.tag)
        {
            case "ChallSpeed":
                other.gameObject.GetComponent<StartChallenge>().StartChallengeSpeed();
                break;
            case "ChallPoints":
                other.gameObject.GetComponent<StartChallenge>().CmdStartChallengePoints();
                break;
            case "AimSpeed":
                other.gameObject.GetComponent<StartChallenge>().CmdStartAimSpeed();
                //other.gameObject.GetComponent<StartChallenge>().CmdTest();
                break;
            case "AimPoints":
                other.gameObject.GetComponent<StartChallenge>().CmdStartAimPoints();
                //other.gameObject.GetComponent<StartChallenge>().CmdTest();
                break;
        }

        // Destroy the bullet on all clients
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdPlayHitSound()
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
    }
}
