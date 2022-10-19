using System.Text.Json.Serialization;

namespace Uthef.MusicResolver
{
    public class SearchResult
    {
        public ItemType Type { get; }
        public SearchResult(ItemType type)
        {
            Type = type;
        }

        public List<SearchItem> Items { get; } = new();
        public List<ExceptionView> Exceptions { get; } = new();
    }
}
