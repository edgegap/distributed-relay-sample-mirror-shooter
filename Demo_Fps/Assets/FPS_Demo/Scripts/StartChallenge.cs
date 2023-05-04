using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using System.Linq;
using System.Threading.Tasks;

public class StartChallenge : NetworkBehaviour
{
    #region Variable
    [Header("Challenge Speed")]
    public GameObject prefabChallSpeed;
    bool ChallSpeedAsStart;
    public TMP_Text TimeChallSpeed;
    [SerializeField] private GameObject[] robotsChallSpeed;
    private GameObject cloneChallSpeed;
    private double currentTime;
    [SyncVar(hook = nameof(OnTimerValueChanged))]
    public double syncTimer;

    [Header("Challenge Points")]
    public GameObject prefabChallPoints;
    public GameObject robotPrefab;
    private GameObject cloneChallPoints;
    GameObject[] spawnPoints;
    bool ChallPointsAsStart;
    
    [SerializeField] private GameObject[] SpawnPoints;
    [SyncVar(hook = nameof(OnChangeScore))]
    public uint ChallPointsScore;
    [SyncVar(hook = nameof(OnChangeTime))]
    double challPointsTimerSync;
    double challPointsTimer = 0;
    

    public TMP_Text ChallPointsScoreTxt;
    public TMP_Text TimeChallPoints;

    [Header("Challenge Aim Speed")]
    public GameObject prefabAimSpeed;
    public bool AimSpeedAsStart;
    public TMP_Text TimeAimSpeed;
    public TMP_Text AimSpeedScoreTxt;
    GameObject ChallAimSpeed;
    [SyncVar(hook = nameof(OnChangeScoreAimSpeed))]
    public uint AimSpeedScore;
    [SyncVar(hook = nameof(OnChangeTimeAimSpeed))]
    double AimSpeedTimerSync;
    double AimSpeedTimer = 0;


    [Header("Challenge Aim Points")]
    public GameObject prefabAimPoints;
    public bool AimPointsAsStart;
    double AimPointsTimer = 0;
    public TMP_Text TimeAimPoints;
    public TMP_Text AimPointsScoreTxt;
    GameObject ChallAimPoints;
    List<GameObject> SpawnPointCible = new List<GameObject>() { };
    public List<GameObject> CopieSpawnPointCible;
    [SyncVar(hook = nameof(OnChangeScoreAimPoints))]
    public uint AimPointsScore;
    [SyncVar(hook = nameof(OnChangeTimeAimPoints))]
    double AimPointsTimerSync;


    #endregion

    private void Start()
    {
        ChallAimPoints = GameObject.Find("ChallAimPoints");
        ChallAimSpeed = GameObject.Find("ChallAimSpeed");
    }

    public void StartChallengeSpeed()
    {
        cloneChallSpeed = GameObject.Find("ChallSpeed");
        if (cloneChallSpeed == null) 
        {
            ChallSpeedAsStart = true;
            currentTime = 0;
            cloneChallSpeed = Instantiate(prefabChallSpeed, prefabChallSpeed.transform.position, Quaternion.identity);
            cloneChallSpeed.name = cloneChallSpeed.name.Replace("(Clone)", "");

        }
    }

    void OnTimerValueChanged(double oldValue, double newValue)
    {
        if (!isServer)
        {
            currentTime = newValue;
        }
    }
    [Command(requiresAuthority = false)]
    public void StartChallengePoints()
    {
        if (challPointsTimer == 0)
        {
            challPointsTimer = 30;
            ChallPointsScore = 0;
            ChallPointsAsStart = true;
            InvokeRepeating("CheckForSpawn", 0.1f, 3f);
        }
    }
    
