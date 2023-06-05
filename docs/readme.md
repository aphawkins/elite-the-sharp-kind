# Elite - The Sharp Kind - Beta Release  

A C# port of the classic BBC home computer game 'Elite'.  It is meant to look, feel and play the same as the original 8bit and 16bit versions of the game.  

Currently the objectice of this port is authenticity, object oriented code and cross platform compatibility using dotnet.  
Framerate is fixed at 13.5, which using the current engine implentation, runs at approximately the same speed as the original.  
Performance, or maximum FPS, are a secondary objective, which may come later.  

## Controls  

Press Y or N on the intro screen.  
Press Space on the ship parade screen.  

F1  - Launch when docked, Front View when in flight.  
F2  - Rear View  
F3  - Left View  
F4  - Right View when in flight.  
	  When docked, buy equipment for ship:  
      up and down arrow keys to select item  
      return/enter key to buy.  
F5  - Display Galactic Chart.  
F6  - Short Range Chart.  
F7  - Show information on selected planet.  
F8  - Buy and sell items on the stock market:  
      up and down arrow keys to select item  
      right arrow key to buy  
      left arrow key to sell  
F9  - Commander information.  
F10 - Inventory.  
F11 - Options screen (Save Game, Load Game, Game Settings, Quit).  
      Use up and down arrows keys to select option, return/enter to select.  

A - Fire  
S or Up Arrow    - Dive  
X or Down Arrow  - Climb  
&lt; or Left Arrow  - Roll Left  
&gt; or Right Arrow - Roll Right  
/ - Slow Down  
Space - Speed up  

 C  - Activate docking computer, if fitted.  
 D  - De-activate docking computer if switched on.  
 E  - Active ECM, if fitted.  
 H  - Hyperspace.  
 J  - Warp Jump.  
 M  - Fire missile.  
 T  - Target a missile.  
 U  - Un-target missile.  
TAB - Detonate energy bomb, if fitted.  
CTRL+H - Galactic Hyperspace, if fitted.  
ESC - Launch escape capsule, if fitted.  
 P  - Pause game.  
 R  - Resume game when paused.  

### Chart Screens  
D - Select a planet and show distance to it.  
F - Find planet by name.  
O - Return cursor to current planet.  
Cursor Keys - Move cross hairs around.  

### Game Settings Screen  
From the Options Screen (F11) you can enter the Game Settings Screen. From here you can change a number of settings that control how the game looks and plays.  Use the cursor keys to select an option and the Enter/Return key to change it. The options can be saved as default for future games by pressing Enter while on the Save Settings option (NB this is not necessary if you want to change the settings just for the current game).  Game settings are held in the newkind.cfg file which should be in the same directory as the newkind.exe file.  

## Credits  

'Elite - The Sharp Kind' re-engineered in C# by Andy Hawkins 2023.  
- Converted into C#/.NET from C.J.Pinder's C version.  
- Forked from fesh0r/newkind 06 Dec 2022  

'Elite - The New Kind' re-engineered in C by C.J.Pinder 1999-2001.  
- christian@newkind.co.uk  |  www.newkind.co.uk  
- Reverse engineered from the BBC disk version of Elite.  
- Additional material by C.J.Pinder.  
- Face information for the ships. Adapted from the Elite ship data published by Ian Bell.  
- Alterations to vertex ordering by Thomas Harte. <T.Harte@excite.com>  
- Routines for drawing anti-aliased lines and circles by T.Harte.  
- Check for hidden surface supplied by T.Harte.  

The original Elite code is (C) I.Bell & D.Braben 1984.  
