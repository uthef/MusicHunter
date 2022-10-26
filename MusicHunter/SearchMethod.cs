namespace Uthef.MusicHunter
{
    internal delegate Task<SearchItemList> SearchMethod(string query, 
        ItemType itemType, 
        int limit = MusicHunter.DefaultLimit, 
        CancellationToken cancellationToken = default);
}
