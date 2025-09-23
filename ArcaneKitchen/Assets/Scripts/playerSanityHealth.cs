using UnityEngine;
using UnityEngine.Events;

public class playerSanityHealth : MonoBehaviour
{
    [Header("Configuración de Cordura y Vida")]
    [SerializeField] int corduraMaxima = 100;
    [SerializeField] int corduraActual;
    [SerializeField] int vidaMaxima = 3;
    [SerializeField] int vidaActual;

    [Header("Eventos")]
    [SerializeField] UnityEvent onCorduraCambiada;
    [SerializeField] UnityEvent onVidaCambiada;
    [SerializeField] UnityEvent onMuerte;


    public int CorduraMaxima => corduraMaxima;
    public int CorduraActual => corduraActual;

    public int VidaMaxima => vidaMaxima;
    public int VidaActual => vidaActual;

    void Start()
    {
        corduraActual = corduraMaxima;
        vidaActual = vidaMaxima;
    }

    public void RecibirDanioCordura(int danioRealizado)
    {
        if (corduraActual <= 1) return;

        corduraActual = Mathf.Max(0, corduraActual - danioRealizado);

        if (corduraActual < 1)
        {
            PerderVida();
            corduraActual = corduraMaxima;
        }

        Debug.Log($"Tu cordura está en : {CorduraActual}/{CorduraMaxima}");
    }

    public void Tranquilizarse()
    {
        if (corduraActual == corduraMaxima) return;

        corduraActual = Mathf.Min(corduraMaxima, corduraActual + 1);
        onCorduraCambiada.Invoke();

        Debug.Log($"Tu cordura está en : {CorduraActual}/{CorduraMaxima}");

    }

    private void RestablecerCordura()
    {
        corduraActual = corduraMaxima;
        onCorduraCambiada.Invoke();
    }

    private void PerderVida()
    {
        vidaActual = Mathf.Max(0, vidaActual - 1);
        onVidaCambiada.Invoke();

        if (vidaActual < 1)
        {
            Morir();
        }
        else
        {
            Debug.Log("Pierde vida");
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
    }

    public void ReiniciarTodo()
    {
        vidaActual = vidaMaxima;
        corduraActual = corduraMaxima;
        onVidaCambiada.Invoke();
        onCorduraCambiada.Invoke();
    }

}