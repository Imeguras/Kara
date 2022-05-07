using System;
using UnityEngine;
namespace Kara.Entities
{
    public class Board_Entities {
        protected uint baseHP;
		protected int curHP;
		protected string name; 
		public Board_Entities(string name, uint baseHP){
			this.name= name; 
			this.baseHP=baseHP;

			this.curHP=(int)baseHP;

		}
	}
}
