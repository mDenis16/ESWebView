using ESWebViewInternal.Configuration;

namespace ESWebViewInternal
{
    public interface InternalApp
    {
        public bool Startup();

        public void Shutdown();
      
        public Config Config { get; set; }

        public DataDirectory Directory { get; set; }

    }
}