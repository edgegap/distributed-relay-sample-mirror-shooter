# Mirror Multiplayer Shooter - P2P Relay
## How to use this asset

•	Create your Account on Edgegap: https://edgegap.com/

•	Go to the Relay menu, go to the bottom and click on the + sign to create a New Relay Profile

•	Copy the Relay API Token created

•	Go in Unity, find the FPS_Demo/Scripts/CanvasHUD.cs file

•	Around line 22, find the "public static string token = "INSERT RELAY API TOKEN HERE";" and change the text between quotes to the Relay API token you copied before

•	In Unity, you might have to add the Scenes to the Build Settings to be able to transition from the GameList to the Main scene. Go to FPS_Demo/Scenes/Main. Then go to File/Build Settings and click "Add Open Scene" in the top right

•	Now open the GameList scene in the same folder. Hit Play in the Editor, then Host in the interface should launch a relay and launch the Main scene.

•	To connect a 2nd player, hit the Escape key in the Host client, and click Copy ID on the interface

•	Now from the 2nd player client, paste the ID that was copied in the "Enter code here" text area, and click the Join button

------------------------

## Creating a new game or switching an existing game to the Edgegap Distributed Relay

•	Install Mirror: https://assetstore.unity.com/packages/tools/network/mirror-129321

•	Download The EdgegapTransport available here: https://github.com/edgegap/relay-transport-examples

•	Slide the EdgegapTransport Folder inside the Mirror transport Folder: Assets/Mirror/Transports

•	Change the Transport script inside your NetworkManager GameObject to be the EdgegapTransport.cs and make sure the NetworkManager script as his reference in the inspector. 

•	In the dashboard, under the section Relays, go ahead and click on “Create New Profile”. The API Token will be useful to communicate with the API.

•	Make an API request from Unity C# to launch a relay session using the token as Authorization, and Ips as content. The variables that we need to change in order to connect to a relay are the: relayAdress, relayGameSErverPort, relayGameClientPort, UserId and the sessionId;
 
•	Get the info from the API response such as Authorization_token, users_authorization, relay ports, etc.

•	Inside the EdgegapTransport script, override the values that are necessary to connect with each other. 
In the code snippet given previously, we call the method ChangeValue in the EdgegapTransport script.
 
•	Connect one player as the Host and the other as client. To connect a client, you can do another API Get request to access the relay information and parse them into the EdgegapTransport.

Feel free to optimize and adapt the script to your needs.  

Full documentation on the Edgegap Distributed Relay available here:
https://docs.edgegap.com/docs/distributed-relay-manager


------------------------

## Assets used in the project:

Stylized Rocket Launcher Complete Kit with Visual Effects and Sound - Hand Painted Bazooka - HQ Toon Weapon Set - BigRookGames

https://assetstore.unity.com/packages/3d/props/guns/stylized-rocket-launcher-complete-kit-with-visual-effects-and-so-178718


Stylized M4 Assault Rifle with Scope Complete Kit with Gunshot VFX and Sound - Hand Painted AR Machine Gun Automatic Rifle - BigRookGames

https://assetstore.unity.com/packages/3d/props/guns/stylized-m4-assault-rifle-with-scope-complete-kit-with-gunshot-v-178197


Customizable skybox - Key Mouse

https://assetstore.unity.com/packages/2d/textures-materials/sky/customizable-skybox-174576


Galaxy Fire Skybox - Sean Duffy

https://assetstore.unity.com/packages/2d/textures-materials/galaxy-fire-skybox-10976


LowPoly Water - Ebru Dogan

https://assetstore.unity.com/packages/tools/particles-effects/lowpoly-water-107563


Mirror - vis2k

https://assetstore.unity.com/packages/tools/network/mirror-129321
