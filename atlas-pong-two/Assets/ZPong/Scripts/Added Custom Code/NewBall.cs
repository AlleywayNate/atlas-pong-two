using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class NewBall : MonoBehaviour
    {
        public float speed = 5f;
        public float launchDelay = 1f; // Delay before launching the ball

        private Vector2 direction;
        private bool ballActive;
        private bool isLaunched = false;
        private RectTransform rectTransform;
        private AudioSource bounceSFX;
        private NewPaddle playerPaddle; // Reference to the player paddle

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            bounceSFX = GetComponent<AudioSource>();
            playerPaddle = FindObjectOfType<NewPaddle>(); // Find the player paddle in the scene

            if (playerPaddle != null)
            {
                SetBallStartPosition();
                var paddlePosition = playerPaddle.AnchorPos();
                rectTransform.anchoredPosition = paddlePosition;
            Debug.Log("Paddle Position: " + paddlePosition);
            Debug.Log("Ball Start Position: " + rectTransform.anchoredPosition);
            }

            StartCoroutine(LaunchBallAfterDelay(launchDelay)); // Launch the ball after a delay
        }

        private void Update()
        {
            if (ballActive && isLaunched)
            {
                Vector2 newPosition = rectTransform.anchoredPosition + (direction * speed * Time.deltaTime);
                rectTransform.anchoredPosition = newPosition;
            }
        }

        private void SetBallStartPosition()
        {
            rectTransform.anchoredPosition = new Vector2(playerPaddle.AnchorPos().x, playerPaddle.AnchorPos().y);
        }

        public IEnumerator LaunchBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            direction = new Vector2(1f, 0f); // Set initial direction to the right
            isLaunched = true;
            SetBallActive(true);
        }

        public void SetBallActive(bool value)
        {
            ballActive = value;
        }
    }

}
