// MWO Config File
// version 1.1
// by von Pilsner (thanks to HackNFly!@!!!)
//
// Uses default MWO keybindings and axis as of October 2, 2012
//
// For use with Steel-Batallion-64_v2_beta.zip 
// 64 bit driver code/glue by HackNFly  http://code.google.com/p/steel-batallion-64/
//
// I suggest you add the folowing to user.cfg (remove the //'s)
//
// cl_joystick_gain = 1.35
// cl_joystick_invert_throttle = 0
// cl_joystick_invert_pitch = 1
// cl_joystick_invert_yaw = 0
// cl_joystick_invert_turn = 0
// cl_joystick_throttle_range = 0
// gp_mech_view_look_sensitivity = 0.0090 //Normal view
//
using SBC;
using System;
namespace SBC
{
    public class DynamicClass
    {
        SteelBattalionController controller;
        vJoy joystick;
        bool acquired;
        int ticka = 0;
        int loopa = 0;
        int tickb = 0;
        int loopb = 0;
        int tickc = 0;
        int loopc = 0;
        int jj = 0;

        const int refreshRate = 30; // Number of milliseconds between call to mainLoop

        // This gets called once by main program
        public void Initialize()
        {
            int baseLineIntensity = 3; // Just an average value for LED intensity
            int emergencyLightIntensity = 15; // For stuff like eject,cockpit Hatch,Ignition, and Start

            controller = new SteelBattalionController();
            controller.Init(50); // 50 is refresh rate in milliseconds
			
            //set all buttons by default to light up only when you press them down
            for (int i = 4; i < 4 + 30; i++)
            {
				if (i != (int)ButtonEnum.Eject) // Excluding eject since we are going to flash that one
				controller.AddButtonLightMapping((ButtonEnum)(i - 1), (ControllerLEDEnum)(i), true, baseLineIntensity);
            }

			// Button Bindings
			controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainFire, Microsoft.DirectX.DirectInput.Key.R,true);
			controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D2,true);
			controller.AddButtonKeyMapping(ButtonEnum.RightJoyLockOn, Microsoft.DirectX.DirectInput.Key.R, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Eject, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Ignition, true, 3, Microsoft.DirectX.DirectInput.Key.P, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.Start,					true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose, true, 3, Microsoft.DirectX.DirectInput.Key.B, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut, true, 3, Microsoft.DirectX.DirectInput.Key.B, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn, true, 3, Microsoft.DirectX.DirectInput.Key.Z, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut, true, 3, Microsoft.DirectX.DirectInput.Key.Z, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange, true, 3, Microsoft.DirectX.DirectInput.Key.H, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Washing, true, 3, Microsoft.DirectX.DirectInput.Key.C, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.Chaff,					true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope, true, 3, Microsoft.DirectX.DirectInput.Key.N, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, true, 3, Microsoft.DirectX.DirectInput.Key.Tab, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3,				true, 4,    Microsoft.DirectX.DirectInput.Key.LeftControl, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain, true, 3, Microsoft.DirectX.DirectInput.Key.RightControl, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub, true, 3, Microsoft.DirectX.DirectInput.Key.BackSpace, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine,		true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Comm1, true, 3, Microsoft.DirectX.DirectInput.Key.F6, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Comm2, true, 3, Microsoft.DirectX.DirectInput.Key.F8, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Comm3, true, 3, Microsoft.DirectX.DirectInput.Key.F9, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.Comm4,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.Comm5, true, 3, Microsoft.DirectX.DirectInput.Key.RightBracket, true);
			controller.AddButtonKeyMapping(ButtonEnum.LeftJoySightChange, Microsoft.DirectX.DirectInput.Key.Z, true);

