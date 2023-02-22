using System.Collections.Immutable;

namespace Uthef.MusicHunter
{
    public static class ServicePack
    {
        public static readonly MusicService All;
        public static readonly ImmutableList<MusicService> AsList = Enum.GetValues<MusicService>().ToImmutableList();

        static ServicePack()
        {
            foreach (var service in AsList) All |= service;
        }

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
