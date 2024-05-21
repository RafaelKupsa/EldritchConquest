using System;

namespace Backend
{
    [Serializable]
    public abstract class MiniGame
    {
        public abstract int GetScore();
    }
}