using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace GlobalKeyListener
{
    public class ShortcutListener : IDisposable
    {
        private bool isDisposed;
        private GlobalKeyListener listener;
        private Keys[] keys;
        private bool[] buffer;
        public event OnActivatedDelegate OnActivated;
        public ShortcutListener(GlobalKeyListener listener, Keys[] keys)
        {
            this.listener = listener;
            this.keys = keys;
            this.buffer = new bool[keys.Length];
            listener.OnKeyDown += OnKeyDown;
            listener.OnKeyUp += OnKeyUp;
        }
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                listener.OnKeyDown -= OnKeyDown;
                listener.OnKeyUp -= OnKeyUp;
            }
        }
        private void OnKeyDown(Keys key)
        {
            for (int i = 0; i < buffer.Length; i++)
                if (!buffer[i])
                {
                    if (key == keys[i])
                    {
                        buffer[i] = true;
                        if (i == buffer.Length - 1)
                        {
                            OnActivated?.Invoke();
                            Array.Clear(buffer, 0, buffer.Length);
                        }
                    }
                    else
                        break;
                }
        }
        private void OnKeyUp(Keys key)
        {
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i])
                {
                    if (key == keys[i])
                    {
                        Array.Clear(buffer, i, buffer.Length - i);
                        break;
                    }
                }
                else
                    break;
        }
        public delegate void OnActivatedDelegate();
    }
}
