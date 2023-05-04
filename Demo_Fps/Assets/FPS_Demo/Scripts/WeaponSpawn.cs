using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponSpawn : NetworkBehaviour
{
    public GameObject[] weapons;
    [SerializeField] private uint Delay;

    private void Start()
    {
        Invoke("SpawnWeapon", 0.1f);
    }

    private void SpawnWeapon()
    {
        if (transform.childCount == 0)
        {
            gameObject.GetComponentInParent<ParticleSystem>().enableEmission = true;
            GameObject clone = Instantiate(weapons[Random.Range(0, weapons.Length)], transform.position, transform.rotation);
            clone.transform.parent = transform;
            clone.name = clone.name.Replace("(Clone)", "");
        }
        else
        {
            return;
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdWeaponTaken()
    {
        RpcWeaponTaken();
    }
    [ClientRpc]
    public void RpcWeaponTaken()
    {
        Invoke("SpawnWeapon", Delay);
        gameObject.GetComponentInParent<ParticleSystem>().enableEmission = false;
    }
}
