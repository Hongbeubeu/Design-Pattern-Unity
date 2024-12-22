using UnityEngine;

public abstract class BaseEntityConfigData : ScriptableObject, IBaseEntityConfigData
{
    [SerializeField]
    protected string _id;

    public string Id => _id;

    public virtual IEntity CreateEntity()
    {
        return null;
    }
}