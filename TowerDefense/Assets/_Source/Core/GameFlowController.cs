namespace Core
{
    using System;
using System.Collections.Generic;
using EnemySystem;
using UnityEngine;

namespace Core
{
    public class GameFlowController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private MinigameHost minigameHost;
        [SerializeField] private MinigameConfig lastWaveMinigame;

        private readonly Dictionary<GamePhase, IGameState> _states = new();
        private readonly List<IGameplayPausable> _pausables = new();
        private IGameState _current;

        public GamePhase CurrentPhase { get; private set; } = GamePhase.Playing;
        public event Action<GamePhase, GamePhase> OnPhaseChanged;

        public void RegisterPausable(IGameplayPausable p)
        {
            if (p != null && !_pausables.Contains(p)) _pausables.Add(p);
        }

        private void Awake()
        {
            _states[GamePhase.Playing] = new PlayingState(this);
            _states[GamePhase.Minigame] = new MinigameState(this, minigameHost, lastWaveMinigame);
            _states[GamePhase.Ended]    = new EndedState(this);

            if (spawner != null)
                spawner.OnLastWaveReached += HandleLastWaveReached;

            if (minigameHost != null)
                minigameHost.Finished += HandleMinigameFinished;
        }

        private void Start() => TransitionTo(GamePhase.Playing);

        private void OnDestroy()
        {
            if (spawner != null) spawner.OnLastWaveReached -= HandleLastWaveReached;
            if (minigameHost != null) minigameHost.Finished -= HandleMinigameFinished;
        }
        

        private void HandleLastWaveReached()
        {
            if (CurrentPhase != GamePhase.Playing) return;
            TransitionTo(GamePhase.Minigame);
        }

        private void HandleMinigameFinished(bool won)
        {
            if (CurrentPhase != GamePhase.Minigame) return;
            TransitionTo(GamePhase.Playing);
            spawner?.ResolveLastWave(skipLastWave: won);
        }
        

        public void TransitionTo(GamePhase next)
        {
            if (CurrentPhase == next) return;

            var prev = CurrentPhase;
            _current?.Exit();
            CurrentPhase = next;
            _current = _states[next];
            _current.Enter();
            OnPhaseChanged?.Invoke(prev, next);
        }

        public void EndGame(bool won) => TransitionTo(GamePhase.Ended);
        
        internal void PauseAll()
        {
            foreach (var p in _pausables) p.PauseGameplay();
        }

        internal void ResumeAll()
        {
            foreach (var p in _pausables) p.ResumeGameplay();
        }
        
    }
}
}