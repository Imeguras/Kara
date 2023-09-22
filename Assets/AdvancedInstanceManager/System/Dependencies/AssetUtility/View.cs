using UnityEngine;

namespace AssetUtility
{

    abstract class View
    {

        /// <summary>Loads view from player prefs.</summary>
        public void Load()
        {
            if (PlayerPrefs.GetString("AssetUtility." + GetType().Name, null) is string json)
                JsonUtility.FromJsonOverwrite(json, this);
        }

        /// <summary>Saves view to player prefs.</summary>
        public void Save() =>
            PlayerPrefs.SetString("AssetUtility." + GetType().Name, JsonUtility.ToJson(this));

        public abstract string header { get; }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnFocus() { }
        public virtual void OnLostFocus() { }
        public virtual void OnGUI() { }

    }

}