			joystick = new vJoy();
			acquired = joystick.acquireVJD(1);
			joystick.resetAll(); //have to reset before we use it
        }

        //this is necessary, as main program calls this to know how often to call mainLoop
        public int getRefreshRate()
        {
            return refreshRate;
        }

        private uint getDegrees(double x, double y)
        {
            uint temp = (uint)(System.Math.Atan(y / x) * (180 / Math.PI));
            if (x < 0)
                temp += 180;
            if (x > 0 && y < 0)
                temp += 360;

            temp += 90; //origin is vertical on POV not horizontal

            if (temp > 360)//by adding 90 we may have gone over 360
                temp -= 360;

            temp *= 100;

            if (temp > 35999)
                temp = 35999;
            if (temp < 0)
                temp = 0;

            return temp;
        }

		// POV to Arrow Keys
        public void updatePOVhat()
        {
            int thumbstickDeadZone = 75;
            SBC.POVdirection lastDirection = controller.POVhat;

            if (((Math.Abs(controller.SightChangeX) > thumbstickDeadZone) || (Math.Abs(controller.SightChangeY) > thumbstickDeadZone)))
            {
                if (Math.Abs(controller.SightChangeX) > Math.Abs(controller.SightChangeY))
                    if (controller.SightChangeX < 0)
                        controller.POVhat = SBC.POVdirection.LEFT;
                    else
                        controller.POVhat = SBC.POVdirection.RIGHT;
                else
                    if (controller.SightChangeY < 0)
                        controller.POVhat = SBC.POVdirection.DOWN;
                    else
                        controller.POVhat = SBC.POVdirection.UP;

            }
            else
            {
                controller.POVhat = SBC.POVdirection.CENTER;
            }

            if (lastDirection != controller.POVhat)
            {
                switch (lastDirection)
                {
                    case SBC.POVdirection.LEFT:
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Left);
                        break;
                    case SBC.POVdirection.RIGHT:
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Right);
                        break;
                    case SBC.POVdirection.DOWN:
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Up);
                        break;
                    case SBC.POVdirection.UP:
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Down);
                        break;
                }
            }
            else
            {
                switch (controller.POVhat)
                {
                    case SBC.POVdirection.LEFT:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Left);
                        break;
                    case SBC.POVdirection.RIGHT:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Right);
                        break;
                    case SBC.POVdirection.DOWN:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Up);
                        break;
                    case SBC.POVdirection.UP:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Down);
                        break;
                }
            }
        }

        //this gets called once every refreshRate milliseconds by main program
        public void mainLoop()
        {
            updatePOVhat();
            int loopdelay = 2; // My pedal timer... this allows slow pedal updating with fast controller polling
            if (ticka > 0)
            {
                loopa++;
                if (loopa > loopdelay)
                {
                    ticka = 0;
                    loopa = 0;
                }
            }
            if (tickb > 0)
            {
                loopb++;
                if (loopb > loopdelay)
                {
                    tickb = 0;
                    loopb = 0;
                }
            }
            if (tickc > 0)
            {
                loopc++;
                if (loopc > loopdelay)
                {
                    tickc = 0;
                    loopc = 0;
                }
            }
			
			// Joystick Axes
            joystick.setAxis(1, controller.AimingX, HID_USAGES.HID_USAGE_X);
            joystick.setAxis(1, controller.AimingY, HID_USAGES.HID_USAGE_Y);
            joystick.setAxis(1, (controller.RightPedal - controller.MiddlePedal), HID_USAGES.HID_USAGE_Z); //throttle
            joystick.setAxis(1, controller.RotationLever, HID_USAGES.HID_USAGE_RZ);
            joystick.setAxis(1, controller.SightChangeX, HID_USAGES.HID_USAGE_SL1);
            joystick.setAxis(1, controller.SightChangeY, HID_USAGES.HID_USAGE_RX);
            joystick.setAxis(1, controller.LeftPedal, HID_USAGES.HID_USAGE_RY);
            joystick.setAxis(1, controller.GearLever, HID_USAGES.HID_USAGE_SL0);
            joystick.setContPov(1, getDegrees(controller.SightChangeX, controller.SightChangeY), 1);

            // toggle tricks!!!
            if (controller.GetButtonState(ButtonEnum.ToggleFilterControl)) //FILT Toggle
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire, Microsoft.DirectX.DirectInput.Key.BackSlash, true); // Alpha Strike
                joystick.setButton((bool)controller.GetButtonState(ButtonEnum.RightJoyFire), (uint)1, (char)(15)); // unused button (placeholder)
            }
            else
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire, Microsoft.DirectX.DirectInput.Key.NoConvert, true); // unused button (placeholder)
                joystick.setButton((bool)controller.GetButtonState(ButtonEnum.RightJoyFire), (uint)1, (char)(0)); // Fire Selected Weapon (joystick button 0)
            }

            if (controller.GetButtonState(ButtonEnum.ToggleOxygenSupply)) // O2 Supply Toggle
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D6, true);
            }
            else
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D5, true);
            }
			
			// Pedals Section
            if ((jj == 1) && ((controller.LeftPedal) < 50)) // Left pedal released
            {
                controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Space);
                jj = 0;
            }
			
            if (((controller.LeftPedal) > 50) && (ticka == 0)) // Left pedal pressed
            {
                if (controller.GetButtonState(ButtonEnum.ToggleFuelFlowRate)) // Is the FUEL Flow toggle UP?
                {
                    controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Space);
                    ticka = 0;	// bypass timer so we can jump!
                    loopa = 0;
                    jj = 1;		// Jets are on...
                }
                else // FUEL Flow toggle must be down...
                {
                    controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.X);
                    ticka = 1;
                    loopa = 0;
                    controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.X);
                }
            }

            if (((controller.MiddlePedal) > 50) && (tickb == 0)) // Middle pedal pressed
            {
                controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.S);
                tickb = 1;
                loopb = 0;
                controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.S);
            }

            if (((controller.RightPedal) > 50) && (tickc == 0)) // Right pedal pressed
            {
                controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.W);
                tickc = 1;
                loopc = 0;
                controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.W);
            }
            joystick.sendUpdate(1);
        }

        // This gets called at the end of the program and must be present, as it cleans up resources
        public void shutDown()
        {
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftControl); // Release latched key
            controller.UnInit();
            joystick.Release(1);
        }
    }
}