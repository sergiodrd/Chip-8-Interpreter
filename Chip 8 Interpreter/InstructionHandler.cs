using System;

namespace ChipSharp
{
    class InstructionHandler
    {
        private DisplayHandler displayHandler;
        private MemoryHandler memoryHandler;
        private ByteTimer byteTimer;
        private SoundTimer soundTimer;

        private bool loopShouldRun;
        private HzTimer timer = new HzTimer(700);

        public InstructionHandler(DisplayHandler displayHandler, MemoryHandler memoryHandler, ByteTimer byteTimer, SoundTimer soundTimer)
        {
            this.displayHandler = displayHandler;
            this.memoryHandler = memoryHandler;
            this.byteTimer = byteTimer;
            this.soundTimer = soundTimer;
        }

        public void Start()
        {
            memoryHandler.SetPC(0x200);
            timer.Tick += SetBool;
            timer.Start();
            while (!displayHandler.WindowShouldClose())
            {
                if(loopShouldRun)
                {
                    GameLoop();
                    loopShouldRun = false;
                }
            }
            displayHandler.CloseWindow();
        }

        private void GameLoop()
        {
            var instruction = Fetch();
            DecodeAndExecute(instruction);
        }

        private void DecodeAndExecute(ushort instruction)
        {
            var firstNibble = (instruction & 0xf000) >> 12;
            switch(firstNibble)
            {
                case 0x0:
                    if(instruction == 0xE0)
                    {
                        displayHandler.Clear();
                        displayHandler.Render();
                    }
                    else if(instruction == 0xEE)
                    {
                        memoryHandler.PopStackToPC();
                    }
                    break;
                case 0x1:
                    memoryHandler.SetPC
                        (
                            (short)(instruction & 0x0fff)
                        );
                    break;
                case 0x6:
                    memoryHandler.SetVar
                        (
                            (byte)((instruction & 0x0f00) >> 8), (byte)(instruction & 0x00ff)
                        );
                    break;
                case 0x7:
                    memoryHandler.SetVar
                        (
                            (byte)((instruction & 0x0f00) >> 8),
                            (byte)(memoryHandler.ReadVar((byte)((instruction & 0x0f00) >> 8)) +
                            instruction & 0x00ff)
                        );
                    break;
                case 0xa:
                    memoryHandler.SetI
                        (
                            (short)(instruction & 0x0fff)
                        );
                    break;
                case 0xd:
                    var x = (byte)(memoryHandler.ReadVar((byte)((instruction & 0x0f00) >> 8)) % displayHandler.Width);
                    var y = (byte)(memoryHandler.ReadVar((byte)((instruction & 0x00f0) >> 4)) % displayHandler.Height);
                    memoryHandler.SetVar(0xf, 0);

                    var n = instruction & 0x000f;
                    var i = memoryHandler.ReadI();
                    for (int j = 0; j < n; j++)
                    {
                        var nb = new bool[8];
                        for(int k = 0; k < 8; k++)
                        {
                            nb[k] = (i & (1 << k)) == 0 ? false : true;
                        }
                        Array.Reverse(nb);

                        foreach(bool b in nb)
                        {
                            var vf = (byte)(displayHandler.FlipPixel(x - 1, y - 1) ? 0 : 1);
                            if(b)
                            {
                                memoryHandler.SetVar(0xf, vf);
                            }
                            else
                            {
                                memoryHandler.SetVar(0xf, (byte)(displayHandler.FlipPixel(x - 1, y - 1) ? 0 : 1));
                            }

                            if (x == displayHandler.Width) break;

                            x++;
                        }

                        x = (byte)(memoryHandler.ReadVar((byte)((instruction & 0x0f00) >> 8)) % displayHandler.Width);
                        y++;

                        if (y == displayHandler.Height) break;
                    }
                    displayHandler.Render();
                    break;
            }
        }

        private ushort Fetch()
        {
            var firstByte = (ushort)(memoryHandler.ReadPC() * 0x100);
            memoryHandler.IncreasePC(1);
            var secondByte = memoryHandler.ReadPC();
            memoryHandler.IncreasePC(1);

            return (ushort)(firstByte + secondByte);
        }

        private void SetBool() => loopShouldRun = true;
    }
}