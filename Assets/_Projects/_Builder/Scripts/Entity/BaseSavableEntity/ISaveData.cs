public interface ISaveData
{
    string Id { get; }
    void Serialize();
    void Deserialize();
}