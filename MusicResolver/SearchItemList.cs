using System.Collections.Immutable;

namespace Uthef.MusicResolver
{
    public class SearchItemList : List<SearchItem>
    {
        public ItemType Type { get; }

        public SearchItemList(ItemType type)
        {
            Type = type;
        }
    }
}
