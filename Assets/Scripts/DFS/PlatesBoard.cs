using System.Collections.Generic;
using UnityEngine;

public class PlatesBoard
{
    private readonly IGrid<StackOfPlates> _grid;
    private readonly int _size;

    public PlatesBoard(IGrid<StackOfPlates> grid)
    {
        _size = grid.Size;
        _grid = grid;
    }

    public void PlaceStack(Vector2Int position, StackOfPlates stack)
    {
        _grid.SetGridValue(position, stack);
    }

    private List<(Vector2, Vector2)> GetMoveableNeighbours(Vector2 currentPosition)
    {
        var moveableNeighbours = new List<(Vector2, Vector2)>();
        foreach (var dir in _grid.GetAllDirections())
        {
            var neighbourPosition = currentPosition + dir;
            if (IsEmptyCell(currentPosition) || !IsWithinBounds(neighbourPosition) || IsEmptyCell(neighbourPosition))
            {
                continue;
            }

            if (_grid.GetGridValue(currentPosition).TopPlate().Id == _grid.GetGridValue(neighbourPosition).TopPlate().Id)
            {
                moveableNeighbours.Add((currentPosition, neighbourPosition));
                moveableNeighbours.Add((neighbourPosition, currentPosition));
            }
        }

        return moveableNeighbours;
    }

    public List<MoveData> FindBestMoveSequence(Vector2 currentPosition)
    {
        // If the current position is empty, return an empty list
        var resultMoveSequence = new List<MoveData>();

        if (IsEmptyCell(currentPosition)) return resultMoveSequence;

        var toVisits = new GenericStack<Vector2>();
        var branchPositions = new Stack<Vector2>(); // Store the positions from which the sequence branches
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

    private List<MoveData> CalculateSequence(GenericStack<Vector2> toVisits, List<MoveData> currentMoveSequence, Stack<Vector2> branchPositions, List<MoveData> resultMoveSequence)
    {
        var (from, to) = toVisits.Pop();
        if (_grid.GetGridValue(from).TopPlate().Id != _grid.GetGridValue(to).TopPlate().Id) return resultMoveSequence;

        var movedPlates = _grid.GetGridValue(from).MoveMatchingPlatesTo(_grid.GetGridValue(to));
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
            var plate = _grid.GetGridValue(moveData.to).PopPlate();
            _grid.GetGridValue(moveData.from).PushPlate(plate);
        }
    }

    private bool IsEmptyCell(Vector2 position)
    {
        return _grid.GetGridValue(position) == null || _grid.GetGridValue(position).IsEmpty();
    }

    private bool IsWithinBounds(Vector2 position)
    {
        return position.x >= 0 && position.x < _size && position.y >= 0 && position.y < _size;
    }
}