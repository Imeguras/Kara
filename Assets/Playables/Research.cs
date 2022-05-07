using System.ComponentModel.Design;
using System.Reflection;
using System;
using System.Collections.Generic;
using Kara.Entities;
using UnityEngine;

namespace Kara.Playables
{
    public class Research{
		
		public List<_Prototype<Building_Abstract>> availableBuild;
		
		public Research(){
			availableBuild= new List<_Prototype<Building_Abstract>>();
			
			availableBuild.Add(new Capital._Capital_Prototype(false));
			//availableBuild.Add(Capital._Capital_Prototype); 
		}
	
    }
	public interface _Prototype<out R>{
		GameObject getObj_Prtp();
		void obj_Preview(Vector3 pos);
		Sprite getIcn();
	}
}
