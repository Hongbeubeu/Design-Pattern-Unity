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
    private StackOfPlates[,] _grid;
    private int _size;
    private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
        var stack = new Stack<(Vector2Int from, Vector2Int to)>();
        var moveSequence = new List<MoveData>();
        var resultSequence = new List<MoveData>();

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

        while (stack.Count > 0)
        {
            var (from, to) = stack.Pop();
            if (_grid[from.x, from.y].TopPlate().Id == _grid[to.x, to.y].TopPlate().Id)
            {
                var movedPlates = _grid[from.x, from.y].MoveMatchingPlatesTo(_grid[to.x, to.y]);
                moveSequence.Add(new MoveData(from, to, movedPlates.Count));
                Debug.LogError($"Move from {from} to {to}, number of plates moved: {movedPlates.Count}");

                var canMove = false;
                foreach (var dir in _directions)
                {
                    var neighbourFromPosition = from + dir;
                    if (IsWithinBounds(neighbourFromPosition) && !IsEmptyCell(neighbourFromPosition))
                    {
                        if (_grid[from.x, from.y].TopPlate().Id == _grid[neighbourFromPosition.x, neighbourFromPosition.y].TopPlate().Id)
                        {
                            stack.Push((from, neighbourFromPosition));
                            stack.Push((neighbourFromPosition, from));
                            canMove = true;
                        }
                    }

                    var neighbourToPosition = to + dir;
                    if (IsWithinBounds(neighbourToPosition) && !IsEmptyCell(neighbourToPosition))
                    {
                        if (_grid[to.x, to.y].TopPlate().Id == _grid[neighbourToPosition.x, neighbourToPosition.y].TopPlate().Id)
                        {
                            stack.Push((to, neighbourToPosition));
                            stack.Push((neighbourToPosition, to));
                            canMove = true;
                        }
                    }
                }

                if (!canMove)
                {
                    if (moveSequence.Count > resultSequence.Count)
                    {
                        resultSequence = new List<MoveData>(moveSequence);
                    }

                    var lastMove = moveSequence[^1];
                    RevertMove(lastMove);

                    moveSequence.RemoveAt(moveSequence.Count - 1);
                }
            }
        }

        return resultSequence;
    }

    private void RevertMove(MoveData moveData)
    {
        Debug.LogError($"Move back from {moveData.to} to {moveData.from}, number of plates moved: {moveData.numberMoved}");

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