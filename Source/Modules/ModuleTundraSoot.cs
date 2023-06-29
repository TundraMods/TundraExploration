using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [KSPEvent(active = false, guiActiveEditor = true, guiName = "Flag1Selector")]
        public void Flag1Selector() => FlagSelector("1");

        [KSPEvent(active = false, guiActiveEditor = true, guiName = "Flag2Selector")]
        public void Flag2Selector() => FlagSelector("2");

        [KSPEvent(active = false, guiActiveEditor = true, guiName = "Flag3Selector")]
        public void Flag3Selector() => FlagSelector("3");

        [KSPEvent(guiActiveEditor = true, guiName = "Flag1Toggle")]
        public void Flag1Toggle() => FlagToggle("1");

        [KSPEvent(guiActiveEditor = true, guiName = "Flag2Toggle")]
        public void Flag2Toggle() => FlagToggle("2");

        [KSPEvent(guiActiveEditor = true, guiName = "Flag3Toggle")]
        public void Flag3Toggle() => FlagToggle("3");

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
        /// Speed of the Soot Transition
        /// </summary>
        [KSPField]
        public float SootySpeed = 0.5f;

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
        public List<FlagVariant> flagVariants = new List<FlagVariant>();

        private float Soot1_State = 0;
        private float Soot2_State = 0;
        private Coroutine TransitionRoutine;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            if (node.HasNode("SOOT"))
            {
                multipleSootTextures = true;

                List<ConfigNode> subtypes = node.GetNodes("SOOT").ToList();
                foreach (ConfigNode subtype in subtypes)
                {
                    SootyVariant sootyVariant = new SootyVariant();
                    sootyVariant.name = subtype.GetValue("name");
                    sootyVariant.displayName = subtype.GetValue("displayName");
                    sootyVariant.soot1texturePath = subtype.GetValue("soot1texturePath");
                    sootyVariant.soot2texturePath = subtype.GetValue("soot2texturePath");
                    sootyVariant.sootState = Array.ConvertAll(subtype.GetValue("sootState").Split(','), float.Parse);
                    sootyVariant.primaryHexColor = subtype.GetValue("primaryHexColor");
                    sootyVariant.secondaryHexColor = subtype.GetValue("secondaryHexColor");
                    subtype.TryGetValue("transitionsFrom", ref sootyVariant.transitionsFrom);

                    sootyVariants.Add(sootyVariant);
                }

                Debug.Log($"[TundraExploration] [{part.name}] {sootyVariants.Count} Soot subtypes loaded!");
            }
            if (node.HasNode("FLAG"))
            {
                List<ConfigNode> subtypes = node.GetNodes("FLAG").ToList();

                foreach (ConfigNode subtype in subtypes)
                {
                    FlagVariant flagVariant = new FlagVariant();
                    flagVariant.name = subtype.GetValue("name");
                    flagVariant.active = bool.Parse(subtype.GetValue("active"));
                    flagVariant.texturePath = subtype.GetValue("texturePath");
                    flagVariant.flagPrefix = subtype.GetValue("flagPrefix");
                    flagVariant.Tiling = Array.ConvertAll(subtype.GetValue("Tiling").Split(','), float.Parse);
                    flagVariant.Offset = Array.ConvertAll(subtype.GetValue("Offset").Split(','), float.Parse);
                    flagVariant.Alpha = float.Parse(subtype.GetValue("Alpha"));
                    flagVariant.Spec = float.Parse(subtype.GetValue("Spec"));
                    flagVariant.isSelectable = bool.Parse(subtype.GetValue("isSelectable"));
                    flagVariant.guiName = subtype.GetValue("guiName");
                    if (string.IsNullOrEmpty(flagVariant.guiName))
                    {
                        flagVariant.guiName = String.Concat(flagVariant.flagPrefix, flagVariant.name);
                    }

                    flagVariants.Add(flagVariant);
                }

                Debug.Log($"[TundraExploration] [{part.name}] {flagVariants.Count} flag subtypes loaded!");

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

                //LoadMaterial();
                OnTextureSwitch(true);
                Debug.Log($"[Tundra Exploration] Texture switched to: {selectedIndex}");
            }

            if (toggleSoot)
            {
                Soot1_State = sootyVariants[selectedIndex].sootState[0];
                Soot2_State = sootyVariants[selectedIndex].sootState[1];
                SetMaterialState();
            }
            else
            {
                if (!string.IsNullOrEmpty(sootyVariants[selectedIndex].transitionsFrom))
                {
                    SootyVariant prevVariant = sootyVariants.Where(v => v.name == sootyVariants[selectedIndex].transitionsFrom).FirstOrDefault();
                    Soot1_State = prevVariant.sootState[0];
                    Soot2_State = prevVariant.sootState[1];
                    SetMaterialState();
                }
            }

            foreach (FlagVariant flagVariant in flagVariants)
            {
                if (flagVariant.isSelectable)
                {
                    Events["Flag" + flagVariant.name + "Selector"].active = true;
                    Events["Flag" + flagVariant.name + "Selector"].guiName = String.Concat("Select ", flagVariant.guiName);

                }
                Events["Flag" + flagVariant.name + "Toggle"].guiName = String.Concat("Toggle ", flagVariant.guiName);

                SetFlag(flagVariant);
            }
        }
        private void FlagSelector(string name)
        {
            FlagVariant flagVariant = flagVariants.Where(v => v.name == name).FirstOrDefault();
            if (flagVariant != null)
            {
                var flagBrowser = (Instantiate((UnityEngine.Object)(new FlagBrowserGUIButton(null, null, null, null)).FlagBrowserPrefab) as GameObject).GetComponent<FlagBrowser>();
                flagBrowser.OnFlagSelected = flag => { OnCustomFlagSelected(flag, flagVariant); };
            }
        }

        private void FlagToggle(string name)
        {
            FlagVariant flagVariant = flagVariants.Where(v => v.name == name).FirstOrDefault();
            if (flagVariant != null)
            {
                flagVariant.active = !flagVariant.active;
                SetFlag(flagVariant);
            }
        }
        private void OnCustomFlagSelected(FlagBrowser.FlagEntry newFlagEntry, FlagVariant flagVariant)
        {
            flagVariant.texturePath = newFlagEntry.textureInfo.name;
            SetFlag(flagVariant);
        }

        private void OnTextureSwitch(bool isNewPart = false)
        {
            Debug.Log(selectedIndex);
            SootyVariant sootyVariant = sootyVariants[selectedIndex];

            SetMaterial(sootyVariant.soot1texturePath, "_Soot1");
            SetMaterial(sootyVariant.soot2texturePath, "_Soot2");


            if (!isNewPart)
            {
                Soot1_State = sootyVariants[selectedIndex].sootState[0];
                Soot2_State = sootyVariants[selectedIndex].sootState[1];
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

        private void SetFlag(FlagVariant flag)
        {
            foreach (Transform transform in ModelObjects)
            {
                if (flag.active) { 
                    Renderer renderer = transform.gameObject.GetComponent<Renderer>();
                    Texture texture = GameDatabase.Instance.GetTexture(flag.texturePath, false);
                    string currentflag = string.Concat(flag.flagPrefix, flag.name);

                    if (renderer == null)
                        continue;

                    renderer.material.SetTexture(currentflag, texture);
                    renderer.material.SetColor(string.Concat(currentflag, "_ST"), new Vector4(flag.Tiling[0], flag.Tiling[1], flag.Offset[0], flag.Offset[1]));
                    renderer.material.SetFloat(string.Concat(currentflag, "Alpha"), flag.Alpha);
                    renderer.material.SetFloat(string.Concat(currentflag, "Spec"), flag.Spec);
                }
                else
                {
                    Renderer renderer = transform.gameObject.GetComponent<Renderer>();
                    string currentflag = string.Concat(flag.flagPrefix, flag.name);

                    renderer.material.SetFloat(string.Concat(currentflag, "Alpha"), 0);
                }
            }
        }

        private void SetMaterialState()
        {
            foreach (Transform transform in ModelObjects)
            {
                Renderer renderer = transform.gameObject.GetComponent<Renderer>();

                if (renderer == null)
                    continue;

                renderer.material.SetFloat("_Soot1Alpha", Soot1_State);
                renderer.material.SetFloat("_Soot2Alpha", Soot2_State);
            }
        }

        private IEnumerator AnimateMaterial(bool isForward)
        {
            bool hasPrevious = false;
            SootyVariant prevVariant = null;
            float adjustedSoot1Speed;
            float adjustedSoot2Speed;
            if (!string.IsNullOrEmpty(sootyVariants[selectedIndex].transitionsFrom))
            {
                prevVariant = sootyVariants.Where(v => v.name == sootyVariants[selectedIndex].transitionsFrom).FirstOrDefault();
                adjustedSoot1Speed = SootySpeed * Mathf.Abs(sootyVariants[selectedIndex].sootState[0] - prevVariant.sootState[0]);
                adjustedSoot2Speed = SootySpeed * Mathf.Abs(sootyVariants[selectedIndex].sootState[1] - prevVariant.sootState[1]);
                hasPrevious = true;
            }
            else
            {
                adjustedSoot1Speed = SootySpeed * sootyVariants[selectedIndex].sootState[0];
                adjustedSoot2Speed = SootySpeed * sootyVariants[selectedIndex].sootState[1];
            }
            //Debug.Log($"[{moduleName}] Soot1 Speed: {adjustedSoot1Speed}, Soot2 Speed: {adjustedSoot2Speed} Soot1 State: {Soot1_State}, Soot2 State: {Soot2_State}");
            if (toggleSoot)
            {
                while (Soot1_State < sootyVariants[selectedIndex].sootState[0] || Soot2_State < sootyVariants[selectedIndex].sootState[1])
                {
                    if (Soot1_State != sootyVariants[selectedIndex].sootState[0])
                        Soot1_State = Mathf.Lerp(Soot1_State, sootyVariants[selectedIndex].sootState[0], TimeWarp.deltaTime * (HighLogic.LoadedSceneIsEditor ? adjustedSoot1Speed * 5 : adjustedSoot1Speed));

                    if (Soot2_State != sootyVariants[selectedIndex].sootState[1])
                        Soot2_State = Mathf.Lerp(Soot2_State, sootyVariants[selectedIndex].sootState[1], TimeWarp.deltaTime * (HighLogic.LoadedSceneIsEditor ? adjustedSoot2Speed * 5 : adjustedSoot2Speed));

                    if (Soot1_State > sootyVariants[selectedIndex].sootState[0] - 0.01f)
                        Soot1_State = sootyVariants[selectedIndex].sootState[0];

                    if (Soot2_State > sootyVariants[selectedIndex].sootState[1] - 0.01f)
                        Soot2_State = sootyVariants[selectedIndex].sootState[1];

                    SetMaterialState();

                    yield return null;
                }
            }
            else
            {
                while (Soot1_State > (hasPrevious ? prevVariant.sootState[0] : 0) || Soot2_State > (hasPrevious ? prevVariant.sootState[1] : 0))
                {
                    if (Soot1_State != (hasPrevious ? prevVariant.sootState[0] : 0))
                        Soot1_State = Mathf.Lerp(Soot1_State, (hasPrevious ? prevVariant.sootState[0] : 0), TimeWarp.deltaTime * (HighLogic.LoadedSceneIsEditor ? adjustedSoot1Speed * 5 : adjustedSoot1Speed));

                    if (Soot2_State != (hasPrevious ? prevVariant.sootState[1] : 0))
                        Soot2_State = Mathf.Lerp(Soot2_State, (hasPrevious ? prevVariant.sootState[1] : 0), TimeWarp.deltaTime * (HighLogic.LoadedSceneIsEditor ? adjustedSoot2Speed * 5 : adjustedSoot2Speed));


                    if (Soot1_State < (hasPrevious ? prevVariant.sootState[0] : 0) + 0.01f)
                        Soot1_State = (hasPrevious ? prevVariant.sootState[0] : 0);

                    if (Soot2_State < (hasPrevious ? prevVariant.sootState[1] : 0) + 0.01f)
                        Soot2_State = (hasPrevious ? prevVariant.sootState[1] : 0);

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

            LoadObjects();
            for (int i = ModelObjects.Count - 1; i >= 0; i--)
            {
                Renderer render = ModelObjects[i].gameObject.GetComponent<Renderer>();

                if (render == null)
                    continue;

                render.material.shader = shader;
            }

            loaded = true;

            Debug.Log("[TundraExploration] - Processed shader switch...");
        }
    }
}
