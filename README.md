*NOTE: 
this repository was alternatively created to solve a GUI issue.
The original repository when it is at 100x Windows scale, works perfectly. But if you have a 2k ->4k type screen, Windows scaling will be affected. and this causes a lot of confusion when using the script

![before](https://github.com/user-attachments/assets/4d158088-4b54-4925-9ac1-daecda5f192a)


*Updated 9/26/2024

## Installation 
**If you use a Vegas version older than 14, change "using ScriptPortal.Vegas" to "using Sony.Vegas" on line 13 in the script.**

Locate your Vegas installation folder, commonly `C:\Program Files\Vegas Pro {version}\`
Put the .cs file inside the Script Menu folder
If Vegas is already open, go to `Tools -> Scripting -> Rescan Script Menu folder`

And finally if you have this, Congrats!

![image](https://github.com/vhoda/flippin.cs/assets/70240006/3ba59679-ef97-4ea7-b805-bc4a50534435)


**Optional**
Add this script to your Vegas toolbar for faster access. 
Just go on Options -> Customize Toolbar; then, scroll down until you see the script name, and add it to your toolbar in the desired position. 

## Usage
- [ ] Select the first clip from which the flipping will happen.
- [ ] Run the script (using the toolbar or Tools -> Scripting -> scriptname.cs)
- [ ] Input the number of the track that contains the clip to be flipped
- [ ] Enjoy

## Menu
- [ ] 1 Getting to (Does nothing)
- [ ] 1 2 Only Horizontal Flip
- [ ] 1 3 Only Vertical Flip
- [ ] 1 4 Both (Arps)

## License
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

Originally credits for the original repository by 
sykhro: https://gist.github.com/sykhro/923c79923463b5fe68b5

Thank you `@Gaopringao` for the multiple options!
