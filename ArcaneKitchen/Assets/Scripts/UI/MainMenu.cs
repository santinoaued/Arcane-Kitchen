using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // Reanudar el tiempo al iniciar la escena
        Time.timeScale = 1f;
    }
    public void CambiarNivel(int numeroNivel)
    {
        SceneManager.LoadScene(numeroNivel);
        Time.timeScale = 1f;
    }
    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }
}
