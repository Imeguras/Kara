using System;

namespace InstanceManager.Utility
{

    /// <summary>A utility for generating ids.</summary>
    static class IDUtility
    {

        /// <summary>Generates an id.</summary>
        /// <param name="validate">Callback to validate id, return false to keep generating id.</param>
        public static string Generate(Func<string, bool> validate = null)
        {

            string id = null;
            while (id is null || !(validate?.Invoke(id) ?? true))
            {
                var ticks = new DateTime(2016, 1, 1).Ticks;
                var ans = DateTime.Now.Ticks - ticks;
                id = ans.ToString("x");
            }

            return id;

        }

    }

}
