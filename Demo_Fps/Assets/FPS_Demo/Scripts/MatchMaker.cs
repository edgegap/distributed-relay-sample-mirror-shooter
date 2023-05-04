//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using UnityEngine;

//[System.Serializable]
//public class Match
//{
//    public string matchID;
//    public SyncListGameObject players = new SyncListGameObject();
//    public Match(string matchID, GameObject player)
//    {
//        this.matchID = matchID;
//        players.Add(player);
//    }
//    public Match() { }
//}
//[System.Serializable]
//public class SyncListGameObject : SyncList<GameObject> { }
//[System.Serializable]
//public class SyncListMatch : SyncList<Match> { }
//[System.Serializable]
//public class SyncListString : SyncList<string> { }
//public class MatchMaker : NetworkBehaviour
//{
//    public static MatchMaker instance;

//    public SyncListMatch matches = new SyncListMatch();
//    public SyncListString matchIDs = new SyncListString();
//    private void Start()
//    {
//        instance = this;
//    }
//    public bool HostGame(string _matchID, GameObject _player)
//    {
//        if (matchIDs.Contains(_matchID))
//        {
//            matchIDs.Add(_matchID);
//            matches.Add(new Match(_matchID, _player));
//            Debug.Log($"Match Generated");
//            return true;
//        }
//        else
//        {
//            Debug.Log($"Match ID already Exists");
//            return false;
//        }
        
//    }
//    public static string GetRandomMatchID()
//    {
//        string _id = string.Empty;
//        for(int i = 0; i <5; i++)
//        {
//            int random = Random.Range(0,36);
//            if(random < 26)
//            {
//                _id += (char)(random + 65);

//            }
//            else
//            {
//                _id += (random - 26).ToString();
//            }
//        }
//        Debug.Log($"Random Match ID: {_id}");
//        return _id;
//    }
//}
