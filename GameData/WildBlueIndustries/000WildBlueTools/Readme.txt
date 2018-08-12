Wild Blue Tools

A KSP mod that provides common functionality for mods by Wild Blue Industries.

---INSTALLATION---

Copy the contents of the mod's GameData directory into your GameData folder.

1.55.11
- Bug fixes

1.55.9
- Un-broke the template managers.

1.55.7
- Recompiled for KSP 1.4.4

1.55.6
- Fixes issue where resources were consumed when trying to reconfigure a disassembled module.

1.55.5
- Fixes to OmniStorage.

1.55.4
- Bug fixes

1.55.3
- Bug fixes

1.55.2
- Bug fixes
- Classic Stock templates update - thanks JadeOfMaar! :)

1.55.1
- Bug fixes

1.55
- Improved resource summary in the geology lab.
- Fixed NRE issues with the WBIProspector.
- Classic Stock play mode now provides a new storage template: Omni Storage. This template lets you add and configure any number of resources up to the container's maximum storage volume. The part module that handles the capability is the new WBIOmniStorage.
- Added the new WBIOmniConverter. It lets you configure individual converters from a set of converter templates. There are a number of pre-defined templates for Classic Stock play mode.
- Adjusted Classic Stock resource densities to reflect the 5-liter standard used by most stock resources.
- Adjusted Classic Stock storage capacities to reflect the 5-liter standard used by most stock resources. These changes will affect new parts and when you reconfigure an existing part.
- Classic Stock is now the default Play Mode for new installs of WBI mods. Existing games are unchanged.

1.50
- Recompiled for KSP 1.4.1
- Gave ElectroPlasma a small amount of density.
- Fixed an edge case where WBIProspector would generate an NRE.
- WBIModuleScienceExperiment can now check for proximity to an anomaly as a requirement.
- New experiment result: WBIUnlockTechResult - You can use this experiment result to flag one or more parts as experimental. Just like with a part test contract, an experimental part can be used even if its tech node is unlocked.
- Added new Breakthrough Research contract. It makes use of the WBIUnlockTechResult module described above.
- New WBIModuleAsteroidResource allows you to guarantee that a resource will be available if the asteroid is a magic boulder.
- New WBIToolTipManager can read PART_TIP config nodes and provide tips to players. It's an alternate way to teach players how to use specific parts and handy for those who don't read the KSPedia, part descriptions, or wiki pages.
- Many updates for Classic Stock play mode - thanks JadeOfMarr! :)

Refinery

The Refinery is an new app available at the space center and in flight. It lets you produce and/or store resources in limited amounts. Such resources could be rare and not readily available for purchase. In fact, if you try to launch a craft with Refinery resources, those resources will be cleared before pre-launch. Once you launch the craft, you can purchase the Refinery resource, and when you recover the craft, its Refinery resources will be stored in the Refinery up to its maximum capacity.

Refinery nodes specify resources that are produced and/or stored in the Refinery.
You can specify one or more Refinery nodes with the same resource. Each node will become a production tier and appear in the order that you list them in the config file. Below is an example.

//New tiers appear in the same order as these nodes appear in the config file.
REFINERY
{
	resourceName = Graviolium

	//Optional tech node required to unlock the tier.
	techRequired = wbiSaucerTechnologies

	//How many units per day of the resource to produce
	//Set to 0 if you want to just store the resource and not produce it.
	unitsPerDay = 10

	//Multiplied by the resource's unit cost to determine the cost to produce a unit of the resource.
	unitCostMultiplier = 1.75

	//Maximum number of units that can be stored at the Refinery.
	maxAmount = 50000

	//The cost to unlock the production tier
	unlockCost = 250000
}

1.40
- Streamlined the WBIModuleResourceConverter
- WBIProspector now supports one or more harvest types.
- WBIProspector can now prospect resources from the atmosphere, exosphere, ocean, and planet.

1.31.2
- Bug Fixes

