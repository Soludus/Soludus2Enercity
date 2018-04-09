using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Handles the reference to the active <see cref="MapConfiguration"/>.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Tooltip("Handle to the current map. The same handle should be referenced everywhere.")]
        public MapHandle handle = null;
        [Tooltip("The handle will point to this configuration after the scene has been loaded.")]
        public MapConfiguration loadedConfiguration = null;

        private void Awake()
        {
            if (loadedConfiguration != null)
                handle.mapConfiguration = loadedConfiguration;
            MapHandle.current = handle;
        }

        private void OnDestroy()
        {
            handle.mapConfiguration = null;
        }
    }

}