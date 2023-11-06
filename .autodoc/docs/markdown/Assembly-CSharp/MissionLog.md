[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MissionLog.cs)

The `MissionLog` class is a script that is used to display mission logs in the game. It is a part of the larger Brick-Force project. 

The purpose of this code is to handle the display of mission logs, including the progress, title, subtitle, and text of the mission. It also handles the display of weapon icons and their corresponding names and explanations. 

The class has several private variables, including `progress`, `title`, `sub`, `text`, `fmtType`, `guiDepth`, `missionBg`, `wpnicons`, `iconidx`, `titley`, `titleh`, `suby`, `subh`, `drawWeaponIcon`, `wpnNames`, and `wpnEXpls`. These variables store the necessary information for displaying the mission logs.

The `SetMission` method is used to set the mission details, including the progress, title, subtitle, and text. It also determines the `fmtType` based on the `fmt` parameter. The `fmtType` is used to determine the format of the text displayed in the mission log.

The `needPicture` method is used to indicate that a weapon icon needs to be displayed. It sets the `drawWeaponIcon` variable to true and increments the `iconidx` variable.

The `Start` method is called when the script starts and initializes some variables.

The `calcTitleBoxHeight` method calculates the height of the title box based on the length of the title and subtitle. It uses the `CalcHeight` method of the `GUIStyle` class to determine the height of the text.

The `checkStringFormat` method checks the `fmtType` and formats the `text` variable accordingly. It uses the `string.Format` method to replace placeholders in the text with the corresponding input keys.

The `OnGUI` method is called every frame to handle the GUI rendering. It sets the GUI skin and depth, and enables GUI interaction if there are no modal dialogs. It then calculates the height of the title box and displays the mission log elements, including the progress, title, subtitle, and text. If a weapon icon needs to be displayed, it also displays the icon, name, and explanation.

In summary, this code is responsible for displaying mission logs in the game. It sets the mission details, formats the text based on the mission type, and renders the mission log elements on the screen. It also handles the display of weapon icons and their corresponding names and explanations.
## Questions: 
 1. What is the purpose of the `SetMission` method and how is it used?
- The `SetMission` method is used to set the values of the `progress`, `title`, `sub`, `text`, and `drawWeaponIcon` variables. It is likely used to update the mission information displayed in the UI.

2. What is the purpose of the `needPicture` method and when is it called?
- The `needPicture` method is called to indicate that a weapon icon should be drawn in the UI. It sets the `drawWeaponIcon` variable to true and increments the `iconidx` variable.

3. What is the purpose of the `checkStringFormat` method and when is it called?
- The `checkStringFormat` method is called to format the `text` variable based on the value of the `fmtType` variable. It uses custom_inputs.Instance.InputKey to replace placeholders in the `text` string with corresponding key values.