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
    public class SootyVariant : ScriptableObject
    {
        public string name;
        public string displayName;
        public string soot1texturePath;
        public string soot2texturePath;
        public float[] sootState;
        public string primaryHexColor;
        public string secondaryHexColor;
        public string transitionsFrom;
    }
}
