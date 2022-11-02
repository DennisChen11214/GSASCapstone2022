using UnityEngine.SceneManagement;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
