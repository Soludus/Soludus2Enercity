using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// A proxy object for referencing the currently active <see cref="MapConfiguration"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Internal/MapHandle")]
    public class MapHandle : ScriptableObject
    {
        public static MapHandle current = null;

        internal MapConfiguration mapConfiguration = null;
    }

}