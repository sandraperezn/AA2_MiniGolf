using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        int sceneToLoadIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(sceneToLoadIndex);
    }
}