using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct SequentialElementGroup
{
    public ParallelElementGroup parallelElementGroup;
    public float openDelay;
    public float closeDelay;
}

[Serializable]
public struct ParallelElementGroup
{
    public UIElement[] parallelElements;
}

[Serializable]
public class UICompositeElement : MonoBehaviour, IUIElement
{
    [SerializeField]
    private SequentialElementGroup[] _sequentialPopupGroups;

    private bool _isAnimating;
    private bool _isOpen;


    public void Show()
    {
        StartCoroutine(DoOpen());
    }

    public void Close()
    {
        StartCoroutine(DoClose());
    }

    private void PrepareOpenPopup()
    {
        foreach (var sequentialPopupGroup in _sequentialPopupGroups)
        {
            var parallelPopups = sequentialPopupGroup.parallelElementGroup.parallelElements;
            foreach (var popup in parallelPopups)
            {
                popup.PrepareOpenPopup();
            }
        }
    }

    private IEnumerator DoOpen()
    {
        PrepareOpenPopup();
        foreach (var sequentialPopupGroup in _sequentialPopupGroups)
        {
            // Wait for the delay before opening the parallel popups
            yield return new WaitForSeconds(sequentialPopupGroup.openDelay);

            var parallelPopups = sequentialPopupGroup.parallelElementGroup.parallelElements;
            foreach (var popup in parallelPopups)
            {
                popup.Show();
            }
        }

        _isOpen = true;
    }

    private IEnumerator DoClose()
    {
        foreach (var sequentialPopupGroup in _sequentialPopupGroups)
        {
            // Wait for the delay before closing the parallel popups
            yield return new WaitForSeconds(sequentialPopupGroup.closeDelay);

            var parallelPopups = sequentialPopupGroup.parallelElementGroup.parallelElements;
            foreach (var popup in parallelPopups)
            {
                popup.Close();
            }
        }

        _isOpen = false;
    }

    [Button("Toggle Popups")]
    public void Toggle()
    {
        StopAllCoroutines();

        if (_isOpen)
        {
            Close();
        }
        else
        {
            Show();
        }
    }
}