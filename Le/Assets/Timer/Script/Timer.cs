using System;

namespace Bee
{
    public class Timer
    {
        public string key;
        private int _group;
        private float _delay;
        private float _loopInterval;
        private int _loopTimes;
        private Action _action;

        public static Timer Once(Action action,float delay,int group = 0)
        {
            return null;
        }

        public static Timer Loop()
        {
            return null;
        }
    }
}