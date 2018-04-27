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
        int pedalmode = 1;

        const int refreshRate = 30; // Number of milliseconds between call to mainLoop

        int speed = 50;
        int speedPrevious = 50; //used to check state change of GearLever
        int peddleDeadZone = 30; //used to avoid peddle input when at rest
        int peddleLoop = 0;//used to stop resting peddle loop

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

            //this is an example                  1st parameter                         2nd   3rd                                        4th
            //controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,              true, 3,    Microsoft.DirectX.DirectInput.Key.A, true);

            //First variable defines which key on the SteelBatallionController you want to affect.
            //The second parameter controls state change. if you change the parameter from true to false, then the button becomes a statebutton, useful for modes such as nightvision where you press the button to toggle the state.
            //The third parameter controls the light intensity, any value from 1 - 15 is acceptable.
            //The fourth variable for AddButtonKeyLightMapping controls whether or not the button is held down when you press it. If changed to "false" the button will press only one time even if you hold it down.
            //In order to define what key you want the button to press you need to know the DirectX.DirectInput key enumerations, they are available here:http://msdn.microsoft.com/en-us/library/windows/desktop/bb321074(v=vs.85).aspx

            //Button Bindings

            //controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,              true, 3,    Microsoft.DirectX.DirectInput.Key.A, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.RightJoyLockOn,            true, 3,    Microsoft.DirectX.DirectInput.Key.R, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Eject,					    true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Ignition,				    true, 3,    Microsoft.DirectX.DirectInput.Key.P, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Start, true, 15, Microsoft.DirectX.DirectInput.Key.S, true); //startup toggle
            //            controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose,           false, 3,   Microsoft.DirectX.DirectInput.Key.LeftControl, Microsoft.DirectX.DirectInput.Key.R, true); // not working, nothing sent to game
            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, false, 3, Microsoft.DirectX.DirectInput.Key.LeftShift, Microsoft.DirectX.DirectInput.Key.F, false); // not working, nothing sent to game			
            //controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut,	    true, 3,    Microsoft.DirectX.DirectInput.Key.B, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect,	    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor,	    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn,			    true, 3,    Microsoft.DirectX.DirectInput.Key.Z, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut,		    true, 3,    Microsoft.DirectX.DirectInput.Key.Z, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS,			    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator,	    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange,	false, 3,   Microsoft.DirectX.DirectInput.Key.H, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Washing,					true, 3,    Microsoft.DirectX.DirectInput.Key.C, true);
            controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher, true, 3, Microsoft.DirectX.DirectInput.Key.F, false);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Chaff,					    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach,	    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride,		    true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
