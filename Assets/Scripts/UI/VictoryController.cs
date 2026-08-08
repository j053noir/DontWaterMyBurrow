using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    public class VictoryController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private Button _mainMenuButton;
        private Button _quitButton;

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
                    @event.NewState == GameState.Victory ? DisplayStyle.Flex : DisplayStyle.None;

        }

        private void BindButtons()
        {
            if (_menuDocument == null && !TryGetComponent(out _menuDocument)) Debug.LogError("VictoryController: No UIDocument found");

            _root = _menuDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            _mainMenuButton = _root.Q<VisualElement>("main-menu-button")?.Q<Button>("button");
            if (_mainMenuButton != null) _mainMenuButton.clicked += OnMainMenuButtonPressed;

            _quitButton = _root.Q<VisualElement>("quit-button")?.Q<Button>("button");
            if (_quitButton != null) _quitButton.clicked += OnQuitButtonPressed;
        }

        private void UnbindButtons()
        {
            if (_mainMenuButton != null) _mainMenuButton.clicked -= OnMainMenuButtonPressed;
            if (_quitButton != null) _quitButton.clicked -= OnQuitButtonPressed;
        }

        public void OnMainMenuButtonPressed()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }

        public void OnQuitButtonPressed()
        {
            EventBus.Clear();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}