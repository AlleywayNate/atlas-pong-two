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

        protected RectTransform rectTransform;
        private AudioSource bounceSFX;

        private Vector2 defaultDirection;
        public Ball activeBall;

        void StartGame()
        {
        activeBall.SetBallActive(true); // This should make the ball active and start moving
        }

        public void DisableBall()
        {
            ballActive = false; // This will stop the ball from moving
            // Optionally, you can also hide the ball or disable its collision if needed
            gameObject.SetActive(false); // Disables the ball GameObject
        }

        protected float BallHitPaddleWhere(Vector2 ball, Vector2 paddle, float paddleHeight)
        {
            return (ball.y - paddle.y) / paddleHeight;
        }

        public Vector2 GetPosition()
        {
            return rectTransform.anchoredPosition;
        }

        public void Reflect(Vector2 newDirection)
        {
            direction = newDirection.normalized;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            bounceSFX = GetComponent<AudioSource>();

            // Initialization of direction and defaultDirection
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
        }

        private void Update()
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
            direction = defaultDirection;
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

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey("BallSpeed"))
                speed = PlayerPrefs.GetFloat("BallSpeed");

            if (PlayerPrefs.HasKey("BallSize"))
                rectTransform.sizeDelta = new Vector2(PlayerPrefs.GetFloat("BallSize"), PlayerPrefs.GetFloat("BallSize"));

            if (PlayerPrefs.HasKey("PitchDirection"))
            {
                string pitchDirectionValue = PlayerPrefs.GetString("PitchDirection");
                direction = pitchDirectionValue == "Right" ? new Vector2(1f, 0f) : new Vector2(-1f, 0f);
            }
            else
            {
                direction = new Vector2(-1f, 0f);
            }
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
                
                Vector2 ballPosition = GetPosition();
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
