using System;

namespace Uthef.MusicHunter
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
