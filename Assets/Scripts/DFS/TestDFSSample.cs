using System.Collections.Generic;
using UnityEngine;

public class TestDFSSample : MonoBehaviour
{
    private void Start()
    {
        // Create a board of size 5x5
        var board = new PlatesBoard(new GenericSquareGrid<StackOfPlates>(3));

        // Create some stacks of plates
        var stack1 = new StackOfPlates(new List<Plate> { new() { Id = 1 }, new() { Id = 1 } });
        var stack2 = new StackOfPlates(new List<Plate> { new() { Id = 1 } });
        var stack3 = new StackOfPlates(new List<Plate> { new() { Id = 2 }, new() { Id = 1 } });
        var stack4 = new StackOfPlates(new List<Plate> { new() { Id = 1 }, new() { Id = 2 } });
        var stack5 = new StackOfPlates(new List<Plate> { new() { Id = 1 } });
        var stack6 = new StackOfPlates(new List<Plate> { new() { Id = 2 } });

        // Place stacks on the board
        board.PlaceStack(new Vector2Int(0, 0), stack1);
        board.PlaceStack(new Vector2Int(0, 1), stack2);
        board.PlaceStack(new Vector2Int(1, 0), stack3);
        board.PlaceStack(new Vector2Int(2, 0), stack4);
        board.PlaceStack(new Vector2Int(2, 1), stack5);
        board.PlaceStack(new Vector2Int(1, 1), stack6);

        // Find the best move sequence starting from position (0, 0)
        var bestMoveSequence = board.FindBestMoveSequence(new Vector2Int(0, 0));

        // Print the results
        foreach (var move in bestMoveSequence)
        {
            Debug.Log($"Move from {move.from} to {move.to}, number of plates moved: {move.numberMoved}");
        }
    }
}