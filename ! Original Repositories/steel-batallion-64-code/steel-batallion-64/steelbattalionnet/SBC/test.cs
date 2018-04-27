using SBC;
using System;
namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;

const int numJoysticks = 1;
const int defaultAxisValue = 100;

//Variables used in extraCode section
bool currentResetValue;
bool lastResetValue;
bool jumpPressed = false;

bool stopPressed = false;//used in special handling of left pedal
int pedalTriggerLevel = 50;
bool performingAlphaStrike = false;
int thumbstickDeadZone = 25;
bool inverseThumbStick = false;
int inverseThumbStickMultiplier;
double maxZoomMultiplier = 1.5;//used for integrating thumbstick into zooming system


	public int getNumJoysticks()
	{
	    return numJoysticks;
	}

    public void Initialize()
    {
        int baseLineIntensity = 1;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds
		//set all buttons by default to light up only when you press them down
        /*
		for(int i=4;i<4+30;i++)
		{
			if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((ButtonEnum)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}
         setting these manually, leaving loop for example*/
        
		//add simple buttonlight mapping,false = keep light on, i.e. a button that changes states
        //public void AddButtonKeyLightMapping(ButtonEnum button, bool lightOnHold, int intensity, VirtualKeyCode keyCode, bool holdDown)
        controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    VirtualKeyCode.VK_V, true);//last true means if you hold down the button,
        controller.AddButtonKeyLightMapping(ButtonEnum.Ignition,                true, 15,   VirtualKeyCode.VK_S, true);//its the same as holding down the key
        controller.AddButtonKeyLightMapping(ButtonEnum.Start,                   true, 15,   VirtualKeyCode.VK_O, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut,    true, 3,    VirtualKeyCode.VK_R, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut,          true, 3,    VirtualKeyCode.VK_N, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator,     true, 3,    VirtualKeyCode.OEM_5, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange, false, 7,   VirtualKeyCode.VK_L, true);//false means button changes states
        controller.AddButtonKeyLightMapping(ButtonEnum.Washing,                 true, 3,    VirtualKeyCode.SPACE, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher,            true, 3,    VirtualKeyCode.VK_F, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Chaff,                   true, 3,    VirtualKeyCode.VK_P, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach,      true, 3,    VirtualKeyCode.TAB, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope,      false, 7,   VirtualKeyCode.VK_A, true);//false means button changes states
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1,              true, 3,    VirtualKeyCode.VK_E, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3,              true, 3,    VirtualKeyCode.VK_W, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain,           true, 3,    VirtualKeyCode.OEM_6, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub,            true, 3,    VirtualKeyCode.OEM_4, true);

        /*controller.AddButtonKeyLightMapping(ButtonEnum.Comm1, true, 3,
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm2, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm3, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm4, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm5, true, 3);dealing with these manually*/




        //COMPOUND button light mapping, these use a modifier key, followed by another key
        controller.AddButtonKeyLightMapping(ButtonEnum.Eject,               true, 15,   VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose,   true, 3,    VirtualKeyCode.MENU,    VirtualKeyCode.VK_C, true);//MENU is the same as alt
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect,  true, 3,    VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor,  true, 7,    VirtualKeyCode.SHIFT,   VirtualKeyCode.VK_M, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn,       true, 3,    VirtualKeyCode.CONTROL, VirtualKeyCode.VK_N, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS,         true, 7,    VirtualKeyCode.MENU,    VirtualKeyCode.VK_H, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride,    true, 3,    VirtualKeyCode.SHIFT,   VirtualKeyCode.VK_O, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2,          true, 3,    VirtualKeyCode.SHIFT,   VirtualKeyCode.VK_E, true);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine,   true, 7,    VirtualKeyCode.SHIFT,   VirtualKeyCode.VK_C, true);

        //simple key mapping, toggles don't have lights
        controller.AddButtonKeyMapping(ButtonEnum.ToggleFilterControl,                  VirtualKeyCode.OEM_4,                        true);
        controller.AddButtonKeyMapping(ButtonEnum.ToggleOxygenSupply,                   VirtualKeyCode.OEM_4,                        true);
        controller.AddButtonKeyMapping(ButtonEnum.LeftJoySightChange,                   VirtualKeyCode.NUMPAD0,                      true);
        controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,                   VirtualKeyCode.DELETE_key,                         false);//true means send separate keydown/keyup commands
        controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire,                         VirtualKeyCode.RETURN,                              false);


        if(inverseThumbStick)
	        inverseThumbStickMultiplier = 1;
        else
	        inverseThumbStickMultiplier = -1;

	}
    // return number of buttons per joystick, program will then ask for values for each button (only using one joystick right now)
    public int getNumButtons(int joyNum)
    {
        return 13;
    }

    //return number of axis per joystick, program will then ask for values for each axis  (only using one joystick right now)
    public int getNumAxis(int joyNum)
    {
        return 8;//8 happens to be the maximum number of axes that PPJoy/Windows currently supports
    }
    public int getAxisValue(int joyNum, int axisNum)
    {
        double zoomMultiplier;
        int zoomLever;
        if (controller.GearLever > 0)
            zoomLever = controller.GearLever + 1;
        else
            zoomLever = Math.Abs(controller.GearLever);

        zoomMultiplier = (zoomLever / 6.0) * maxZoomMultiplier;//want to start at a zoom level of 1

        //technically we should be accounting the joystickNumber as well, but only dealing with one joystick for the moment
        //we should handle all cases from 0 to getNumAxis()
        switch (axisNum)
        {
            //case 0 means we asked for what is the value to axis number 0
            case 0:
                return controller.AimingX;//we are assigning controller.AimingX to PPJoy axis 0
            case 1:
                if (controller.GearLever > -2)//reverse
                    return (int)(controller.AimingY + inverseThumbStickMultiplier * (zoomMultiplier * controller.SightChangeY));
                else
                    return controller.AimingY;
            case 2:
                return -1*(controller.RightPedal - controller.MiddlePedal);//throttle;
            case 3:
                if (controller.GearLever > -2)//reverse
                    return (int)(controller.RotationLever + (zoomMultiplier * controller.SightChangeX));
                else
                    return controller.RotationLever;
            case 4:
                return controller.SightChangeX;
            case 5:
                return controller.SightChangeY;
            case 6:
                return controller.LeftPedal;
            case 7:
                return controller.GearLever;

        }
        return defaultAxisValue;//we always have to return something
    }
    //we only want to use keymapping or buttonmapping for now, no reason you can't do both, but it might end up being confusing having
    //a button that both presses a joystick button and a key
    public bool useButtons()
    {
        return false;//using keymapping for now
    }
    public bool getButtonValue(int joyNum, int buttonNumber)
    {
        //technically we should be accounting the joystickNumber as well, but only dealing with one joystick for the moment
        return controller.GetButtonState(buttonNumber);
    }

    
