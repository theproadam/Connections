using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Connections_Client
{
    public static class ConsoleReader
    {
        //Special Thanks To Mario Cossi
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput, [Out] StringBuilder lpCharacter, uint length, COORD bufferCoord, out uint lpNumberOfCharactersRead);

        public static char ReadCharacterAt(int x, int y)
        {
            IntPtr consoleHandle = GetStdHandle(-11);

            if (consoleHandle == IntPtr.Zero) return '\0';

            COORD position = new COORD(x, y);

            StringBuilder result = new StringBuilder(1);
            uint read = 0;
            if (ReadConsoleOutputCharacter(consoleHandle, result, 1, position, out read))
                return result[0];
            else
                return '\0';
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;

        public COORD(int x, int y)
        {
            X = (short)x;
            Y = (short)y;
        }
    }
}
