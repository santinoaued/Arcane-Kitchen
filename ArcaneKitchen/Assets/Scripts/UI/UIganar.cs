using UnityEngine;

public class UIganar : MonoBehaviour
{
    public GameObject winCanvas; // asignar el Canvas de ganar en el Inspector

    private void Start()
    {
        if (winCanvas != null)
            winCanvas.SetActive(false); // lo desactivamos al inicio

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // si el jugador toca el objeto
        {
            if (winCanvas != null)
                winCanvas.SetActive(true); // activamos el Canvas
            Time.timeScale = 0f; // pausa el juego
            Cursor.visible = true;     // hacemos visible el cursor
            Cursor.lockState = CursorLockMode.None; // desbloqueamos el cursor

        }
        gameObject.SetActive(false); // desactiva el trigger para que no se repita
    }
}
