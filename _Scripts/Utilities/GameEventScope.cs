using System;

namespace com.game.utilities
{
    public struct GameEventScope : IDisposable
    {
        private readonly GameRuntimeEvent _previousEvent;

        public GameEventScope(GameRuntimeEvent newEvent)
        {
            _previousEvent = Game.Event;
            Game.Event = newEvent;
        }
        public void Dispose()
        {
            Game.Event = _previousEvent;
        }
    }
}