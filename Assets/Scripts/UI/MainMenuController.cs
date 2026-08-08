using System;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private Button _startButton;
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
                    @event.NewState == GameState.MainMenu ? DisplayStyle.Flex : DisplayStyle.None;

        }

        private void BindButtons()
        {
            if (_menuDocument == null && !TryGetComponent(out _menuDocument)) Debug.LogError("MainMenuController: No UIDocument found");

            _root = _menuDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            _startButton = _root.Q<VisualElement>("start-button")?.Q<Button>("button");
            if (_startButton != null) _startButton.clicked += OnStartButtonPressed;

            _quitButton = _root.Q<VisualElement>("quit-button")?.Q<Button>("button");
            if (_quitButton != null) _quitButton.clicked += OnQuitButtonPressed;
        }

        private void UnbindButtons()
        {
            if (_startButton != null) _startButton.clicked -= OnStartButtonPressed;
            if (_quitButton != null) _quitButton.clicked -= OnQuitButtonPressed;
        }

        private void OnStartButtonPressed()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.GamePlay));
        }

        private void OnQuitButtonPressed()
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