using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollider : MonoBehaviour
{
    public bool isInCollider = false;
    Collider other;

    void OnTriggerEnter( Collider other )
 {
     isInCollider = true;
     this.other = other;
 }
    void Update()
 {
     if ( isInCollider && !other )
     {
        isInCollider = false;
     }
 }

}
