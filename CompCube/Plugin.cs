using CompCube.Configuration;
using CompCube.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace CompCube
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }

        internal static IPALogger Log;
        
        internal static PluginConfig _config;

        [Init]
        public void Init(Zenjector zenjector, IPALogger logger, Config config)
        {
            Instance = this;
            Log = logger;
            zenjector.UseHttpService();
            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();

            _config = config.Generated<PluginConfig>();

            zenjector.Install<AppInstaller>(Location.App, _config);
            zenjector.Install<MenuInstaller>(Location.Menu);
            zenjector.Install<GameInstaller>(Location.StandardPlayer);
            
            if (_config.SkipServerCertificateValidation)
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }
    }
}