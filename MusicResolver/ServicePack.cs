using System.Collections.Immutable;

namespace Uthef.MusicResolver
{
    public class ServicePack
    {
        public readonly ImmutableHashSet<MusicService> Items;
        public readonly static ServicePack All;

        public ServicePack(IEnumerable<MusicService> services)
        {
            Items = services.ToImmutableHashSet();
        }

        public ServicePack(params MusicService[] services)
        {
            Items = services.ToImmutableHashSet();
        }

        static ServicePack()
        {
            All = new ServicePack(Enum.GetValues<MusicService>());
        }

        public static implicit operator ServicePack(MusicService service)
        {
            return new ServicePack(service);
        }

        public static implicit operator ServicePack(HashSet<MusicService> services)
        {
            return new ServicePack(services);
        }
    }
}
