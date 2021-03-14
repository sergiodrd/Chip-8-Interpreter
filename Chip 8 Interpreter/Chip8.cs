using Raylib_cs;

namespace ChipSharp
{
    class Chip8
    {
        static void Main()
        {
            var romName = "IBM Logo.ch8";
            var rl = new RomLoader(romName);

            var fontName = "font.txt";
            var fl = new FontLoader(fontName);

            var memoryHandler = new MemoryHandler(rl, fl);
            memoryHandler.LoadRom(0x200);
            memoryHandler.LoadFont(0x50);

            var width = 64;
            var height = 32;
            var resolutionMultiplier = 15;
            var bgColor = new Color(177, 82, 65, 255);
            var fgColor = new Color(246, 192, 82, 255);
            var displayHandler = new DisplayHandler(width, height, resolutionMultiplier, bgColor, fgColor);

            displayHandler.Start();
        }
    }
}