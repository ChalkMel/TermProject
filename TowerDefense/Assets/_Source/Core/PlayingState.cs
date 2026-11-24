namespace Core
{
    namespace Core
    {
        internal sealed class PlayingState : IGameState
        {
            private readonly GameFlowController _flow;
            public PlayingState(GameFlowController flow) => _flow = flow;

            public void Enter()  => _flow.ResumeAll();
            public void Exit()   { }
        }
        
        internal sealed class MinigameState : IGameState
        {
            private readonly GameFlowController _flow;
            private readonly MinigameHost _host;
            private readonly MinigameConfig _config;

            public MinigameState(GameFlowController flow, MinigameHost host, MinigameConfig config)
            {
                _flow = flow; _host = host; _config = config;
            }

            public void Enter()
            {
                _flow.PauseAll();
                _host?.Launch(_config);
            }

            public void Exit()
            {
                
            }
        }
        
        internal sealed class EndedState : IGameState
        {
            private readonly GameFlowController _flow;
            public EndedState(GameFlowController flow) => _flow = flow;

            public void Enter() => _flow.PauseAll();
            public void Exit()  { }
        }
    }
}