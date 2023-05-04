using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class SceneScript : NetworkBehaviour
    {
        public PlayerScript playerScript;
        //public TMP_Text canvasStatusText;
        

        //[SyncVar(hook = nameof(OnStatusTextChanged))]
        //public string statusText;

        
        //void OnStatusTextChanged(string _Old, string _New)
        //{
        //    //called from sync var hook, to update info on screen for all players
        //    canvasStatusText.text = statusText;
        //}

        //public void ButtonChangeScene()
        //{
        //    if (isServer)
        //    {
        //        Scene scene = SceneManager.GetActiveScene();
        //        if (scene.name == "Main")
        //            NetworkManager.singleton.ServerChangeScene("Other");
        //        else
        //            NetworkManager.singleton.ServerChangeScene("Main");
        //    }
        //    else
        //        Debug.Log("You are not Host.");
        //}
    
        
    }//end class
}//end namespace