1.39.1
- Cruise Control fix for single-mode engines. They still need ModuleEnginesFX though.
- Fix for deprecated parts.
- ModuleManager update.

1.39

New Props
- Disco Ball: With spinning lights!
- Dance Floor: Animated dance floor!
- Jukebox: Actually plays music! Music files must be in the .ogg format and go in the 000WildBlueTools/Music folder.

ARP Icons
Added new Alternate Resource Panel icons courtesy of JadeOfMaar. These look great! :)

Bug Fixes And Enhancements
- Fixed missing resource requirements when templates require multiple resources.
- Inflatable parts can now be inflated only once.
- Added new WBICruiseControl part module. WBICruiseControl must be placed after your engine part module and multimode switch modules. It enables you to conduct engine burns during timewarp. NOTE: Heat mechanics don't work during cruise control, this is a known limitation. To use cruise control, be sure to set the desired throttle in the context menu, and set the fuel reserve percentage. Finally turn on Cruise Control. Then engage timewarp. Note that KSP doesn't allow you to adjust the controls in the context menu during timewarp, that's a game limitation.


1.38
- Bug fixes.

1.37
- Fixed missing resource icons.

1.36
- Far Future Technologies support: NuclearSaltWater used in place of Explodium.
- The Docking port helper now supports docking ports making use of WBILight. Hence, ports light up and you can set the color on the light.

1.35
- When dumping resources, any resources that are locked won't be dumped.
- Added resource distribution to the Buckboard 6000.
- WBISelfDestruct now supports explosions when staged.

1.31
- The S.A.F.E.R. fuel supply will last up to 10.86 years of continuous output.
- Fix for parts not remembering what template they're using.
- Blutonium and NuclearFuel are now Flow Mode all vessel. Reactors that use them are still restricted to resources in the part itself.
- Added NuclearWaste resource for Classic Stock.
- The S.A.F.E.R. now produces NuclearWaste as part of its outputs. With Pathfinder installed, it can be reprocessed into NuclearFuel.

1.28
- KIS storage volumes now properly calculated.
- Code cleanup
- Play Mode selection moved from Pathfinder to Wild Blue Tools.
- Added new Classic Stock play mode.
- CRP is now a separate download.

1.26
- BARIS is now an optional download as originally intended- just took awhile for me to figure out how to make that work. DO NOT DELETE the 000ABARISBridgeDoNotDelete FOLDER! That plugin is the bridge between this mod and BARIS.
- Recalibrated templates for Cryo Engines and added cryo tank cooling to LiquidHydrogen storage.

1.25
- BARIS update. 

1.23
- NRE fixes.

1.20
- Recompiled for KSP 1.3.
- Possible click-through fix.

1.19
- Minor bug fixes

1.18
- Experiments can now check the entire vessel via their "checkVesselResources" flag, to see if there are enough resources to complete the experiment.

1.17
- Added a Tweakscale patch for the plasma screens. Thanks for the patch, Violet_Wyvern!
- S.A.F.E.R. : The Safe Affordable Fission Engine for Rovers generates ElectricCharge for your spacecraft needs. It is based upon the real-world SAFE - 400 reactor created by NASA.

1.15
- Restricted the number of contracts that are offered and/or active.
- Fixed a situation where experiments weren't registering as completed.
- Contracts won't be offered until you've orbited the target world and have unlocked the proper tech tree.

1.14
- New WBIEnhancedExperiment PartModule. It brings the enhanced experiment requirements of WBIModuleScienceExperiments to science parts that don't need to be loaded into an experiment lab.
- Science system now generates research contracts.
- Bug fixes

1.12.0
- Bug Fixes

1.11.0
- Bug Fixes

1.10.0
New Props
- Holoscreen: This prop works the same way as the internal plasma screen, but you can toggle the screen on and off.

