using System;

namespace Kara.Entities{
    public class Unit : Board_Entities{
        protected float speed;
		protected float baseSpeed;
		protected string name;
		public Unit(string name, uint baseHP, float baseSpeed): base(name, baseHP){
			this.baseSpeed=baseSpeed; 
			this.speed=baseSpeed;
		}
		
    }
}
