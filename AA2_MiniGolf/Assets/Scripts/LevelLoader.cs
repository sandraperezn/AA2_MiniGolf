using System.Collections;
using Collisions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

[RequireComponent(typeof(CustomCollider))]
public class LevelLoader : MonoBehaviour
{
    private CustomCollider winTrigger;

    private void OnWinTriggerEnter()
    {
        if (PhysicsManager.Instance.BallVelocity.magnitude < 5f)
        {
            StartCoroutine(Win());
        }
    }

    private IEnumerator Win()
    {
        yield return new WaitForSeconds(1f);

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