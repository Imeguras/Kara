using System;

namespace Kara.Entities{
    public class Unit : Board_Entities{
        private float speed;
		private float baseSpeed;
		public Unit(uint baseHP, float baseSpeed): base(baseHP){
			this.baseSpeed=baseSpeed; 
			this.speed=baseSpeed;
		}
		
    }
}
