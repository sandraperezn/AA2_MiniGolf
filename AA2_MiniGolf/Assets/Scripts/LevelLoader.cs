using Collisions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

[RequireComponent(typeof(CustomCollider))]
public class LevelLoader : MonoBehaviour
{
    private CustomCollider winTrigger;

    private static void OnWinTriggerEnter()
    {
        if (PhysicsManager.Instance.BallVelocity.magnitude < 5f)
        {
            Win();
        }
    }

    private static void Win()
    {
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfx(AudioManager.SfxType.Goal);
        }

        int sceneToLoadIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (sceneToLoadIndex >= SceneManager.sceneCountInBuildSettings)
        {
            sceneToLoadIndex = 0;
        }

        SceneManager.LoadScene(sceneToLoadIndex);
    }

    private void Awake() => winTrigger = GetComponent<CustomCollider>();
    private void OnEnable() => winTrigger.OnTriggerEnterEvent += OnWinTriggerEnter;
    private void OnDisable() => winTrigger.OnTriggerEnterEvent -= OnWinTriggerEnter;
}