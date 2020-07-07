using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GlobalKeyListener
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isRunning = true;
            Console.Title = "Global Key Listener";
            Console.WriteLine("Use [E + X + I + T] shortcut key to exit peogram!");
            GlobalKeyListener listener = new GlobalKeyListener();
            listener.OnKeyPressed += (Keys k, char c) =>
            {
                if (c >= 32 && c <= 126)
                    Console.Write(c);
                else if (k == Keys.Return)
                    Console.WriteLine();
                else if (k == Keys.Back)
                    Console.Write("\b \b");
                else if (k == Keys.Tab)
                    Console.Write("\t");
            };
            ShortcutListener shorcut = new ShortcutListener(listener, new Keys[] { Keys.E, Keys.X, Keys.I, Keys.T });
            shorcut.OnActivated += () => isRunning = false;
            listener.Start();
            while (true)
                if (isRunning)
                    Thread.Sleep(100);
                else
                    break;

        }
    }
}
