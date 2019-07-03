using System;

namespace ExampleOfCSConfig
{
    [Serializable]
    class Config
    {
        public string ServiceUrl = "https://service.test";

        public Config()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ServiceUrl = "https://service.windows.test";
            }
        }
    }
}
