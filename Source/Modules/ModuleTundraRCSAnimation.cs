using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TundraExploration.Modules
{
    public class ModuleTundraRCSAnimation : PartModule
    {
        public const string MODULENAME = "ModuleTundraRCSAnimation";

        [KSPField]
        public int RCSModuleIndex = 0;

        [KSPField]
        public int deployedPosition = 1;

        [KSPField(isPersistant = true)]
        public bool rcsState;

        [KSPField]
        public string errorMessage;

        private ModuleAnimateGeneric moduleAnimate;
        private ModuleRCSFX moduleRCS;
        private bool initialized = true;

        public void Start()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            moduleAnimate = part.Modules.GetModule<ModuleAnimateGeneric>();
            moduleRCS = part.Modules.GetModule(RCSModuleIndex) as ModuleRCSFX;

            if (moduleAnimate == null)
            {
                Debug.LogError($"[{MODULENAME}] Could not find ModuleAnimateGeneric on part '{part.name}'");
                initialized = false;
            }

            if (moduleRCS == null)
            {
                Debug.LogError($"[{MODULENAME}] Could not find ModuleRCSFX at index '{RCSModuleIndex}' on part '{part.name}'");
                initialized = false;
            }

            if (initialized)
            {
                moduleAnimate.OnStop.Add(OnAnimationStop);
                moduleAnimate.OnMoving.Add(OnAnimationMoving);
                rcsState = moduleRCS.rcsEnabled;

                if (moduleRCS.Fields.TryGetFieldUIControl("rcsEnabled", out UI_Toggle rcsEnabledField))
                {
                    rcsEnabledField.onFieldChanged += OnRCSEnable;
                }

                if (moduleRCS.rcsEnabled && moduleAnimate.GetScalar != deployedPosition)
                {
                    moduleRCS.rcsEnabled = false;
                }
            }
        }

        private void OnRCSEnable(BaseField field, object sender)
        {
            if (moduleAnimate.GetScalar != deployedPosition && moduleRCS.rcsEnabled)
            {
                ScreenMessages.PostScreenMessage(errorMessage, 5f, ScreenMessageStyle.UPPER_CENTER);
                moduleRCS.rcsEnabled = false;
            }
            else
            {
                rcsState = moduleRCS.rcsEnabled;
            }
        }

        private void OnAnimationMoving(float current, float target)
        {
            if (target != deployedPosition)
            {
                moduleRCS.rcsEnabled = false;
            }
        }

        private void OnAnimationStop(float position)
        {           
            if (position == deployedPosition)
            {
                moduleRCS.rcsEnabled = rcsState;
            }
            else
            {
                moduleRCS.rcsEnabled = false;
            }
        }
    }
}
