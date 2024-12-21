using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct SequentialPopupGroup
{
    public ParallelPopupGroup parallelPopupGroupGroup;
    public float openDelay;
    public float closeDelay;
}

[Serializable]
public struct ParallelPopupGroup
{
    public Popup[] parallelPopups;
}

public sealed class CompositePopup : MonoBehaviour, IPopup
{
    [SerializeField]
    private SequentialPopupGroup[] _sequentialPopupGroups;

    private bool _isAnimating;
    private bool _isOpen;


    public void OpenPopup()
    {
        StartCoroutine(DoOpen());
    }

    public void ClosePopup()
    {
        StartCoroutine(DoClose());
    }

    public void PrepareOpenPopup()
    {
        foreach (var sequentialPopupGroup in _sequentialPopupGroups)
        {
            var parallelPopups = sequentialPopupGroup.parallelPopupGroupGroup.parallelPopups;
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

            var parallelPopups = sequentialPopupGroup.parallelPopupGroupGroup.parallelPopups;
            foreach (var popup in parallelPopups)
            {
                popup.OpenPopup();
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

            var parallelPopups = sequentialPopupGroup.parallelPopupGroupGroup.parallelPopups;
            foreach (var popup in parallelPopups)
            {
                popup.ClosePopup();
            }
        }

        _isOpen = false;
    }

    [Button("Toggle Popups")]
    public void TogglePopup()
    {
        StopAllCoroutines();

        if (_isOpen)
        {
            ClosePopup();
        }
        else
        {
            OpenPopup();
        }
    }
}