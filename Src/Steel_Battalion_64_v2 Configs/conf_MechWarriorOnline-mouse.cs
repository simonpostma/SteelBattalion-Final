// MWO Config File
// version 3.1 (WiP)
// by von Pilsner (thanks to HackNFly!@!!!)
//
// NOTE: Calibrate using BvP-Simple-3.0.cs
//       Or your pedals may not work properly!!
//
// Uses default MWO keybindings and axis as of Jan 28, 2013
//
// For use with Steel-Batallion-64_v2_beta.zip 
// 64 bit driver code/glue by HackNFly  http://code.google.com/p/steel-batallion-64/
//
// I suggest you add the folowing to user.cfg (remove the //'s)
//
// cl_joystick_gain = 5.05
// cl_joystick_sensitivity = 1.00
// cl_joystick_invert_throttle = 0
// cl_joystick_invert_pitch = 1
// cl_joystick_invert_yaw = 0
// cl_joystick_invert_turn = 0
// cl_joystick_throttle_range = 0
// 
// ; Joystick DeadZone, requires both the i and cl lines to make the joystick deadzone change work
// ; Increase in 0.02 incriments if you get an unstable center point on either stick (0.08 works for some).
// i_joystick_deadzone = 0.04
// cl_joystick_deadzone = 0.04
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
		String debugString = "";
		
		int desiredX;
		int desiredY;
		int currentX = 0;
		int currentY = 0;
		int numPixelX = 600;//number of pixels to move in X direction
		int numPixelY = 300;//number of pixels to move in X direction
		int numPixelExtraX = 25;//used at extreme edges, number of pixels per poll
		int numPixelExtraY = 20;//used at extreme edges

        const int refreshRate = 30; // Number of milliseconds between call to mainLoop
		const int maxAxisValue = 32768;
		double sideZone = 0.05;//percent of swing that will cause mouse to move continuously
		int jj = 0;
		bool startedTracking = false;//used to make switching mouse on and off not jumpy

        // This gets called once by main program
        public void Initialize()
        {
            int baseLineIntensity = 3; // Just an average value for LED intensity
            int emergencyLightIntensity = 15; // For stuff like eject,cockpit Hatch,Ignition, and Start

            controller = new SteelBattalionController();
            controller.Init(30); // 50 is refresh rate in milliseconds

            //set all buttons by default to light up only when you press them down
            for (int i = 4; i < 4 + 30; i++)
            {
                if (i != (int)ButtonEnum.Eject) // Excluding eject since we are going to flash that one
                    controller.AddButtonLightMapping((ButtonEnum)(i - 1), (ControllerLEDEnum)(i), true, baseLineIntensity);
            }

            // Button Bindings
			controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire, Microsoft.DirectX.DirectInput.Key.D1,true);
			controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D2,true);
            controller.AddButtonKeyMapping(ButtonEnum.RightJoyLockOn, Microsoft.DirectX.DirectInput.Key.R, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.Eject, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.Ignition, true, 3, Microsoft.DirectX.DirectInput.Key.P, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Start, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);			
            controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose, true, 3, Microsoft.DirectX.DirectInput.Key.B, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut, true, 3, Microsoft.DirectX.DirectInput.Key.I, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect, true, 3, Microsoft.DirectX.DirectInput.Key.Q, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn, true, 3, Microsoft.DirectX.DirectInput.Key.Z, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut, true, 3, Microsoft.DirectX.DirectInput.Key.V, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator, true, 3, Microsoft.DirectX.DirectInput.Key.J, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange, true, 3, Microsoft.DirectX.DirectInput.Key.H, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.Washing, true, 3, Microsoft.DirectX.DirectInput.Key.C, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher, true, 3, Microsoft.DirectX.DirectInput.Key.Delete, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Chaff, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);
			controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach, true, 3, Microsoft.DirectX.DirectInput.Key.Slash, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope, true, 3, Microsoft.DirectX.DirectInput.Key.N, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, true, 3, Microsoft.DirectX.DirectInput.Key.Tab, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3, true, 4, Microsoft.DirectX.DirectInput.Key.LeftControl, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain, true, 3, Microsoft.DirectX.DirectInput.Key.RightControl, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub, true, 3, Microsoft.DirectX.DirectInput.Key.BackSpace, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine, true, 3, Microsoft.DirectX.DirectInput.Key.X, true);
			//controller.AddButtonKeyLightMapping(ButtonEnum.ToggleBufferMaterial, true, 3, Microsoft.DirectX.DirectInput.Key.LeftShift, true);
            // controller.AddButtonKeyMapping(ButtonEnum.LeftJoySightChange, Microsoft.DirectX.DirectInput.Key.V, true);
            controller.AddButtonKeyMapping(ButtonEnum.LeftJoySightChange, Microsoft.DirectX.DirectInput.Key.Z, true);

            joystick = new vJoy();
            acquired = joystick.acquireVJD(1);
            joystick.resetAll(); //have to reset before we use it
			
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_SL1);			
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_X);
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_Y);
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_Z);//throttle
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_RZ);
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_SL0);		
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_RX);				
			joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_RY);
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
		
			
		private double expo(int value)
		{
			double tempIn = ((double)value / (double)maxAxisValue)*2 - 1;//scale to -1 to 1
			tempIn *= tempIn * 0.5;//this applies expo, we get 0 to 1 regardless of sign then multiply by 0.5 since it only represents
								  //half of the value
			tempIn = Math.Abs(tempIn);
			
			if(value >= maxAxisValue/2)
				tempIn += 0.5;
			else
				tempIn = 0.5 - tempIn;
			

			//debugString += "value:" + value.ToString() + " tempIn : " + tempIn.ToString() + "\n";
			
			return tempIn;
		}
	
	
		public int scaleValue(int value, int low, int middle, int high, int deadzone)
		{
			double temp;
			debugString += value + " " + low + " " + middle + " " + high + " " + deadzone + "\n";
			if(Math.Abs(value - middle) < deadzone)
				temp = ((middle-low)/(double)(high - low))*maxAxisValue;
			else
			{			
				if(value < middle)
					temp = ((double)(value - low) / (double)(middle - low) * 0.5)*maxAxisValue;
				else
					temp = ((double)(value - middle) / (double)(high - middle) * 0.5 + 0.5)*maxAxisValue;
			}
			//clamp for extraneous values
			if(temp > maxAxisValue)
				temp = maxAxisValue;
			if(temp < 0)
				temp = 0;
			return (int) temp;
		}
		
		public int scaleValue(int value, int low, int high)
		{
			double temp;
			
			temp = ((double)(value - low) / (double)(high - low) * 0.5)*maxAxisValue;

			//clamp for extraneous values
			if(temp > maxAxisValue)
				temp = maxAxisValue;
			if(temp < 0)
				temp = 0;
			return (int) temp;
		}

		private int getDeltaS(int axisVal, int desiredVal, int currentVal,int pixelExtra)
		{
			int delta = desiredVal - currentVal;
			//int temp  = (int) (expo(axisVal)*pixels);
			//debugString += "axisVal" + axisVal.ToString() + " " + expo(axisVal).ToString() + " temp " + temp.ToString() + " "
			//+ "desiredVal:" + desiredVal.ToString() + " currentVal : " + currentVal.ToString() + "\n";
			double tempD = (double)axisVal/(double)maxAxisValue;
			if(tempD > (1 - sideZone))//sidezone is a percentage, i.e. 0.05 for 5 percent
			{
				tempD = (tempD - (1- sideZone));
				tempD = tempD/sideZone * pixelExtra;//defined at top
				return (int)tempD;
			}
			if(tempD < sideZone)
			{
				tempD = (sideZone - tempD)/sideZone * pixelExtra;
				return (int)(-1*tempD);
			}
			return delta;
		}
		
		private int reverse(int val)
		{
			return (maxAxisValue - val);
		}
		/*
		//new optional function used for debugging purposes, comment out when running in game as it causes issues
		public String getDebugString()
		{
			return debugString;
		}
		*/
		

        //this gets called once every refreshRate milliseconds by main program
        public void mainLoop()
        {
			debugString = "";
            //updatePOVhat();
			
			int aimingX = scaleValue(controller.AimingX,1,512,1023,5);
			int aimingY = reverse(scaleValue(controller.AimingY,1,512,1021,5));//calibration values
			int rAxis	= scaleValue(controller.RotationLever,-421,1,510,5);//calibration values
			int sCX		= scaleValue(controller.SightChangeX,-461,0,470,5);
			int sCY		= scaleValue(controller.SightChangeX,-480,-5,463,5);
			int lPedal	= scaleValue(controller.LeftPedal,30,1022);
			int mPedal	= scaleValue(controller.MiddlePedal,128,1021);
			int rPedal	= scaleValue(controller.RightPedal,0,1020);
			
			desiredX	= (int)(expo(aimingX)*numPixelX);//numPixels stores resolution, i.e. how much we move mouse
			desiredY	= (int)(expo(aimingY)*numPixelY);
			rAxis		= (int)(expo(rAxis)*maxAxisValue);
			debugString += "expoX " + expo(aimingX) + "\n";
			debugString += "expoY " + expo(aimingY) + "\n";
			debugString += "expoR " + expo(rAxis) + "\n";
			if((bool)controller.GetButtonState(ButtonEnum.ToggleBufferMaterial))
			{
				
				//debugString += "aimingX:" + aimingX.ToString()  + " desiredX : " + desiredX.ToString() + "currentX " + currentX.ToString() + "\n";
				//debugString += "aimingY:" + aimingY.ToString() + " desiredY : " + desiredY.ToString() + "currentY " + currentY.ToString() + "\n";			
				int deltaX = getDeltaS(aimingX,desiredX,currentX,numPixelExtraX);
				int deltaY = getDeltaS(aimingY,desiredY,currentY,numPixelExtraY);
				currentX = desiredX;
				currentY = desiredY;
				if(startedTracking)//makes it so you can flip the switch and recenter the joystick
					InputSimulator.MoveMouseBy(deltaX,deltaY);
				else
					startedTracking = true;
			}
			else
			{
				startedTracking = false;
			}
		
            // Joystick Axes
			debugString += "rAxis = " + rAxis + "\n";
				
			//joystick.setAxis(1, xaxis, HID_USAGES.HID_USAGE_X);
            //joystick.setAxis(1, yaxis, HID_USAGES.HID_USAGE_Y);
			joystick.setAxis(1, rAxis, HID_USAGES.HID_USAGE_RZ);
            //joystick.setAxis(1, sCX, HID_USAGES.HID_USAGE_SL1);
            //joystick.setAxis(1, sCY, HID_USAGES.HID_USAGE_RX);
			joystick.setAxis(1,(rPedal - mPedal) + maxAxisValue/2,HID_USAGES.HID_USAGE_SL0);//throttle
            //joystick.setAxis(1, controller.GearLever, HID_USAGES.HID_USAGE_SL0);
            joystick.setContPov(1, getDegrees(controller.SightChangeX, controller.SightChangeY), 1);

            // Pedals Section
			if ((jj == 0) && (lPedal > (maxAxisValue*0.10))) // Left pedal pressed
            {
                controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Space);
                jj = 1;
            }
            else if ((jj == 1) && (lPedal < (maxAxisValue*0.10))) // Left pedal released
            {
                controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Space);
                jj = 0;
            }
		joystick.sendUpdate(1);
        }

        // This gets called at the end of the program and must be present, as it cleans up resources
        public void shutDown()
        {

            controller.UnInit();
            joystick.Release(1);
        }
    }
}