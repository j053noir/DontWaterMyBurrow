using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseMenuController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private Button _resumeButton;
        private Button _mainMenuButton;

        /// <summary>
        /// Stores the previous state of the game, excluding pause.
        /// </summary>
        private GameState _previousGameState;

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
                    @event.NewState == GameState.Pause ? DisplayStyle.Flex : DisplayStyle.None;

            if (@event.NewState != GameState.Pause)
            {
                _previousGameState = @event.NewState;
            }
        }

        private void BindButtons()
        {
            if (_menuDocument == null && !TryGetComponent(out _menuDocument)) Debug.LogError("PauseMenuController: No UIDocument found");

            _root = _menuDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            _resumeButton = _root.Q<VisualElement>("resume-button")?.Q<Button>("button");
            if (_resumeButton != null) _resumeButton.clicked += OnResumeButtonPressed;

            _mainMenuButton = _root.Q<VisualElement>("main-menu-button")?.Q<Button>("button");
            if (_mainMenuButton != null) _mainMenuButton.clicked += OnMainMenuButtonPressed;
        }

        private void UnbindButtons()
        {
            if (_resumeButton != null) _resumeButton.clicked -= OnResumeButtonPressed;
            if (_mainMenuButton != null) _mainMenuButton.clicked -= OnMainMenuButtonPressed;
        }

        private void OnResumeButtonPressed()
        {
            EventBus.Publish(new GameStateChangedEvent(_previousGameState));
        }

        private void OnMainMenuButtonPressed()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }
    }
}