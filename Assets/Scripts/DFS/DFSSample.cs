using System.Collections.Generic;
using UnityEngine;

public class Plate
{
    public int Id { get; set; }
}

public class StackOfPlates
{
    public Stack<Plate> Plates { get; private set; }
    private Plate _emptyPlate = new Plate { Id = -1 };

    public StackOfPlates(IEnumerable<Plate> plates)
    {
        Plates = new Stack<Plate>(plates);
    }

    public Plate TopPlate()
    {
        return Plates.Count > 0 ? Plates.Peek() : _emptyPlate; // Return an empty plate if the stack is empty, prevents null reference exceptions
    }

    public Plate PopPlate()
    {
        return Plates.Count > 0 ? Plates.Pop() : null;
    }

    public void PushPlate(Plate plate)
    {
        Plates.Push(plate);
    }

    public bool IsEmpty()
    {
        return Plates.Count == 0;
    }

    public List<Plate> MoveMatchingPlatesTo(StackOfPlates destinationStack)
    {
        var movedPlates = new List<Plate>();

        // Get the top plate ID
        var topPlate = TopPlate();
        if (topPlate == null)
        {
            return movedPlates;
        }

        // Move all plates with the same ID from the current stack to the destination stack
        while (TopPlate() != null && TopPlate().Id == topPlate.Id)
        {
            var plate = PopPlate();
            destinationStack.PushPlate(plate);
            movedPlates.Add(plate);
        }

        return movedPlates;
    }
}

public struct MoveData
{
    public Vector2Int from;
    public Vector2Int to;
    public int numberMoved;

    public MoveData(Vector2Int from, Vector2Int to, int numberMoved)
    {
        this.from = from;
        this.to = to;
        this.numberMoved = numberMoved;
    }
}

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

    public List<MoveData> FindBestMoveSequence(Vector2Int currentPosition)
    {
        // If the current position is empty, return an empty list
        if (IsEmptyCell(currentPosition)) return new List<MoveData>();

        var branchPositions = new Stack<Vector2Int>();
        var stack = new Stack<(Vector2Int from, Vector2Int to)>();
        var moveSequence = new List<MoveData>();
        var resultSequence = new List<MoveData>();

        // Initialize the stack with the current position and its neighbours
        foreach (var dir in _directions)
        {
            var neighbourPosition = currentPosition + dir;
            if (!IsWithinBounds(neighbourPosition) || IsEmptyCell(neighbourPosition))
            {
                continue;
            }

            if (_grid[currentPosition.x, currentPosition.y].TopPlate().Id == _grid[neighbourPosition.x, neighbourPosition.y].TopPlate().Id)
            {
                stack.Push((currentPosition, neighbourPosition));
                stack.Push((neighbourPosition, currentPosition));
            }
        }

        // Find the best move sequence
        while (stack.Count > 0)
        {
            var (from, to) = stack.Pop();
            if (_grid[from.x, from.y].TopPlate().Id == _grid[to.x, to.y].TopPlate().Id)
            {
                var movedPlates = _grid[from.x, from.y].MoveMatchingPlatesTo(_grid[to.x, to.y]);
                moveSequence.Add(new MoveData(from, to, movedPlates.Count));

                var hasBranch = false;
                var canMoveTo = false;
                var fromStackTemp = new Stack<(Vector2Int from, Vector2Int to)>();

                // Check if the current position has a branch
                foreach (var dir in _directions)
                {
                    var neighbourFromPosition = from + dir;
                    if (!IsEmptyCell(from) && IsWithinBounds(neighbourFromPosition) && !IsEmptyCell(neighbourFromPosition))
                    {
                        if (_grid[from.x, from.y].TopPlate().Id == _grid[neighbourFromPosition.x, neighbourFromPosition.y].TopPlate().Id)
                        {
                            fromStackTemp.Push((from, neighbourFromPosition));
                            fromStackTemp.Push((neighbourFromPosition, from));
                            hasBranch = true;
                        }
                    }
                }

                // Check if the destination position has a neighbour with the same ID
                foreach (var dir in _directions)
                {
                    var neighbourToPosition = to + dir;
                    if (!IsEmptyCell(to) && IsWithinBounds(neighbourToPosition) && !IsEmptyCell(neighbourToPosition))
                    {
                        if (_grid[to.x, to.y].TopPlate().Id == _grid[neighbourToPosition.x, neighbourToPosition.y].TopPlate().Id)
                        {
                            stack.Push((to, neighbourToPosition));
                            stack.Push((neighbourToPosition, to));
                            canMoveTo = true;
                        }
                    }
                }


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
                    while (fromStackTemp.Count > 0)
                    {
                        var move = fromStackTemp.Pop();
                        stack.Push(move);
                    }
                }

                // If the current position does not have a branch,
                // the destination position does not have a neighbour with the same ID,
                // and there are branch positions available, then pop the branch position
                var canMove = hasBranch || canMoveTo || branchPositions.Count > 0;
                if (!hasBranch && !canMoveTo && branchPositions.Count > 0)
                {
                    var branchPosition = branchPositions.Pop();
                    foreach (var dir in _directions)
                    {
                        var neighbourBranchPosition = branchPosition + dir;
                        if (!IsEmptyCell(branchPosition) && IsWithinBounds(neighbourBranchPosition) && !IsEmptyCell(neighbourBranchPosition))
                        {
                            if (_grid[branchPosition.x, branchPosition.y].TopPlate().Id == _grid[neighbourBranchPosition.x, neighbourBranchPosition.y].TopPlate().Id)
                            {
                                stack.Push((branchPosition, neighbourBranchPosition));
                                stack.Push((neighbourBranchPosition, branchPosition));
                            }
                        }
                    }
                }

                // If there are no more moves,
                // check if the current sequence is the best sequence
                if (canMove) continue;

                // If there are no more moves, check if the current sequence is the best sequence
                if (moveSequence.Count > resultSequence.Count)
                {
                    resultSequence = new List<MoveData>(moveSequence);
                }

                // Revert the last move
                RevertLastMove(moveSequence);
            }
        }

        // Revert all moves
        while (moveSequence.Count > 0)
        {
            RevertLastMove(moveSequence);
        }

        return resultSequence;
    }

    private void RevertLastMove(List<MoveData> moveSequence)
    {
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