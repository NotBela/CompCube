using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using LoungeSaber.AffinityPatches.MenuPatches;
using LoungeSaber.Game;
using LoungeSaber.Server;
using LoungeSaber.UI;
using LoungeSaber.UI.BSML;
using LoungeSaber.UI.BSML.Components;
using LoungeSaber.UI.BSML.Components.CustomLevelBar;
using LoungeSaber.UI.BSML.Disconnect;
using LoungeSaber.UI.BSML.Events;
using LoungeSaber.UI.BSML.Info;
using LoungeSaber.UI.BSML.Leaderboard;
using LoungeSaber.UI.BSML.Match;
using LoungeSaber.UI.BSML.Menu;
using LoungeSaber.UI.BSML.Settings;
using LoungeSaber.UI.FlowCoordinators;
using LoungeSaber.UI.FlowCoordinators.Events;
using LoungeSaber.UI.Sound;
using LoungeSaber.UI.ViewManagers;
using Zenject;

namespace LoungeSaber.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MenuButtonManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchmakingMenuFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchmakingMenuViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<VotingScreenViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AwaitingMapDecisionViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<WaitingForMatchToStartViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<AwaitMatchEndViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InfoViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchResultsViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<LoungeSaberSettingsViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<LoungeSaberLeaderboardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<CantConnectToServerViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<CheckingServerStatusViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<MissingMapsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<DisconnectedViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<OpponentViewController>().FromNewComponentAsViewController().AsSingle();
            
            Container.BindInterfacesAndSelfTo<ServerCheckingFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<InfoFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

            Container.BindInterfacesAndSelfTo<SoundEffectManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            Container.BindInterfacesAndSelfTo<DisconnectFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            
            Container.BindInterfacesAndSelfTo<EventsFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<EventsListViewController>().FromNewComponentAsViewController().AsSingle();
            
            Container.BindInterfacesAndSelfTo<EventMatchFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

            Container.BindInterfacesAndSelfTo<GameplaySetupViewManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<StandardLevelDetailViewManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            Container.BindInterfacesAndSelfTo<BeatmapDifficultySegmentedControlPatch>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<MapDownloader>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialServerChecker>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<EventWaitingOnNextMatchViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<WaitingForEventMatchFlowCoordinator>().FromNewComponentOnNewGameObject()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<RankingDataTabSwitcherViewController>().FromNewComponentAsViewController().AsSingle();

            Container.Bind<BSMLTag>().To<LevelBarTag>().AsSingle();
            Container.Bind<TypeHandler<CustomLevelBar>>().To<LevelBarHandler>().AsSingle();
        }
    }
}