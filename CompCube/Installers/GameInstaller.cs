using System.Threading.Tasks;
using CompCube.AffinityPatches.EnergyPatches;
using CompCube.AffinityPatches.PausePatches;
using CompCube.AffinityPatches.ScorePatches;
using CompCube.Game;
using CompCube.UI.BSML.PauseMenu;
using CompCube.AffinityPatches;
using Zenject;

namespace CompCube.Installers
{
    public class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (!Container.Resolve<MatchManager>().InMatch) 
                return;
            
            Container.BindInterfacesAndSelfTo<MatchStartUnpauseController>().AsSingle();
            Container.BindInterfacesAndSelfTo<PauseMenuViewController>().FromNewComponentAsViewController().AsSingle();
            
            // affinity patches
            Container.BindInterfacesAndSelfTo<PausePatch>().AsSingle();
            Container.BindInterfacesAndSelfTo<PauseMenuStartPatch>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContinuePausePatch>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<EnergyBarInitPatch>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<ScoreDisplayPatch>().AsSingle();
            Container.BindInterfacesAndSelfTo<ImmediateRankDisplayPatch>().AsSingle();
        }
    }
}