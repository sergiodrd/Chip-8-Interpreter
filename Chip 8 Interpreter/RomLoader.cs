using System.IO;

namespace ChipSharp
{
    class RomLoader
    {
        private string path;
        private FileInfo info;

        public RomLoader(string path)
        {
            this.path = path;
            info = new FileInfo(path);
        }

        public byte[] GetRomArray()
        {
            int size = (int)info.Length;
            byte[] rom;
            using(BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                rom = reader.ReadBytes(size);
            }
            return rom;
        }
    }
}