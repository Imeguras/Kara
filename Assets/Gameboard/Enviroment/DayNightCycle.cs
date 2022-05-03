
using System;
using UnityEngine;
using UnityEditor; 

namespace Kara.ProceduralGen
{
	
	/*[CustomEditor(typeof(DayNightCycle))]
	public class DayNightCycleEdit : Editor
	{
		private DayNightCycle script;

		private void OnEnable()
		{
			// Method 1
			script = (DayNightCycle) target;

			// Method 2
			script = target as DayNightCycle;
		}
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			//GUILayout.TextField(script.currentCycle.clock+"");
			script.currentCycle.refreshVars();
			GUILayout.TextField(script.currentCycle.ToString());
		}
		
	}*/
    public class DayNightCycle: MonoBehaviour{
        
		Light sun;
		public readonly int ticksCycle=864000;
		public ign_time currentCycle; 
		void Start(){
			currentCycle=new ign_time(0);
			sun=this.gameObject.AddComponent<Light>();
			this.gameObject.transform.position=new Vector3(0,50,0); 
			
			sun.type= LightType.Directional; 
			sun.colorTemperature=6500;
			sun.useColorTemperature=true; 

		}
		void FixedUpdate(){
			currentCycle.clock+=1; 
			this.gameObject.transform.RotateAround(Vector3.zero, Vector3.right, (float)(2*Math.PI*currentCycle.clock/ticksCycle));
			
			sun.colorTemperature=3250*((float)Math.Cos(2*Math.PI*currentCycle.clock/ticksCycle)+1);
		}
    }
	public class ign_time: System.Object{
		public ulong clock{get; set;}
		protected int Days{get;set;}
		protected int Hours{get;set;}
		protected int Minutes{get;set;}
		protected int Seconds{get;set;}
		//protected int milliSeconds{get;set;} 		
		
		public ign_time(ulong clock){
			this.clock=clock;
		}
		public void refreshVars(){
			this.Days=(int)clock/864000;
			this.Hours=(int)(clock%864000)/36000;
			this.Minutes=(int)(clock%864000%36000/600);
			this.Seconds=(int)(clock%864000%36000%600/10);
		}
		public override string ToString(){
			return "Day: "+this.Days+" - "+ this.Hours+":"+this.Minutes+":"+this.Seconds;
		}
		
		
	}
}