void updatePOVhat()
{
	SBC.POVdirection lastDirection = controller.POVhat;

	if(( (Math.Abs(controller.SightChangeX) > thumbstickDeadZone) || (Math.Abs(controller.SightChangeY) > thumbstickDeadZone) ) && (controller.GearLever == -2) )//reverse)
	{
		if(Math.Abs(controller.SightChangeX) > Math.Abs(controller.SightChangeY))
			if(controller.SightChangeX <0)
				controller.POVhat = SBC.POVdirection.LEFT;
			else
				controller.POVhat = SBC.POVdirection.RIGHT;
		else
			if(controller.SightChangeY <0)
				controller.POVhat = SBC.POVdirection.DOWN;
			else
				controller.POVhat = SBC.POVdirection.UP;

	}
	else
	{
		controller.POVhat = SBC.POVdirection.CENTER;	
	}

	if(lastDirection != controller.POVhat)
	{
		switch(lastDirection)
		{
			case SBC.POVdirection.LEFT:
				controller.sendKeyUp(SBC.VirtualKeyCode.LEFT);
				break;
			case SBC.POVdirection.RIGHT:
				controller.sendKeyUp(SBC.VirtualKeyCode.RIGHT);
				break;
			case SBC.POVdirection.DOWN:
				controller.sendKeyUp(SBC.VirtualKeyCode.VK_I);
				break;
			case SBC.POVdirection.UP:
				controller.sendKeyUp(SBC.VirtualKeyCode.VK_M);
				break;
		}
	}
	else
	{
		switch(controller.POVhat)
		{
			case SBC.POVdirection.LEFT:
				controller.sendKeyDown(SBC.VirtualKeyCode.LEFT);
				break;
			case SBC.POVdirection.RIGHT:
				controller.sendKeyDown(SBC.VirtualKeyCode.RIGHT);
				break;
			case SBC.POVdirection.DOWN:
				controller.sendKeyDown(SBC.VirtualKeyCode.VK_I);
				break;
			case SBC.POVdirection.UP:
				controller.sendKeyDown(SBC.VirtualKeyCode.VK_M);
				break;
		}
	}
}

