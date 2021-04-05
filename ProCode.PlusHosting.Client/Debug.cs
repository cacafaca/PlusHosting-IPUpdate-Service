﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public static class Debug
    {
        public static void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + $"(Thread id: {System.Threading.Thread.CurrentThread.ManagedThreadId})");
        }
    }
}
