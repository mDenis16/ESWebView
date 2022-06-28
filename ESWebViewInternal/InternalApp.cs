using ESWebViewInternal.Configuration;

namespace ESWebViewInternal
{
    public enum StartupResult
    {
        OPEN_CONFIG_WINDOW,
        CLOSE_APPLICATION,
        OPEN_NORMAL
    }

    public interface InternalApp
    {
        public StartupResult Startup();

        public void Shutdown();
      
        public Config Config { get; set; }

        public DataDirectory Directory { get; set; }

        public Version Version { get; set; }

        public string BuidLoadUrl();

        public void PlayNativeSound(string location);



    }
}