using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class Ball : MonoBehaviour
    {
        public float speed = 5f;
        public float launchDelay = 1f; // Delay before launching the ball

        private float screenTop = 527;
        private float screenBottom = -527;

        private Vector2 direction;
        private Vector2 defaultDirection;

        private bool ballActive;
        private bool isLaunched = false; // Tracks whether the ball has been launched

        protected RectTransform rectTransform;
        private AudioSource bounceSFX;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            // Remove this line to avoid setting the position again in Start
            // rectTransform.anchoredPosition = Vector2.zero;

            // Load saved ball speed from PlayerPrefs
            if (PlayerPrefs.HasKey("BallSpeed"))
            {
                speed = PlayerPrefs.GetFloat("BallSpeed");
            }

            // Load saved ball size from PlayerPrefs
            if (PlayerPrefs.HasKey("BallSize"))
            {
                var value = PlayerPrefs.GetFloat("BallSize");
                rectTransform.sizeDelta = new Vector2(value, value);
            }

            // Load saved pitch direction from PlayerPrefs and set the ball's initial direction
            if (PlayerPrefs.HasKey("PitchDirection"))
            {
                string pitchDirectionValue = PlayerPrefs.GetString("PitchDirection");

                if (pitchDirectionValue == "Random")
                {
                    // Generate a random direction between -1 and 1 for the x-axis.
                    float randomX = Random.Range(-1f, 1f);
                    direction = new Vector2(randomX, 0f).normalized;
                }
                else if (pitchDirectionValue == "Right")
                {
                    // Set the direction to move right.
                    direction = new Vector2(1f, 0f);
                }
                else
                {
                    // Default to moving left if the value is not recognized.
                    direction = new Vector2(-1f, 0f);
                }
            }
            else
            {
                // Default to moving left if no value is found in PlayerPrefs.
                direction = new Vector2(-1f, 0f);
            }

            defaultDirection = direction;

            SetHeightBounds();

            bounceSFX = GetComponent<AudioSource>();

            // Start the coroutine to launch the ball after a delay
            StartCoroutine(LaunchBallAfterDelay(launchDelay));
        }

        private void Update()
        {
            if (ballActive && isLaunched) // Only move the ball if it's active and launched
            {
                Vector2 newPosition = rectTransform.anchoredPosition + (direction * speed * Time.deltaTime);
                rectTransform.anchoredPosition = newPosition;

                if (rectTransform.anchoredPosition.y >= screenTop || rectTransform.anchoredPosition.y <= screenBottom)
                {
                    direction.y *= -1f;
                    PlayBounceSound();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Paddle"))
            {
                Paddle paddle = collision.gameObject.GetComponent<Paddle>();
                float y = BallHitPaddleWhere(GetPosition(), paddle.AnchorPos(), paddle.GetComponent<RectTransform>().sizeDelta.y / 2f);
                Vector2 newDirection = new Vector2(paddle.isLeftPaddle ? 1f : -1f, y);
                Reflect(newDirection);
                PlayBounceSound();
            }
            else if (collision.gameObject.CompareTag("Goal"))
            {
                // Debug.Log("pos: " + rectTransform.anchoredPosition.x);
                // Left goal
                if (rectTransform.anchoredPosition.x < -1)
                {
                    ScoreManager.Instance.ScorePointPlayer2();
                }
                // Right goal
                else
                {
                    ScoreManager.Instance.ScorePointPlayer1();
                }
            }
        }

        public void Reflect(Vector2 newDirection)
        {
            direction = newDirection.normalized;
        }

        public void SetBallActive(bool value)
        {
            ballActive = value;
            direction = defaultDirection;
        }

        public Vector2 GetPosition()
        {
            return rectTransform.anchoredPosition;
        }

        public void SetHeightBounds()
        {
            var height = UIScaler.Instance.GetUIHeightPadded();
            screenTop = height / 2;
            screenBottom = -1 * height / 2;
        }

        protected float BallHitPaddleWhere(Vector2 ball, Vector2 paddle, float paddleHeight)
        {
            return (ball.y - paddle.y) / paddleHeight;
        }

        void PlayBounceSound()
        {
            bounceSFX.pitch = Random.Range(.8f, 1.2f);
            bounceSFX.Play();
        }

        public void DisableBall()
        {
            ballActive = false;
        }

        // Coroutine to launch the ball after a delay
        private IEnumerator LaunchBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay
            isLaunched = true; // Set the ball as launched
            SetBallActive(true); // Activate the ball
        }
    }
}
