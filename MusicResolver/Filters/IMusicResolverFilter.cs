namespace Uthef.MusicResolver.Filters
{
    public interface IMusicResolverFilter
    {
        public int Limit { get; }
        public bool IsItemValid(SearchItem item, ItemType type);
    }
}
