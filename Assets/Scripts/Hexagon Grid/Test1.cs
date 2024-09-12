using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private void Start()
    {
        for (var i = -3; i < 4; i++)
        {
            for (var j = -3; j < 4; j++)
            {
                var pos = new Vector3(i, j, 0 - i - j);
                var cube = Instantiate(prefab, pos, Quaternion.identity);
                cube.SetActive(true);
            }
        }
    }
}