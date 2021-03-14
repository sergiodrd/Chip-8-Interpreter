using Raylib_cs;

namespace ChipSharp
{
    class DisplayHandler
    {
        private int width;
        private int height;
        private int resolutionMultiplier;
        private Color bgColor;
        private Color fgColor;

        private bool[,] grid;

        public DisplayHandler(int width, int height, int resolutionMultiplier, Color bgColor, Color fgColor)
        {
            this.width = width;
            this.height = height;
            this.resolutionMultiplier = resolutionMultiplier;
            this.bgColor = bgColor;
            this.fgColor = fgColor;

            grid = new bool[width, height];
        }

        public void Start()
        {
            Raylib.InitWindow(width * resolutionMultiplier, height * resolutionMultiplier, "Chip Sharp");
        }

        public void Clear()
        {
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++) { grid[x, y] = false; }
            }
            Raylib.ClearBackground(bgColor);
        }

        public void FlipPixel(int a, int b)
        {
            grid[a, b] = !grid[a, b];
            var x = a * resolutionMultiplier;
            var y = b * resolutionMultiplier;
            var color = grid[a, b] ? fgColor : bgColor;
            Raylib.DrawRectangle(x, y, resolutionMultiplier, resolutionMultiplier, color);
        }
    }
}