//            controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope, false, 3, Microsoft.DirectX.DirectInput.Key.A, true); //light amplification toggle
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2,			    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3,			    true, 3,    Microsoft.DirectX.DirectInput.Key.LeftControl, true);
  //          controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain, true, 3, Microsoft.DirectX.DirectInput.Key.Z, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub,			    true, 3,    Microsoft.DirectX.DirectInput.Key.BackSpace, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine,		    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Comm1,	                    true, 3,    Microsoft.DirectX.DirectInput.Key.F6, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Comm2,	                    true, 3,    Microsoft.DirectX.DirectInput.Key.F8, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Comm3,	                    true, 3,    Microsoft.DirectX.DirectInput.Key.F9, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Comm4,	                    true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(ButtonEnum.Comm5,	                    true, 3,    Microsoft.DirectX.DirectInput.Key.RightBracket, true);
            //controller.AddButtonKeyMapping(ButtonEnum.LeftJoySightChange,                         Microsoft.DirectX.DirectInput.Key.Z, true);
            //controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D6, Microsoft.DirectX.DirectInput.Key.D5, true);

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
            int thumbstickDeadZone = 25;
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


        public void setThrottleSlider()
        {
            speed = controller.GearLever;

            //if (controller.GetButtonState(ButtonEnum.ToggleFilterControl))
            //{

            if ((speedPrevious != speed) && (ticka == 0))
            {
                switch (speed)
                {
                    case -2:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D1);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D1);
                        speedPrevious = -2;
                        break;
                    case -1:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D1);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D1);
                        speedPrevious = -1;
                        break;
                    case 1:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D2);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D2);
                        speedPrevious = 1;
                        break;
                    case 2:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D4);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D4);
                        speedPrevious = 2;
                        break;
                    case 3:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D6);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D6);
                        speedPrevious = 3;
                        break;
                    case 4:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D8);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D8);
                        speedPrevious = 4;
                        break;
                    case 5:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D0);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D0);
                        speedPrevious = 5;
                        break;
                }
            }
            //}
        }

        public void setThrottlePeddle()
        {
            if ((speed > 0) && (Math.Abs(controller.RightPedal) > peddleDeadZone) && (tickb == 0))
            {
                joystick.setAxis(1, -1 * (controller.RightPedal), HID_USAGES.HID_USAGE_Z);//forward throttle
                peddleLoop = 1;
            }

            else if ((speed == -2) && (Math.Abs(controller.RightPedal) > peddleDeadZone) && (tickb == 0))
            {
                joystick.setAxis(1, ((controller.RightPedal) + 120), HID_USAGES.HID_USAGE_Z);//reverse throttle
                peddleLoop = 1;
            }

            if ((speed > 0) && (Math.Abs(controller.RightPedal) < peddleDeadZone) && (peddleLoop == 1) && (tickb == 0))
            {
                switch (speed)
                {
                    case -2:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D1);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D1);
                        speedPrevious = -2;
                        break;
                    case -1:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D1);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D1);
                        speedPrevious = -1;
                        break;
                    case 1:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D2);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D2);
                        speedPrevious = 1;
                        break;
                    case 2:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D4);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D4);
                        speedPrevious = 2;
                        break;
                    case 3:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D6);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D6);
                        speedPrevious = 3;
                        break;
                    case 4:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D8);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D8);
                        speedPrevious = 4;
                        break;
                    case 5:
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.D0);
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.D0);
                        speedPrevious = 5;
                        break;
                }
                peddleLoop = 0;
            }
        }



        //this gets called once every refreshRate milliseconds by main program
        public void mainLoop()
        {
            /*
                updatePOVhat();
                setThrottleSlider();
                setThrottlePeddle();

                // My pedal timer... this allows slow pedal updating with fast controller polling
                int loopdelay = 4;
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

            */
            /*
            HID_USAGES.HID_USAGE_X      // shows up as X axis of calibration square on first screen
            HID_USAGES.HID_USAGE_Y      // shows up as Y axis of calibration square on first screen
            HID_USAGES.HID_USAGE_Z      // Z axis --third screen
            HID_USAGES.HID_USAGE_RZ     // Z Rotation - sixth screen
            HID_USAGES.HID_USAGE_SL0    // Slider - seventh screen
            HID_USAGES.HID_USAGE_RX     // X Rotation -- fourth screen
            HID_USAGES.HID_USAGE_RY     // Y Rotation -- fifth screen
            HID_USAGES.HID_USAGE_SL1    // Dial -- eighth screen
            */

            // Joystick Axes
            /*joystick.setAxis(1, controller.AimingX, HID_USAGES.HID_USAGE_X);
            joystick.setAxis(1, controller.AimingY, HID_USAGES.HID_USAGE_Y);
            joystick.setAxis(1, (controller.RightPedal - controller.LeftPedal), HID_USAGES.HID_USAGE_Z); // Throttle or Rudder
            joystick.setAxis(1, controller.RotationLever, HID_USAGES.HID_USAGE_RZ);
            joystick.setAxis(1, controller.SightChangeX, HID_USAGES.HID_USAGE_SL1);
            joystick.setAxis(1, controller.SightChangeY, HID_USAGES.HID_USAGE_RX);
            joystick.setAxis(1, controller.MiddlePedal, HID_USAGES.HID_USAGE_RY);
            joystick.setAxis(1, controller.GearLever, HID_USAGES.HID_USAGE_SL0);
            joystick.setContPov(1, getDegrees(controller.SightChangeX, controller.SightChangeY), 1);*/
            /*
                        joystick.setAxis(1, controller.AimingX, HID_USAGES.HID_USAGE_X);
                        joystick.setAxis(1, controller.AimingY, HID_USAGES.HID_USAGE_Y);
                        joystick.setAxis(1, controller.RotationLever, HID_USAGES.HID_USAGE_RZ);
                        joystick.setAxis(1, controller.SightChangeX, HID_USAGES.HID_USAGE_SL0);
                        joystick.setAxis(1, controller.SightChangeY, HID_USAGES.HID_USAGE_RX);
                        joystick.setAxis(1, controller.LeftPedal, HID_USAGES.HID_USAGE_RY);
                        joystick.setAxis(1, controller.GearLever, HID_USAGES.HID_USAGE_SL1);
                        joystick.setContPov(1, getDegrees(controller.SightChangeX, controller.SightChangeY), 1);*/
            //joystick.setAxis(1,(controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle




            // toggle tricks!!!

            /*
            if (controller.GetButtonState(ButtonEnum.ToggleFilterControl)) //FILT Toggle (Fire Selected / Alpha Strike)
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire, Microsoft.DirectX.DirectInput.Key.BackSlash, true); // Alpha Strike
                joystick.setButton((bool)controller.GetButtonState(ButtonEnum.RightJoyFire), (uint)1, (char)(15)); // unused button (placeholder)
            }
            else
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire, Microsoft.DirectX.DirectInput.Key.NoConvert, true); // unused button (placeholder)
                joystick.setButton((bool)controller.GetButtonState(ButtonEnum.RightJoyFire), (uint)1, (char)(0)); // Fire Selected Weapon (joystick button 0)
            }

            if (controller.GetButtonState(ButtonEnum.ToggleOxygenSupply)) // O2 Supply Toggle (Wpn group 5 / Wpn group 6)
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D6, true);
            }
            else
            {
                controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D5, true);
            }
            */



            /*
            
            if (controller.GetButtonState(ButtonEnum.ToggleBufferMaterial)) // Toggle Buffer Material (Alt. Pedal Mode)
            {
                pedalmode = 0;  // Set Pedals to Alt Mode
            }
            else
            {
                pedalmode = 1;
            }

            // Pedals Section
            if ((jj == 1) && ((controller.LeftPedal) < 50)) // Left pedal released
            {
                controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Space);
                jj = 0;
            }

            if (((controller.LeftPedal) > 50) && (ticka == 0)) // Left pedal pressed
            {
                if (controller.GetButtonState(ButtonEnum.ToggleFuelFlowRate)) // Is the FUEL Flow toggle UP? (UP = Jump Mode)
                {
                    controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Space);
                    ticka = 0;	// bypass timer so we can jump!
                    loopa = 0;
                    jj = 1;		// Jets are on...
                }
                else // FUEL Flow toggle must be down...
                {
                    if (pedalmode == 0) // Default Pedal Mode
                    {
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.X);
                        ticka = 1;
                        loopa = 0;
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.X);
                    }
                    else	// Alt Mode (Swap Left and Middle Pedal)
                    {
                        controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.S);
                        ticka = 1;
                        loopa = 0;
                        controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.S);
                    }
                }
            }

            if (((controller.MiddlePedal) > 50) && (tickb == 0)) // Middle pedal pressed
            {
                if (pedalmode == 0) // Default Pedal Mode
                {
                    controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.S);
                    tickb = 1;
                    loopb = 0;
                    controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.S);
                }
                else	// Alt Mode (Swap Left and Middle Pedal)
                {
                    controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.X);
                    tickb = 1;
                    loopb = 0;
                    controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.X);
                }
            }

            if (((controller.RightPedal) > 50) && (tickc == 0)) // Right pedal pressed
            {
                controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.W);
                tickc = 1;
                loopc = 0;
                controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.W);
            }
            */
            /*
                        for (int i = 1; i <= 32; i++)
                        {
                            joystick.setButton((bool)controller.GetButtonState(i - 1), (uint)1, (char)(i - 1));
                        }
            
                        joystick.sendUpdate(1);*/
        }

        // This gets called at the end of the program and must be present, as it cleans up resources
        public void shutDown()
        {
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftShift); // Release latched key
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftControl);
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.W);
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.S);
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.X);
            controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Space);
            controller.UnInit();
            joystick.Release(1);
        }
    }
}

