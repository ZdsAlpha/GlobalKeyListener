using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GlobalKeyListener
{
    public class GlobalKeyListener : IDisposable
    {
        private bool isDisposed;
        private bool isRunning;
        private Thread thread;
        private List<Keys> downMap;
        public int SleepTime { get; set; } = 25;
        public event OnKeyPressDelegate OnKeyPressed;
        public event OnKeyDelegate OnKeyDown;
        public event OnKeyDelegate OnKeyUp;
        public GlobalKeyListener()
        {
            thread = new Thread(Work);
            downMap = new List<Keys>();
            thread.IsBackground = true;
            thread.Start();
        }
        public void Start()
        {
            if (!isDisposed) isRunning = true;
        }
        public void Stop()
        {
            isRunning = false;
        }
        public void Dispose()
        {
            Stop();
            isDisposed = true;
        }
        private void Work()
        {
            while (true)
            {
                byte[] keyboardState = new byte[256];
                StringBuilder buffer = new StringBuilder(256);
                if (isDisposed) break;
                if (isRunning)
                {
                    foreach (Keys key in keyMap)
                    {
                        bool contains = downMap.Contains(key);
                        if (GetAsyncKeyState(key) != 0)
                        {
                            if (!contains)
                            {
                                OnKeyDown?.Invoke(key);
                                downMap.Add(key);
                                if (OnKeyPressed != null)
                                {
                                    bool isShift = downMap.Contains(Keys.ShiftKey) || downMap.Contains(Keys.LShiftKey) || downMap.Contains(Keys.RShiftKey);
                                    bool numLock = Control.IsKeyLocked(Keys.NumLock);
                                    bool capsLock = Control.IsKeyLocked(Keys.CapsLock);
                                    bool scrollLock = Control.IsKeyLocked(Keys.Scroll);
                                    Array.Clear(keyboardState, 0, keyboardState.Length);
                                    buffer.Clear();
                                    if (isShift) keyboardState[(int)Keys.ShiftKey] = byte.MaxValue;
                                    if (numLock) keyboardState[(int)Keys.NumLock] = byte.MaxValue;
                                    if (capsLock) keyboardState[(int)Keys.CapsLock] = byte.MaxValue;
                                    if (scrollLock) keyboardState[(int)Keys.Scroll] = byte.MaxValue;
                                    int output = ToUnicode((uint)key, 0, keyboardState, buffer, 256, 0);
                                    string text = buffer.ToString();
                                    if (text.Length == 0)
                                        OnKeyPressed?.Invoke(key, (char)0);
                                    else
                                        OnKeyPressed?.Invoke(key, text[0]);
                                }
                            }
                        }
                        else
                        {
                            if (contains)
                            {
                                OnKeyUp?.Invoke(key);
                                downMap.Remove(key);
                            }
                        }
                    }
                }
                Thread.Sleep(SleepTime);
            }
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        [DllImport("user32.dll")]
        private static extern int ToUnicode(uint virtualKeyCode, uint scanCode, byte[] keyboardState, [MarshalAs(UnmanagedType.LPWStr, SizeConst = 256)] StringBuilder receivingBuffer, int bufferSize, uint flags);
        public delegate void OnKeyDelegate(Keys key);
        public delegate void OnKeyPressDelegate(Keys key, char characters);

        private static Keys[] keyMap;
        static GlobalKeyListener()
        {
            keyMap = (Keys[])Enum.GetValues(typeof(Keys));
        }
    }
}
