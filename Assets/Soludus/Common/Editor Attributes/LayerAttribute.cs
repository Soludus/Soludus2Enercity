using System;
using UnityEngine;

namespace Soludus
{

    /// <summary>
    /// Allows ints to be shown in the inspector as a layer dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class LayerAttribute : PropertyAttribute
    {
    }

}