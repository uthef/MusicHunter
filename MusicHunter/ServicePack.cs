using System.Collections.Immutable;

namespace Uthef.MusicHunter
{
    public static class ServicePack
    {
        public static readonly ImmutableList<MusicService> AsList = Enum.GetValues<MusicService>().ToImmutableList();
        public static readonly MusicService All = (MusicService)Math.Pow(2, AsList.Count) - 1;

        public static int Count(MusicService services)
        {
            var count = 0;

            foreach (var service in AsList)
            {
                if ((service & services) != 0) count++;
            }

            return count;
        }

    }
}
