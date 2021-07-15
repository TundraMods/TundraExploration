using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace TundraExploration.Modules
{
    public class ModuleTundraEngineSwitch : PartModule, IEngineStatus
    {
        public const string MODULENAME = "ModuleTundraEngineSwitch";

        [KSPField(guiName = "Mode", isPersistant = true, guiActive = true, guiActiveEditor = true)]
        public string currentEngineDisplay;

        [KSPField(isPersistant = true)]
        public int selectedIndex = 0;

        [KSPField]
        public string primaryEngineID;

        [KSPField]
        public string secondaryEngineID;

        [KSPField]
        public string tertiaryEngineID;

        [KSPEvent(guiActive = true, guiName = "Next Engine Mode")]
        public void NextEngineModeEvent() => SetNextEngine();

        [KSPEvent(guiActive = true, guiName = "Previous Engine Mode")]
        public void PreviousEngineModeEvent() => SetPreviousEngine();

        [KSPAction(guiName = "Activate Engine")]
        public void ActivateEngineAction(KSPActionParam param) => ActivateActiveEngine();

        [KSPAction(guiName = "Shutdown Engine")]
        public void ShutdownEngineAction(KSPActionParam param) => ShutdownActiveEngine();

        [KSPAction(guiName = "Toggle Engine")]
        public void ToggleEngineAction(KSPActionParam param) => ToggleActiveEngine();

        [KSPAction(guiName = "Next Engine Mode")]
        public void NextEngineModeAction(KSPActionParam param) => SetNextEngine();

        [KSPAction(guiName = "Previous Engine Mode")]
        public void PreviousEngineModeAction(KSPActionParam param) => SetPreviousEngine();

        [KSPAction(guiName = "Toggle Engine Mode")]
        public void ToggleEngineModeAction(KSPActionParam param) => SetNextEngine();

        private ModuleEnginesFX activeEngine;
        private ModuleEnginesFX oldEngine;
        private List<ModuleEnginesFX> engines = new List<ModuleEnginesFX>();

        public override void OnStart(StartState state)
        {

            base.OnStart(state);

            List<string> engineIDs = new List<string> { primaryEngineID, secondaryEngineID, tertiaryEngineID };
            for (int i = 0; i < 3; i++)
            {
                ModuleEnginesFX moduleEnginesFX = part.Modules.GetModules<ModuleEnginesFX>()
                                                              .Where(eng => eng.engineID == engineIDs[i])
                                                              .FirstOrDefault();
                if (moduleEnginesFX == null)
                {
                    Debug.LogError($"[{MODULENAME}] Could not find engine with ID '{engineIDs[i]}' on part '{part.name}'");
                    continue;
                }

                for (int i2 = 0; i2 < moduleEnginesFX.Actions.Count; i2++)
                {
                    moduleEnginesFX.Actions[i2].active = false;
                }

                moduleEnginesFX.manuallyOverridden = true;
                moduleEnginesFX.isEnabled = false;

                engines.Add(moduleEnginesFX);
            }

            SetActiveEngine(selectedIndex, true);
        }

        public void ShutdownActiveEngine()
        {
            activeEngine.Shutdown();
        }

        public void ActivateActiveEngine()
        {
            activeEngine.Activate();
        }

        public void ToggleActiveEngine()
        {
            if (activeEngine.EngineIgnited)
                activeEngine.Shutdown();
            else
                activeEngine.Activate();
        }

        public void SetNextEngine()
        {
            int newIndex = selectedIndex + 1;

            if (newIndex >= 3)
                newIndex = 0;

            SetActiveEngine(newIndex);
        }

        public void SetPreviousEngine()
        {
            int newIndex = selectedIndex - 1;

            if (newIndex < 0)
                newIndex = 2;

            SetActiveEngine(newIndex);
        }

        public void SetActiveEngine(int index, bool setup = false)
        {
            oldEngine = activeEngine;
            activeEngine = engines[index];
            
            activeEngine.manuallyOverridden = false;
            activeEngine.isEnabled = true;

            currentEngineDisplay = Regex.Replace(activeEngine.engineName, "([a-z])([A-Z])", "$1 $2");
            selectedIndex = index;

            if (setup)
                return;

            if (oldEngine.EngineIgnited)
            {
                activeEngine.Activate();
                activeEngine.currentThrottle = oldEngine.currentThrottle;
                oldEngine.Shutdown();
            }

            oldEngine.manuallyOverridden = true;
            oldEngine.isEnabled = false;
        }

        public bool isOperational
        {
            get
            {
                return activeEngine.isOperational;
            }
        }

        public float normalizedOutput
        {
            get
            {
                return activeEngine.normalizedOutput;
            }
        }

        public float throttleSetting
        {
            get
            {
                return activeEngine.throttleSetting;
            }
        }

        public string engineName
        {
            get
            {
                return activeEngine.engineName;
            }
        }
    }
}
