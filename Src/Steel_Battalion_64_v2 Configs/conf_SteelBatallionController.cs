//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.

using System;
namespace SBC{
public class DynamicClass
{        String debugString = "";
        SteelBattalionController controller;
        vJoy joystick;
		int vJoyButtons = 39;//change to 39 to fully support SBC, need to use vJoy config to change
		//number of buttons to 39.
		bool acquired;
        const int refreshRate = 5;//number of milliseconds between call to mainLoop


        //this gets called once by main program
        public void Initialize()
        {

            int baseLineIntensity = 10;//just an average value for LED intensity
            int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

            controller = new SteelBattalionController();
            controller.Init(5);//50 is refresh rate in milliseconds
            //set all buttons by default to light up only when you press them down

            for (int i = 4; i < 4 + 30; i++)
            {
                if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
                    controller.AddButtonLightMapping((ButtonEnum)(i - 1), (ControllerLEDEnum)(i), true, baseLineIntensity);
            }
            
			//no longer using Microsoft.DirectX.DirectInput but included this in here to show that I made
			//last true means if you hold down the button,
			//code backwards compatible
			
            //controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch, true, 3, SBC.Key.A, true);		
			//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, true, 3, Microsoft.DirectX.DirectInput.Key.BackSpace, true);
            //controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, SBC.Key.C, true);

            joystick = new vJoy();
            acquired = joystick.acquireVJD(1);
            joystick.resetAll();//have to reset before we use it

        }

		
		
        //this is necessary, as main program calls this to know how often to call mainLoop
        public int getRefreshRate()
        {
            return refreshRate;
        }
		
//		private uint getDegrees(double x,double y)
//		{
//		  uint temp = (uint)(System.Math.Atan(y/x)* (180/Math.PI));
//		  if(x < 0)
//		   temp +=180;
//		  if(x > 0 && y < 0)
//		   temp += 360;
//
//		  temp += 90;//origin is vertical on POV not horizontal
//
//		  if(temp > 360)//by adding 90 we may have gone over 360
//		   temp -=360;
//
//		  temp*=100;
//
//		  if (temp > 35999)
//		   temp = 35999;
//		  if (temp < 0)
//		   temp = 0;
//		  return temp;
//		}

        //this gets called once every refreshRate milliseconds by main program
        public void mainLoop()
        {
            joystick.setAxis(1,controller.Scaled.GearLever,HID_USAGES.HID_USAGE_SL1);		// Schakelbak
            joystick.setAxis(1,controller.Scaled.AimingX,HID_USAGES.HID_USAGE_X);			// Rechter pook X as
            joystick.setAxis(1,controller.Scaled.AimingY,HID_USAGES.HID_USAGE_Y);			// REchter pook Y as
            joystick.setAxis(1,controller.Scaled.RightMiddlePedal,HID_USAGES.HID_USAGE_Z);	// throttle, rechter en middel pedaal
            joystick.setAxis(1,controller.Scaled.RotationLever+4000,HID_USAGES.HID_USAGE_RZ);	// Linker pook
            //joystick.setAxis(1,(controller.Scaled.RotationLever+7920),HID_USAGES.HID_USAGE_RZ);	// Linker pook
            joystick.setAxis(1,controller.Scaled.SightChangeX,HID_USAGES.HID_USAGE_SL0);	// Duimpookje X as
            joystick.setAxis(1,controller.Scaled.SightChangeY,HID_USAGES.HID_USAGE_RX);		// Duimpookje Y as
            joystick.setAxis(1,controller.Scaled.LeftPedal,HID_USAGES.HID_USAGE_RY);		// Linker pedaal

			
            for (int i = 1; i <= vJoyButtons; i++)
            {
                joystick.setButton((bool)controller.GetButtonState(i - 1), (uint)1, (char)(i - 1));
            }

            joystick.sendUpdate(1);
        }

        //new necessary function used for debugging purposes
        public String getDebugString()
        {
            return debugString;
        }

        //this gets called at the end of the program and must be present, as it cleans up resources
        public void shutDown()
        {
            controller.UnInit();
            joystick.Release(1);
        }
    }
}
	