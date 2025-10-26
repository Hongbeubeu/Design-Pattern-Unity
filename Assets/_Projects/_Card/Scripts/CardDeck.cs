using System.Collections.Generic;
using hcore.Tool;
using UnityEngine;

[RequireComponent(typeof(CardLayoutStrategy))]
public class CardDeck : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] private SpriteRenderer _preview;
    [SerializeField] private Sprite _previewBackFace;
    
    [Header("Deck Config")] 
    [SerializeField] private CardPool _cardPool;
    [SerializeField, Range(0, 52)] private int _deckSize = 10;
    
    [Header("Layout")] 
    [SerializeField] private CardLayoutStrategy _layoutStrategy;
    
    [Header("Animation")] 
    [SerializeField] private float _animationDuration = 0.5f;
    
    [Header("Spawn/Despawn")] 
    [SerializeField] private Vector3 _spawnPoint = new(-10f, -10f, 0);
    [SerializeField] private Vector3 _despawnPoint = new(10f, -10f, 0);
    
    private LayoutData _currentLayoutData;
    private bool _isDeckOpen;
    private readonly List<Card> _cards = new();
    private int _prevDeckSize;
    
    private void Start()
    {
        InitializeDeck();
        RecalculateAndApplyLayout(immediate: true);
    }

    private void InitializeDeck()
    {
        _cards.Clear();

        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        for (var i = 0; i < _deckSize; i++)
        {
            var card = _cardPool.GetRandomCardFromPool();
            card.transform.SetParent(transform);
            card.name = $"Card_{i}";
            card.SetOrderInLayer(i);
            _cards.Add(card);
            card.OnCardClicked += RemoveCard;
            card.OnMouseEnter += OnCardMouseEnter;
            card.OnMouseExit += OnCardMouseExit;
        }

        _prevDeckSize = _deckSize;
    }

    private void OnCardMouseExit(Card c)
    {
        _preview.sprite = _previewBackFace;
    }

    private void OnCardMouseEnter(Card c)
    {
        _preview.sprite = c.GetFrontSprite();
    }

    private void UpdateSortingOrder()
    {
        for (var i = 0; i < _cards.Count; i++)
        {
            _cards[i].SetOrderInLayer(i);
        }
    }

    private void RemoveCardAt(int index)
    {
        if (index < 0 || index >= _cards.Count)
        {
            return;
        }

        var cardToRemove = _cards[index];
        _cards.RemoveAt(index);

        _deckSize--;
        _prevDeckSize--;

        cardToRemove.Despawn(_despawnPoint, _animationDuration * 2f);

        UpdateSortingOrder();

        RecalculateAndApplyLayout(immediate: false);
    }

    private void RemoveCard(Card card)
    {
        var index = _cards.IndexOf(card);

        if (index != -1)
        {
            RemoveCardAt(index);
        }
    }

    private void RecalculateAndApplyLayout(bool immediate = false)
    {
        if (!_layoutStrategy) return;

        _currentLayoutData = _layoutStrategy.CalculateLayouts(_deckSize);

        ApplyTargetPositions(immediate);
    }

    private void ApplyTargetPositions(bool immediate)
    {
        var targetPositions = _isDeckOpen ? _currentLayoutData.openPositions : _currentLayoutData.closedPositions;
        var targetRotations = _isDeckOpen ? _currentLayoutData.openRotations : _currentLayoutData.closedRotations;

        if (targetPositions == null || targetPositions.Length != _cards.Count)
        {
            return;
        }

        for (var i = 0; i < _cards.Count; i++)
        {
            if (immediate)
            {
                _cards[i].transform.localPosition = targetPositions[i];
                _cards[i].transform.localRotation = targetRotations[i];
            }

            _cards[i].SetTargetPosition(targetPositions[i]);
            _cards[i].SetTargetRotation(targetRotations[i]);
        }
    }


    #region Unchanged Button Methods

    [Button]
    public void ToggleDeck()
    {
        _isDeckOpen = !_isDeckOpen;
        ApplyTargetPositions(immediate: false);
    }

    [Button]
    public void OpenDeck()
    {
        if (_isDeckOpen && Application.isPlaying) return;
        _isDeckOpen = true;
        ApplyTargetPositions(immediate: false);
    }

    [Button]
    public void CloseDeck()
    {
        if (!_isDeckOpen && Application.isPlaying) return;
        _isDeckOpen = false;
        ApplyTargetPositions(immediate: false);
    }
    
    [Button]
    public void ShuffleDeck()
    {
        if (_cards.Count <= 1) return;

        for (var i = _cards.Count - 1; i > 0; i--)
        {
            var k = Random.Range(0, i + 1);

            (_cards[i], _cards[k]) = (_cards[k], _cards[i]);
        }

        UpdateSortingOrder();

        RecalculateAndApplyLayout(immediate: false);
    }

    #endregion

    #region Unchanged Update

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_cards.Count != _deckSize) InitializeDeck();
            RecalculateAndApplyLayout(immediate: true);

            return;
        }
#endif

        var needsRecalculate = false;

        if (_deckSize != _prevDeckSize)
        {
            if (_deckSize > _prevDeckSize)
            {
                var amountToAdd = _deckSize - _prevDeckSize;

                for (var i = 0; i < amountToAdd; i++)
                {
                    var card = _cardPool.GetRandomCardFromPool();
                    card.transform.SetParent(transform);
                    card.transform.localPosition = _spawnPoint;
                    card.transform.localRotation = Quaternion.identity;
                    _cards.Add(card);
                    card.OnCardClicked += RemoveCard;
                    card.OnMouseEnter += OnCardMouseEnter;
                    card.OnMouseExit += OnCardMouseExit;
                }
            }
            else
            {
                var amountToRemove = _prevDeckSize - _deckSize;

                for (var i = 0; i < amountToRemove; i++)
                {
                    if (_cards.Count == 0) break;
                    var cardToRemove = _cards[^1];
                    _cards.RemoveAt(_cards.Count - 1);
                    cardToRemove.Despawn(_despawnPoint, _animationDuration * 2f);
                }
            }

            _prevDeckSize = _deckSize;
            UpdateSortingOrder();
            needsRecalculate = true;
        }

        if (_layoutStrategy&& _layoutStrategy.CheckForRuntimeChanges())
        {
            needsRecalculate = true;
        }

        if (needsRecalculate)
        {
            RecalculateAndApplyLayout(immediate: false);
        }
    }

    #endregion


    #region Unchanged Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_spawnPoint, 0.2f);
        Gizmos.DrawLine(_spawnPoint, _spawnPoint + Vector3.up * 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_despawnPoint, 0.2f);
        Gizmos.DrawLine(_despawnPoint, _despawnPoint + Vector3.up * 1f);
    }

    #endregion
}