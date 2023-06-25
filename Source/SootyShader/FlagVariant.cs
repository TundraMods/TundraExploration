using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TundraExploration.SootyShader
{
    // Despite it not being a scriptable object, it's nessecary in order to 
    // serialize the data that's being loaded from the Module in OnLoad()
    public class FlagVariant : ScriptableObject
    {
        public string name;
        public bool active;
        public string texturePath;
        public string flagPrefix;
        public float[] Tiling;
        public float[] Offset;
        public float Alpha;
        public float Spec;
        public bool isSelectable;
        public string guiName;
    }
}