Bug Fixes & Enhancements
- WBIGeoLab now integrates into the Operations Manager.
- You can properly configure a part to be a battery by using the ConverterSkill.
- Fixed an issue with WBIHeatRadiator not showing up in the Operations Manager.
- Fixed an issue with IVAs spawning in the editor when inflating parts.
- You can now select the default image for the Plasma Screen in addition to screens in the Screenshots folder.
- Moved the kPad and plasma screens to the Utility tab.
- The experiment lab now accounts for the science multiplier difficulty setting when generating bonus science.

1.9.0
- Added WBINameTag, WBIGroundStabilizer, and WBIGeoLab.
- Added the Buckboard 6000

1.8.10
- KSP 1.2.2 update.

1.8.9
- Greenhouse fixes.

1.8.8
- Bug fixes & enhancements.

1.8.7
- Disabled angle snap.

1.8.6
- If the target docking port supports angle snap that you can turn on/off (all WBI docking ports do), and it's turned off, then it will be turned on if the active port's angle snap is turned on.

1.8.5

WBIDockingNodeHelper
- Added ability to enable/disable angle snap, and the ability to set the snap angle.

Other
- Cleaned up some logging issues related to missing part modules and textures when supported mods aren't installed.

1.8.4
- Updated to KSP 1.2.1
- Minor bug fixes with WBILight

1.8.3
- Fixed some welding issues.
- Greenhouses won't harvest crops if you run out of resources.

1.8.1
- You can now weld ports during eva.

1.8.0
- Added WBIConvertibleMPL. Use this when you want science labs with stock Mobile Processing Lab functionality to be able to switch to a different configuration.

1.7.3
- Fixed an issue where the greenhouse would freeze the game on load.

1.7.0
Updated to KSP 1.2. Expect additional patches as KSP is fixed and mods are updated.

1.6.5
- Growth time is no longer reduced based upon experienced Scientists. Yield is still affected by experience though.
- Greenhouses now show where they're at in the growth cycle and show up in the Ops Manager.

1.6.0
- Experiments can now be created in the field by some labs. To that end, experiments have the option to specify what resources they need and how much. If not specified, then a default value will be used that's equal to the experiment mass times 10 in the default resource, or a minimum amount of the default resource, whichever is greater.
- Labs have the ability to restrict the experiments they create based upon a list of tags. Hence experiments may list a set of tags as well. If an experiment has no tags that match the tags required by the lab then it won't show up in the list of experiments that it can create.
- Experiments can now require asteroids with a minimum mass.
NOTE: Basic and DeepFreeze experiments are now located in WildBlueTools; there is no effect to MOLE users.

1.5.0
- Bug fixes and new ice cream flavors.

1.4.2
- Minor fixes to the science system.

1.4.1
- Part mass is now correctly calculated.

1.4.0
- Added animation button prop to control external animations from the IVA.
- The cabin lights button prop can now control external light animations.
- Fixed an issue where resources required by experiments wouldn't be accumulated.

1.3.13
- Added template for Uraninite.

1.3.12
- You can now change the configration on tanks with symmetrical parts. In the SPH/VAB it will happen automatically when you select a new configuration. After launch, you'll have the option to change symmetrical tanks.

1.3.11
- Added WBISelfDestruct and WBIOmniDecouple.
- If fuel tanks are arrayed symmetrically, you'll no longer be able to reconfigure them. It's either that or let the game explode (ie nothing I can do about it except prevent players from changing symmetrical tanks).

1.3.10
- Fixed an issue where the greenhouse wasn't properly calculating the crop growth time.
- Fixed an NRE with lights
- Improved rendering performance for the Operations Manager.

1.3.9
- Fixed an issue with the CryoFuels MM support to avoid duplicate templates.

1.3.8
- Fixed an issue with crew transfers not working after changing a part's crew capacity during a template switch.

1.3.7
- Fixed an issue with WBILight throwing NREs in the VAB/SPH.

1.3.6
- Minor bug fixes

1.3.5
- Fixed an issue where lab experimetns would be completed before even being started.

1.3.4
- Fixed an issue where the "Bonus Science" tab would break the operations manager in the VAB/SPH.

1.3.3 Science Overhaul

