// See https://aka.ms/new-console-template for more information
using MusicResolver;

Console.WriteLine("Hello, World!");

var result = await new MainMusicResolver(new ResolverConfiguration()).SearchBandcamp("da", ItemType.Track);
Console.WriteLine(result.Count);
