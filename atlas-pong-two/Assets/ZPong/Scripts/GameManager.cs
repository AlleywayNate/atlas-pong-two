using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float startDelay = 3f;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject canvasParent;

        public Ball activeBall;

        public static GameManager Instance { get; private set; }

        private Goal[] goals;

        public AIPlayer aIPlayer;

        private void Awake()
        {
            Instance = this;

            goals = new Goal[2];
        }

        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            ScoreManager.Instance.ResetGame();
            StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            SetGame();
            yield return new WaitForSeconds(startDelay);

            SetBounds();

            // Start the coroutine to launch the ball after a delay
            if (activeBall != null)
            {
                StartCoroutine(activeBall.LaunchBallAfterDelay(1f));
            }

            StartGame();
        }

        private void SetGame()
        {
            if (activeBall != null)
            {
                Destroy(activeBall.gameObject);
            }

            // Instantiate the ball and set its parent
            activeBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, canvasParent.transform)
                .GetComponent<Ball>();

            // Ensure ball's position is centered
            activeBall.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            aIPlayer.StartCoroutine(aIPlayer.StartDelay(1));
        }

        private void StartGame()
        {
            // This function starts the ball movement and game
            activeBall.SetBallActive(true);
        }

        private void SetBounds()
        {
            // Set the height bounds for the ball and goals
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
            // Reset ball's position and state
            activeBall.transform.position = Vector3.zero;
            activeBall.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            activeBall.DisableBall();

            yield return null;

            // Start the timer again after resetting
            StartCoroutine(StartTimer());
        }

        public void SetGoalObj(Goal g)
        {
            if (goals[0] == null)
            {
                goals[0] = g;
            }
            else
            {
                goals[1] = g;
            }
        }
    }
}
