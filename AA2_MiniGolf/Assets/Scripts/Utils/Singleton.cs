using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;

                // Find existing instances
                if (FindObjectsOfType(typeof(T)) is T[] { Length: > 0 } objs)
                {
                    _instance = objs[0];

                    if (objs is { Length: > 1 })
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }

                    return _instance;
                }

                return null;
            }
        }
    }
}