using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TundraTools
{
    public class UVRemap : AssetPostprocessor
    {
		Regex modelRegex = new Regex(@".*");

        private void OnPostprocessModel(GameObject g)
        {
            if(!modelRegex.IsMatch(g.name)) { return; }

            foreach (var meshfilter in g.GetComponentsInChildren<MeshFilter>()) {
                var mesh = meshfilter.sharedMesh;

                bool foundMatch = false;
                if(mesh.uv3.Length != mesh.vertexCount) continue;

                foreach (var uv in mesh.uv3) {
                    if (Mathf.Abs(uv.magnitude) >= Mathf.Epsilon) {
                        foundMatch = true;
                        break;
                    }
                }

                if (foundMatch) {
                    var uv3 = mesh.uv3;
                    var colors = new Color[mesh.vertexCount];
                    
                    Debug.Log(colors.Length);
                    Debug.Log(uv3.Length);
                    for (var i = 0; i < mesh.vertexCount; i++) {
                        colors[i] = new Color(uv3[i].x, uv3[i].y, 0, 0);
                    }

                    mesh.colors = colors;
                } else {
                    Debug.Log($"Skipping {meshfilter.name}");
                }
            }

        }
    }
}