using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class Ball : MonoBehaviour
    {
        public float speed = 5f;
        public float launchDelay = 1f;
        private float screenTop;
        private float screenBottom;
        private Vector2 direction;
        private bool ballActive;
        private bool isLaunched = false;
        private AudioSource bounceSFX;
        private Vector2 defaultDirection;
        protected RectTransform rectTransform; // Ensuring accessibility to derived classes


        protected Vector2 RetrievePosition() // Changed to protected
        {
            return rectTransform.anchoredPosition;
        }
        public void Reflect(Vector2 newDirection) // Ensure this is accessible
        {
            direction = newDirection.normalized;
        }
        protected float BallHitPaddleWhere(Vector2 ball, Vector2 paddle, float paddleHeight) // Ensure accessibility
        {
            return (ball.y - paddle.y) / paddleHeight;
        }
        protected Vector2 GetPosition() // Ensure this is protected or public
        {
            return rectTransform.anchoredPosition;
        }
        public void DisableBall()
        {
            // Your logic for disabling the ball
            gameObject.SetActive(false);
        }

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            bounceSFX = GetComponent<AudioSource>();
        }

        void Start()
        {
            if (PlayerPrefs.HasKey("PitchDirection"))
            {
                string pitchDirectionValue = PlayerPrefs.GetString("PitchDirection");
                if (pitchDirectionValue == "Random")
                {
                    float randomX = Random.Range(-1f, 1f);
                    direction = new Vector2(randomX, 0f).normalized;
                }
                else if (pitchDirectionValue == "Right")
                {
                    direction = new Vector2(1f, 0f);
                }
                else
                {
                    direction = new Vector2(-1f, 0f);
                }
            }
            else
            {
                direction = new Vector2(-1f, 0f);
            }

            defaultDirection = direction; // Set defaultDirection after initializing direction

            SetHeightBounds();
            StartCoroutine(LaunchBallAfterDelay(launchDelay)); // Start the launch coroutine
        }

        void Update()
        {
            if (ballActive && isLaunched)
            {
                Vector2 newPosition = rectTransform.anchoredPosition + (direction * speed * Time.deltaTime);
                rectTransform.anchoredPosition = newPosition;
                CheckBounds();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HandleCollision(collision);
        }

        public void SetBallActive(bool value)
        {
            ballActive = value;
            if (value)
            {
                direction = defaultDirection;
            }
            Debug.Log($"Ball Active: {ballActive}, Direction: {direction}");
        }

        public void SetPosition(Vector2 newPosition)
        {
            rectTransform.anchoredPosition = newPosition;
        }

        public void SetHeightBounds()
        {
            var height = UIScaler.Instance.GetUIHeightPadded();
            screenTop = height / 2;
            screenBottom = -1 * height / 2;
        }

        private void CheckBounds()
        {
            if (rectTransform.anchoredPosition.y >= screenTop || rectTransform.anchoredPosition.y <= screenBottom)
            {
                direction.y *= -1f;
                PlayBounceSound();
            }
        }

        private void HandleCollision(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Paddle"))
            {
                Paddle paddle = collision.gameObject.GetComponent<Paddle>();

                Vector2 ballPosition = RetrievePosition(); // Use RetrievePosition
                float y = BallHitPaddleWhere(ballPosition, paddle.AnchorPos(),
                    paddle.GetComponent<RectTransform>().sizeDelta.y / 2f);

                Vector2 newDirection = new Vector2(paddle.isLeftPaddle ? 1f : -1f, y);
                Reflect(newDirection);
                PlayBounceSound();
            }
            else if (collision.gameObject.CompareTag("Goal"))
            {
                if (rectTransform.anchoredPosition.x < -1)
                    ScoreManager.Instance.ScorePointPlayer2();
                else
                    ScoreManager.Instance.ScorePointPlayer1();
            }
        }

        private void PlayBounceSound()
        {
            bounceSFX.pitch = Random.Range(.8f, 1.2f);
            bounceSFX.Play();
        }

        public IEnumerator LaunchBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            isLaunched = true;
            SetBallActive(true);
        }
    }
}
