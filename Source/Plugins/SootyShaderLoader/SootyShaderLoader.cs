using System;
using UnityEngine;

namespace SootyShaderLoader
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class SootyShaderLoader : MonoBehaviour
    {
        private const string shadersAssetName = "tundraexpshader";
        private const string winShaderName = "-windows.ksp";
        private const string linuxShaderName = "-linux.ksp";
        private const string macShaderName = "-macosx.ksp";

        private static bool loaded;
        
        private static Shader tundraShader;
        
        public static Shader TundraShader
        {
            get { return tundraShader; }
        }

        private void Start()
        {
            if (loaded)
                return;

            loadBundle();
        }

        //This code is borrowed from Textures Unlimited: https://github.com/shadowmage45/TexturesUnlimited/blob/master/Plugin/SSTUTools/KSPShaderTools/Addon/TexturesUnlimitedLoader.cs#L198-L254
        private void loadBundle()
        {
            string shaderPath;

            if (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
                shaderPath = shadersAssetName + linuxShaderName;
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                shaderPath = shadersAssetName + winShaderName;
            else if (Application.platform == RuntimePlatform.LinuxPlayer)
                shaderPath = shadersAssetName + linuxShaderName;
            else
                shaderPath = shadersAssetName + macShaderName;

            shaderPath = KSPUtil.ApplicationRootPath + "GameData/TundraExploration/Resources/" + shaderPath;
            
            // KSP-PartTools built AssetBunldes are in the Web format, 
            // and must be loaded using a WWW reference; you cannot use the
            // AssetBundle.CreateFromFile/LoadFromFile methods unless you 
            // manually compiled your bundles for stand-alone use
            
            WWW www = CreateWWW(shaderPath);

            if (!string.IsNullOrEmpty(www.error))
            {
                print("[TundraExploration] - Error while loading shader AssetBundle: " + www.error);
                return;
            }
            else if (www.assetBundle == null)
            {
                print("[TundraExploration] - Could not load AssetBundle from WWW - " + www);
                return;
            }

            AssetBundle bundle = www.assetBundle;

            string[] assetNames = bundle.GetAllAssetNames();
            int len = assetNames.Length;
            Shader shader;
            for (int i = 0; i < len; i++)
            {
                if (assetNames[i].EndsWith(".shader"))
                {
                    shader = bundle.LoadAsset<Shader>(assetNames[i]);
                    print("[TundraExploration] - Loaded Shader: " + shader.name + " :: " + assetNames[i] + " from path: " + shaderPath);
                    if (shader == null || string.IsNullOrEmpty(shader.name))
                    {
                        print("[TundraExploration] - ERROR: Shader did not load properly for asset name: " + assetNames[i]);
                    }
                    GameDatabase.Instance.databaseShaders.AddUnique(shader);
                    tundraShader = shader;
                }
            }
            //this unloads the compressed assets inside the bundle, but leaves any instantiated shaders in-place
            bundle.Unload(false);

            loaded = true;
        }

        private static WWW CreateWWW(string bundlePath)
        {
            try
            {
                string name = Application.platform == RuntimePlatform.WindowsPlayer ? "file:///" + bundlePath : "file://" + bundlePath;
                return new WWW(Uri.EscapeUriString(name));
            }
            catch (Exception e)
            {
                print("[TundraExploration] - Error while creating AssetBundle request: " + e);
                return null;
            }
        }
    }
}
