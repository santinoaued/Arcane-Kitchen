using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject pauseMenu; // arrastrá tu Canvas de pausa en el Inspector
    private bool isPaused = false;

    void Start()
    {
        // Solo aplicar en escenas >= 1 (escenas de juego)
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (pauseMenu != null)
                pauseMenu.SetActive(false);
        }
    }

    void Update()
    {
        // Solo aplicar en escenas >= 1
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            // Atajo con ESC solo si existe un menú de pausa asignado
            if (pauseMenu != null && Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    ReanudarJuego();
                else
                    PausarJuego();
            }

            // Seguridad: mantener el cursor bloqueado si no está pausado
            if (!Cursor.visible && !isPaused)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void CambiarNivel(int numeroNivel)
    {
        SceneManager.LoadScene(numeroNivel);

        if (numeroNivel >= 1)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }

    public void PausarJuego()
    {
        if (pauseMenu == null) return; // seguridad

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;

        pauseMenu.SetActive(true);
    }

    public void ReanudarJuego()
    {
        if (pauseMenu == null) return; // seguridad

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;

        pauseMenu.SetActive(false);
    }

    public void ReiniciarNivel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);

        if (index >= 1)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
