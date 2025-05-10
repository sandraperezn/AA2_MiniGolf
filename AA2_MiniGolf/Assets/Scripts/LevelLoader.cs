using UnityEngine;

[RequireComponent(typeof(CustomCollider))]
public class LevelLoader : MonoBehaviour
{
    private CustomCollider winTrigger;

    private void OnWinTriggerEnter()
    {
        print("collided with win trigger");
        // TODO: Load the next level
    }

    private void Awake() => winTrigger = GetComponent<CustomCollider>();

    private void OnEnable()
    {
        winTrigger.OnTriggerEnterEvent += OnWinTriggerEnter;
    }

    private void OnDisable()
    {
        winTrigger.OnTriggerEnterEvent -= OnWinTriggerEnter;
    }
}