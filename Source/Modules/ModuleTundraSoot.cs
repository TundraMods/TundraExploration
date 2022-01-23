using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TundraExploration.SootyShader;
using UnityEngine;

namespace TundraExploration.Modules
{
    public class ModuleTundraSoot : PartModule
    {
        [UI_VariantSelector(affectSymCounterparts = UI_Scene.None, controlEnabled = true, scene = UI_Scene.Editor)]
        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = false, guiName = "Soot Variant")]
        public int selectedIndex;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Toggle Soot", guiActiveUnfocused = true, isPersistant = true)]
        [UI_Toggle(affectSymCounterparts = UI_Scene.None, disabledText = "Off", enabledText = "On")]
        public bool toggleSoot;

        [KSPEvent(guiActive = false, guiActiveEditor = false, guiActiveUnfocused = true, guiName = "Clean Soot", active = false)]
        public void CleanSootEvent() => CleanSoot();

        [KSPAction("Toggle Soot")]
        public void ToggleSootyAction(KSPActionParam param) => ToggleSooty();

        /// <summary>
        /// Name of the shader
        /// </summary>
        [KSPField]
        public string ShaderName;

        /// <summary>
        /// Where to apply the shader material
        /// </summary>
        [KSPField]
        public string ObjectNames;

        /// <summary>
        /// Path of the Soot Texture. Use the SUBTYPE {} node for multiple soots
        /// </summary>
        [KSPField]
        public string TextureName;

        /// <summary>
        /// Name of the Default Texture ID in the shader
        /// </summary>
        [KSPField]
        private string DefaultTextureID = "_MainTex";

        /// <summary>
        /// Name of the Texture ID in the Shader
        /// </summary>
        [KSPField]
        private string TextureID = "_MainTex2";

        /// <summary>
        /// Speed of the Soot Transition
        /// </summary>
        [KSPField]
        public float SootySpeed = 2;

        [KSPField]
        public bool ShowTransitionEvent = true;

        [KSPField]
        public bool ShowTransitionAction = true;

        [KSPField]
        public bool OneShot = false;

        [KSPField]
        public bool EVAClean = false;

        public bool multipleSootTextures;
        public bool loaded;

        public List<Transform> ModelObjects = new List<Transform>();
        public List<SootyVariant> sootyVariants = new List<SootyVariant>();

        private float materialState = 0;
        private Coroutine TransitionRoutine;

        private Texture defaultTexture;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            if (node.HasNode("SUBTYPE"))
            {
                multipleSootTextures = true;

                List<ConfigNode> subtypes = node.GetNodes("SUBTYPE").ToList();
                foreach (ConfigNode subtype in subtypes)
                {
                    SootyVariant sootyVariant = new SootyVariant();
                    sootyVariant.name = subtype.GetValue("name");
                    sootyVariant.displayName = subtype.GetValue("displayName");
                    sootyVariant.texturePath = subtype.GetValue("texturePath");
                    sootyVariant.primaryHexColor = subtype.GetValue("primaryHexColor");
                    sootyVariant.secondaryHexColor = subtype.GetValue("secondaryHexColor");
                    subtype.TryGetValue("transitionsFrom", ref sootyVariant.transitionsFrom);

                    sootyVariants.Add(sootyVariant);
                }

                foreach (SootyVariant sootyVariant in sootyVariants)
                {
                    if (!string.IsNullOrEmpty(sootyVariant.transitionsFrom))
                    {
                        SootyVariant variant = sootyVariants.Where(v => v.name == sootyVariant.transitionsFrom).FirstOrDefault();
                        sootyVariant.transitionsTexture = variant.texturePath;
                    }
                }

                Debug.Log($"[TundraExploration] [{part.name}] {sootyVariants.Count} Subtypes loaded!");
            }

            if (loaded)
                return;

            LoadShader();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            LoadObjects();

            Events["CleanSootEvent"].active = EVAClean;
            Actions["ToggleSootyAction"].active = ShowTransitionAction;
            Fields["selectedIndex"].guiActiveEditor = multipleSootTextures;

            Fields["toggleSoot"].guiActive = ShowTransitionEvent;
            Fields["toggleSoot"].guiActiveEditor = ShowTransitionEvent;

            if (Fields.TryGetFieldUIControl("toggleSoot", out UI_Toggle toggleSootField))
            {
                toggleSootField.onFieldChanged += delegate { SetSoot(); };
            }

            if (multipleSootTextures)
            {
                List<PartVariant> partVariants = new List<PartVariant>();
                foreach(SootyVariant sootyVariant in sootyVariants)
                {
                    PartVariant partVariant = new PartVariant(sootyVariant.name, sootyVariant.displayName, new List<AttachNode>());
                    partVariant.PrimaryColor = sootyVariant.primaryHexColor;
                    partVariant.SecondaryColor = sootyVariant.secondaryHexColor;
                    partVariants.Add(partVariant);
                }

                if (Fields.TryGetFieldUIControl("selectedIndex", out UI_VariantSelector selectedIndexField) && HighLogic.LoadedSceneIsEditor)
                {
                    selectedIndexField.variants = partVariants;
                    selectedIndexField.onFieldChanged += delegate { OnTextureSwitch(); };
                }

                LoadMaterial();
                OnTextureSwitch(true);
                Debug.Log($"[Tundra Exploration] Texture switched to: {selectedIndex}");
            }

            if (toggleSoot)
            {
                materialState = 1f;
                SetMaterialState();
            }            
        }

        private void OnTextureSwitch(bool isNewPart = false)
        {
            SootyVariant sootyVariant = sootyVariants[selectedIndex];
            bool doTransition = !string.IsNullOrEmpty(sootyVariant.transitionsFrom);

            if (doTransition)
                SetMaterial(sootyVariant.transitionsTexture, DefaultTextureID);
            else
                SetMaterial(defaultTexture, DefaultTextureID);

            SetMaterial(sootyVariant.texturePath, TextureID);

            if (!isNewPart)
            {
                materialState = 1f;
                SetMaterialState();
                toggleSoot = true;
            }
            
            Debug.Log($"[Tundra Exploration] Texture switched to: {selectedIndex}");
        }

        public void SetSoot()
        {
            if (toggleSoot && OneShot && HighLogic.LoadedSceneIsFlight)
            {
                Fields["toggleSoot"].guiInteractable = toggleSoot && OneShot && HighLogic.LoadedSceneIsFlight;
                return;
            }

            if (TransitionRoutine != null)
                StopCoroutine(TransitionRoutine);

            TransitionRoutine = StartCoroutine(AnimateMaterial(toggleSoot));
        }

        public void ToggleSooty()
        {
            if (toggleSoot && OneShot && HighLogic.LoadedSceneIsFlight)
                return;

            toggleSoot = !toggleSoot;

            if (TransitionRoutine != null)
                StopCoroutine(TransitionRoutine);

            TransitionRoutine = StartCoroutine(AnimateMaterial(toggleSoot));
        }

        public void CleanSoot()
        {
            if (!toggleSoot)
                return;

            if (FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA)
            {
                toggleSoot = false;

                if (TransitionRoutine != null)
                    StopCoroutine(TransitionRoutine);

                TransitionRoutine = StartCoroutine(AnimateMaterial(toggleSoot));
            }
        }

        private void SetMaterialState()
        {
            foreach (Transform transform in ModelObjects)
            {
                Renderer renderer = transform.gameObject.GetComponent<Renderer>();

                if (renderer == null)
                    continue;

                renderer.material.SetFloat("_State", materialState);
            }
        }

        private IEnumerator AnimateMaterial(bool isForward)
        {
            if (toggleSoot)
            {
                while (materialState < 1)
                {
                    materialState = Mathf.Lerp(materialState, 1, TimeWarp.deltaTime * (HighLogic.LoadedSceneIsEditor ? SootySpeed * 5 : SootySpeed));

                    if (materialState > 0.99f)
                        materialState = 1;

                    SetMaterialState();

                    yield return null;
                }
            }
            else
            {
                while (materialState > 0)
                {
                    materialState = Mathf.Lerp(materialState, 0, TimeWarp.deltaTime * SootySpeed);
                    //print("[TundraExploration] - Setting soot to: " + materialState.ToString());

                    if (materialState < 0.001f)
                        materialState = 0;

                    SetMaterialState();

                    yield return null;
                }
            }

            TransitionRoutine = null;
        }

        private void SetMaterial(string path, string textureID)
        {
            foreach(Transform transform in ModelObjects)
            {
                Renderer renderer = transform.gameObject.GetComponent<Renderer>();

                if (renderer == null)
                    continue;

                Texture texture = GameDatabase.Instance.GetTexture(path, false);
                renderer.material.SetTexture(textureID, texture);
            }
        }

        private void SetMaterial(Texture texture, string textureID)
        {
            foreach (Transform transform in ModelObjects)
            {
                Renderer renderer = transform.gameObject.GetComponent<Renderer>();

                if (renderer == null)
                    continue;

                renderer.material.SetTexture(textureID, texture);
            }
        }

        private void LoadMaterial()
        {
            Renderer renderer = ModelObjects[0].gameObject.GetComponent<Renderer>();
            defaultTexture = renderer.material.GetTexture(DefaultTextureID);
        }

        public void LoadObjects()
        {
            var names = ObjectNames.Split(';');
            for (int i = names.Length - 1; i >= 0; i--)
            {
                string name = names[i];

                ModelObjects.AddRange(part.FindModelTransforms(name));
            }
        }

        private void LoadShader()
        {
            if (string.IsNullOrEmpty(ObjectNames))
                return;

            if (string.IsNullOrEmpty(ShaderName))
                return;

            Shader shader = GameDatabase.Instance.GetShader(ShaderName);
            if (shader == null)
            {
                Debug.LogError("[TundraExploration] - Error loading shader from GameDatabase: " + ShaderName);
                shader = SootyShaderLoader.TundraShader;
            }

            if (shader == null)
                return;

            Texture tex = GameDatabase.Instance.GetTexture(multipleSootTextures ? sootyVariants[selectedIndex].texturePath : TextureName, false);

            if (tex == null)
                return;


            LoadObjects();
            for (int i = ModelObjects.Count - 1; i >= 0; i--)
            {
                Renderer render = ModelObjects[i].gameObject.GetComponent<Renderer>();

                if (render == null)
                    continue;

                render.material.shader = shader;
                render.material.SetTexture(TextureID, tex);
            }

            loaded = true;

            Debug.Log("[TundraExploration] - Processed shader switch...");
        }
    }
}
