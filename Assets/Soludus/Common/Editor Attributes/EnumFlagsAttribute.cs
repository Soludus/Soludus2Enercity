using System;
using UnityEngine;

namespace Soludus
{

    /// <summary>
    /// Allows to use enums as bitmasks in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class EnumFlagsAttribute : PropertyAttribute
    {
    }

}