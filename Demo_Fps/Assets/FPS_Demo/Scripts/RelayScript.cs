using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Edgegap;
using Mirror;

public class RelayScript : NetworkBehaviour
{
    public EdgegapTransport _EdgegapTransport = EdgegapTransport.GetInstance();
    private readonly HttpClient _httpClient = new HttpClient();
    public static string apiUrl = "https://api.edgegap.com";

    public async Task SendRequest(string token, int numPlayer)
    {
        // Set the authorization header
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);
        //Set the Ips for the request
        RootObject objectToSerialize = new RootObject();
        objectToSerialize.users = new List<Users>();
        //For the sake of a demo, I hardcoded Ips
        for (int i = 0; i < numPlayer; i++)
        {
            Users user = new Users { ip = $"10.10.10.1{i}" };
            objectToSerialize.users.Add(user);
        }

        
        // Serialize the IP array to JSON
        var jsonContent = new StringContent(JsonConvert.SerializeObject(objectToSerialize), Encoding.UTF8, "application/json");
        // Send the POST request and get the response
        var response = await _httpClient.PostAsync($"{apiUrl}/v1/relays/sessions", jsonContent);


        var responseContent = await response.Content.ReadAsStringAsync();
        print(responseContent);

        //Deserialize the response of the API
        ApiResponse content = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

        //Sends a loop to wait for a positive responce
        //The first answer of the API contain very few informations, but with the session_id that it gives us,
        //we can find our session and wait for it to be ready
        await PollDataAsync(_httpClient, content, content.session_id);
        //Reinitialize our content
        var newResponse = await _httpClient.GetAsync($"{apiUrl}/v1/relays/sessions/" + content.session_id);
        var newResponseContent = await newResponse.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse>(newResponseContent);

        if (data.ready)
        {
            //Convert uint? to uint
            uint authorization_token = data.authorization_token ?? 0;
            uint usr_authorization_tkn = data.session_users[0].authorization_token ?? 0;

            _EdgegapTransport.ChangeValue(data.relay.ip, data.relay.ports.client.port, data.relay.ports.server.port, data.session_id, authorization_token, usr_authorization_tkn);

            NetworkManager.singleton.StartHost();
        }
        else
        {
            Debug.LogError($"Error: {response.RequestMessage} - {response.ReasonPhrase}");
            Debug.LogError($"Error: Couldn't found a session relay");

        }
    }
    public static async Task PollDataAsync(HttpClient client, ApiResponse content, string sessionId)
    {
        //TODO say something when waiting for too long
        while (!content.ready)
        {
            Debug.Log("Waiting for data to be ready...");
            await Task.Delay(3000); // Wait 3 seconds between each iteration
            var response = await client.GetAsync($"{apiUrl}/v1/relays/sessions/" + sessionId);
            var responseContent = await response.Content.ReadAsStringAsync();
            print("Response from client -----------" + responseContent);
            content = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
            print("Is the game ready : " + content.ready);
        }
        // The "ready" property is now true, output a message
        Debug.Log("Data is now ready!");
    }
    public async Task JoinRoom(string sessionId, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);
        var response = await _httpClient.GetAsync($"{apiUrl}/v1/relays/sessions/" + sessionId);
        //Catch bad session ID
        if (!response.IsSuccessStatusCode)
        {
            return;
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        ApiResponse content = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

        uint authorization_token = content.authorization_token ?? 0;
        uint usr_authorization_tkn = content.session_users[1].authorization_token ?? 0;
        _EdgegapTransport.ChangeValue(content.relay.ip, content.relay.ports.client.port, content.relay.ports.server.port, content.session_id, authorization_token, usr_authorization_tkn);
        NetworkManager.singleton.StartClient();
        await Task.Delay(10000);
    }
}
public class Users
{
    public string ip { get; set; }
}

public class RootObject
{
    public List<Users> users { get; set; }
}
public class Client
{
    public ushort port { get; set; }
    public string protocol { get; set; }
    public string link { get; set; }
}

public class Ports
{
    public Server server { get; set; }
    public Client client { get; set; }
}

public class Relay
{
    public string ip { get; set; }
    public string host { get; set; }
    public Ports ports { get; set; }
}

public class ApiResponse
{
    public string session_id { get; set; }
    public uint? authorization_token { get; set; }
    public string status { get; set; }
    public bool ready { get; set; }
    public bool linked { get; set; }
    public object? error { get; set; }
    public List<SessionUser>? session_users { get; set; }
    public Relay relay { get; set; }
    public object? webhook_url { get; set; }
}

public class Server
{
    public ushort port { get; set; }
    public string protocol { get; set; }
    public string link { get; set; }
}

public class SessionUser
{
    public string ip_address { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public uint? authorization_token { get; set; }
}
