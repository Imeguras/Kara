using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kara.Entities{
	public abstract class Building_Abstract: Board_Entities{
		protected int uuid_owner;
		protected uint morale_receive;
		protected uint morale_transmit;
		protected Texture2D building_Icon;
		public Building_Abstract(string name, uint baseHP, uint morale_receive, uint morale_transmit):base(name, baseHP){
			this.morale_receive =morale_receive;
			this.morale_transmit=morale_transmit;
		}
		
	}
	
}