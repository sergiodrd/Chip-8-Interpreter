using Raylib_cs;

namespace ChipSharp
{
    class InputHandler
    {
        KeyboardKey[] keyboardKeys = new KeyboardKey[0xf];

        public InputHandler()
        {
            keyboardKeys[0] = KeyboardKey.KEY_ONE;
            keyboardKeys[1] = KeyboardKey.KEY_TWO;
            keyboardKeys[2] = KeyboardKey.KEY_THREE;
            keyboardKeys[3] = KeyboardKey.KEY_FOUR;
            keyboardKeys[4] = KeyboardKey.KEY_Q;
            keyboardKeys[5] = KeyboardKey.KEY_W;
            keyboardKeys[6] = KeyboardKey.KEY_E;
            keyboardKeys[7] = KeyboardKey.KEY_R;
            keyboardKeys[8] = KeyboardKey.KEY_A;
            keyboardKeys[9] = KeyboardKey.KEY_S;
            keyboardKeys[10] = KeyboardKey.KEY_D;
            keyboardKeys[11] = KeyboardKey.KEY_F;
            keyboardKeys[12] = KeyboardKey.KEY_Z;
            keyboardKeys[13] = KeyboardKey.KEY_X;
            keyboardKeys[14] = KeyboardKey.KEY_C;
            keyboardKeys[15] = KeyboardKey.KEY_V;
        }

        public bool isKeyDown(byte b)
        {
            return Raylib.IsKeyDown(keyboardKeys[b]);
        }
    }
}