using UnityEngine;
using UnityEngine.UI;

public class LivesHUD : MonoBehaviour
{
    [Tooltip("Referencia al objeto que contiene playerSanityHealth. Si se deja vacío, se buscará por tag 'Player'.")]
    public GameObject playerObject;

    [Tooltip("Images de los corazones en orden (Heart1 = vida 1, Heart2 = vida 2, ...).")]
    public Image[] heartImages; 
    playerSanityHealth playerHealth;

    void Start()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerObject != null)
            playerHealth = playerObject.GetComponent<playerSanityHealth>();

        
        RefreshLives();
    }

    
    public void RefreshLives()
    {
        if (playerHealth == null)
        {
            
            if (playerObject != null) playerHealth = playerObject.GetComponent<playerSanityHealth>();
            if (playerHealth == null) return;
        }

        int vida = Mathf.Clamp(playerHealth.VidaActual, 0, playerHealth.VidaMaxima);

        
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            heartImages[i].gameObject.SetActive(i < vida);
        }
    }
}
