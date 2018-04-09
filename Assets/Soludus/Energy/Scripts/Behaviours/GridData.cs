using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Contains the data for a grid.
    /// </summary>
    [SelectionBase]
    public class GridData : MonoBehaviour
    {
        /// <summary>
        /// Width of the grid.
        /// </summary>
        public int width = 1;
        /// <summary>
        /// Height of the grid.
        /// </summary>
        public int height = 1;

        /// <summary>
        /// Size of a single unit.
        /// </summary>
        public Vector3 unitSize = new Vector3(1.0f, 1.0f, 1.0f);

        private GameObject[] elementObjects = null;

        /// <summary>
        /// Size of the box encapsulating the grid.
        /// </summary>
        public Vector3 size
        {
            get
            {
                return new Vector3(width * unitSize.x, unitSize.y, height * unitSize.z);
            }
        }

        /// <summary>
        /// Area of the grid. width * height.
        /// </summary>
        public float area
        {
            get
            {
                return width * height * unitSize.x * unitSize.z;
            }
        }

        /// <summary>
        /// Get the object repesenting this grid position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GameObject GetElementObject(int x, int y)
        {
            return elementObjects[y * width + x];
        }

        /// <summary>
        /// Set the object repesenting this grid position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="go"></param>
        public void SetElementObject(int x, int y, GameObject go)
        {
            if (elementObjects == null || elementObjects.Length != width * height)
            {
                elementObjects = new GameObject[width * height];
            }
            elementObjects[y * width + x] = go;
        }
    }

}