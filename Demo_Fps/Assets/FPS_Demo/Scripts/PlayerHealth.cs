using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using QuickStart;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    private int health = 100;
    public Slider healthSlider;

    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Main" && gameObject.GetComponent<PlayerScript>().isDead != true)
        {
            health -= amount;
            print(health);

            if (health <= 0)
            {
                health = 0;
                gameObject.GetComponent<PlayerRespawn>().RpcOnPlayerDeath();
                
            }
        }
    }
    
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        healthSlider.value = newHealth;
    }
    [Command]
    public void CmdResetHealth()
    {
        print("resetHealth");
        health = 100;
    }

    
}
