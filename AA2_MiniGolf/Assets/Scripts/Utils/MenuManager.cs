using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        int sceneToLoadIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(sceneToLoadIndex);
    }
}