using Raylib_cs;

namespace ChipSharp
{
    class Chip8
    {
        static void Main()
        {
            var romName = "IBM Logo.ch8";
            var fontName = "font.txt";
            var width = 64;
            var height = 32;
            var resolutionMultiplier = 15;
            var bgColor = new Color(177, 82, 65, 255);
            var fgColor = new Color(246, 192, 82, 255);

            var rl = new RomLoader(romName);
            var fl = new FontLoader(fontName);
            var memoryHandler = new MemoryHandler(rl, fl);
            var displayHandler = new DisplayHandler(width, height, resolutionMultiplier, bgColor, fgColor);

            memoryHandler.LoadFont(0x50);
            memoryHandler.LoadRom(0x200);

            displayHandler.Start();

        }
    }
}