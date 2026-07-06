using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using CompCube.AffinityPatches.MenuPatches;
using CompCube.Game;
using CompCube.Server;
using CompCube.UI;
using CompCube.UI.BSML.Components.CustomLevelBar;
using CompCube.UI.BSML.Info;
using CompCube.UI.BSML.Match;
using CompCube.UI.BSML.Menu;
using CompCube.UI.BSML.Settings;
using CompCube.UI.FlowCoordinators;
using CompCube.UI.Sound;
using CompCube.UI.ViewManagers;
using CompCube.UI.BSML.EarlyLeaveWarning;
using CompCube.UI.BSML.Match.Modal;
using Zenject;

namespace CompCube.Installers
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
            Container.BindInterfacesAndSelfTo<WaitingForMatchToStartViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<WaitingViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InfoViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<RoundResultsViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<UI.BSML.Leaderboard.CompCubeLeaderboardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<CantConnectToServerViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<CheckingServerStatusViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<MissingMapsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<OpponentViewController>().FromNewComponentAsViewController().AsSingle();

            Container.BindInterfacesAndSelfTo<PhasePopupViewController>().FromNewComponentAsViewController().AsSingle();
            
            Container.BindInterfacesAndSelfTo<ServerCheckingFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<InfoFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

            Container.BindInterfacesAndSelfTo<SoundEffectManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            
            Container.BindInterfacesAndSelfTo<GameplaySetupViewManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<StandardLevelDetailViewManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            Container.BindInterfacesAndSelfTo<BeatmapDifficultySegmentedControlPatch>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<MapDownloader>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialServerChecker>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<MatchResultsViewController>().FromNewComponentAsViewController()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<WarningModalViewController>().FromNewComponentAsViewController()
                .AsSingle();

            Container.Bind<BSMLTag>().To<LevelBarTag>().AsSingle();
            Container.Bind<TypeHandler<CustomLevelBar>>().To<LevelBarHandler>().AsSingle();
        }
    }
}