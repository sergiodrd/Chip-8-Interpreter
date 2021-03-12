using Interpreter.Core;
using System.Collections.Generic;

namespace Interpreter.Memory
{
    class MemoryHandler
    {
        private RomLoader rl;
        private FontLoader fl;

        private byte[] memory = new byte[4096];
        private short PC;
        private short I;

        private Stack<short> stack = new Stack<short>();

        private byte[] variables = new byte[16];

        public MemoryHandler(RomLoader rl, FontLoader fl)
        {
            this.rl = rl;
            this.fl = fl;
        }

        public void LoadRom(short startingAddress)
        {
            var rom = rl.GetRomArray();
            for(int i = 0; i < rom.Length; i++)
            {
                memory[startingAddress + i] = rom[i];
            }
        }

        public void LoadFont(short startingAddress)
        {
            var font = fl.GetFontArray();
            for(int i = 0; i < font.Length; i++)
            {
                memory[startingAddress + i] = font[i];
            }
        }

        public void SetPC(short location) => PC = memory[location];
        public short ReadPC() { return memory[PC]; }

        public void SetI(short location) => I = memory[location];
        public short ReadI() { return memory[I]; }

        public void PushPCToStack() => stack.Push(PC);
        public void PopStackToPC() => PC = stack.Pop();

        public void SetVar(byte var, byte val) => variables[var] = val;
        public byte ReadVar(byte var) { return variables[var]; }
    }
}