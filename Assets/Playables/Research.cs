using System.ComponentModel.Design;
using System.Reflection;
using System;
using System.Collections.Generic;
using Kara.Entities;
using UnityEngine;

namespace Kara.Playables
{
    public class Research{
		public List<GameObject> availableBuild;
		
		public Research(){
			availableBuild= new List<GameObject>();
			availableBuild.Add(Capital._Capital_Prototype._getObj_Prtp()); 
		}
	
    }
	public interface _Prototype{
		GameObject getObj_Prtp();
		void _Capital_Preview(Vector3 pos);
	}
}
