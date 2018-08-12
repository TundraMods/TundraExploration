KerbalActuators

A KSP mod that provides several part modules to manipulate various mesh transforms in a part in order to animate it. Modules include:

WBIMagnetController: Primarily used by robot arms to grab parts and move them around.
WBILightController: Controls a light.
WBIRotationController: Rotates mesh transforms to help animate a robot arm.
WBITranslationController: Moves a mesh transform back and forth.
WBIServoManager: Added after all of the above part modules in a config file, the servo manager coordinates all the controllers. It provides GUI controls to manipulate the controllers. You can create, load, and save a series of servo snapshots into a sequence and then play back the snapshots to animate a part.

WBIHoverController: Provides hover management to engines.
WBIPropSpinner: Spins props for propeller-driven engines.
WBIVTOLManager: Added after all of the above, the VTOL Manager handles hover management and can provide GUI controls for WBIRotationController.

WBIAirParkController: Enables a vessel to "part" in mid air and be treated as if landed. It's finicky but it works...

For a description of the API, go to https://github.com/Angel-125/KerbalActuators/wiki

KerbalActuators comes with two sample parts: SampleArm and SampleCrane. To use these parts, you'll need to rename the config files, found in Parts/Utility from .txt to .cfg.

---INSTALLATION---

Copy the contents of the mod's GameData directory into your GameData folder.

---REVISION HISORY---

1.5.1
- Bug fixes

1.5.0
- WBIPropSpinner now supports RCS! You don't need a ModuleEnginesFX; just set enabledByRCS to true in WBIPropSpinner, and minThrottleBlur to determine when to start blurring the rotor.

1.4.8
- Bug Fixes

1.4.6
- Recompiled for KSP 1.4.4

1.4.5
- Added additional infrastructure to support 3rd party mods like MOARdV Avionics Systems.

1.4.0.0
- Added new WBIMultiModeEngine. It works like the stock MultiModeEngine except that you can switch between any number of engines instead of just two. It also is tied into the WBIHoverController.

1.3.0.5
- Minor edits to support VTOL ops.

1.3.0.4
- Minor bug fixes

1.3.0.3
- Minor fixes for reverse-thrust in WBIPropSpinner.

1.3.0.2
New Actuators
- WBITranslationController: This part module lets you move mesh transforms around.

New Sample Parts
- Mk1 Station Arm (Advanced Construction): This arm can be use to maneuver payloads around. Found in Parts/Utility/SampleArm.
- Konstruction Krane (Advanced Construction): The ground equivalent to the Mk1 Station Arm, the Konstruction krane is helpful for picking stuff up on the ground and placing them were you want to. Found in Parts/Utility/SampleCrane.

Bug Fixes & Enhancements
- Added an emergency stop button to halt all servo movement.
- When your rotation controller is moving in one direction and you rotate in the opposite direction, the controller will now immediately stop and start rotating in the proper direction.
- Add icons for several servo control buttons.
- WBILightController now works properly.
- You can now specify where to find the icons using a KerbalActuators config node:
KerbalActuators
{
	iconsFolder = WildBlueIndustries/001KerbalActuators/Icons
	VTOLAppButtonIcon = WildBlueIndustries/001KerbalActuators/Icons/VTOLAppButton
}

1.2.0
- Added WBIMagnetController and WBILightController.
- Added XML document comments to all the modules.

---ACKNOWLEDGEMENTS---

WBIMagnetController incorporates code by sirkut. MUCH appreciated, sirkut! :)

---LICENSE---
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