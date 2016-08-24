/*------------------------------------------------------------------------
  Windows IoT Core application to demonstrate  the Adafruit Character LCD Plate.

  Written by Rick Lesniak for Adafruit Industries.

  Adafruit invests time and resources providing this open source code,
  please support Adafruit and open-source hardware by purchasing products
  from Adafruit!

  ------------------------------------------------------------------------
  This solution requires the Adafruit Windows IoT Class Library

  Adafruit CharLCDPlate is free software: you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  as published by the Free Software Foundation, either version 3 of
  the License, or (at your option) any later version.

  Adafruit CharLCDPlate is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with DotStar.  If not, see <http://www.gnu.org/licenses/>.
  ------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using AdafruitClassLibrary;


// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace CharLCDPlateTest
{
    public sealed class StartupTask : IBackgroundTask
    {
        CharLCDPlate Plate;

        ThreadPoolTimer counterTimer;
        ThreadPoolTimer buttonTimer;
        int ticks;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            Plate = new CharLCDPlate();

            await Plate.Begin(16, 2).ConfigureAwait(false);

            Plate.setCursor(0, 0);
            Plate.print("Hello, world!");
            Plate.setBacklight(CharLCDPlate.WHITE);
            Plate.setCursor(0, 1);

            ticks = 0;
            counterTimer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(1000));

            while (true)
                CheckButtons();
            
 //           deferral.Complete();
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            lock (Plate)
            {
                // set the cursor to column 0, line 1
                // (note: line 1 is the second row, since counting begins with 0):
                Plate.setCursor(0, 1);
                Plate.print(ticks++);
            }
        }

        private void CheckButtons()
        {
            lock (Plate)
            {
                byte buttons = Plate.readButtons();

                if (0 != buttons)
                {
                    Plate.clear();
                    Plate.setCursor(0, 0);
                    if (0 != (buttons & CharLCDPlate.BUTTON_UP))
                    {
                        Plate.print("UP ");
                        Plate.setBacklight(CharLCDPlate.RED);
                    }
                    if (0 != (buttons & CharLCDPlate.BUTTON_DOWN))
                    {
                        Plate.print("DOWN ");
                        Plate.setBacklight(CharLCDPlate.YELLOW);
                    }
                    if (0 != (buttons & CharLCDPlate.BUTTON_LEFT))
                    {
                        Plate.print("LEFT ");
                        Plate.setBacklight(CharLCDPlate.GREEN);
                    }
                    if (0 != (buttons & CharLCDPlate.BUTTON_RIGHT))
                    {
                        Plate.print("RIGHT ");
                        Plate.setBacklight(CharLCDPlate.TEAL);
                    }
                    if (0 != (buttons & CharLCDPlate.BUTTON_SELECT))
                    {
                        Plate.print("SELECT ");
                        Plate.setBacklight(CharLCDPlate.VIOLET);
                    }
                }
            }
        }
    }
}
