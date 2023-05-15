using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        #region Variables
        [Header("Movement")]
        public float vitesseHorizontale;
        public float vitesseVerticale;
        public float vitesseDeplacement;
        float rotationH;
        float rotationV;
        float vDeplacement;
        float hDeplacement;
        [Header("Component")]
        [SerializeField] private Animator animator;
        Rigidbody rb;
        public Slider slider;
        public TMP_Text playerNameText;
        public GameObject floatingInfo;
        [SerializeField] private Camera camera;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject leftArmPoint;
        [SerializeField] private GameObject rightArmPoint;
        

        private Material playerMaterialClone;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;
        public Munition munitionScript;
        private SceneScript sceneScript;
        private float nextFire = 0.0f;
        private float nextFire2 = 0.0f;
        public GameObject weapon1;
        public GameObject weapon2;
        [SerializeField] private Image replaceImg;
        bool onPad;
        bool canSwapWeapon1;
        bool canSwapWeapon2;
        bool menuOpen;
        bool isRefreshed = true;
        public bool isDead;
        Scene scene;
        #endregion
        void Awake()
        {
            //allow all players to run this
            sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;

        }
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            rb = GetComponent<Rigidbody>();
            munitionScript.UIAmmo1(weapon1.GetComponent<Weapon>().weaponAmmo);
            munitionScript.UIAmmo2(weapon2.GetComponent<Weapon>().weaponAmmo);
            scene = SceneManager.GetActiveScene();
            // Find all children of this game object
            GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("DesactivateOnPlay");


            // Disable mesh renderers on all children with the tag "ADesactiver"
            foreach (GameObject obj in objectsToDisable)
            {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                // Disable MeshRenderer of all children of the current object
                foreach (Transform child in obj.transform)
                {
                    MeshRenderer childRenderer = child.gameObject.GetComponent<MeshRenderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.enabled = false;
                    }
                }
            }
        }
        ////send message to other players
        //[Command]
        //public void CmdSendPlayerMessage()
        //{
        //    if (sceneScript)
        //        sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
        //}

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        public override void OnStartLocalPlayer()
        {
            sceneScript.playerScript = this;

            camera.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);
            //place the camera to the good place
            //Camera.main.transform.SetParent(transform);
            //Camera.main.transform.localPosition = new Vector3(0, 1.6f, 0);

            //sets a "Random" name and color to the new player

            string name = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);

            
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            playerColor = _col;
            //sceneScript.statusText = $"{playerName} joined.";
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                // make non-local players run this
                return;
            }
            if (!isDead) 
            {

                if (!menuOpen)
                {
                    rotationH = Input.GetAxis("Mouse X") * vitesseHorizontale;
                    transform.Rotate(0, rotationH, 0);

                    rotationV += Input.GetAxis("Mouse Y") * vitesseVerticale;
                    rotationV = Mathf.Clamp(rotationV, -45, 45);

                    camera.transform.localEulerAngles = new Vector3(-rotationV, 0, 0);
                    vDeplacement = Input.GetAxis("Vertical") * vitesseDeplacement;
                    hDeplacement = Input.GetAxis("Horizontal") * vitesseDeplacement;
                    animator.SetFloat("moveX", Input.GetAxis("Horizontal"));
                    animator.SetFloat("moveY", Input.GetAxis("Vertical"));
                    GetComponent<Rigidbody>().velocity = transform.forward * vDeplacement + transform.right * hDeplacement + new Vector3(0, rb.velocity.y, 0);

                    //handle firing
                    WeaponsFiring();
                
                    //handle weapon swaping
                    if (Input.GetKeyDown(KeyCode.Alpha1) && onPad && isRefreshed)
                    {
                        isRefreshed = false;
                        Invoke("Refresh", 1f);
                        Vector3 startPoint = transform.position;
                        Vector3 endPoint = startPoint + transform.forward * 2f; // Adjust the distance as needed
                        float radius = 1f; // Adjust the radius as needed
                        
                        Collider[] colliders = new Collider[5]; // Adjust the size as needed
                        int numColliders = Physics.OverlapCapsuleNonAlloc(startPoint, endPoint, radius, colliders);

                        for (int i = 0; i < numColliders; i++)
                        {
                            if (colliders[i].gameObject.transform.tag == "WeaponPad" && colliders[i].gameObject.transform.childCount > 0 )
                            {
                                canSwapWeapon1 = true;
                                break;
                            }
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2) && onPad && isRefreshed)
                    {
                        isRefreshed = false;
                        Invoke("Refresh", 1f);
                        Vector3 startPoint = transform.position;
                        Vector3 endPoint = startPoint + transform.forward * 2f; // Adjust the distance as needed
                        float radius = 1f; // Adjust the radius as needed
                        
                        Collider[] colliders = new Collider[5]; // Adjust the size as needed
                        int numColliders = Physics.OverlapCapsuleNonAlloc(startPoint, endPoint, radius, colliders);

                        for (int i = 0; i < numColliders; i++)
                        {
                            if (colliders[i].gameObject.transform.tag == "WeaponPad" && colliders[i].gameObject.transform.childCount > 0 )
                            {
                                canSwapWeapon2 = true;
                                break;
                            }
                        }
                        
                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape) && !menuOpen)
                {
                    menuOpen = !menuOpen;
                    menu.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                }else if(Input.GetKeyDown(KeyCode.Escape) && menuOpen)
                {
                    menuOpen = !menuOpen;
                    menu.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        #region weaponFire
        void Refresh()
        {
            isRefreshed = true;
        }

        void WeaponsFiring()
        {
            if (Input.GetButton("Fire1")) //Fire gun Left
            {
                if (weapon1.GetComponent<Weapon>().weaponAmmo > 0 && Time.time > nextFire)
                {
                    nextFire = Time.time + weapon1.GetComponent<Weapon>().weaponCooldown;
                    
                    if(weapon1.gameObject.TryGetComponent<RocketLauncher>(out RocketLauncher script))
                    {
                        script.SubstractRocket(weapon1.GetComponent<Weapon>().weaponAmmo);
                    }
                    if (scene.name == "Main")
                    {
                        weapon1.GetComponent<Weapon>().weaponAmmo -= 1;
                    }
                    munitionScript.UIAmmo1(weapon1.GetComponent<Weapon>().weaponAmmo);
                    

                    RaycastHit hitInfo;
                    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo))
                    {
                        if (hitInfo.point != null)
                        {

                            //Give a direction 
                            Vector3 direction = (hitInfo.point - weapon1.GetComponent<Weapon>().weaponFirePosition.position).normalized;
                            CmdShootRay(1, direction);
                        }
                    }
                    else
                    {
                        CmdShootRayVoid(1);
                    }
                }
            }
            if (Input.GetButton("Fire2")) //Fire gun Right
            {
                if (weapon2.GetComponent<Weapon>().weaponAmmo > 0 && Time.time > nextFire2)
                {
                    nextFire2 = Time.time + weapon2.GetComponent<Weapon>().weaponCooldown;
                    if(weapon2.gameObject.TryGetComponent<RocketLauncher>(out RocketLauncher script))
                    {
                        script.SubstractRocket(weapon2.GetComponent<Weapon>().weaponAmmo);
                    }
                    if (scene.name == "Main")
                    {
                        weapon2.GetComponent<Weapon>().weaponAmmo -= 1;
                    }
                    munitionScript.UIAmmo2(weapon2.GetComponent<Weapon>().weaponAmmo);

                    RaycastHit hitInfo;
                    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo))
                    {
                        if (hitInfo.point != null)
                        {
                            //Give a direction 
                            Vector3 direction = (hitInfo.point - weapon2.GetComponent<Weapon>().weaponFirePosition.position).normalized;
                            CmdShootRay(2, direction);
                        }
                    }
                    else
                    {
                        CmdShootRayVoid(2);
                    }
                }
            }
        }
        [Command]
        void CmdShootRay(int _weapon, Vector3 _direction)
        {
            if (_weapon == 1)
            {
                RpcFireWeapon(1,_direction);
            }
            if (_weapon == 2)
            {
                RpcFireWeapon(2,_direction);
            }

        }
        [Command]
        void CmdShootRayVoid(int _weapon)
        {
            if (_weapon == 1)
            {
                RpcFireWeaponVoid(1);
            }
            if (_weapon == 2)
            {
                RpcFireWeaponVoid(2);
            }

        }

        [ClientRpc]
        void RpcFireWeapon(int _weapon, Vector3 _direction)
        {
            if (_weapon == 1 && _direction != null)
            {
                GameObject bullet = Instantiate(weapon1.GetComponent<Weapon>().weaponBullet, weapon1.GetComponent<Weapon>().weaponFirePosition.position, weapon1.GetComponent<Weapon>().weaponFirePosition.rotation);
                bullet.GetComponent<Rigidbody>().velocity = _direction * weapon1.GetComponent<Weapon>().weaponSpeed;
                weapon1.GetComponent<AudioSource>().PlayOneShot(weapon1.GetComponent<Weapon>().FireSfx, 0.6f);
                Destroy(bullet, weapon1.GetComponent<Weapon>().weaponLife);
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (_weapon == 2 && _direction != null)
            {
                GameObject bullet = Instantiate(weapon2.GetComponent<Weapon>().weaponBullet, weapon2.GetComponent<Weapon>().weaponFirePosition.position, weapon2.GetComponent<Weapon>().weaponFirePosition.rotation);
                bullet.GetComponent<Rigidbody>().velocity = _direction * weapon2.GetComponent<Weapon>().weaponSpeed;
                weapon2.GetComponent<AudioSource>().PlayOneShot(weapon2.GetComponent<Weapon>().FireSfx, 0.6f);
                Destroy(bullet, weapon2.GetComponent<Weapon>().weaponLife);
            }
        }
        [ClientRpc]
        void RpcFireWeaponVoid(int _weapon)
        {
            if(_weapon == 1)
            {
                GameObject bullet = Instantiate(weapon1.GetComponent<Weapon>().weaponBullet, weapon1.GetComponent<Weapon>().weaponFirePosition.position, weapon1.GetComponent<Weapon>().weaponFirePosition.rotation);
                bullet.GetComponent<Rigidbody>().velocity = camera.transform.forward * weapon1.GetComponent<Weapon>().weaponSpeed;
                bullet.GetComponent<Bullet1>().player = gameObject;
                weapon1.GetComponent<AudioSource>().PlayOneShot(weapon1.GetComponent<Weapon>().FireSfx, 0.4f);
                Destroy(bullet, weapon1.GetComponent<Weapon>().weaponLife);
            }
            if (_weapon == 2)
            {
                GameObject bullet = Instantiate(weapon2.GetComponent<Weapon>().weaponBullet, weapon2.GetComponent<Weapon>().weaponFirePosition.position, weapon2.GetComponent<Weapon>().weaponFirePosition.rotation);
                bullet.GetComponent<Rigidbody>().velocity = camera.transform.forward * weapon2.GetComponent<Weapon>().weaponSpeed;
                weapon2.GetComponent<AudioSource>().PlayOneShot(weapon2.GetComponent<Weapon>().FireSfx, 0.4f);
                Destroy(bullet, weapon2.GetComponent<Weapon>().weaponLife);
            }
        }
        #endregion weaponFire
        [Command]
        public void PlayHitSound()
        {
            //gameObject.GetComponent<AudioSource>().PlayOneShot()
        }

        [Command]
        void CmdChangeWeapon(GameObject weapon, string name, bool isRight)
        {
            RpcChangeWeapon(weapon, name, isRight);
        }
        [ClientRpc]
        void RpcChangeWeapon(GameObject weapon,string name, bool isRight)
        {
            //Destroy that object on weaponPad
            if (weapon != null && scene.name == "Main") 
            {
                Destroy(weapon.transform.GetChild(0).gameObject);
            }
            if (isRight)
            {
                //Destroy rightArm
                Destroy(rightArmPoint.transform.GetChild(0).gameObject);

                //instantiate this object as a gameobject
                GameObject arm = Instantiate(Resources.Load(name), rightArmPoint.transform.position, rightArmPoint.transform.rotation) as GameObject;
                //make it child of a spawnpoint
                arm.transform.parent = rightArmPoint.gameObject.transform;
                //tag it
                arm.gameObject.tag = "WeaponRight";
                //make the script know that it as changed
                weapon2 = arm;
                //update weapon ammo
                munitionScript.UIAmmo2(weapon2.GetComponent<Weapon>().weaponAmmo);
            }
            else
            {
                //Destroy leftArm
                Destroy(leftArmPoint.transform.GetChild(0).gameObject);

                //instantiate this object as a gameobject
                GameObject arm = Instantiate(Resources.Load(name), leftArmPoint.transform.position, leftArmPoint.transform.rotation) as GameObject;
                //make it child of a spawnpoint
                arm.transform.parent = leftArmPoint.gameObject.transform;
                //tag it
                arm.gameObject.tag = "WeaponLeft";
                //make the script know that it as changed
                weapon1 = arm;
                //make the weapon spawning to right side
                weapon1.transform.localScale = new Vector3(weapon1.transform.localScale.x * -1, weapon1.transform.localScale.y, weapon1.transform.localScale.z);
                //update weapon ammo
                munitionScript.UIAmmo1(weapon1.GetComponent<Weapon>().weaponAmmo);
            }

        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Collider>().gameObject.tag == "poison")
            {
                PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(100);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<Collider>().gameObject.tag == "WeaponPad" && other.transform.childCount > 0 && isLocalPlayer)
            {
                print("I call replaceImg false");
                replaceImg.enabled = true;
                onPad = true;
                if (canSwapWeapon1 && other.gameObject.transform.GetChild(0) != null)
                {
                    canSwapWeapon1 = false;
                    GameObject weaponpad = other.gameObject;
                    //Get the name of the weapon on pad
                    name = weaponpad.transform.GetChild(0).gameObject.name;
                    CmdChangeWeapon(weaponpad, name, false);
                    Invoke("HideUI", 0.4f);
                    other.gameObject.GetComponentInParent<WeaponSpawn>().CmdWeaponTaken();
                }
                if (canSwapWeapon2 && other.gameObject.transform.GetChild(0) != null)
                {
                    canSwapWeapon2 = false;
                    GameObject weaponpad = other.gameObject;
                    //Get the name of the weapon on pad
                    name = weaponpad.transform.GetChild(0).gameObject.name;
                    CmdChangeWeapon(weaponpad, name, true);
                    Invoke("HideUI", 0.4f);
                    
                    other.gameObject.GetComponentInParent<WeaponSpawn>().CmdWeaponTaken();
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {

            if (other.GetComponent<Collider>().gameObject.tag == "WeaponPad")
            {
                onPad = false;
                replaceImg.enabled = false;
            }
        }
        void HideUI()
        {
            //hide img replace weapon
            print("Hide UI");
            replaceImg.enabled = false;
        }

        public void ResetPlayer()
        {
            isDead = false;
            //gameObject.GetComponent<PlayerHealth>().ExplosionVfx.SetActive(false);
            CmdChangeWeapon(null, "Desert_eagle", false);
            CmdChangeWeapon(null, "Desert_eagle", true);
        }

    }//end class
}//end namespace