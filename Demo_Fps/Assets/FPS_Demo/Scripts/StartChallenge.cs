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
    public GameObject ChallAimPoints;
    List<GameObject> SpawnPointCible = new List<GameObject>() { };
    public List<GameObject> CopieSpawnPointCible;
    [SyncVar(hook = nameof(OnChangeScoreAimPoints))]
    public uint AimPointsScore;
    [SyncVar(hook = nameof(OnChangeTimeAimPoints))]
    double AimPointsTimerSync;


    #endregion

    private void Start()
    {
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
    public void CmdStartChallengePoints()
    {
        if (challPointsTimer == 0)
        {
            RpcStartChallengePoints();
            InvokeRepeating("CmdCheckForSpawn", 0.1f, 3f);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdCheckForSpawn()
    {
        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
       
        foreach (GameObject spawnPoint in SpawnPoints)
        {
            if(spawnPoint.GetComponent<SpawnCollider>().isInCollider == false)
            {
                GameObject robot = Instantiate(robotPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                NetworkServer.Spawn(robot);  
            }
            
            

            
        }
    }
    [ClientRpc]
    void RpcStartChallengePoints()
    {
        if (challPointsTimer == 0)
        {
            challPointsTimer = 30;
            ChallPointsScore = 0;
            ChallPointsAsStart = true;
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
    #region hide
    [Command(requiresAuthority = false)]
    public void CmdStartAimSpeed()
    {
        if (AimSpeedTimer == 0)
        {
            RpcStartAimSpeed();
            CmdSpawnNewTargetAimSpeed();
        }
    }
    [ClientRpc]
    void RpcStartAimSpeed()
    {
        if (AimSpeedTimer == 0)
        {
            AimSpeedAsStart = true;
            AimSpeedTimer = 30;
            AimSpeedScore = 0;
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdSpawnNewTargetAimSpeed()
    {
            GameObject clone = Instantiate(prefabAimSpeed, prefabAimSpeed.transform.position, prefabAimSpeed.transform.rotation);
            clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(10f, 15f), UnityEngine.Random.Range(1f, 4f), UnityEngine.Random.Range(-4.5f, 4.5f));
            NetworkServer.Spawn(clone);
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
        RpcCmdStartAimPoints();
        if (AimPointsTimer == 0)
        {
            foreach (GameObject spawnPoint in CopieSpawnPointCible)
            {
                GameObject clone = Instantiate(prefabAimPoints, prefabAimPoints.transform.position, prefabAimPoints.transform.rotation);
                clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(-15f, -10f), UnityEngine.Random.Range(0.5f, 4f), UnityEngine.Random.Range(-5.5f, 5.5f));
                
                SpawnPointCible.Add(clone);
                NetworkServer.Spawn(clone);
            }
        }
    }
    [ClientRpc]
    void RpcCmdStartAimPoints(){
        if (AimPointsTimer == 0)
        {
            AimPointsAsStart = true;
            AimPointsTimer = 30;
            AimPointsScore = 0;
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
    [Command(requiresAuthority = false)]
    public void CmdSpawnNewTarget()
    {
        print("spawn new cible");
        //Random.range cause problem with the library of Math.Random
        GameObject clone = Instantiate(prefabAimPoints, prefabAimPoints.transform.position, prefabAimPoints.transform.rotation);
        clone.transform.localPosition = new Vector3(UnityEngine.Random.Range(-15f, -10f), UnityEngine.Random.Range(1f, 4f), UnityEngine.Random.Range(-4.5f, 4.5f));
        SpawnPointCible.Add(clone);
        NetworkServer.Spawn(clone);
    }
#endregion
    private void Update()
    {
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
                CmdChallPointStart();
                
                GameObject[] robots = GameObject.FindGameObjectsWithTag("robot");
                for (var i = 0; i< robots.Length ;i++)
                {
                    CancelInvoke("CmdCheckForSpawn");
                    Destroy(robots[i]);
                    NetworkServer.Destroy(robots[i]);
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
                RpcAimSpeedStart();
                GameObject[] targets = GameObject.FindGameObjectsWithTag("targetReverse");
                for (var i = 0; i< targets.Length ;i++)
                {
                    
                    Destroy(targets[i]);
                    NetworkServer.Destroy(targets[i]);
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
                CmdAimPointsStart();
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
    [Command(requiresAuthority = false)]
    void CmdChallPointStart()
    {
        RpcChallPointStart();
    }
    [ClientRpc]
    void RpcChallPointStart()
    {
        ChallPointsAsStart = false;
        challPointsTimer = 0;
        TimeChallPoints.text = "0.00";
    }
    
    [Command(requiresAuthority = false)]
    void CmdAimSpeedStart()
    {
        RpcAimSpeedStart();
    }
    [ClientRpc]
    void RpcAimSpeedStart()
    {
        AimSpeedAsStart = false;
        AimSpeedTimer = 0;
        TimeAimSpeed.text = "0.00";
    }
    [Command(requiresAuthority = false)]
    void CmdAimPointsStart()
    {
        RpcAimPointsStart();
    }
    [ClientRpc]
    void RpcAimPointsStart()
    {
        AimPointsAsStart = false;
        AimPointsTimer = 0;
        TimeAimPoints.text = "0.00";
    }
}
