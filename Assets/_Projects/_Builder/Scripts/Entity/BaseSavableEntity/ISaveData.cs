using IoC;

public interface ISaveData : IInjectable
{
    string Id { get; }
    void Serialize();
    void Deserialize();
}