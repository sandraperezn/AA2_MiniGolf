using UnityEngine;

[RequireComponent(typeof(CustomCollider))]
public class LevelLoader : MonoBehaviour
{
    private CustomCollider winTrigger;

    private void OnWinTriggerEnter()
    {
        // TODO: Load the next level
        AudioManager.Instance.PlaySfx(AudioManager.SfxType.Goal);
    }
    
    private void Awake() => winTrigger = GetComponent<CustomCollider>();
    private void OnEnable() => winTrigger.OnTriggerEnterEvent += OnWinTriggerEnter;
    private void OnDisable() => winTrigger.OnTriggerEnterEvent -= OnWinTriggerEnter;
}