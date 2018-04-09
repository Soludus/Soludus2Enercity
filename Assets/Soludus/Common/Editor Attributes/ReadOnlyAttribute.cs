using System;
using UnityEngine;

namespace Soludus
{

    /// <summary>
    /// Allows any property to be uneditabled and displayed as greyed out in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

}