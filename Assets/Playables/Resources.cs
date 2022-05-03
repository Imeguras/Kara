using System.Reflection;
using System;

namespace Kara.Playables
{
    public class Resources{
        public uint food{get; set;}
		public uint stone{get; set;}
		public uint wood{get; set;}
		public uint gold{get; set;}
		public uint iron{get; set;}

		public Resources(uint food=0, uint stone=0, uint wood=0, uint gold=0, uint iron=0){
			this.food=food; 
			this.stone=stone; 
			this.wood=wood; 
			this.gold=gold; 
			this.iron=iron; 
		}

    }
}
