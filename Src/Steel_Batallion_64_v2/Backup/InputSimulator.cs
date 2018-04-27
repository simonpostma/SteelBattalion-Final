using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;
using WindowsInput;
//http://damiproductions.darkbb.com/t504-c-sending-keys-with-the-sendinput-api

namespace SBC
{
    public static class InputSimulator
    {
        #region Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        #endregion

        

        public static int ModifiedKeyStrokeDelay = 5;//number of milliseconds combination keys will be held down.
                                              //this number was experimentally found out as the minimum necessary for MW4 to respond to modified
                                              //keystroke

         /// <summary>
        /// Determines if the <see cref="VirtualKeyCode"/> is an ExtendedKey
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>true if the key code is an extended key; otherwise, false.</returns>
        /// <remarks>
        /// The extended keys consist of the ALT and CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP, PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK key; the BREAK (CTRLPAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in the numeric keypad.
        /// 
        /// See http://msdn.microsoft.com/en-us/library/ms646267(v=vs.85).aspx Section "Extended-Key Flag"
        /// </remarks>
        public static bool IsExtendedKey(Microsoft.DirectX.DirectInput.Key keyCode)
        {
            if (
                keyCode == Microsoft.DirectX.DirectInput.Key.Insert ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Delete ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Home ||
                keyCode == Microsoft.DirectX.DirectInput.Key.End ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Prior ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Next ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Right ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Up ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Left ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Down ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Numlock ||
                keyCode == Microsoft.DirectX.DirectInput.Key.BackSpace ||
                keyCode == Microsoft.DirectX.DirectInput.Key.Divide)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void SimulateKeyDown(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            //structure.ki.wVk = VkKeyScan((char)key);
            structure.ki.wScan = (ushort)key;
            if(IsExtendedKey(key))
                structure.ki.dwFlags = (uint)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE | (uint)KEYEVENTF.EXTENDEDKEY);
            else
                structure.ki.dwFlags = (uint)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE);
            structure.ki.time = 0;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure };

            SendInput(1, pInputs, Marshal.SizeOf(structure));
        }

        public static void SimulateKeyUp(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            //structure.ki.wVk = VkKeyScan((char)key);
            structure.ki.wScan = (ushort)key;
            if (IsExtendedKey(key))
                structure.ki.dwFlags = (uint)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE | (uint)KEYEVENTF.EXTENDEDKEY);
            else
                structure.ki.dwFlags = (uint)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE);
            structure.ki.time = 0;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure };

            SendInput(1, pInputs, Marshal.SizeOf(structure));
        }

        /// <summary>
        /// Simulates mouse movement by the specified distance measured as a delta from the current mouse location in pixels.
        /// </summary>
        /// <param name="pixelDeltaX">The distance in pixels to move the mouse horizontally.</param>
        /// <param name="pixelDeltaY">The distance in pixels to move the mouse vertically.</param>
        public static void MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            WindowsInput.InputSimulator sim = new WindowsInput.InputSimulator();
            //sim.Mouse.MoveMouseTo(absoluteX, absoluteY);
            //sim.Mouse.MoveMouseTo(10000, 10000);
            sim.Mouse.MoveMouseBy(pixelDeltaX, pixelDeltaY);

        }




        public static void SimulateKeyPress(Microsoft.DirectX.DirectInput.Key key)
        {
            SimulateKeyDown(key);
            SimulateKeyUp(key);
        }

        public static void SimulateModifiedKeyStroke(Microsoft.DirectX.DirectInput.Key modifier, Microsoft.DirectX.DirectInput.Key keycode)
        {
            SimulateKeyDown(modifier);
            SimulateKeyDown(keycode);
            System.Threading.Thread.Sleep(ModifiedKeyStrokeDelay);
            SimulateKeyUp(keycode);
            SimulateKeyUp(modifier);            
        }

        public static void SimulateModifiedKeyStroke(Microsoft.DirectX.DirectInput.Key[] modifierKeyCodes, Microsoft.DirectX.DirectInput.Key keycode)
        {
            foreach (Microsoft.DirectX.DirectInput.Key k in modifierKeyCodes) { SimulateKeyDown(k); }
            SimulateKeyDown(keycode);
            System.Threading.Thread.Sleep(ModifiedKeyStrokeDelay);
            SimulateKeyUp(keycode);
            foreach (Microsoft.DirectX.DirectInput.Key k in modifierKeyCodes) { SimulateKeyUp(k); }
        }

                    
        

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public int type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        public enum InputType
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [Flags]
        public enum MOUSEEVENTF
        {
            MOVE = 0x0001, /* mouse move */
            LEFTDOWN = 0x0002, /* left button down */
            LEFTUP = 0x0004, /* left button up */
            RIGHTDOWN = 0x0008, /* right button down */
            RIGHTUP = 0x0010, /* right button up */
            MIDDLEDOWN = 0x0020, /* middle button down */
            MIDDLEUP = 0x0040, /* middle button up */
            XDOWN = 0x0080, /* x button down */
            XUP = 0x0100, /* x button down */
            WHEEL = 0x0800, /* wheel button rolled */
            MOVE_NOCOALESCE = 0x2000, /* do not coalesce mouse moves */
            VIRTUALDESK = 0x4000, /* map to entire virtual desktop */
            ABSOLUTE = 0x8000 /* absolute move */
        }

        [Flags]
        public enum KEYEVENTF
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
    }
}