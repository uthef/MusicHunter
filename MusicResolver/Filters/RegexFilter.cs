using System.Text.RegularExpressions;

namespace Uthef.MusicResolver
{
    public class RegexFilter : IMusicResolverFilter
    {
        public int Limit { get; } = 0;

        public readonly Regex? Artist, Title;
        public RegexFilter(Regex? artist, Regex? title, int limit = MusicResolver.DefaultLimit)
        {
            Artist = artist;
            Title = title;
            Limit = limit;
        }

        public bool IsItemValid(SearchItem item, ItemType type)
        {
            if (Artist != null && item.Artists.Find(x => Artist.IsMatch(x)) is null)
            {
                return false;
            }

            if (Title != null && !Title.IsMatch(item.Title))
            {
                return false;
            }

            return true;
        }
    }
}
