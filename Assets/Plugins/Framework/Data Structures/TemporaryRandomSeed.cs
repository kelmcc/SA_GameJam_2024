using System;
using Random = UnityEngine.Random;

namespace Framework
{
    public class TemporaryRandomSeed : IDisposable
    {
        private Random.State _orignialState;

        public TemporaryRandomSeed(int seed)
        {
            _orignialState = Random.state;
            Random.InitState(seed);
        }

        public TemporaryRandomSeed() : this(new System.Random().Next())
        {

        }

        public void Dispose()
        {
            Random.state = _orignialState;
        }
    }

    public class TemporaryRandomState : IDisposable
    {
        private Random.State _orignialState;

        public TemporaryRandomState(Random.State state)
        {
            _orignialState = Random.state;
            Random.state = state;
        }

        public void Dispose()
        {
            Random.state = _orignialState;
        }
    }
}