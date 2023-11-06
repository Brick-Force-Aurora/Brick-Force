[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\TeamMatchScore.cs)

The `TeamMatchScore` class is a script that is used to display the scores and goals in a team match in the game. It is a part of the larger Brick-Force project. 

The purpose of this code is to update and display the scores of the red and blue teams, as well as the number of goals achieved in the match. It also handles the flickering effect for the team scores. 

The class has several public variables that can be set in the Unity editor, such as the GUI depth, fonts for the scores and goals, and the background texture for the score display. It also has private variables to store the current scores of the red and blue teams. 

The `Start` method is called when the script is initialized. It sets the initial scores of both teams to 0 and sends a request to the server to get the current team scores if the player is currently breaking into a room. 

The `OnTeamScore` method is called when the server sends an update on the team scores. It updates the scores of the red and blue teams and adjusts the scale of the score fonts to create a visual effect. 

The `OnGUI` method is called to draw the GUI elements on the screen. It checks if the GUI is enabled and if there are no modal dialogs currently open. It then sets the GUI skin and depth, and begins a GUI group to draw the score display. It draws the background texture, the flickering effect for the current team, and the scores and goals using the specified fonts and coordinates. 

The `Update` method is called every frame to update the flickering effect for the team scores. 

Overall, this code provides a visual representation of the team scores and goals in a team match. It is used to enhance the gameplay experience and provide feedback to the players.
## Questions: 
 1. What is the purpose of the `OnTeamScore` method?
- The `OnTeamScore` method is responsible for updating the red and blue team scores and adjusting the scale of the corresponding score fonts.

2. What is the purpose of the `Start` method?
- The `Start` method initializes the red and blue team scores to 0 and sends a team score request if the player is currently breaking into a room.

3. What is the purpose of the `OnGUI` method?
- The `OnGUI` method is responsible for rendering the team match score GUI, including the background, team scores, and kill count.