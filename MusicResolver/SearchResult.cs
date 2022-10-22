using System.Collections.Immutable;

namespace Uthef.MusicResolver
{
    public class SearchResult
    {
        public ItemType Type { get; }
        public SearchResult(ItemType type, ImmutableList<SearchItem> items, ImmutableList<ExceptionView> exceptions)
        {
            Type = type;
            Items = items;
            Exceptions = exceptions;
        }

        public ImmutableList<SearchItem> Items { get; }
        public ImmutableList<ExceptionView> Exceptions { get; }
    }
}
