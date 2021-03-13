using System;
using System.Collections.Generic;
using System.IO;

namespace Interpreter.Core
{
    class FontLoader
    {
        private string path;
        public FontLoader(string path) => this.path = path;

        public byte[] GetFontArray()
        {
            var font = new List<byte>();
            string[] values;
            values = File.ReadAllText(path).Split(",");
            for (int i = 0; i < values.Length; i++)
            {
                font.Add(Convert.ToByte(values[i].Replace("\r\n", ""), 16));
            }
            return font.ToArray();
        }
    }
}