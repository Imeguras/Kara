using System.Collections;
using System.Collections.Generic;
using Kara.Playables; 
using Kara.Entities;
using Kara.ProceduralGen;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror; 
using Kara_.Assets.Settings.General_Settings;
using Kara_.Assets.Settings.Player_Settings;

public class GameBoard : NetworkBehaviour {
	public readonly static string gme_terrain_tag="Earth";
	public readonly static string gme_ocean_tag="Ocean";

    private GameObject gme_terrain;
	private GameObject gme_ocean; 
	protected NetworkIdentity id_terrain;
	protected NetworkIdentity id_ocean; 
	protected NetworkIdentity id_sun; 
	public GameObject server_builder{get; private set;}
	public GameObject sun;
	
    void Start(){
		GraphicsSettings.lightsUseLinearIntensity=true;
		GraphicsSettings.lightsUseColorTemperature=true;
			
			gme_ocean= new GameObject("MainOcean");
			//put gme_ocean in 200 y position
			gme_ocean.transform.position=new Vector3(0,200,0);
			gme_ocean.tag=gme_ocean_tag; 
			gme_ocean.AddComponent<OceanGeneral>();
			gme_ocean.AddComponent<MeshFilter>();
			gme_ocean.AddComponent<MeshRenderer>();
			id_ocean=gme_ocean.AddComponent<NetworkIdentity>(); 
			
			gme_terrain = new GameObject("MainTerrain"); 
			gme_terrain.tag=gme_terrain_tag;
			gme_terrain.AddComponent<Terrain>();
			gme_terrain.AddComponent<TerrainCollider>();
			gme_terrain.AddComponent<TerrainGeneral>();
			id_terrain=gme_terrain.AddComponent<NetworkIdentity>();

			sun= new GameObject("Sun");
			sun.AddComponent<DayNightCycle>();
			id_sun=sun.AddComponent<NetworkIdentity>();
		
		server_builder= GameObject.Find("ServerBuilder");
    }
	
	public void setVisibilityTo(NetworkConnectionToClient obj, bool visible=true){
		if(visible){
			NetworkServer.Spawn(gme_ocean);
			NetworkServer.Spawn(gme_terrain);
			NetworkServer.Spawn(sun);
			
			//id_ocean.observers.Add(obj.connectionId, obj);
			//id_terrain.observers.Add(obj.connectionId, obj);
			//id_sun.observers.Add(obj.connectionId, obj);
			//NetworkServer.RebuildObservers(obj.identity, true);
		}else{
			//id_ocean.observers.Remove(obj.connectionId);
			//id_terrain.observers.Remove(obj.connectionId);
			//id_sun.observers.Remove(obj.connectionId);
			//NetworkServer.RebuildObservers(obj.identity, false);
		}
		
	}
 
	
}
