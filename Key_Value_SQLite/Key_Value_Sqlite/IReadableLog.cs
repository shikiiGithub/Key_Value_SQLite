using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetLab.Common 
{
    public delegate void ErrorHandler(Object sender,Exception exception);
    public delegate void InfoHandler(Object obj); 
    public  interface IReadableLog
    {
       event ErrorHandler ErrorHandler;
        event InfoHandler InfoHandler;

    }
}