void evaluateModifiableButton(SBC.ButtonEnum toggleButton,SBC.ButtonEnum mainButton,SBC.VirtualKeyCode state1,SBC.VirtualKeyCode state2)
{
	//deal with RightJoyMainWeapon and firing groups 3/4
	if(controller.GetButtonState((int)toggleButton))//up position
	{
		if((int)controller.GetButtonKey(mainButton) == (int)state1)
			controller.AddButtonKeyMapping(mainButton,state2,false);//had to rename delete to delete_key because delete is reserved
	}
	else
	{
		if(controller.GetButtonKey(mainButton) == state2)
			controller.AddButtonKeyMapping(mainButton,state1,false);
	}
}

    public void extraCode()//place any extra code you want executed during the loop here.
    {
        
	updatePOVhat();//updates POVhat, and sends appropriate keypress downs and ups depending on gear lever and thumbstick position

        currentResetValue = controller.GetButtonState((int)ButtonEnum.ToggleFuelFlowRate);
	    if(currentResetValue != lastResetValue && currentResetValue)
	    {
    		controller.TestLEDs(1);//reset lights
	    }
	    lastResetValue = currentResetValue;

        evaluateDualLeftPedal(controller,VirtualKeyCode.VK_J,VirtualKeyCode.VK_1);
	    evaluateModifiableButton(controller,ButtonEnum.ToggleFilterControl,ButtonEnum.RightJoyMainWeapon,VirtualKeyCode.PRIOR,VirtualKeyCode.DELETE_key);

	    if(!controller.GetButtonState((int)ButtonEnum.RightJoyFire) && performingAlphaStrike)//we were performing alphastrike and now we're not
		    performingAlphaStrike = false;

	    if(controller.GetButtonState((int)ButtonEnum.ToggleOxygenSupply))
		    if(controller.GetButtonState((int)ButtonEnum.RightJoyFire) && !performingAlphaStrike)
		    {
			    controller.sendKeyPress(VirtualKeyCode.INSERT);//group1
			    controller.sendKeyPress(VirtualKeyCode.HOME);//group2
			    controller.sendKeyPress(VirtualKeyCode.PRIOR);//group3
			    controller.sendKeyPress(VirtualKeyCode.DELETE_key);//group4
			    controller.sendKeyPress(VirtualKeyCode.END);//group5
			    controller.sendKeyPress(VirtualKeyCode.NEXT);//group6
			    performingAlphaStrike = true;
		    }
    }

    //extra function added for use in "extraCode" specific to MW4 file
    private void evaluateModifiableButton(SteelBattalionController controller,ButtonEnum toggleButton,ButtonEnum mainButton,VirtualKeyCode state1,VirtualKeyCode state2)
    {
	    //deal with RightJoyMainWeapon and firing groups 3/4
	    if(controller.GetButtonState((int)toggleButton))//up position
	    {
    		if((int)controller.GetButtonKey(mainButton) == (int)state1)
			    controller.AddButtonKeyMapping(mainButton,state2,false);//had to rename delete to delete_key because delete is reserved
	    }
	    else
	    {
    		if(controller.GetButtonKey(mainButton) == state2)
			   controller.AddButtonKeyMapping(mainButton,state1,false);
	    }
    }
    //extra function added for use in "extraCode" specific to MW4 file
    void evaluateDualLeftPedal(SteelBattalionController controller,VirtualKeyCode jumpKey,VirtualKeyCode stopKey)
    {
    	if(controller.LeftPedal > pedalTriggerLevel)
    	{
	    	//take care of the button logic separately, to be less confusing
    		if(!jumpPressed)//if not currently holding down jump key
		    {
			    if(controller.RightPedal > pedalTriggerLevel || controller.MiddlePedal > pedalTriggerLevel)
			    {
    				controller.sendKeyDown(jumpKey);
				    jumpPressed = true;
			    }
		    }
		    else//jump button was pressed
		    {
                //adding these so that else if won't get optimized into one statement
    			if(controller.RightPedal < pedalTriggerLevel && controller.MiddlePedal < pedalTriggerLevel)
	    		{
				    controller.sendKeyUp(jumpKey);
				    jumpPressed = false;
			    }
		    }   

		    if(!stopPressed)//if not currently holding down stop key
		    {
			    if(controller.RightPedal < pedalTriggerLevel && controller.MiddlePedal < pedalTriggerLevel)
			    {
				    controller.sendKeyDown(stopKey);//send fullstop command
				    stopPressed = true;
			    }
		    }
		    else//stop button was pressed
		    {
			    if(controller.RightPedal > pedalTriggerLevel || controller.MiddlePedal > pedalTriggerLevel)
			    {
				    controller.sendKeyUp(stopKey);
				    stopPressed = false;
			    }
		    }
	    }
	    else
	    {
		    if(stopPressed)
		    {
			    controller.sendKeyUp(stopKey);
			    stopPressed = false;
		    }
		    if(jumpPressed)
		    {
			    controller.sendKeyUp(jumpKey);
			    jumpPressed = false;
		    }
	    }
    }

}
}