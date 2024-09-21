using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private readonly StackOfPlates[,] _grid;
    private readonly int _size;
    private readonly Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public Board(int size)
    {
        _size = size;
        _grid = new StackOfPlates[size, size];
    }

    public void PlaceStack(Vector2Int position, StackOfPlates stack)
    {
        _grid[position.x, position.y] = stack;
    }

    private List<(Vector2Int, Vector2Int)> GetMoveableNeighbours(Vector2Int currentPosition)
    {
        var moveableNeighbours = new List<(Vector2Int, Vector2Int)>();
        foreach (var dir in _directions)
        {
            var neighbourPosition = currentPosition + dir;
            if (IsEmptyCell(currentPosition) || !IsWithinBounds(neighbourPosition) || IsEmptyCell(neighbourPosition))
            {
                continue;
            }

            if (_grid[currentPosition.x, currentPosition.y].TopPlate().Id == _grid[neighbourPosition.x, neighbourPosition.y].TopPlate().Id)
            {
                moveableNeighbours.Add((currentPosition, neighbourPosition));
                moveableNeighbours.Add((neighbourPosition, currentPosition));
            }
        }

        return moveableNeighbours;
    }

    public List<MoveData> FindBestMoveSequence(Vector2Int currentPosition)
    {
        // If the current position is empty, return an empty list
        var resultMoveSequence = new List<MoveData>();

        if (IsEmptyCell(currentPosition)) return resultMoveSequence;

        var toVisits = new GenericStack<Vector2Int>();
        var branchPositions = new Stack<Vector2Int>(); // Store the positions from which the sequence branches
        var currentMoveSequence = new List<MoveData>();

        // Initialize the stack with the current position and its neighbours
        var moveableNeighbours = GetMoveableNeighbours(currentPosition);
        toVisits.PushRange(moveableNeighbours);

        // Find the best move sequence
        while (toVisits.Count > 0)
        {
            resultMoveSequence = CalculateSequence(toVisits, currentMoveSequence, branchPositions, resultMoveSequence);
        }

        // Revert all moves
        RevertAllMoves(currentMoveSequence);

        return resultMoveSequence;
    }

    private List<MoveData> CalculateSequence(GenericStack<Vector2Int> toVisits, List<MoveData> currentMoveSequence, Stack<Vector2Int> branchPositions, List<MoveData> resultMoveSequence)
    {
        var (from, to) = toVisits.Pop();
        if (_grid[from.x, from.y].TopPlate().Id != _grid[to.x, to.y].TopPlate().Id) return resultMoveSequence;

        var movedPlates = _grid[from.x, from.y].MoveMatchingPlatesTo(_grid[to.x, to.y]);
        currentMoveSequence.Add(new MoveData(from, to, movedPlates.Count));

        // Check if the current position has a branch
        var fromStackTemp = GetMoveableNeighbours(from);
        var hasBranch = fromStackTemp.Count > 0;

        // Check if the destination position has a neighbour with the same ID
        var moveableNeighboursTo = GetMoveableNeighbours(to);
        var canMoveTo = moveableNeighboursTo.Count > 0;
        toVisits.PushRange(moveableNeighboursTo);


        // If the current position has a branch and the destination position has a neighbour with the same ID,
        // push the branch position to the stack
        if (hasBranch && canMoveTo)
        {
            branchPositions.Push(from);
        }

        // If the current position has a branch but the destination position does not have a neighbour with the same ID,
        // then let find the next move from the branch position
        if (hasBranch && !canMoveTo)
        {
            toVisits.PushRange(fromStackTemp);
        }

        // If the current position does not have a branch,
        // the destination position does not have a neighbour with the same ID,
        // and there are branch positions available, then pop the branch position
        var canMove = hasBranch || canMoveTo || branchPositions.Count > 0;
        if (!hasBranch && !canMoveTo && branchPositions.Count > 0)
        {
            var branchPosition = branchPositions.Pop();

            var moveableNeighboursBranch = GetMoveableNeighbours(branchPosition);
            toVisits.PushRange(moveableNeighboursBranch);
        }

        // If there are no more moves,
        // check if the current sequence is the best sequence
        if (canMove) return resultMoveSequence;

        // If there are no more moves, check if the current sequence is the best sequence
        if (currentMoveSequence.Count > resultMoveSequence.Count)
        {
            resultMoveSequence = new List<MoveData>(currentMoveSequence);
        }

        // Revert the last move
        RevertLastMoves(currentMoveSequence);
        return resultMoveSequence;
    }

    private void RevertAllMoves(List<MoveData> moveSequence)
    {
        while (moveSequence.Count > 0)
        {
            var lastMove = moveSequence[^1];
            RevertMove(lastMove);
            moveSequence.RemoveAt(moveSequence.Count - 1);
        }
    }

    private void RevertLastMoves(List<MoveData> moveSequence)
    {
        if (moveSequence.Count == 0) return;
        var lastMove = moveSequence[^1];
        RevertMove(lastMove);
        moveSequence.RemoveAt(moveSequence.Count - 1);
    }

    private void RevertMove(MoveData moveData)
    {
        for (var i = 0; i < moveData.numberMoved; i++)
        {
            var plate = _grid[moveData.to.x, moveData.to.y].PopPlate();
            _grid[moveData.from.x, moveData.from.y].PushPlate(plate);
        }
    }

    private bool IsEmptyCell(Vector2Int position)
    {
        return _grid[position.x, position.y] == null || _grid[position.x, position.y].IsEmpty();
    }

    private bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < _size && position.y >= 0 && position.y < _size;
    }
}