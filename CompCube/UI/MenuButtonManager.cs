using BeatSaberMarkupLanguage.MenuButtons;
using CompCube.UI.FlowCoordinators;
using CompCube.Configuration;
using CompCube.Server;
using Zenject;

namespace CompCube.UI
{
    public class MenuButtonManager : IInitializable, IDisposable
    {
        [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null!;
        [Inject] private readonly MatchmakingMenuFlowCoordinator _matchmakingMenuFlowCoordinator = null!;
        [Inject] private readonly MenuButtons _menuButtons = null!;
        
        private readonly MenuButton _menuButton;
        
        public MenuButtonManager()
        {
            _menuButton = new MenuButton("CompCube", OnClick);
        }

        private void OnClick() => _mainFlowCoordinator.PresentFlowCoordinator(_matchmakingMenuFlowCoordinator);

        public void Initialize() => _menuButtons.RegisterButton(_menuButton);

        public void Dispose() => _menuButtons.UnregisterButton(_menuButton);
    }
}