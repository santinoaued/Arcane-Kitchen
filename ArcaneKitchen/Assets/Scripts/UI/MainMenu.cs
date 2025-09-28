using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject pauseMenu; // arrastrá tu Canvas de pausa en el Inspector
    private bool isPaused = false;

    void Start()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        // Solo aplicar en escenas de juego (>=1) y que no sean la de "Perder"
        if (index >= 1 && index != 2) // <-- cambia 2 por el índice real de tu escena perder
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
        int index = SceneManager.GetActiveScene().buildIndex;

        // Solo aplicar en escenas de juego (>=1) y que no sean la de "Perder"
        if (index >= 1 && index != 2)
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

    public void IrAInstrucciones()
    {
        SceneManager.LoadScene("Instrucciones");

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CambiarNivel(int numeroNivel)
    {
        SceneManager.LoadScene(numeroNivel);

        if (numeroNivel >= 1 && numeroNivel != 2) // excluir escena perder
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

        if (index >= 1 && index != 2) // excluir escena perder
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
