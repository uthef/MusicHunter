using System.Collections.Immutable;

namespace Uthef.MusicHunter
{
    public class SearchResult
    {
        public ItemType Type { get; }
        public SearchResult(ItemType type, ImmutableList<SearchItem> items, ImmutableList<ExceptionView> exceptions, TimeSpan overallExecutionTime)
        {
            Type = type;
            Items = items;
            Exceptions = exceptions;
            OverallExecutionTimeMs = overallExecutionTime.TotalMilliseconds;
        }

        public double OverallExecutionTimeMs { get; }

        public ImmutableList<SearchItem> Items { get; }
        public ImmutableList<ExceptionView> Exceptions { get; }
    }
}
