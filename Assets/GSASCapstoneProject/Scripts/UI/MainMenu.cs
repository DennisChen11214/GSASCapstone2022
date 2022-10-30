///
/// Created by Dennis Chen
///

using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainMenu;
    [SerializeField]
    private GameObject _controlsMenu;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowControls()
    {
        _controlsMenu.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void BackToMenu()
    {
        _controlsMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
