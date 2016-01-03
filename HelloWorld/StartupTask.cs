﻿using Glovebox.Graphics.Components;
using Glovebox.Graphics.Drivers;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HelloWorld
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;   // for a headless Windows 10 for IoT projects you need to hold a deferral to keep the app active in the background
        double temperature;
        bool blink = false;
        StringBuilder data = new StringBuilder(20);

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();  // get the deferral handle

            int count = 0;

            MAX7219 driver = new MAX7219(2); 
            SevenSegmentDisplay ssd = new SevenSegmentDisplay(driver);
            Glovebox.IoT.Devices.Sensors.BMP180 bmp = new Glovebox.IoT.Devices.Sensors.BMP180(Glovebox.IoT.Devices.Sensors.BMP180.Mode.HIGHRES);


            ssd.FrameClear();
            ssd.FrameDraw();
            ssd.SetBrightness(4);


            while (true)
            {
                temperature = bmp.Temperature.DegreesCelsius;

                data.Clear();

                // is temperature less than 3 digits and there is a decimal part too then right pad to 5 places as decimal point does not take up a digit space on the display
                if (temperature < 100 && temperature != (int)temperature) { data.Append($"{Math.Round(temperature, 1)}C".PadRight(5)); }
                else { data.Append($"{Math.Round(temperature, 0)}C".PadRight(4)); }

                data.Append(Math.Round(bmp.Pressure.Hectopascals, 0));

                if (blink = !blink) { data.Append("."); }  // add a blinking dot on bottom right as a I'm alive indicator
                                

                ssd.DrawString(data.ToString());

                //var now = DateTime.Now;

                //Debug.WriteLine(now.ToString());

                //data.Clear();

                //data.Append(now.Month.ToString().PadLeft(2));
                //data.Append(".");
                //data.Append(now.Day.ToString().PadLeft(2));

                //data.Append(now.Hour.ToString().PadLeft(2));
                //data.Append(".");
                //data.Append(now.Minute.ToString().PadLeft(2));


                //ssd.DrawString(data.ToString(), 1);
                ssd.DrawString(count++, 1);

                ssd.FrameDraw();

                Task.Delay(2000).Wait();             
            }
        }
    }
}