/*
BVP-MWO.cs Toggle and Pedals Explanation

It uses the default game button mapping. I got the toggles to work - they can be a bit confusing....

If FILT Toggle is down your Sub Weapon (trigger) is 'Fire Selected'
If FILT Toggle is up your Sub Weapon (trigger) is 'Alpha Strike'

If O2 Toggle is down your Main weapon button fires weapon group 5
If O2 Toggle is up your Main weapon button fires weapon group 6

If Fuel Flow Toggle is down your left pedal is 'full stop'
If Fuel Flow Toggle is up your left pedal is 'jump jet'

Right pedal is accelerate/increase throttle.
Middle pedal is decelerate/reverse.
Left pedal is dependent on the position of the 'Fuel Flow Rate' toggle. Up = Jump Jets, Down = Full Stop.

The 'Function F3' button on the controller is latched, press once to enable free look and press again to disable it.


SETUP

As of profile version 1.0 it should use the default bindings for buttons and all axis.

If you find the 'sub weapon trigger' is always alphaing then make sure that 'Fire Selected Weapon Group' is bound to 'j0_button0'.


TROUBLESHOOTING

Calibrate 'vJoy Device' in 'Game Controllers, this step is very important!!!

Remove your 'actionmaps.xml' file (it's at '..\MechWarrior Online\USER\Profiles\<your profile name>\actionmaps.xml') and then re-bind any non-working keys/axis.

POV twitch fix:
If the POV / weapon group control seems twitchy remap the 4 'Weapon Group Hilight' commands to 'button0' (trigger/sub weapon) one at a time. Then remap 'button0' back to 'Fire Selected Weapon Group' and save your options.

Alt. POV twitch fix - Remove the POV using the vJoyConf utility:
http://bs.beckament.net/files_pub/Mechwarrior%20Online/VTController/64bit/vJoyConfx64.zip. Tick all the Basic and Additional axis boxes and click save (leave it at 0 POVs), it will still work in-game.

'vJoy Device' does not show up in 'Game Controllers' but 'steel battalion' driver is loading in 'Device Manager':
I installed the vJoy client (directly) and the controller showed up in 'Game Controllers' http://vjoystick.sourceforge.net/site/index.php/download-a-install/72-download

Buttons do not work in-game:
Right click 'runSteelBatallion64' and select 'Run as Administrator'.

Buttons do not work in-game:
Disable UAC (this could make your computer slightly less secure): http://www.mydigitallife.info/how-to-disable-and-turn-off-uac-in-windows-7/

Trigger does not seem to work or is always alpha striking:
Verify that 'Fire Selected Weapon Group' is bound to 'j0_button0' (Sub Weapon Trigger) and that FILT toggle is set to down.


NOTES

AS Of version 1.0 the axes are mapped differently to better accomodate the default game controls.

For calibration and reference purposes here is the new axis mapping:

X/Y-Axis: Right stick
Z-Axis: Middle and Right Pedal.
X-Rotation: Site Change Vertical
Y-Rotation: Left Pedal
Z-Rotation: Left Stick
Slider: Throttle
Dial: Site Change Horizontal

In my personal install I have removed the POV from the 'vJoy device' using using the vJoyConf utility: http://bs.beckament.net/files_pub/Mechwarrior%20Online/VTController/64bit/vJoyConfx64.zip 
Tick all the Basic and Additional axis boxes and click save (leave it at 0 POVs), it will still work in-game.


FILES

Download site: 		http://code.google.com/p/steel-batallion-64/downloads/list
LibUSBDotNet_Setup:	http://code.google.com/p/steel-batallion-64/downloads/list
MWO profile:		http://bs.beckament.net/files_pub/Mechwarrior%20Online/VTController/64bit/BvP-MWO.cs
Example user.cfg:	http://bs.beckament.net/files_pub/Mechwarrior%20Online/VTController/64bit/user.cfg	
Button diagram:		http://bs.beckament.net/files_pub/Mechwarrior%20Online/VTController/64bit/controller-mwo-64-0.3.png

Good Luck,
-von Pilsner
*/