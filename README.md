# UnityTurnBasedGame
Device to build game:
Mac Silicon M1
OS Sequoia v15.2
Windows, Mac, Linux - Unity 2022.3.55f1

Documentation:
Player 
Enemy
if the enemy have defense how do i use the defense for?
reduce attack? or for defense state?
give options :
Attack		Defend
Heal(3/3)	Utility
If Attack the character can’t reduce damage income
If Defend the character can’t attack but reduce damage income

Updated:
- Defense is for reduce enemy damage with defend animation state. 
- Options Changed to Attack, Defend, Utility, Run.
Attack is for player giving damage to opponent.
Defend is for player reducing damage from opponent.
- Utility is for player’s item, there’re only two options: Power, Dust.
Power is for powering up player damage.
Dust is for blinding enemy which is can make enemy blind and may miss his attack.

Unit Stat:

Steve:
45
49
49
65
65
45

Ronin:
39
52
43
60
50
65

Bug:
- the damage outcome isn’t the actual damage because the damage can give critical hit which has 50% chance
- the damage outcome is from Unit move (move.Base.Power) but the critical hit is from Player TakeDamage()
- Player can stay alive if the player spam “Defend” (1hp left)
- if player use utility and choose power then dust, the power up is gone but not the same as dust then power, the dust effect can still remain and player have power up.
- if player use dust, the opponent is blinded but if the opponent successfully attack, the effect of dust is still remain until opponent misses his attack. (Solution: make the dust effect 100% effective)
- if Enter/return is pressed in the middle of the dialog text animation, the dialog will get skipped but the next dialog will absurd

How to Play:
- use arrow key (Up, down, right, left) to select
- press enter/return to confirm selection
