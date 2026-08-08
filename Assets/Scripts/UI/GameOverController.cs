using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    public class GameOverController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private Button _restartButton;
        private Button _mainMenuButton;

        private void OnEnable()
        {
            BindButtons();

            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);

            UnbindButtons();
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (_root != null)
                _root.style.display =
                    @event.NewState == GameState.GameOver ? DisplayStyle.Flex : DisplayStyle.None;

        }

        private void BindButtons()
        {
            if (_menuDocument == null && !TryGetComponent(out _menuDocument)) Debug.LogError("GameOverController: No UIDocument found");

            _root = _menuDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            _restartButton = _root.Q<VisualElement>("restart-button")?.Q<Button>("button");
            if (_restartButton != null) _restartButton.clicked += OnRestartButtonPressed;

            _mainMenuButton = _root.Q<VisualElement>("main-menu-button")?.Q<Button>("button");
            if (_mainMenuButton != null) _mainMenuButton.clicked += OnMainMenuButtonPressed;
        }

        private void UnbindButtons()
        {
            if (_restartButton != null) _restartButton.clicked -= OnRestartButtonPressed;
            if (_mainMenuButton != null) _mainMenuButton.clicked -= OnMainMenuButtonPressed;
        }

        private void OnRestartButtonPressed()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.Restart));
        }

        private void OnMainMenuButtonPressed()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }
    }
}