using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fyrenest
{
    /// <summary>
    /// Handles the loading of prefabs defined in the Prefabs class
    /// </summary>
    internal class PrefabManager
    {
        /// <summary>
        /// Returns a list of all prefabs to be loaded for the "Prefabs" class
        /// </summary>
        public List<(string, string)> GetPreloadNames()
        {
            List<(string, string)> preloadNames = new List<(string, string)> ();

            foreach (FieldInfo prop in typeof(Prefabs).GetFields())
            {
                if (prop.IsStatic && prop.FieldType == typeof(Prefab))
                {
                    Prefab prefab = (Prefab)prop.GetValue(null);
                    preloadNames.Add((prefab.OriginRoom, prefab.OriginName));
                }
            }
            return preloadNames;
        }
        /*
        public override List<(string, string)> GetPreloadNames() => new()
        {
            ("GG_Mantis_Lords", "Shot Mantis Lord")
        };
         */

        /// <summary>
        /// Assigns the loaded prefabs to their Prefab objects
        /// </summary>
        public void InitializePrefabs(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Fyrenest.instance.Log("Loaded Prefabs:" + preloadedObjects.Count);
            foreach (FieldInfo prop in typeof(Prefabs).GetFields())
            {
                if (prop.IsStatic && prop.FieldType == typeof(Prefab))
                {
                    Prefab prefab = (Prefab)prop.GetValue(null);
                    prefab.Object = preloadedObjects[prefab.OriginRoom][prefab.OriginName];
                }
            }
        }
    }
}
