using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float startDelay = 3f; // Adjust this to control delay before the ball starts
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

        void SetGame()
        {
            if (activeBall != null)
            {
                Destroy(activeBall.gameObject);
            }

            // Set initial position for the ball
            Vector3 initialPosition = new Vector3(100, 0, 0); // Change to desired position
            activeBall = Instantiate(ballPrefab, initialPosition, this.transform.rotation, canvasParent.transform)
                .GetComponent<Ball>();
            activeBall.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 0); // Change to desired position

            aIPlayer.StartCoroutine(aIPlayer.StartDelay(1));
        }

        void StartGame()
        {
            // Debug.Log("Starting game!");
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
            // Set reset position for the ball
            Vector3 resetPosition = new Vector3(100, 0, 0); // Change to desired position
            activeBall.transform.position = resetPosition;
            activeBall.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 0); // Change to desired position
            activeBall.SetBallActive(false);

            yield return null;

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
