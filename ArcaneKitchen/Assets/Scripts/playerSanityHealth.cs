using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerSanityHealth : MonoBehaviour
{
    [Header("Configuración de Cordura y Vida")]
    [SerializeField] int corduraMaxima = 100;
    [SerializeField] int corduraActual;
    [SerializeField] int vidaMaxima = 3;
    [SerializeField] int vidaActual;

    [Header("UI")]
    [SerializeField] Slider corduraSlider;

    [Header("Eventos")]
    [SerializeField] UnityEvent onCorduraCambiada;
    [SerializeField] UnityEvent onVidaCambiada;
    [SerializeField] UnityEvent onMuerte;

    
    bool processingLifeLoss = false;

    public int CorduraMaxima => corduraMaxima;
    public int CorduraActual => corduraActual;

    public int VidaMaxima => vidaMaxima;
    public int VidaActual => vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
        corduraActual = corduraMaxima;
        if (PlayerPrefs.HasKey("VidasRestantes"))
            vidaActual = PlayerPrefs.GetInt("VidasRestantes");
        else
            vidaActual = vidaMaxima;
        ActualizarCorduraUI();
    }

    public void RecibirDanioCordura(int danioRealizado)
    {
        if (processingLifeLoss) return;

        if (corduraActual <= 1 && danioRealizado > 0) return;

        corduraActual = Mathf.Max(0, corduraActual - danioRealizado);
        ActualizarCorduraUI();

        if (corduraActual < 1)
        {
            processingLifeLoss = true;

            PerderVida();

            if (vidaActual > 0)
            {
                RestablecerCordura();
            }
        }

        Debug.Log($"Tu cordura está en : {CorduraActual}/{CorduraMaxima}");
    }


    public void Tranquilizarse()
    {
        if (corduraActual == corduraMaxima) return;

        corduraActual = Mathf.Min(corduraMaxima, corduraActual + 1);
        ActualizarCorduraUI();
        onCorduraCambiada.Invoke();

        Debug.Log($"Tu cordura está en : {CorduraActual}/{CorduraMaxima}");
    }

    private void RestablecerCordura()
    {
        corduraActual = corduraMaxima;
        ActualizarCorduraUI();
        onCorduraCambiada.Invoke();

        
        processingLifeLoss = false;
    }

    private void PerderVida()
    {
        
        if (processingLifeLoss == false)
            processingLifeLoss = true;

        vidaActual = Mathf.Max(0, vidaActual - 1);
        onVidaCambiada.Invoke();
        ActualizarCorduraUI();

        if (vidaActual < 1)
        {
            Morir();
        }
        else
        {
            Debug.Log("Pierde vida");

            
            PlayerPrefs.SetInt("VidasRestantes", vidaActual);
            PlayerPrefs.Save();

            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    public void CurarVida(int cantidad)
    {
        vidaActual = Mathf.Min(vidaMaxima, vidaActual + cantidad);
        onVidaCambiada.Invoke();
    }

    private void Morir()
    {
        Debug.Log("Moriste we");
        onMuerte.Invoke();

         
        PlayerPrefs.DeleteKey("VidasRestantes");

        
        SceneManager.LoadScene("GameOver");
    }

    public void ReiniciarTodo()
    {
        vidaActual = vidaMaxima;
        corduraActual = corduraMaxima;
        ActualizarCorduraUI();
        onVidaCambiada.Invoke();
        onCorduraCambiada.Invoke();

        processingLifeLoss = false;
    }


    private void ActualizarCorduraUI()
    {
        if (corduraSlider != null)
        {
            corduraSlider.maxValue = corduraMaxima;
            corduraSlider.value = corduraActual;
        }
    }
}
