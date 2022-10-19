namespace Uthef.MusicResolver
{
    public class ExceptionView
    {
        public string Message { get; }
        public MusicService Service { get; }

        public ExceptionView(string message, MusicService service)
        {
            Message = message;
            Service = service;
        }
    }
}
