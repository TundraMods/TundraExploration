using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TundraExploration.Modules
{
	// Full credit to Jsolson and the rest of the Bluedog Design Bureau team for allowing us to use and adapt this module for Tundra!
	class ModuleTundraCutDrogue : PartModule
	{
		public const string MODULENAME = "ModuleTundraCutDrogue";

		[KSPField]
		public bool isDrogueChute = false;

		[UI_Toggle(scene = UI_Scene.All, disabledText = "No", enabledText = "Yes")]
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Auto-Cut Drogue Chute")]
		public bool autoCutDrogue = true;

		[KSPField(isPersistant = true)]
		public bool triggered = false;

		private ModuleParachute chute = null;

		public override void OnStart(StartState state)
		{
			chute = part.FindModulesImplementing<ModuleParachute>().FirstOrDefault();
			if (chute == null)
				Debug.LogError($"[{MODULENAME}] ModuleParachute not found on part {part.partInfo.title}");

			Fields[nameof(autoCutDrogue)].guiActive = !isDrogueChute;
			Fields[nameof(autoCutDrogue)].guiActiveEditor = !isDrogueChute;
		}

		public override void OnUpdate()
		{
			if (isDrogueChute)
				return;

			if (chute == null)
				return;

			if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED || chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED)
			{
				if (!triggered)
				{
					List<ModuleTundraCutDrogue> drogues = vessel.FindPartModulesImplementing<ModuleTundraCutDrogue>().ToList();
					foreach (ModuleTundraCutDrogue d in drogues)
					{
						if (d.isDrogueChute && d.chute != null)
						{
							if (d.chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED || d.chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED)
								d.chute.CutParachute();
						}
					}
					triggered = true;
				}

			}
			else if (chute.deploymentState == ModuleParachute.deploymentStates.STOWED)
			{
				triggered = false;
			}
		}
	}
}