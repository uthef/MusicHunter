namespace Uthef.MusicHunter.Filters
{
    public interface ISearchFilter
    {
        public int Limit { get; }
        public bool IsItemValid(SearchItem item, ItemType type);
    }
}
