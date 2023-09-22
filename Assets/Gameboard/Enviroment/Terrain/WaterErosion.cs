using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine; 
using TestSimpleRNG;
using Kara.MathS;
using UnityEngine.InputSystem;

namespace Kara.ProceduralGen{
    
	public class _WaterErosion{
		class heightsDescend : IComparer<Tuple<double, Tuple<int,int>>>{
			public int Compare(Tuple<double, Tuple<int,int>> x,Tuple<double, Tuple<int,int>> y){
				return  Math.Abs(y.Item1).CompareTo(Math.Abs(x.Item1));
			}
		}
		private const double hardness=0.5d;
		private uint iter;
		private double precipitation;
		private double precipitation_sd; 
		private double rainInitInercia; 
		private double minSteep;
		private uint peakSize;
		private List<Tuple<double, Tuple<int,int>>> highestPeaks; 
		protected double[,] heightMap;
		private int maxX; 
		private int maxY;
		private float seaHeight; 
		public _WaterErosion(uint iter, double[,] heightMap, double precipitation=0.6f, double precipitation_sd=0.1f, double minSteep=0.09f, double rainInitInercia=0.8f, float seaHeight=0.5f, uint peakSize=2){
			this.iter=iter;
			this.heightMap=heightMap;
			this.precipitation=precipitation;
			this.precipitation_sd=precipitation_sd;
			this.rainInitInercia=rainInitInercia;
			this.minSteep=minSteep;
			this.peakSize=peakSize;
			this.seaHeight=seaHeight; 
			
			highestPeaks=new List<Tuple<double, Tuple<int, int>>>();
			maxX=heightMap.GetLength(1)-1;
			maxY=heightMap.GetLength(0)-1;
		}
		public void findHighest(){
			int fill=0; 
			for (int y = 0; y <= maxY; y++){
				for (int x = 0; x <= maxX; x++){
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
		/// <summary>
		/// Returns a tuple containing the "partial" derivatives of X and Z axis(on Unity3D) on point X, Y, using lacunarity to interpolate
		/// </summary>
		/// <param name="x">X coordinates of the point</param>
		/// <param name="y">Z coordintes of the point in Unity3D coordinate system, y coordinates when taking heightmap into consideration</param>
		/// <param name="lacunarity">How much to interpolate away from the point, this "shouldn't" be 0 </param>
		/// <returns></returns>
		public Tuple<double,double> s_slope(int x, int y, double lacunarity){
			double dx=0;
			double dy=0;

			double returnPartialX(double x_){
				double orig= heightMap[y,(int)x_];
				double limDir= heightMap[y,(int)Math.Max(x_+1, maxX)];
				return KaraMath.INSTANCE.lerp(orig, limDir, x_-(int)x_);  
			}

			double returnPartialY(double y_){
				double orig= heightMap[(int)y_,x];
				double limDir= heightMap[(int)Math.Max(y_+1, maxY),x];
				return KaraMath.INSTANCE.lerp(orig, limDir, y_-(int)y_);  
			}

			dx=KaraMath.INSTANCE.DerivativeAt( (x) => returnPartialX(x), x, lacunarity);
			dy=KaraMath.INSTANCE.DerivativeAt( (y) => returnPartialY(y), y, lacunarity);

			return new Tuple<double,double>(dy,dx);
		}
		//Decided to be required to erode... If its fucked too bad...
		public Task<int> erode(ref double[,] heightMap){
			findHighest();
			int err=0; 
			for (int i = 0; i < iter; i++){
				//88mm is the average amount of rain in a month globally
				double amount=SimpleRNG.GetNormal(9*precipitation, precipitation_sd);
				
				for(int f=0; f<highestPeaks.Count; f++){
					err+=runCourse(highestPeaks[f].Item2, ref heightMap, ref amount);
				}						
			}
			return Task.FromResult<int>(err); 
		}
		public int runCourse( Tuple<int, int> point, ref double[,] heightsMap, ref double amount, int err=0){
			
			int x = point.Item2;
			int y= point.Item1;
			Tuple<double,double> slope=s_slope(x, y, rainInitInercia);
			int lowerPointCoordsY=Math.Clamp((int)Math.Ceiling(y+slope.Item1), 0, maxY);
			int lowerPointCoordsX=Math.Clamp((int)Math.Ceiling(x+slope.Item2), 0, maxX);
			Tuple<int,int> lowerPointCoords = new Tuple<int, int>(lowerPointCoordsY, lowerPointCoordsX);
			
			double lowerPointHeight= heightsMap[lowerPointCoords.Item2, lowerPointCoords.Item1];
			double maxDebryRemovable=0; 
			try{
				maxDebryRemovable= (heightsMap[y,x]-lowerPointHeight)/2; 
			}catch(Exception e){
				Debug.LogWarning(e);
				err+=1; 
			}
			//Its not the hypothenuse as it removes the sign of the slope
			double combinedGradient=(slope.Item1*slope.Item2)/2;
			//double hypotenuse=Math.Sqrt(slope.Item1*slope.Item1+slope.Item2*slope.Item2);
			//Tupple that has the signed identity of the slope
			//Tuple<int,int> slopeSign = new Tuple<int, int>(Math.Sign(slope.Item1), Math.Sign(slope.Item2));
			Debug.Log("Angle is: "+KaraMath.INSTANCE.gravityForce*Math.Atan(combinedGradient));
			double toPlace=Math.Min(
				amount*Math.Cos(
					KaraMath.INSTANCE.gravityForce*Math.Atan(combinedGradient)
				), maxDebryRemovable);
			//Main Place
			heightsMap[lowerPointCoords.Item2, lowerPointCoords.Item1]+=toPlace*combinedGradient;
			Debug.Log("Placed: "+toPlace*combinedGradient+"on: "+lowerPointCoords.Item1+", "+lowerPointCoords.Item2);
			//Sides
			if(Math.Abs(slope.Item1)>Math.Abs(slope.Item2)){
				heightsMap[lowerPointCoords.Item1, Math.Clamp(lowerPointCoords.Item2+1, 0, maxX)]+=toPlace*(1-combinedGradient)/2;
				heightsMap[lowerPointCoords.Item1, Math.Clamp(lowerPointCoords.Item2-1, 0, maxX)]+=toPlace*(1-combinedGradient)/2;
			}else if(Math.Abs(slope.Item1)<Math.Abs(slope.Item2)){
				heightsMap[Math.Clamp(lowerPointCoords.Item1+1, 0, maxY), lowerPointCoords.Item2]+=toPlace*(1-combinedGradient)/2;
				heightsMap[Math.Clamp(lowerPointCoords.Item1-1, 0, maxY), lowerPointCoords.Item2]+=toPlace*(1-combinedGradient)/2;
			}else{
				//deposit the rest
				heightsMap[lowerPointCoords.Item2, lowerPointCoords.Item1]+=toPlace*(1-combinedGradient);
			}
			heightsMap[y,x]-=toPlace;
			//heightsMap[Math.Clamp((int)Math.Ceiling(yslope.Item1), 0, maxY), Math.Clamp((int)Math.Ceiling(x+slope.Item2), 0, maxX)]+=toPlace*(1-hypotenuse)/2;

			//double toPlace=Math.Min(amount/ Math.Sqrt(Math.Pow(slope.Item1/hardness,2)+Math.Pow(slope.Item2/hardness,2)), maxDebryRemovable); 
			/*for(int y_=1; y_<Math.Abs(slope.Item1*amount); y_++){
				for(int x_=1; x_<Math.Abs(slope.Item2*amount); x_++){
					double toPlace=Math.Min(amount/ Math.Sqrt(Math.Pow(slope.Item1*(y_),2)+Math.Pow(slope.Item2*(x_),2)), maxDebryRemovable);
					heightsMap[Math.Clamp(y+y_*slopeSign.Item1, 0, maxY),Math.Clamp(x+x_*slopeSign.Item1, 0, maxX)]+=toPlace;
					heightsMap[y,x]-=toPlace;
					maxDebryRemovable-=toPlace;
				}
			}*/
			return err; 
		}
		
		public double[,] getInternalMap(){
			return heightMap; 
		}
	}
}

