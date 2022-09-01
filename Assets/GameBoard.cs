using System.Collections;
using System.Collections.Generic;
using Kara.Playables; 
using Kara.Entities;
using Kara.ProceduralGen;
using UnityEngine;
using UnityEngine.Rendering;
using Kara_.Assets.Settings.General_Settings;
using Kara_.Assets.Settings.Player_Settings;
public class GameBoard : MonoBehaviour{
	public readonly static string gme_terrain_tag="Earth";
	public readonly static string gme_ocean_tag="Ocean";

	protected GameSettings gameSettings;
	protected ControlSettings controlSettings;
    private GameObject gme_terrain;
	private GameObject gme_ocean; 
	private List<Player> players;
	public GameObject curScreenCamera;
	public GameObject sun;  
    void Start(){
		gameSettings=GameSettings.CreateInstance<GameSettings>();
		gameSettings.Init(); 
		controlSettings=ControlSettings.CreateInstance<ControlSettings>();
		
		GraphicsSettings.lightsUseLinearIntensity=true;
		GraphicsSettings.lightsUseColorTemperature=true;
		gme_ocean= new GameObject("MainOcean");
		gme_ocean.tag=gme_ocean_tag; 
		gme_ocean.AddComponent<OceanGeneral>();
		gme_ocean.AddComponent<MeshFilter>();
		gme_ocean.AddComponent<MeshRenderer>();

		gme_terrain = new GameObject("MainTerrain"); 
		gme_terrain.tag=gme_terrain_tag;
		gme_terrain.AddComponent<Terrain>();
		gme_terrain.AddComponent<TerrainCollider>();
		gme_terrain.AddComponent<TerrainGeneral>();
		
		
		

		sun= new GameObject("Sun");
		sun.AddComponent<DayNightCycle>();

		players= new List<Player>();
		Player me = new Player("Imeguras", new Color32(0,0,255,255), controlSettings);
		me.getCurCamera(ref curScreenCamera); 
		var resource_tab =curScreenCamera.AddComponent<Resources_Tab>();
		resource_tab.setTrackingPlayer(me);
		curScreenCamera.transform.position=new Vector3 (100, 50, 100); 
		curScreenCamera.transform.Rotate(new Vector3(65, 0,0));
		//curScreenCamera.GetComponent<GameEyes>().terrainTrack=td;

		players.Add(me); 
		
	
		
    }

    // Update is called once per frame
    void Update(){
        
    }
}
