using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DataLoader _dataLoader;
    [SerializeField] private ContentLoader _contentLoader;

    void Start()
    {
        _dataLoader.onDataLoaded += _contentLoader.LoadContent;
        _dataLoader.LoadData();
    }
}