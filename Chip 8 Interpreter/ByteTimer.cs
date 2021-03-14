namespace ChipSharp
{
    class ByteTimer
    {
        protected int hertz;
        private byte b;
        private HzTimer timer;
        private bool isStopped;

        public ByteTimer(int hertz)
        {
            this.hertz = hertz;
            timer = new HzTimer(hertz);
            timer.Tick += Decrease;
            timer.Start();
        }

        private void Decrease()
        {
            if (b > 0) b--;
            else Stop();
        }

        private void Stop()
        {
            timer.Stop();
            isStopped = true;
        }

        public virtual void Set(byte by)
        {
            if(isStopped)
            {
                timer.Start();
                isStopped = false;
            }
            b = by;
        }

        public byte Read() { return b; }
    }
}