using UnityEngine;

public class CardPool : MonoBehaviour
{
    [SerializeField] private Sprite[] _cardSprites;
    [SerializeField] private Card _cardPrefab;

    private Card GetCardFromPool(int index)
    {
        if (index < 0 || index >= _cardSprites.Length)
        {
            Debug.LogError("CardPool: Index out of range.");

            return null;
        }

        var cardInstance = Lean.Pool.LeanPool.Spawn(_cardPrefab, Vector3.zero, Quaternion.identity);
        _cardPrefab.SetCardSprite(_cardSprites[index]);

        return cardInstance;
    }

    public Card GetRandomCardFromPool()
    {
        var randomIndex = Random.Range(0, _cardSprites.Length);

        return GetCardFromPool(randomIndex);
    }
}