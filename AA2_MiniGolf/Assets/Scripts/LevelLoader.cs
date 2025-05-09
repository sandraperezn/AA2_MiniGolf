using UnityEngine;

public abstract class LevelLoader : MonoBehaviour
{
    [Header("Victory Conditions")]
    public float winVelocityThreshold = 0.5f;
    public int maxBouncesAllowed = 2;

    private int bounceCount;

    protected virtual void Start()
    {
        // Base config for level
    }

    public abstract void LoadLevel();

    public virtual void OnBallEnterGoal(Vector3 velocity)
    {
        if (velocity.magnitude < winVelocityThreshold && bounceCount <= maxBouncesAllowed)
        {
            Debug.Log("Level completed successfully!");
            // Load next level or show victory screen if applicable
        }
        else
        {
            Debug.Log("Victory conditions not met. Too many bounces.");
        }
    }

    public void RegisterBounce()
    {
        bounceCount++;
    }
}