using System.Collections.Generic;

public class StackOfPlates
{
    private Stack<Plate> Plates { get; }
    private readonly Plate _emptyPlate = new() { Id = -1 };

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