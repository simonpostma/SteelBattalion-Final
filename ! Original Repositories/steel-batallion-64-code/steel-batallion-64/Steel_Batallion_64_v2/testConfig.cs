// MWO Config File
// version 2.0
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
    public class testConfig
    {
        String debugString = "";
        SteelBattalionController controller;
        vJoy joystick;
        bool acquired;
        bool mouseStarted = false;
        int desiredX;
        int desiredY;
        int currentX = -1;
        int currentY = -1;

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
            /*
            controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch, true, 3, SBC.Key.A, true);//last true means if you hold down the button,		
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, true, 3, SBC.Key.B, true);
            controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, SBC.Key.C, true);*/

            joystick = new vJoy();
            acquired = joystick.acquireVJD(1);
            joystick.resetAll();//have to reset before we use it

            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_SL1);
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_X);
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_Y);
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_Z);//throttle
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_RZ);
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_SL0);
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_RX);
            joystick.setAxis(1,32768 / 2, HID_USAGES.HID_USAGE_RY);

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

            temp += 90;//origin is vertical on POV not horizontal

            if (temp > 360)//by adding 90 we may have gone over 360
                temp -= 360;

            temp *= 100;

            if (temp > 35999)
                temp = 35999;
            if (temp < 0)
                temp = 0;
            return temp;
        }

        //	private int scaledValue(int min, int middle, int max, int deadZone)



        //this gets called once every refreshRate milliseconds by main program
        public void mainLoop()
        {




            joystick.setAxis(1,controller.GearLever,HID_USAGES.HID_USAGE_SL1);

            joystick.setAxis(1,controller.AimingX,HID_USAGES.HID_USAGE_X);
            joystick.setAxis(1,controller.AimingY,HID_USAGES.HID_USAGE_Y);

            joystick.setAxis(1,-1 * (controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle
            joystick.setAxis(1,controller.RotationLever,HID_USAGES.HID_USAGE_RZ);
            joystick.setAxis(1,controller.SightChangeX,HID_USAGES.HID_USAGE_SL0);
            joystick.setAxis(1,controller.SightChangeY,HID_USAGES.HID_USAGE_RX);
            joystick.setAxis(1,controller.LeftPedal,HID_USAGES.HID_USAGE_RY);


            joystick.setContPov(1,getDegrees(controller.SightChangeX, controller.SightChangeY), 1);


            for (int i = 1; i <= 39; i++)
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
	
    