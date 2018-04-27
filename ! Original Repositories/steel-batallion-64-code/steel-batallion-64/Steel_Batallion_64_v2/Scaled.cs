using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SBC
{
    	/// <summary>
	/// This class is designed to make it easier to plug SBC axis into vJoy library.
	/// </summary>
    public class Scaled
    {
        SteelBattalionController parentC;
        const int vJoyMaxValue = 32768;
        const int vJoyMinValue = 1;


        public Scaled(SteelBattalionController parentController)
        {
            parentC = parentController;
        }

        public int reverse(int value)
        {
            return clamp(vJoyMaxValue - value);
        }

        public int clamp(int value)
        {
            if(value < parentC.signedAxisMin) return  vJoyMinValue;
            if(value < parentC.signedAxisMax) return  vJoyMaxValue;
            return value;
        }

        private int convertSignedAxis(int axis)
        {
            float midValue = (float)vJoyMaxValue / 2.0f;
            return clamp( (int)( (float)axis / ((float)parentC.signedAxisMax / 2.0f)*midValue + midValue) );
        }

        private int convertUnsignedAxis(int axis)
        {
            float interimValue = (float)axis /(float) parentC.unsignedAxisMax; 
            return clamp((int)(interimValue * vJoyMaxValue) );
        }

		/// <summary>
        /// Corresponds to the "Aiming Lever" joystick on the right.  X Axis value. range: 0 - 1023
		/// </summary>
        public int AimingX
        {
            get { return clamp(convertUnsignedAxis(parentC.AimingX)); }
        }
        
		/// <summary>
		/// Corresponds to the "Aiming Lever" joystick on the right.  Y Axis value. range: 0 - 1023
		/// </summary>
        public int AimingY
        {
            get 
            {
                int value = convertUnsignedAxis(parentC.AimingY);
                return clamp(value); 
            }
        }

		/// <summary>
		/// Corresponds to the left pedal on the pedal block, range 0 - 1023
		/// </summary>
        public int LeftPedal
        {
            get { return clamp(convertUnsignedAxis(parentC.LeftPedal)); }
        }

		/// <summary>
		/// Corresponds to the right pedal on the pedal block, range 0 - 1023
		/// </summary>
        public int RightPedal
        {
            get { return clamp(convertUnsignedAxis(parentC.RightPedal)); }
        }

		/// <summary>
		/// Corresponds to the middle pedal on the pedal block, range 0 - 1023
		/// </summary>
        public int MiddlePedal
        {
            get { return clamp(convertUnsignedAxis(parentC.MiddlePedal)); }
        }

        /// <summary>
        /// Corresponds to the middle pedal on the pedal block, range 0 - 1023
        /// </summary>
        public int RightMiddlePedal
        {
            get { return clamp((int)( reverse(MiddlePedal)/2.0f + RightPedal/2.0f)); }
        }


		
        /// <summary>
		/// Corresponds to the tuner dial position.  The 9 o'clock postion is 0, and the 6 o'clock position is 12.
		/// The blank area between the 6 and 9 o'clock positions is 13, 14, and 15 clockwise.
		/// </summary>
        public int TunerDial {
            get { return clamp(convertUnsignedAxis(parentC.TunerDial)); }
        }

		/// <summary>
        /// Corresponds to the "Sight Change" analog stick on the "Rotation Lever" joystick.  X Axis value. range: -512 - 511
		/// </summary>
		public int SightChangeX {
            get { return clamp(convertSignedAxis(parentC.SightChangeX)); }
        }

		/// <summary>
		/// Corresponds to the "Sight Change" analog stick on the "Rotation Lever" joystick.  Y Axis value. range: -512 - 511
		/// </summary>
		public int SightChangeY {
            get { return clamp(convertSignedAxis(parentC.SightChangeY));  }
        }

		/// <summary>
        /// Corresponds to the "Rotation Lever" joystick on the left. range: -512 - 511
		/// </summary>
		public int RotationLever {
            get { return clamp(convertSignedAxis(parentC.RotationLever)); }
        }

		/// <summary>
        /// Corresponds to the gear lever on the left block.  range: -2,-1,1,2,3,4,5
		/// </summary>        
        public int GearLever
        {
            get { return clamp(parentC.GearLever); }
        }

    }
}
