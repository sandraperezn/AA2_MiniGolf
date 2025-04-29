using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project.Scripts.UI
{
    public class Scroller : MonoBehaviour
    {
        [FormerlySerializedAs("_img")] [SerializeField] private RawImage img;
        [FormerlySerializedAs("_x")] [SerializeField] private float x;
        [FormerlySerializedAs("_y")] [SerializeField] private float y;

        void Update()
        {
            img.uvRect = new Rect(img.uvRect.position + new Vector2(x, y) * Time.deltaTime, img.uvRect.size);
        }
    }
}

