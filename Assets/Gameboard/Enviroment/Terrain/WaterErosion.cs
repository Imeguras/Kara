using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using UnityEngine; 
using TestSimpleRNG; 

namespace Kara.ProceduralGen{
    
        public class _WaterErosion{
			class heightsDescend : IComparer<Tuple<double, Tuple<int,int>>>
			{
				public int Compare(Tuple<double, Tuple<int,int>> x,Tuple<double, Tuple<int,int>> y)
				{
				return  Math.Abs(y.Item1).CompareTo(Math.Abs(x.Item1));
				}
			}

			private uint iter;
			private double precipitation;
			private double precipitation_sd; 
			private double soilClumps;
			private double minSteep;
			private uint peakSize;
			private List<Tuple<double, Tuple<int,int>>> highestPeaks; 
			protected double[,] heightMap;

			public _WaterErosion(uint iter, double[,] heightMap, double precipitation=0.6f, double precipitation_sd=0.1f, double soilClumps=0.1f, double minSteep=0.09f, uint peakSize=2){
				
				
				this.iter=iter;
				this.heightMap=heightMap;
				this.precipitation=precipitation;
				this.precipitation_sd=precipitation_sd;
				this.soilClumps=soilClumps;
				this.minSteep=minSteep;
				this.peakSize=peakSize;
				highestPeaks=new List<Tuple<double, Tuple<int, int>>>();
			}
			public void findHighest(){
				int fill=0; 
				for (int y = 0; y < heightMap.GetLength(0); y++){
					for (int x = 0; x < heightMap.GetLength(1); x++){
						if(fill<peakSize){
							fill++;
							highestPeaks.Add(new Tuple<double, Tuple<int, int>>(heightMap[y,x],  new Tuple<int, int>(y,x)));
							highestPeaks.Sort(new heightsDescend());
						}else{
							if(highestPeaks.ToArray()[peakSize-1].Item1<heightMap[y,x]){
									highestPeaks.RemoveAt((int)peakSize-1);
									highestPeaks.Add(new Tuple<double, Tuple<int, int>>(heightMap[y,x], new Tuple<int, int>(y,x)));
									highestPeaks.Sort(new heightsDescend());
							}
						}
					}	
				}
				
			}
			//returns the hypothenuse of derivate at point
			public double s_slope(int x, int y, double lacunarity){
				
				double x0=Math.Max(0,x-Math.Sqrt(lacunarity));
				double y0=Math.Max(0,y-Math.Sqrt(lacunarity)); 
				double x1=Math.Min(x+lacunarity*lacunarity,heightMap.GetLength(1)-1); 
				double y1=Math.Min(y+lacunarity*lacunarity, heightMap.GetLength(0)-1);
				double dx=0;
				double dy=0;
				//Debug.Log(x+" d "+x0+" l "+ x1+"f");
				try{
					dx=(heightMap[y,x]- ( (heightMap[y,(int)x0]+heightMap[y,(int)x1]) /2 ) / ( x - ( (x0+x1)/2 ) ) );
					dy=(heightMap[y,x]- ( (heightMap[(int)y0,x]+heightMap[(int)y1,x]) /2 ) / ( y - ( (y0+y1)/2 ) ) );

				}catch(Exception e){
					Debug.Log(" X: "+x+" Y: "+y+"lacunarity");
					//Debug.Log("Stuff:"+heightMap[y,x]+"StuffD:"+(heightMap[y,(int)x0])+"teteta"+heightMap[y,(int)x1]);
				
				}
					

				//Math.Sqrt(
				//Debug.Log(dx);
				return dx.CompareTo(dy)*Math.Sqrt(dx*dx+dy*dy);
				//f(x)-f(c)/x-c

			}
			public Task<double[,]> erode(){
				findHighest();
	
				for (int i = 0; i < iter; i++){
					//88mm is the average amount of rain in a month globally
					double amount=SimpleRNG.GetNormal(9*precipitation, precipitation_sd);
					
					highestPeaks.ForEach(e=>{
						
						int x = e.Item2.Item2;
						int y= e.Item2.Item1;
						
						
						
					});
				}

				return Task.FromResult<double[,]>(heightMap); 
				
			}

			public double[,] getInternalMap(){
				return heightMap; 
			}
			
		}
    
}

