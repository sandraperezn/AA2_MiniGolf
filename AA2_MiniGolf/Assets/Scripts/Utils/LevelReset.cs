using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CustomCollider))]
public class LevelReset : MonoBehaviour
{
    private CustomCollider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<CustomCollider>();
    }

    private static void OnLevelReset()
    {
        Scene sceneToLoad = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceneToLoad.buildIndex);
    }

    private void OnEnable() => triggerCollider.OnTriggerEnterEvent += OnLevelReset;
    private void OnDisable() => triggerCollider.OnTriggerEnterEvent -= OnLevelReset;
}