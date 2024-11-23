using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private Snake _snake;
    [SerializeField] private Board _board;

    private void Start()
    {
        _board.GenerateBoard();
        _snake.Initialize(_board);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _snake.SetDirection(Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _snake.SetDirection(Vector3.back);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _snake.SetDirection(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _snake.SetDirection(Vector3.right);
        }
    }
}