using System;

namespace Kara.MathS{
	public sealed class KaraMath{
		public static KaraMath INSTANCE=new KaraMath();
		public readonly double gravityForce=9.8f; 
		//Avoid POO shenanigans 
		private KaraMath(){}
		#region Lerp
		    //No generics could do something to shorten the code but this will save pain for the future
			public double lerp(double a0, double a1, double w) {
				return (1.0f - w)*a0 + w*a1;
			}
			public float lerp(float a0, float a1, float w) {
				return (1.0f - w)*a0 + w*a1;
			}
			public int lerp(int a0, int a1, int w) {
				return (int)((1.0f - w)*a0 + w*a1);
			}
		#endregion
		#region Derivatives
			public double DerivativeAt(Func<double, double> function, double x, double h = 0.0) {
				
				if (null == function){
					throw new ArgumentNullException("function");
				}
				// If user don't want to provide h, let's compute it 
				if (0 == h){ 
					h = Math.Abs(x) < 1e-10 ? 1e-16 : x / 1.0e6; // Easiest, but not the best 
				}
				// "Central" derivative is often a better choice then right one ((f(x + h) - f(x))/h)
				return (function(x + h) - function(x - h)) / (2.0 * h);
			}
			
		#endregion

	}
}