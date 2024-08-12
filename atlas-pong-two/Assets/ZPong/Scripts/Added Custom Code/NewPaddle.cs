using UnityEngine;

namespace ZPong
{
    public class NewPaddle : MonoBehaviour
    {
        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public Vector2 AnchorPos()
        {
            return rectTransform.anchoredPosition;
        }
    }
}
