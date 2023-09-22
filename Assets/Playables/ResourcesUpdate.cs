using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Security;
using Unity.VisualScripting;

using UnityEngine; 
using UnityEngine.UIElements; 
//using UnityEditor;
//using UnityEditor.UIElements;

namespace Kara.Playables{
	
	public interface IProperty<T> : IProperty{
    	new event Action<T> ValueChanged;
    	new T Value { get; }
	}

	public interface IProperty{
    	event Action<object> ValueChanged;
    	object Value { get; }
	}

	[Serializable]
	public class Property<T> : IProperty<T>{
    	public event Action<T> ValueChanged;
		
		event Action<object> IProperty.ValueChanged{
			add => valueChanged += value;
			remove => valueChanged -= value;
		}
    
		[SerializeField]
		private T value;
		
		public T Value{
			get => value;
			
			set{
				if(EqualityComparer<T>.Default.Equals(this.value, value)){
					return;
				}
				
				this.value = value;
				
				ValueChanged?.Invoke(value);
				valueChanged?.Invoke(value);
			}
		}

		object IProperty.Value => value;

		private Action<object> valueChanged;

		public Property(T value) => this.value = value;

    	public static explicit operator Property<T>(T value) => new Property<T>(value);
    	public static implicit operator T(Property<T> binding) => binding.value;
	}
	
	public static class RuntimeBindingExtensions{
		public static Action<object> BindProperty(this TextElement element, IProperty property){
			//keep the reference to the delegate so we can unbind it later
			Action<object> refEvent = OnPropertyValueChanged;
			//Sync up once to make sure the UI is in sync with the property
			OnPropertyValueChanged(property.Value); 
			property.ValueChanged += refEvent;
			
			void OnPropertyValueChanged(object newValue){
				element.text = newValue?.ToString() ?? "0?";
			}
			//return the reference to the delegate
			return refEvent;
		}
		
		public static void Unbind(this TextElement element, Action<object> refEvent, IProperty property){
			property.ValueChanged -= refEvent;
		}

		
	}
}