using System;

namespace ChipSharp
{
    class InstructionHandler
    {
        private DisplayHandler displayHandler;
        private MemoryHandler memoryHandler;
        private ByteTimer byteTimer;
        private SoundTimer soundTimer;

        private InputHandler input = new InputHandler();

        private bool loopShouldRun;
        private HzTimer timer = new HzTimer(700);

        private const bool shouldSetXToYWhenShifting = true;

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
                    {
                        if (instruction == 0xE0)
                        {
                            displayHandler.Clear();
                            displayHandler.Render();
                        }
                        else if (instruction == 0xEE)
                        {
                            memoryHandler.PopStackToPC();
                        }
                    }
                    break;
                case 0x1:
                    {
                        var location = (short)(instruction & 0x0fff);
                        memoryHandler.SetPC(location);
                    }
                    break;
                case 0x2:
                    {
                        var nnn = (short)(instruction & 0x0fff);
                        memoryHandler.PushPCToStack();
                        memoryHandler.SetPC(nnn);
                    }
                    break;
                case 0x3:
                    {
                        var x = (byte)((instruction & 0x0f00) >> 8);
                        var nn = (byte)(instruction & 0x00ff);

                        if (memoryHandler.ReadVar(x) == nn) memoryHandler.IncreasePC(2);
                    }
                    break;
                case 0x4:
                    {
                        var x = (byte)((instruction & 0x0f00) >> 8);
                        var nn = (byte)(instruction & 0x00ff);

                        if (memoryHandler.ReadVar(x) != nn) memoryHandler.IncreasePC(2);
                    }
                    break;
                case 0x5:
                    {
                        var x = (byte)((instruction & 0x0f00) >> 8);
                        var y = (byte)((instruction & 0x00f0) >> 4);

                        if (memoryHandler.ReadVar(x) == memoryHandler.ReadVar(y)) memoryHandler.IncreasePC(2);
                    }
                    break;
                case 0x6:
                    {
                        var variable = (byte)((instruction & 0x0f00) >> 8);
                        var val = (byte)(instruction & 0x00ff);
                        memoryHandler.SetVar(variable, val);
                    }
                    break;
                case 0x7:
                    {
                        var variable = (byte)((instruction & 0x0f00) >> 8);
                        var read = (memoryHandler.ReadVar((byte)((instruction & 0x0f00) >> 8)));
                        var val = (byte)(read + (instruction & 0x00ff));
                        memoryHandler.SetVar(variable, val);
                    }
                    break;
                case 0x8:
                    {
                        var x = (byte)((instruction & 0x0f00) >> 8);
                        var y = (byte)((instruction & 0x00f0) >> 4);

                        switch (instruction & 0x000f)
                        {
                            case 0x0:
                                {
                                    memoryHandler.SetVar(x, memoryHandler.ReadVar(y));
                                }
                                break;
                            case 0x1:
                                {
                                    var result = (byte)(memoryHandler.ReadVar(x) | memoryHandler.ReadVar(y));
                                    memoryHandler.SetVar(x, result);
                                }
                                break;
                            case 0x2:
                                {
                                    var result = (byte)(memoryHandler.ReadVar(x) & memoryHandler.ReadVar(y));
                                    memoryHandler.SetVar(x, result);
                                }
                                break;
                            case 0x3:
                                {
                                    var result = (byte)(memoryHandler.ReadVar(x) ^ memoryHandler.ReadVar(y));
                                    memoryHandler.SetVar(x, result);
                                }
                                break;
                            case 0x4:
                                {
                                    var result = memoryHandler.ReadVar(x) + memoryHandler.ReadVar(y);
                                    memoryHandler.SetVar(0xf, (byte)(result > 255 ? 1 : 0));
                                    memoryHandler.SetVar(x, (byte)result);
                                }
                                break;
                            case 0x5:
                                {
                                    var a = memoryHandler.ReadVar(x);
                                    var b = memoryHandler.ReadVar(y);
                                    var result = a - b;
                                    memoryHandler.SetVar(0xf, (byte)(a > b ? 1 : 0));
                                    memoryHandler.SetVar(x, (byte)result);
                                }
                                break;
                            case 0x6:
                                {
                                    var a = memoryHandler.ReadVar(x);
                                    if(shouldSetXToYWhenShifting)
                                    {
                                        memoryHandler.SetVar(x, memoryHandler.ReadVar(y));
                                    }
                                    memoryHandler.SetVar(0xf, (byte)(a & 0b0000_0001));
                                    memoryHandler.SetVar(x, (byte)(a >> 1)); 
                                }
                                break;
                            case 0x7:
                                {
                                    var a = memoryHandler.ReadVar(x);
                                    var b = memoryHandler.ReadVar(y);
                                    var result = b - a;
                                    memoryHandler.SetVar(0xf, (byte)(b > a ? 1 : 0));
                                    memoryHandler.SetVar(x, (byte)result);
                                }
                                break;
                            case 0xe:
                                {
                                    var a = memoryHandler.ReadVar(x);
                                    if (shouldSetXToYWhenShifting)
                                    {
                                        memoryHandler.SetVar(x, memoryHandler.ReadVar(y));
                                    }
                                    memoryHandler.SetVar(0xf, (byte)((a & 0b1000_0000) >> 7));
                                    memoryHandler.SetVar(x, (byte)(a << 1));
                                }
                                break;
                        }
                    }
                    break;
                case 0x9:
                    {
                        var x = (byte)((instruction & 0x0f00) >> 8);
                        var y = (byte)((instruction & 0x00f0) >> 4);

                        if (memoryHandler.ReadVar(x) != memoryHandler.ReadVar(y)) memoryHandler.IncreasePC(2);
                    }
                    break;
                case 0xa:
                    {
                        var val = (short)(instruction & 0x0fff);
                        memoryHandler.SetI(val);
                    }
                    break;
                case 0xb:
                    {
                        var nnn = instruction & 0x0fff;
                        var v = memoryHandler.ReadVar(0x0);
                        memoryHandler.SetPC((short)(nnn + v));
                    }
                    break;
                case 0xc:
                    {
                        var x = (instruction & 0x0f00) >> 8;
                        var nn = instruction & 0x00ff;
                        var rand = new Random();
                        var r = rand.Next(0, nn);
                        memoryHandler.SetVar((byte)x, (byte)(r & nn));
                    }
                    break;
                case 0xd:
                    {
                        var x = (byte)(memoryHandler.ReadVar((byte)((instruction & 0x0f00) >> 8)) % displayHandler.Width);
                        var y = (byte)(memoryHandler.ReadVar((byte)((instruction & 0x00f0) >> 4)) % displayHandler.Height);
                        memoryHandler.SetVar(0xf, 0);

                        var n = instruction & 0x000f;
                        var i = memoryHandler.ReadI();
                        for (int j = 0; j < n; j++)
                        {
                            var nb = new bool[8];
                            for (int k = 0; k < 8; k++)
                            {
                                nb[k] = (i & (1 << k)) == 0 ? false : true;
                            }
                            Array.Reverse(nb);

                            foreach (bool b in nb)
                            {
                                var vf = (byte)(displayHandler.FlipPixel(x - 1, y - 1) ? 0 : 1);
                                if (b)
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
                    }
                    break;
                case 0xe:
                    {
                        var key = (byte)((instruction & 0x0f00) >> 8);
                        if((instruction & 0x00ff) == 0x9e)
                        {
                            if (input.isKeyDown(key)) memoryHandler.IncreasePC(2);
                        }
                        else if ((instruction & 0x00ff) == 0xa1)
                        {
                            if (!input.isKeyDown(key)) memoryHandler.IncreasePC(2);
                        }
                    }
                    break;
                case 0xf:
                    {
                        var x = (byte)((instruction & 0x0f00) >> 8);
                        var id = (byte)(instruction & 0x00ff);

                        switch(id)
                        {
                            case 0x07:
                                {
                                    memoryHandler.SetVar(x, byteTimer.Read());
                                }
                                break;
                            case 0x0a:
                                {
                                    int a = 69;
                                    for(byte i = 0; i <= 0xf; i++)
                                    {
                                        if (input.isKeyDown(i)) a = i;
                                    }
                                    if (a != 69) memoryHandler.SetVar(x, (byte)a);
                                    else memoryHandler.IncreasePC(-2);
                                }
                                break;
                            case 0x15:
                                {
                                    byteTimer.Set(memoryHandler.ReadVar(x));
                                }
                                break;
                            case 0x1e:
                                {
                                    var result = memoryHandler.ReadI() + memoryHandler.ReadVar(x);
                                    memoryHandler.SetVar(0xf, (byte)(result > 0x1000 ? 1 : 0));
                                    memoryHandler.SetI((short)result);
                                }
                                break;
                            case 0x18:
                                {
                                    soundTimer.Set(memoryHandler.ReadVar(x));
                                }
                                break;
                            case 0x29:
                                {
                                    var result = (byte)(0x50 + memoryHandler.ReadVar(x));
                                    memoryHandler.SetI(result);
                                }
                                break;
                            case 0x33:
                                {
                                    for(int i = 0; i < 3; i++)
                                    {
                                        var result = x % 10;
                                        x /= 10;
                                        memoryHandler.StoreAtAddress((short)(memoryHandler.ReadI() + i), (byte)result);
                                    }
                                }
                                break;
                            case 0x55:
                                {
                                    for(byte i = 0; i <= x; x++)
                                    {
                                        var a = memoryHandler.ReadVar(i);
                                        memoryHandler.StoreAtAddress((short)(memoryHandler.ReadI() + i), a);
                                    }
                                }
                                break;
                            case 0x65:
                                {
                                    for (byte i = 0; i <= x; x++)
                                    {
                                        var a = memoryHandler.ReadAtAddress((short)(memoryHandler.ReadI() + i));
                                        memoryHandler.SetVar(i, a);
                                    }
                                }
                                break;
                        }
                    }
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