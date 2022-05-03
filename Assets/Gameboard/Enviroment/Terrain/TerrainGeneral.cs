using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEditor;
namespace Kara.ProceduralGen
{

	[CustomEditor(typeof(TerrainGeneral))]
	public class TerrainGeneralEdit : Editor
	{
		private TerrainGeneral script;

		private void OnEnable()
		{
			// Method 1
			script = (TerrainGeneral) target;

			// Method 2
			script = target as TerrainGeneral;
		}
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			if (GUILayout.Button("Draw Terrain"))
			{
				script.LoadTerrain();
				//this.targets..LoadTerrain();
			}
			if (GUILayout.Button("Apply Water	"))
			{
				script.ApplyWater();
			}
		}

	}

    public class TerrainGeneral : MonoBehaviour{
		private _PerlinNoise perNoise; 
		private _WaterErosion errosion;
		private TerrainData td;
		public int res=513;
		
		public uint octaves=20;
		public float lac=0.15f;
		public uint iter=1;
		private double[,] stuff;
		public double precipitation=0.6f;
		public double precipitation_std=0.2f;
		public double soilClumps=0.1f;
		public double minSteep=0.09f;
		
		
		void Start(){
			this.gameObject.transform.position=Vector3.zero;

			td= new TerrainData();
			td.heightmapResolution=res; 
			td.size= new Vector3(500,400,500);
			this.GetComponent<Terrain>().materialTemplate=Resources.Load<Material>("TerrainSpecular");
			List <TerrainLayer> tList = new List <TerrainLayer> ();
			TerrainLayer tl= new TerrainLayer();
			tl.diffuseTexture=Resources.Load<Texture2D>("rock_texture");
			tl.normalMapTexture=Resources.Load<Texture2D>("rock_texture_normal");
			tl.tileSize=new Vector2(20,20);
			tl.metallic=0.3f;
			tl.smoothness=0.4f;
			
			tList.Add(tl);
			td.terrainLayers=tList.ToArray();
			//td.terrainLayers[0]=terLay; 
			LoadTerrain();
			
			GameObject.FindGameObjectWithTag(GameBoard.gme_ocean_tag).SendMessage("OceanGeneral_tileOcean", new _par_OceanGeneral( td.GetHeights(0,0,res,res), (uint)td.size.x,  (uint)td.size.z));
			this.GetComponent<Terrain>().terrainData=td;
			this.GetComponent<TerrainCollider>().terrainData=td;
			//TerrainData td= terrain.GetComponent<Terrain>().terrainData;
			
			
		}
		void Update(){

		}
        public async void LoadTerrain(){
			perNoise= new _PerlinNoise(td.heightmapResolution, td.heightmapResolution,(double)td.size.y,octaves,lac);
			stuff=await perNoise.setPerlinData();
			//await Task.Delay(5000);
			float[,] hmap=new float[td.heightmapResolution,td.heightmapResolution];
			for (int y = 0; y < td.heightmapResolution; y++){
				for (int x = 0; x < td.heightmapResolution; x++){
					hmap[y,x]=(float)stuff[y,x];
				}	
			}
			
			
			td.SetHeights(0,0, hmap);//float[,] hmap= (float[,])stuff.Clone();
			//float[,] hmap=// await perNoise.retHeightsMap(min, max);
			
			
			
		}
		public async void ApplyWater(){
			errosion= new _WaterErosion(iter, stuff,precipitation,precipitation_std,soilClumps,minSteep);
			stuff=await errosion.erode();
			float[,] hmap=new float[td.heightmapResolution,td.heightmapResolution];
			for (int y = 0; y < td.heightmapResolution; y++){
				for (int x = 0; x < td.heightmapResolution; x++){
					hmap[y,x]=(float)stuff[y,x];
				}	
			}
			td.SetHeights(0,0, hmap);
		}
    }
}
