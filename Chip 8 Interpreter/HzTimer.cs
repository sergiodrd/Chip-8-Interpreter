using System;
using System.Timers;

namespace ChipSharp
{
    class HzTimer
    {
        private Timer timer;
        public event Action Tick;

        public HzTimer(double hertz)
        {
            timer = new Timer(1000 / hertz);
            timer.Elapsed += Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            Tick?.Invoke();
        }
    }
}