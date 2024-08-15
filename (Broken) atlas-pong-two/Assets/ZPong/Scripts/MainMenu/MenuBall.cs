using System.Collections;
using UnityEngine;

namespace ZPong
{
    public class MenuBall : Ball
    {
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Paddle"))
            {
                Paddle paddle = collision.gameObject.GetComponent<Paddle>();

                // Ensure you pass the correct number of parameters
                float y = BallHitPaddleWhere(GetPosition(), paddle.AnchorPos(), 
                    paddle.GetComponent<RectTransform>().sizeDelta.y / 2f);
                
                Vector2 newDirection = new Vector2(paddle.isLeftPaddle ? 1f : -1f, y);

                Reflect(newDirection);
            }
            else if (collision.gameObject.CompareTag("Goal"))
            {
                if (rectTransform.anchoredPosition.x < -1)
                {
                    GameManager.Instance.ResetBall();
                }
                else
                {
                    GameManager.Instance.ResetBall();
                }
            }
        }
    }
}