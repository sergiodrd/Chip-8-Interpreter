using Raylib_cs;

namespace ChipSharp
{
    class DisplayHandler
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private int resolutionMultiplier;
        private Color bgColor;
        private Color fgColor;

        private bool[,] grid;

        public DisplayHandler(int width, int height, int resolutionMultiplier, Color bgColor, Color fgColor)
        {
            Width = width;
            Height = height;
            this.resolutionMultiplier = resolutionMultiplier;
            this.bgColor = bgColor;
            this.fgColor = fgColor;

            grid = new bool[width, height];
        }

        public void Start()
        {
            Raylib.InitWindow(Width * resolutionMultiplier, Height * resolutionMultiplier, "Chip Sharp");
        }

        public void Clear()
        {
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++) { grid[x, y] = false; }
            }
        }

        public bool FlipPixel(int x, int y)
        {
            grid[x, y] = !grid[x, y];
            return grid[x, y];
        }

        public void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(bgColor);

            for(int a = 0; a < grid.GetLength(0); a++)
            {
                for(int b = 0; b < grid.GetLength(1); b++)
                {
                    var color = grid[a, b] ? fgColor : bgColor;
                    var x = a * resolutionMultiplier;
                    var y = b * resolutionMultiplier;
                    Raylib.DrawRectangle(x, y, resolutionMultiplier, resolutionMultiplier, color);
                }
            }

            Raylib.EndDrawing();
        }

        public bool WindowShouldClose() { return Raylib.WindowShouldClose(); }
        public void CloseWindow() => Raylib.CloseWindow();
    }
}