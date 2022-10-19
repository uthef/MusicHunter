using System;

namespace Uthef.MusicResolver
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class MethodOf : Attribute
    {
        internal readonly MusicService Service;
        internal MethodOf(MusicService service)
        {
            Service = service;
        }
    }
}
