using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kara.Entities{
	public abstract class Building_Abstract: Board_Entities{
		private int uuid_owner;
		private uint morale_receive;
		private uint morale_transmit;
		public Building_Abstract(uint baseHP, uint morale_receive, uint morale_transmit):base(baseHP){
			
			this.morale_receive =morale_receive;
			this.morale_transmit=morale_transmit;
		}
		
	}
}