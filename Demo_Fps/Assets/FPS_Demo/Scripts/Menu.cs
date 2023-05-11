using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Edgegap;
using TMPro;
using UnityEngine.UI;
namespace QuickStart
    
{
    public class Menu : NetworkBehaviour
    {
        public EdgegapTransport _EdgegapTransport = EdgegapTransport.GetInstance();
        private readonly HttpClient _httpClient = new();
        public TMP_Text sessionID;
        public TMP_Text error;
        public AudioClip buttonClick;

        private void Start()
        {
            sessionID.text = EdgegapTransport.session_ID;
        }
        public void CopyText()
        {
            TextEditor textEditor = new TextEditor();
            textEditor.text = sessionID.text;
            textEditor.SelectAll();
            textEditor.Copy();
            gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        }

        public async void LoadMenu()
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                await DeleteSession(_httpClient);
                NetworkManager.singleton.StopHost();
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }
        }

        public void ButtonChangeScene()
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
            if (NetworkServer.active)
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "Main")
                {
                    NetworkManager.singleton.ServerChangeScene("Other");
                }
                else
                {
                    NetworkManager.singleton.ServerChangeScene("Main");
                }  
            }
            else
            {
                error.text = "You are not the host";
                Invoke("HideMsg", 3f);
            }
                
                
        }
        void HideMsg()
        {
            error.text = "";
        }

        public async void QuitGame()
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                await DeleteSession(_httpClient);
            }
            Application.Quit();
        }


        public async Task DeleteSession(HttpClient client)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", CanvasHUD.token);
            print(CanvasHUD.token);
            var response = await client.DeleteAsync("https://api.edgegap.com/v1/relays/sessions/" + EdgegapTransport.session_ID);
            print(response.Content);
            print("https://api.edgegap.com/v1/relays/sessions/" + EdgegapTransport.sessionId);
            print(EdgegapTransport.sessionId);
            if (response.IsSuccessStatusCode)
            {
                Debug.Log("Session deleted successfully.");
            }
            else
            {
                Debug.LogError($"Failed to delete session. Status code: {response.StatusCode}");
            }
        }
       

    }
}