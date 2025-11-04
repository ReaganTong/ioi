#if __TIZEN__
using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace mobile_app
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // This static void Main is only needed for Tizen
        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
#endif // __TIZEN__