using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    public Button goBackButton;

    void Start()
    {
        if (goBackButton != null)
        {
            goBackButton.onClick.AddListener(GoToMainMenu);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MENU UI");
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad > 2f && Input.anyKeyDown)
        {
            SceneManager.LoadScene("Nivel 1 - Tutorial");
        }
    }
}