    public void CheckForSpawn()
    {
        SpawnPoints = prefabChallPoints.GetComponentsInChildren<Transform>().Where(t => t.gameObject.CompareTag("SpawnPoint")).Select(t => t.gameObject).ToArray();
       
        foreach (GameObject spawnPoint in SpawnPoints)
        {
            if (spawnPoint.transform.childCount == 0)
            {
                GameObject robot = Instantiate(robotPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                robot.transform.parent = spawnPoint.transform;
                NetworkServer.Spawn(robot);
                
            }
           
        }
    }
    public void IncrementScore()
    {
        ChallPointsScore++;
    }
    void OnChangeScore(uint oldValue, uint newValue)
    {
        if (!isServer)
        {
            ChallPointsScore = newValue;
        }
    }
    void OnChangeTime(double oldValue, double newValue)
    {
        if (!isServer)
        {
            challPointsTimer = newValue;
        }
    }
    
    public void StartAimSpeed()
    {
        if (AimSpeedTimer == 0)
        {
            AimSpeedAsStart = true;
            AimSpeedTimer = 30;
            AimSpeedScore = 0;
            GameObject clone = Instantiate(prefabAimSpeed, prefabAimSpeed.transform.position, prefabAimSpeed.transform.rotation, ChallAimSpeed.transform);
            clone.transform.parent = ChallAimSpeed.transform;
            clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(4f, -3f), UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(5f, -5f));
            NetworkServer.Spawn(clone);
            //CmdSpawnNewTargetAimSpeed();
        }
    }
    [Command]
    public void CmdSpawnNewTargetAimSpeed()
    {
        print("CmdSpawnNewTargetAimSpeed is Called");
        //RpcSpawnNewTargetAimSpeed();
    }
  
