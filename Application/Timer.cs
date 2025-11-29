using System.Diagnostics;

namespace Application
{
    public class Timer
    {
        private readonly uint _time_limit;
        private readonly Stopwatch _stopwatch;

        public Timer(uint time_limit) {
            _time_limit = time_limit;
            _stopwatch  = new();
            _stopwatch.Start();
        }

        public bool FramePassed => _stopwatch.ElapsedMilliseconds >= _time_limit;
        // public void Reset()     => _stopwatch.Reset(); 
        public void Reset() {
            _stopwatch.Reset();
            _stopwatch.Start();
        }
    }
}