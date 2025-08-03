namespace Wattle.Infrastructure
{
    public interface ISaveable
    {
        public string FileName { get; }
        public void Deserialize(string data);
        public string Serialize();
    }
}