    public void SpawnNewTargetAimSpeed()
    {
        GameObject clone = Instantiate(prefabAimSpeed, prefabAimSpeed.transform.position, prefabAimSpeed.transform.rotation, ChallAimSpeed.transform);
        clone.transform.parent = ChallAimSpeed.transform;
        clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(4f, -3f), UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(5f, -5f));
        NetworkServer.Spawn(clone);
        print("CmdSpawnNewTargetAimSpeed is Called");
        
    }
    void OnChangeScoreAimSpeed(uint oldValue, uint newValue)
    {
        if (!isServer)
        {
           AimSpeedScore = newValue;
        }
    }
    void OnChangeTimeAimSpeed(double oldValue, double newValue)
    {
        if (!isServer)
        {
            AimSpeedTimer = newValue;
        }
    }
    public void IncrementScoreAimSpeed()
    {
        AimSpeedScore++;
    }
    [Command(requiresAuthority = false)]
    public void CmdStartAimPoints()
    {
        if (AimPointsTimer == 0)
        {
            ChallAimPoints = GameObject.Find("ChallAimPoints");
            AimPointsAsStart = true;
            AimPointsTimer = 30;
            AimPointsScore = 0;
            foreach (GameObject spawnPoint in CopieSpawnPointCible)
            {
                print(ChallAimPoints.transform);
                GameObject clone = Instantiate(prefabAimPoints, prefabAimPoints.transform.position, prefabAimPoints.transform.rotation, ChallAimPoints.transform);
                clone.transform.parent = ChallAimPoints.transform;
                clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(2.4f, -4.5f), UnityEngine.Random.Range(1.3f, -2f), UnityEngine.Random.Range(-5.5f, 5.5f));
                SpawnPointCible.Add(clone);
                NetworkServer.Spawn(clone);
            }
            
        }
    }
    void OnChangeTimeAimPoints(double oldValue, double newValue)
    {
        if (!isServer)
        {
            AimPointsTimer = newValue;
        }
    }
    void OnChangeScoreAimPoints(uint oldValue, uint newValue)
    {
        if (!isServer)
        {
            AimPointsScore = newValue;
        }
    }
    public void IncrementScoreAimPoints()
    {
        AimPointsScore++;
    }
    public void SpawnNewTarget()
    {
        //Random.range cause problem with the library of Math.Random
        GameObject clone = Instantiate(prefabAimPoints, prefabAimPoints.transform.position, prefabAimPoints.transform.rotation, ChallAimPoints.transform);
        clone.transform.parent = ChallAimPoints.transform;
        clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(2.4f, -4.5f), UnityEngine.Random.Range(1.3f, -2f), UnityEngine.Random.Range(-5.5f, 5.5f));
        SpawnPointCible.Add(clone);
        NetworkServer.Spawn(clone);
    }

    private void Update()
    {
        if (ChallSpeedAsStart)
        {
            GameObject[] robotsChallSpeed = cloneChallSpeed.GetComponentsInChildren<Transform>().Where(t => t.gameObject.CompareTag("robot")).Select(t => t.gameObject).ToArray();
            if(robotsChallSpeed.Length != 0)
            {
                currentTime += Time.deltaTime;
                syncTimer = currentTime;
                currentTime = Math.Round(currentTime, 2);
                TimeChallSpeed.text = currentTime.ToString();
            }
            else
            {
                ChallSpeedAsStart = false;
                Destroy(cloneChallSpeed);
            }
            
        }
        if (ChallPointsAsStart)
        {
            if(challPointsTimer > 0)
            {
                challPointsTimer -= 1 * Time.deltaTime;
                challPointsTimerSync = challPointsTimer;
                challPointsTimer = Math.Round(challPointsTimer, 2);
                TimeChallPoints.text = challPointsTimer.ToString();
                ChallPointsScoreTxt.text = ChallPointsScore.ToString();
            }
            else
            {
                ChallPointsAsStart = false;
                challPointsTimer = 0;
                TimeChallPoints.text = "0.00";
                foreach (GameObject spawnPoint in SpawnPoints)
                {
                    if (spawnPoint.transform.childCount != 0)
                    {
                        CancelInvoke();
                        Destroy(spawnPoint.transform.GetChild(0).gameObject);
                        NetworkServer.Destroy(spawnPoint.transform.GetChild(0).gameObject);
                        
                    }

                }
            }
            

        }
        if (AimSpeedAsStart)
        {
            if (AimSpeedTimer > 0)
            {
                AimSpeedTimer -= 1 * Time.deltaTime;
                AimSpeedTimerSync = AimSpeedTimer;
                AimSpeedTimer = Math.Round(AimSpeedTimer, 2);
                TimeAimSpeed.text = AimSpeedTimer.ToString();
                AimSpeedScoreTxt.text = AimSpeedScore.ToString();
            }
            else
            {
                AimSpeedAsStart = false;
                AimSpeedTimer = 0;
                TimeAimSpeed.text = "0.00";
                for (var i = 0; i< ChallAimSpeed.transform.childCount;i++)
                {
                    Transform obj = ChallAimSpeed.transform.GetChild(i);
                    Destroy(obj.gameObject);
                    NetworkServer.Destroy(obj.gameObject);
                }
            }


        }
        if (AimPointsAsStart)
        {
            if (AimPointsTimer > 0)
            {
                AimPointsTimer -= 1 * Time.deltaTime;
                AimPointsTimerSync = AimPointsTimer;
                AimPointsTimer = Math.Round(AimPointsTimer, 2);
                TimeAimPoints.text = AimPointsTimer.ToString();
                AimPointsScoreTxt.text = AimPointsScore.ToString();
            }
            else
            {
                AimPointsAsStart = false;
                AimPointsTimer = 0;
                TimeAimPoints.text = "0.00";
                for (int i = SpawnPointCible.Count - 1; i >= 0; i--)
                {
                    // get the object at index i
                    GameObject obj = SpawnPointCible[i];

                    // destroy the object on the server and propagate the destruction to all clients
                    NetworkServer.Destroy(obj);

                    // remove the object from the list
                    SpawnPointCible.RemoveAt(i);
                    print(SpawnPointCible.Count);
                }
            }


        }

    }
}