- Experiment Manifest and transfer screens now list the part they're associated with.
- Fixed an issue where experiment info wasn't showing up in the VAB/SPH.
- The Load Experiment window now appears slightly offset from the Manifest window to make it easier to distinguish that you're now loading experiments into the part.
- The Transfer Experiment button now makes it more clear that it is a transfer experiment button.
- In the VAB/SPH, the Experiment Manifest will show a new "Load Experiment" button.
- You can now run/pause individual experiments.
- Changed how experiments check for and consume resources; they now go vessel-wide.
- The Experiment Lab will no longer stop if it, say, runs out of resources or the part doesn't have enough crew.
- Improved rendering performance of the experiment windows.
- Fixed an issue where experiments wouldn't show up after reloading a craft.

1.3.2
- New props

1.3.1
- Added Local Operations Manager window to enable controlling all PartModules on all vessels within physics range that implement IOpsView. This is used by mods such as Pathfinder.
- Added Slag and Konkrete resources, templates, and icons.
- Added WBIProspector and WBIAsteroidProcessor. These PartModules can convert non-ElectricCharge resources into all the resources available in a biome/asteroid, and can produce a byproduct resource. A typical use is to convert Ore/Rock into the locally available resources and Slag.
- The Rockhound template now uses the new WBIAsteroidProcessor to convert Ore and/ore Rock.

1.3.0
- Updated to KSP 1.1.3
- Introduced IOpsView to enable command and control of parts from the Operations Manager.
- Refactored the WBIMultiConverter to use a template selector similar to the WBIConvertibleStorage.
- WBIConvertibleStorage/WBIMulticonverter will show you all the templates that a part can use, but templates that you haven't researched yet will be grayed out.

1.2.9
- Removed Dirt from the USI LifeSupport template.
- Added icons for USI-LS templates.
- Added the WBIModuleDecoupler part module that can switch between a decoupler and a separator.

1.2.8
- Fixed an issue where converter text and experiment manifest text wasn't showing up in the VAB/SPH.
- Fixed an issue where you'd see crew portraits in a nearby vessel even though you're focused upon a different vessel.
- Fixed an issue where a science experiment would be run when transferring the experiment out of a lab, even though it hasn't met all the requirements.
- Fixed an issue where a science lab could not transmit data back to KSC when RemoteTech is installed. NOTE: This is a pretty simplistic fix; future updates will account for packet transmission rates etc.

1.2.7
- Fixed issues with USI-LS.

1.2.6
- Added new props

1.2.5
- Improved GUI for selecting resources
- You can now click on the laptop prop's monitor to change the image.

1.2.4
- More Input is NULL error fixes.

1.2.3
- More Input is NULL error fixes.

1.2.2
- Fixed NREs and Input Is NULL errors.

1.2.1
- Minor bug fixes

1.2.0

Science System
- Added a new science system that lets you load experiments into the Coach containers, fly them to your stations and transfer them into a MOLE lab, and once completed, load them back into a Coach for transport back to Kerbin. The experiments have little to no transmit value, encouraging you to bring them home (or if you prefer, load them into an MPL). The new experiments can have many requirements such as orbiting specific planets at specific altitudes, various resources, minimum crew, and required parts. To give players an added challenge, you can optionally specify the percentage chance that an experiment will succeed. You even have the ability to run a specific part module once an experiment has met the prerequisites- that gives you the ability to provide custom benefits. Consult the wiki for more details.

1.1.5
- Adjusted Ore and XenonGas capacities to reflect stock resource volumes.

---LICENSE---
Some resource definitions courtesy of Community Resource Pack. License: CC-BY-NC-SA 4.0

Refinery icon by Goran tek-en License: CC-BY-NC-SA 4.0

Art Assets, including .mu, .mbm, and .dds files are copyright 2014-2016 by Michael Billard, All Rights Reserved.

Wild Blue Industries is trademarked by Michael Billard. All rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

Source code copyright 2014-2016 by Michael Billard (Angel-125)

    This source code is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.