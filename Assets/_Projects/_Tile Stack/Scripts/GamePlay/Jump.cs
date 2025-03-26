using DG.Tweening;
using hcore.Tool;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [Button]
    public void JumpUp()
    {
        transform.DOJump(_target.position, 1, 1, 0.5f);
    }
}