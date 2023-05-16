using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RocketLauncher : NetworkBehaviour
{
    public List <GameObject> rockets;
    public void SubstractRocket(int bullet)
    {
        // CmdSubstractRocket(bullet);
        int num  = 7 - bullet;
        rockets[num].SetActive(false);
        
    }
}
