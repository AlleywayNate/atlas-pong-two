using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float startDelay = 3f;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject canvasParent;
        [SerializeField] private Vector2 ballOffset; // Serialized field for the ball's offset
        [SerializeField] private Vector2 ballStartingOffset = new Vector2(0, 0);
        [SerializeField] private Transform aiPaddle;
        [SerializeField] private float launchDelay = 2f; // Delay before launching the ball



        public Ball activeBall;
        public Transform playerPaddle; // Reference to the player's paddle

        public static GameManager Instance { get; private set; }

        private Goal[] goals;

        public AIPlayer aIPlayer;

        private void Awake()
        {
            Instance = this;

            goals = new Goal[2];
        }

        void SetGame()
        {
            if (activeBall != null)
            {
                Destroy(activeBall.gameObject);
            }

            // Instantiate the ball and set its parent to the player paddle
            activeBall = Instantiate(ballPrefab, playerPaddle.position, Quaternion.identity, playerPaddle).GetComponent<Ball>();

            // Set the ball's local position relative to the paddle
            RectTransform ballRectTransform = activeBall.GetComponent<RectTransform>();
            RectTransform paddleRectTransform = playerPaddle.GetComponent<RectTransform>();

            // Offset the ball’s position relative to the paddle
            ballRectTransform.anchoredPosition = paddleRectTransform.anchoredPosition + ballOffset;

            // Ensure the ball follows the paddle's movement
            ballRectTransform.pivot = new Vector2(0.5f, 0.5f); // Optional, set pivot as needed
            ballRectTransform.anchorMin = new Vector2(0.5f, 0.5f); // Optional, set anchors as needed
            ballRectTransform.anchorMax = new Vector2(0.5f, 0.5f); // Optional, set anchors as needed
            ballRectTransform.anchoredPosition = paddleRectTransform.anchoredPosition + ballStartingOffset;


            aIPlayer.StartCoroutine(aIPlayer.StartDelay(1));

        }

        
        private IEnumerator SetGameAndLaunch()
        {
            // Instantiate the ball and set its position to the player's paddle position
            activeBall = Instantiate(ballPrefab, playerPaddle.position, Quaternion.identity, canvasParent.transform)
                        .GetComponent<Ball>();

            // Match the ball's position with the paddle's position
            RectTransform ballRectTransform = activeBall.GetComponent<RectTransform>();
            RectTransform paddleRectTransform = playerPaddle.GetComponent<RectTransform>();
            ballRectTransform.anchoredPosition = paddleRectTransform.anchoredPosition;

            yield return new WaitForSeconds(startDelay);

            LaunchBallAfterDelay(launchDelay);
        }

        private void LaunchBallAfterDelay(float delay)
        {
            StartCoroutine(LaunchBallAfterDelayCoroutine(delay));
        }

        private IEnumerator LaunchBallAfterDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Calculate direction towards AI paddle
            Vector2 directionToAI = (aiPaddle.position - activeBall.transform.position).normalized;

            // Launch the ball
            activeBall.SetBallActive(true);
            activeBall.Reflect(directionToAI);
        }
        void StartGame()
        {
            activeBall.SetBallActive(true);
        }

        public void Reset()
        {
            ScoreManager.Instance.ResetGame();

            StartCoroutine(StartTimer());
        }

        private void Start()
        {
            Reset();
        }

        IEnumerator StartTimer()
        {
            SetGame();
            yield return new WaitForSeconds(startDelay);

            SetBounds();

            StartGame();
        }

        void SetBounds()
        {
            activeBall.SetHeightBounds();
            foreach (var g in goals)
            {
                g.SetHeightBounds();
            }
        }

        public void ResetBall()
        {
            StartCoroutine(ResetBallCoroutine());
        }

        private IEnumerator ResetBallCoroutine()
        {
            // Reset the ball's position to the player's paddle position
            activeBall.transform.position = playerPaddle.position;

            // Reset the anchored position to align with the paddle in the UI
            activeBall.GetComponent<RectTransform>().anchoredPosition = playerPaddle.GetComponent<RectTransform>().anchoredPosition;

            // Disable the ball's movement until the game restarts
            activeBall.SetBallActive(false);

            yield return null;

            // Restart the game after the delay
            StartCoroutine(StartTimer());
        }
        
        public void SetGoalObj(Goal g)
        {
            if (goals[0])
            {
                goals[1] = g;
            }
            else
            {
                goals[0] = g;
            }
        }
    }
}
