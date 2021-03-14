using System;

namespace ChipSharp
{
    class SoundTimer : ByteTimer
    {
        public SoundTimer(int hertz) : base(hertz)
        {
        }

        public override void Set(byte by)
        {
            base.Set(by);
            if (by > 0) Beep(1000 / hertz * by);
        }

        private void Beep(int duration)
        {
            Console.Beep(880, duration);
        }
    }
}