using Core.GlobalEvents;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GlobalEvent _onGamePaused;
    [SerializeField] private GlobalEvent _onGameUnpaused;
    [SerializeField] private GameObject _pauseMenu;

    private void Pause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        _onGameUnpaused.Raise();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void OnEnable()
    {
        _onGamePaused.Subscribe(Pause);
    }

    private void OnDisable()
    {
        _onGamePaused.Unsubscribe(Pause);
    }
}
