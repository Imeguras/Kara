using System.Collections.Generic;
using System.Dynamic;
using System;
using UnityEngine;

namespace Kara.Playables{
	
    public class Team{
		public static readonly Team defaultTeam=new Team(false); 
		private Color32 teamColor;
		private string team_name;
        private List<Player> Allies; 
		public Team(bool selfKill){
			Allies=new List<Player>();
			
		}
		public Team(bool selfKill, Player t) : this(selfKill){
			if(selfKill){
				Allies.Add(t); 
			} 
		}
		public Team(bool selfKill, Player first, IEnumerable<Player> allies) : this(selfKill, first){
			Allies.AddRange(allies); 
		}
		
    }
}
