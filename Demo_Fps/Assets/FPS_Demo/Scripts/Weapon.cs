using UnityEngine;

namespace QuickStart
{
    public class Weapon : MonoBehaviour
    {

        public float weaponSpeed = 15.0f;
        public float weaponLife = 3.0f;
        public float weaponCooldown = 1.0f;
        public int weaponAmmo = 15;
        public int weaponMaxAmmo = 15;
        public float baseSpray = 0.01f;
        public float maxSpray = 1f;

        public GameObject weaponBullet;
        public Transform weaponFirePosition;

        public AudioClip FireSfx;
    }
    
}