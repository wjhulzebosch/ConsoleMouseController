using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata;
using static ConsoleMouseController.NativeMethods;

namespace ConsoleMouseController
{
    internal class MouseController
    {
        ConsoleHandle handle;
        int mode;
        INPUT_RECORD record;
        uint recordLen;
        public MouseController()
        {
            handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            mode = 0;
            if (!(NativeMethods.GetConsoleMode(handle, ref mode))) { throw new Win32Exception(); }

            mode |= NativeMethods.ENABLE_MOUSE_INPUT;
            mode &= ~NativeMethods.ENABLE_QUICK_EDIT_MODE;
            mode |= NativeMethods.ENABLE_EXTENDED_FLAGS;

            if (!(NativeMethods.SetConsoleMode(handle, mode))) { throw new Win32Exception(); }

            record = new NativeMethods.INPUT_RECORD();
            recordLen = 0;
        }

        public void Update(TimeSpan deltaTime)
        {
            if (!(NativeMethods.ReadConsoleInput(handle, ref record, 1, ref recordLen))) { throw new Win32Exception(); }

            switch (record.EventType)
            {
                case NativeMethods.MOUSE_EVENT:
                    {
                        OnPositionChange(record.MouseEvent.dwMousePosition.X, record.MouseEvent.dwMousePosition.Y);

                        // if right button is clicked
                        if ((record.MouseEvent.dwButtonState & 0x2) != 0)
                        {
                            OnRightClick(record.MouseEvent.dwMousePosition.X, record.MouseEvent.dwMousePosition.Y);
                        }

                        // if left button is clicked
                        if ((record.MouseEvent.dwButtonState & 0x1) != 0)
                        {
                            OnLeftClick(record.MouseEvent.dwMousePosition.X, record.MouseEvent.dwMousePosition.Y);
                        }
                    }
                    break;

                case NativeMethods.KEY_EVENT:
                    {
                        OnKeyEvent(record.KeyEvent);
                    }
                    break;
            }
        }

        void OnPositionChange(UInt16 x, UInt16 y)
        {
            // Set cursor position to 0,0
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Mouse moved to position {x}, {y}          ");
        }

        void OnRightClick(UInt16 x, UInt16 y)
        {
            // Set cursor position to 0,1
            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"Right click at position {x}, {y}          ");
        }

        private void OnLeftClick(ushort x, ushort y)
        {
            // Set cursor position to 0,2
            Console.SetCursorPosition(0, 2);
            Console.WriteLine($"Left click at position {x}, {y}          ");
        }

        void OnKeyEvent(NativeMethods.KEY_EVENT_RECORD keyEvent)
        {
            // Set cursor position to 0,3
            Console.SetCursorPosition(0, 3);
            Console.WriteLine($"Key event - Key: {keyEvent.wVirtualKeyCode} Pressed: {keyEvent.bKeyDown}         ");
        }
    }

    internal class NativeMethods
    {

        public const Int32 STD_INPUT_HANDLE = -10;

        public const Int32 ENABLE_MOUSE_INPUT = 0x0010;
        public const Int32 ENABLE_QUICK_EDIT_MODE = 0x0040;
        public const Int32 ENABLE_EXTENDED_FLAGS = 0x0080;

        public const Int32 KEY_EVENT = 1;
        public const Int32 MOUSE_EVENT = 2;


        [DebuggerDisplay("EventType: {EventType}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public Int16 EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
        }

        [DebuggerDisplay("{dwMousePosition.X}, {dwMousePosition.Y}")]
        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public Int32 dwButtonState;
            public Int32 dwControlKeyState;
            public Int32 dwEventFlags;
        }

        [DebuggerDisplay("{X}, {Y}")]
        public struct COORD
        {
            public UInt16 X;
            public UInt16 Y;
        }

        [DebuggerDisplay("KeyCode: {wVirtualKeyCode}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)]
            [MarshalAsAttribute(UnmanagedType.Bool)]
            public Boolean bKeyDown;
            [FieldOffset(4)]
            public UInt16 wRepeatCount;
            [FieldOffset(6)]
            public UInt16 wVirtualKeyCode;
            [FieldOffset(8)]
            public UInt16 wVirtualScanCode;
            [FieldOffset(10)]
            public Char UnicodeChar;
            [FieldOffset(10)]
            public Byte AsciiChar;
            [FieldOffset(12)]
            public Int32 dwControlKeyState;
        };


        public class ConsoleHandle : SafeHandleMinusOneIsInvalid
        {
            public ConsoleHandle() : base(false) { }

            protected override bool ReleaseHandle()
            {
                return true; //releasing console handle is not our business
            }
        }


        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern Boolean GetConsoleMode(ConsoleHandle hConsoleHandle, ref Int32 lpMode);

        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        public static extern ConsoleHandle GetStdHandle(Int32 nStdHandle);

        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern Boolean ReadConsoleInput(ConsoleHandle hConsoleInput, ref INPUT_RECORD lpBuffer, UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern Boolean SetConsoleMode(ConsoleHandle hConsoleHandle, Int32 dwMode);

    }
}
