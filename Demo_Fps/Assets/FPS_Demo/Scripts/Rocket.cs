using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Rocket : NetworkBehaviour
{
    public GameObject hitEffectPrefab;
    public AudioClip hitSound;
    public GameObject player;
    public int bulletDmg;
    public int maxHit = 25;
    public float radius = 5.0F;
    public float power = 10.0F;
    public LayerMask hitLayer;
    public LayerMask blockExplosionLayer;
    public int maxDmg = 50;
    public int minDmg = 10;
    bool hasHit;
    private Collider[] hitsColl;
    private void Awake()
    {
        hitsColl = new Collider[maxHit];
    }

        private void OnTriggerEnter(Collider other)
        {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        Destroy(hitEffect, 4f);
        int hits = Physics.OverlapCapsuleNonAlloc(transform.position,transform.position, radius,hitsColl ,hitLayer);
            for(int i = 0; i < hits ;i++)
            {
                
                if(hitsColl[i].TryGetComponent<Rigidbody>(out Rigidbody rb) && !hasHit)
                {
                    float distance = Vector3.Distance(transform.position, hitsColl[i].transform.position);
                    hasHit = true;
                    if(rb.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth player)){
                        
                        player.TakeDamage(Mathf.FloorToInt(Mathf.Lerp(maxDmg,minDmg,distance / radius)));
                    }
                    if(!Physics.Raycast(transform.position,(hitsColl[i].transform.position - transform.position).normalized, distance,blockExplosionLayer.value))
                    {
                        rb.AddExplosionForce(power,transform.position,radius,10f);
                        
                        
                    }
                    
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
    void update(){
        transform.Rotate(0,0,90 * Time.deltaTime);
    }
}