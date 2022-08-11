using System;
using UnityEngine;
namespace Kara_.Assets.Settings.General_Settings
{
    public class GameSettings : ScriptableObject{
		//True represents production, false represents Development
        private static Boolean environmentMode;
		//TODO: Production mode should be defined with a flag parameter
		public void Init(){
			environmentMode=false; 
		}
		public static Boolean getEnvironmentMode(){
			return environmentMode;
		}

    }
}
