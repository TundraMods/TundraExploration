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
        public string texturePath;
        public string flagPrefix = "_Flag";
        public float[] Tiling = new float[] {1f, 1f};
        public float[] Offset = new float[] {0f, 0f};
        public float Alpha = 1;
        public float Spec = 0;
        public bool isSelectable = false;
    }
}
