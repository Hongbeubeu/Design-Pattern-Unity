using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _front;
    [SerializeField] private SpriteRenderer _back;
    [SerializeField] private float _animationSpeed = 10f;
    private Vector3 _targetPosition = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;
    
    public event Action<Card> OnCardClicked;
    public event Action<Card> OnMouseEnter;
    public event Action<Card> OnMouseExit;
    
    public void OnCardClick()
    {
        OnCardClicked?.Invoke(this);
    }
    
    public void OnCardMouseEnter()
    {
        OnMouseEnter?.Invoke(this);
    }
    
    public void OnCardMouseExit()
    {
        OnMouseExit?.Invoke(this);
    }

    public void SetCardSprite(Sprite frontSprite)
    {
        _front.sprite = frontSprite;
    }

    public void SetTargetPosition(Vector3 position)
    {
        _targetPosition = position;
    }

    public void SetTargetRotation(Quaternion rotation)
    {
        _targetRotation = rotation;
    }

    public void SetOrderInLayer(int order)
    {
        _front.sortingOrder = order;
        _back.sortingOrder = order;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            transform.localPosition = _targetPosition;
            transform.localRotation = _targetRotation;

            return;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * _animationSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _targetRotation, Time.deltaTime * _animationSpeed);
    }

    public void Despawn(Vector3 despawnPos, float destroyDelay)
    {
        SetTargetPosition(despawnPos);
        SetTargetRotation(Quaternion.Euler(0, 0, Random.Range(-45f, 45f)));

        StartCoroutine(DestroyAfterDelay(destroyDelay));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        OnCardClicked = null;
        yield return new WaitForSeconds(delay);
        transform.SetParent(null);
        Lean.Pool.LeanPool.Despawn(this);
    }

    public Sprite GetFrontSprite()
    {
        return _front.sprite;
    }
}