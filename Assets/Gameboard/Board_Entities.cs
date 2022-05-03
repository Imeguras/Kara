using System;
using UnityEngine;
namespace Kara.Entities
{
    public class Board_Entities {
        private uint baseHP;
		private int curHP;
		public Board_Entities(uint baseHP){
			this.baseHP=baseHP;

			this.curHP=(int)baseHP;

		}
	}
}
