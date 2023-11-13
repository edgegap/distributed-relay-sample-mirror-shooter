using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class CanvasHUD : NetworkBehaviour
{
    private readonly RelayScript _relay = new();

    public GameObject PanelStart;
    public GameObject PanelStop;
    public GameObject loading;
    bool waitingForResponse;
    [SerializeField] private TMP_Text errorTxt;

    public Button buttonHost, buttonClient, buttonStop;

    public InputField inputFieldAddress;
    [Header("Token")]

    [SerializeField] public static string token = "INSERT RELAY API TOKEN HERE";//Change the API key because it won't be available
    int numPlayer = 2;
    public AudioClip buttonClick;
    private void Start()
    {
        
        //Update the canvas text if you have manually changed network managers address from the game object before starting the game scene
        if (NetworkManager.singleton.networkAddress != "localhost") { inputFieldAddress.text = NetworkManager.singleton.networkAddress; }

        //Adds a listener to the main input field and invokes a method when the value changes.
        inputFieldAddress.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        //Make sure to attach these Buttons in the Inspector
        buttonHost.onClick.AddListener(ButtonHost);
        buttonClient.onClick.AddListener(ButtonClient);
        buttonStop.onClick.AddListener(ButtonStop);

        //This updates the Unity canvas, we have to manually call it every change, unlike legacy OnGUI.
        SetupCanvas();
    }

    // Invoked when the value of the text field changes.

    public void ValueChangeCheck()
    {
        NetworkManager.singleton.networkAddress = inputFieldAddress.text;
    }
    public void DropdownChange(int val)
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        switch (val)
        {
            case 0:
                numPlayer = 2;
                break;
            case 1:
                numPlayer = 3;
                break;
            case 2:
                numPlayer = 4;
                break;
            default:
                numPlayer = 2;
                break;
        }
    }

    public async void ButtonHost()
    {
        SetupCanvas();
        gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        loading.SetActive(true);
        if (!waitingForResponse)
        {
            waitingForResponse = true;
            await _relay.SendRequest(token, numPlayer);
            waitingForResponse = false;
        }
        
        loading.SetActive(false);
    }

    public async void ButtonClient()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        SetupCanvas();
        errorTxt.text = "";
        loading.SetActive(true);
        await _relay.JoinRoom(inputFieldAddress.text, token);
        errorTxt.text = "Wrong session ID";
        loading.SetActive(false);
    }

    public void ButtonStop()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
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

        SetupCanvas();
    }

    public void SetupCanvas()
    {
        // Here we will dump majority of the canvas UI that may be changed.

        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                PanelStart.SetActive(false);
                PanelStop.SetActive(true);
            }
            else
            {
                PanelStart.SetActive(true);
                PanelStop.SetActive(false);
            }
        }
        else
        {
            PanelStart.SetActive(false);
            PanelStop.SetActive(true);
        }
    }
    
}
