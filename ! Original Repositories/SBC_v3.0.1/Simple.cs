//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.

using System;
namespace SBC{
public class DynamicClass
{        String debugString = "";
        SteelBattalionController controller;
        vJoy joystick;
		int vJoyButtons = 8;//change to 39 to fully support SBC, need to use vJoy config to change
		//number of buttons to 39.
		bool acquired;
        const int refreshRate = 50;//number of milliseconds between call to mainLoop


        //this gets called once by main program
        public void Initialize()
        {

            int baseLineIntensity = 1;//just an average value for LED intensity
            int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

            controller = new SteelBattalionController();
            controller.Init(50);//50 is refresh rate in milliseconds
            //set all buttons by default to light up only when you press them down

            for (int i = 4; i < 4 + 30; i++)
            {
                if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
                    controller.AddButtonLightMapping((ButtonEnum)(i - 1), (ControllerLEDEnum)(i), true, baseLineIntensity);
            }
            
            controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch, true, 3, SBC.Key.A, true);//last true means if you hold down the button,		
            //no longer using Microsoft.DirectX.DirectInput but included this in here to show that I made
			//code backwards compatible
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, true, 3, Microsoft.DirectX.DirectInput.Key.BackSpace, true);
            controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, SBC.Key.C, true);

            joystick = new vJoy();
            acquired = joystick.acquireVJD(1);
            joystick.resetAll();//have to reset before we use it

        }

        //this is necessary, as main program calls this to know how often to call mainLoop
        public int getRefreshRate()
        {
            return refreshRate;
        }

        //this gets called once every refreshRate milliseconds by main program
        public void mainLoop()
        {
            joystick.setAxis(1,controller.Scaled.GearLever,HID_USAGES.HID_USAGE_SL1);
            joystick.setAxis(1,controller.Scaled.AimingX,HID_USAGES.HID_USAGE_X);
            joystick.setAxis(1,controller.Scaled.AimingY,HID_USAGES.HID_USAGE_Y);
            joystick.setAxis(1,controller.Scaled.RightMiddlePedal,HID_USAGES.HID_USAGE_Z);//throttle
            joystick.setAxis(1,controller.Scaled.RotationLever,HID_USAGES.HID_USAGE_RZ);
            joystick.setAxis(1,controller.Scaled.SightChangeX,HID_USAGES.HID_USAGE_SL0);
            joystick.setAxis(1,controller.Scaled.SightChangeY,HID_USAGES.HID_USAGE_RX);
            joystick.setAxis(1,controller.Scaled.LeftPedal,HID_USAGES.HID_USAGE_RY);

			
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
	