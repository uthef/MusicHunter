namespace Uthef.MusicHunter.Filters
{
    public interface IMusicHunterFilter
    {
        public int Limit { get; }
        public bool IsItemValid(SearchItem item, ItemType type);
    }
}
