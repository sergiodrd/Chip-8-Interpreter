using Interpreter.Memory;

namespace Interpreter.Core
{
    class Chip8
    {
        static void Main()
        {
            var rl = new RomLoader("IBM Logo.ch8");
            var fl = new FontLoader("font.txt");
            var memoryHandler = new MemoryHandler(rl, fl);

            memoryHandler.LoadFont(0x50);
            memoryHandler.LoadRom(0x200);
        }
    }
}