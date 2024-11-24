using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private Snake _snake;
    [SerializeField] private BoardController _boardController;

    private void Start()
    {
        _snake.Initialize(_boardController);
        _boardController.Initialize(_snake);
        _boardController.GenerateBoard();
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