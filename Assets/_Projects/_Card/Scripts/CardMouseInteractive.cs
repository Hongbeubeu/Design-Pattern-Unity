using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InteractiveObject))]
public class CardMouseInteractive : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _animationSpeed = 30f;
    private Vector3 _targetPosition = Vector3.zero;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetPosition = Vector3.up;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetPosition = Vector3.zero;
    }

    private void Update()
    {
        _target.localPosition = Vector3.Lerp(_target.localPosition, _targetPosition, Time.deltaTime * _animationSpeed);
    }
}