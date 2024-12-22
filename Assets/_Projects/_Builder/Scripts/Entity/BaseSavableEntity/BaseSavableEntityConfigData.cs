using UnityEngine;

public abstract class BaseSavableEntityConfigData : ScriptableObject, IBaseSavableEntityConfigData
{
    [SerializeField]
    protected string _id;

    public string Id => _id;

    public virtual IEntity CreateEntity()
    {
        return null;
    }

    public virtual ISaveData CreateSaveData()
    {
        return null;
    }
}