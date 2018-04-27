/*********************************************************************************
 *   SteelBattalion.NET - A library to access the Steel Battalion XBox           *
 *   controller.  Written by Joseph Coutcher.                                    *
 *                                                                               *
 *   This file is part of SteelBattalion.NET                                     *
 *                                                                               *
 *   SteelBattalion.NET is free software: you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by        *
 *   the Free Software Foundation, either version 3 of the License, or           *
 *   (at your option) any later version.                                         *
 *                                                                               *
 *   SteelBattalion.NET is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *   GNU General Public License for more details.                                *
 *                                                                               *
 *   You should have received a copy of the GNU General Public License           *
 *   along with SteelBattalion.NET.  If not, see <http://www.gnu.org/licenses/>. *
 *                                                                               *
 *   While this code is licensed under the LGPL, please let me know whether      *
 *   you're using it.  I'm always interested in hearing about new projects,      *
 *   especially ones that I was able to make a contribution to...in the form of  *
 *   this library.                                                               *
 *                                                                               *
 *   EMail: geekteligence at google mail                                         *
 *                                                                               *
 *   2010-11-26: JC - Initial commit                                             *
 *                                                                               * 
 *********************************************************************************/

using System;
using System.Collections.Generic;

namespace SBC{
    //DO NOT CHANGE ORDER
    //THESE match the order they are presented in the USB packet
	public enum ButtonEnum {
		RightJoyMainWeapon,
		RightJoyFire,
		RightJoyLockOn,
        Eject,
		CockpitHatch,
		Ignition,
		Start,
		MultiMonOpenClose,
		MultiMonMapZoomInOut,
		MultiMonModeSelect,
		MultiMonSubMonitor,
		MainMonZoomIn,
		MainMonZoomOut,
        FunctionFSS,
        FunctionManipulator,
        FunctionLineColorChange,
		Washing,
		Extinguisher,
		Chaff,
        FunctionTankDetach,
        FunctionOverride,
        FunctionNightScope,
        FunctionF1,
        FunctionF2,
        FunctionF3,
		WeaponConMain,
		WeaponConSub,
		WeaponConMagazine,
		Comm1,
		Comm2,
		Comm3,
		Comm4,
		Comm5,
		LeftJoySightChange,
		ToggleFilterControl,
		ToggleOxygenSupply,
		ToggleFuelFlowRate,
		ToggleBufferMaterial,
		ToggleVTLocation,
		TunerDialStateChange,
		GearLeverStateChange
	}
	
	public class ButtonMasks {
		public static ButtonMask[] MaskList = new ButtonMask[50];
		public struct ButtonMask {
			public int bytePos;
			public int maskValue;
			
			public ButtonMask(int bytePos, int maskValue) {
				this.bytePos = bytePos;
				this.maskValue = maskValue;
			}
		}
		
		public static void InitializeMasks() {
            //these use to be listed out, but I changed the order
            //of the enumeration so that the enumeration matches the 
            //way the buttons are actually mapped in the USB packet.
            for (int i = 0; i < 39; i++)
            {
                int offset = 2+(int)((i)/8);
                MaskList[i] = new ButtonMask(offset, 1<<(i%8));
            }

			MaskList[(int) ButtonEnum.TunerDialStateChange]      = new ButtonMask(24, 0x0F);
			MaskList[(int) ButtonEnum.GearLeverStateChange]      = new ButtonMask(25, 0xFF);
		}
	}
	
	public class ButtonState {
		public ButtonEnum button;
		public bool currentState;
		public bool changed;
	}
}
