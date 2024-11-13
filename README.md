# RSClasses
Classes for ROUNDS that leverage unique mechanics.
Note that this mod does not work in sandbox, and is best played online rather than locally.

## Card List:

<details open>
<summary>Astronomer</summary>
<br>

### Astronomer
[Common Class]
Uses orbiting objects to fend off foes
- +2 Scythes
- +2 Shields
- -25% Damage

### Domain Extension
[Trinket]
Increases the size of orbitals and their orbits
- +25% Orbital size
- -25% Scythe damage

### Faster Barriers
[Common]
Increases the speed of barriers
- +100% Barrier speed
- +0.25s Block cooldown

### Gravity Well
[Uncommon]
Increases the speed of orbitals
- +25% Speed
- +100% Orbital speed
- -25% Health

### Sharper Scythes
[Uncommon]
- +50% Scythe damage
- -25% Damage

### Dual Shields
[Scarce]
- +2 Shields
- +0.25s Block cooldown

### Twin Scythes
[Scarce]
- +2 Scythes
- -50% Damage

### Guardian
[Exotic Subclass]
*Requires Dual Shields and Twin Scythes*
Stalwart Defenders. Their barriers defend them from all bullets
- +2 Barriers
- +25% Health
- +1 Block
- +100% Barrier size
- -4 Scythes

### Shield Spikes
[Uncommon]
*Requires Guardian*
When you block, your shields deal damage based on your maximum health
- +0.25s Block cooldown

### Perfect Guard
[Rare]
*Requires Guardian*
An inpenetrable ring of light
- +2 Barriers
- -0.25s Block cooldown
- +50% Shield size

### Harvester
[Uncommon Subclass]
*Requires Dual Shields and Twin Scythes*
Reapers of souls. Defense is not their concern. Their scythes don't cut through bullets
- +2 Scythes
- +100% Rotation speed (Scythes can hit once per rotation, so this is effectively an attack speed buff)
- -4 Barriers

### Harvest Sickle
[Exotic]
*Requires Harvester*
- +1 Scythe
- +10% Lifesteal

### Dark Harvest
[Rare]
*Requires Harvest Sickle*
A horrifying sight to behold
- +50% Scythe damage
- +15% Lifesteal
</details>

<details open>
<summary>Astronomer</summary>
<br>

### Mirror Mage
[Common Class]
Gain the ability to influence the mirror (Your bullets are reflected horizontally across the screen)
- +0.25s Reload time

### Polished Mirror
[Uncommon]
*Requires Mirror Mage*
See others' reflections in the mirror (Also across the prism and kaleidoscope if you have the relevant cards)

### Prism
[Uncommon]
*Requires Mirror Mage*
Gain the ability to perceive the prism (Your bullets are reflected vertically across the screen, for a total of 4 bullets)
- -25% Damage

### Reflection Replacement
[Uncommon]
*Requires Mirror Mage*
Reflect across the mirror and block when you take damage (This block triggers additional blocks)
- -15% Health

### Mirror Mind
[Common]
*Requires Reflection Replacement*
Swap minds with your reflection when you would cross the mirror
- +15% Movement speed
- +15% Jump height

### Fracture
[Uncommon]
*Requires Reflection Replacement*
Fracture the mirror when you take damage (Fracture deals damage and has 1 second duration)
- +100% Gravity

### Voidseer
[Uncommon Subclass]
*Requires Fracture*
Peer through the cracks in the mirror and into the beyond
Press 'E' or DPad-Up to deal damage to yourself (1 damage)
- +25% Lifesteal

### Shatter
[Uncommon]
*Requires Voidseer*
- +200% Fracture duration
- +50% Fracture size
- +50% Reflection cooldown

### Weakened Mirror
[Uncommon]
*Requires Voidseer*
Slip through the cracks more frequently
- -50% Reflection cooldown

### Forced Reflection
[Scarce]
*Requires Voidseer*
Players you shoot are sometimes reflected (50% chance)
Be careful because they get a block after (This block does not trigger additional blocks)

### Forced Refraction
[Uncommon]
*Requires Forced Reflection*
Players you shoot are sometimes refracted (50% chance. Independent of Forced Reflection)
Be careful because they get a block after (This block does not trigger additional blocks, will stack with the Forced Reflection block)

### Kaleido Witch
[Rare Subclass]
*Requires Prism*
Witness the kaleidoscope (Your bullets are additionally reflected across the 2 diagonals, for a total of 8 bullets)
- -100% Bullet gravity
- -25% Damage

### Emerald Glitter
[Uncommon]
*Requires Kaleido Witch*
Some bullets poison enemies (2 of the 8 kaleidoscope sections)
- +25% Poison damage (poison bullets deal 25% extra damage)

### Ruby Dust
[Uncommon]
*Requires Kaleido Witch*
Some bullets dazzle enemies (2 of the 8 kaleidoscope sections)

### Sapphire Shards
[Uncommon]
*Requires Kaleido Witch*
Some bullets chill enemies (2 of the 8 kaleidoscope sections)

### Kaleido Party
[Rare]
*Requires Kaleido Witch*
Your bullets get an extra bounce for each remaining ammo you have
- +1 Bullet bounce
</details>

<details open>
<summary>
Patch Notes:
</summary>
<br>

<details open>
<summary>
2.0.0
</summmary>
<br>
- Barrier counts and size generally reworked
- Reduced Scythe knockback
- Scythes can now only hit 1 player per revolution
- Added Harvest Sickle. Dark Harvest now requires Harvest Sickle
- Added Shield Spikes, a card which allows your barriers to deal damage on block.
- Reduced Dark Harvest's damage and lifesteal
- Altered Domain Extension's math
- Reduced Dual Shields' block cooldown penalty. Rarity to Scarce
- Reduced Faster Shields' speed increase. Removed block cooldown penalty
- Guardian now gives less health, but grants an additional block. Rarity changed to Exotic
- Harvester rarity changed to Exotic. Harvester now only gives +2 scythes
- Perfect Guard now reduces block cooldown by 0.25s. Rarity changed to Exotic
- Twin Scythes rarity changed to Scarce
- Added Mirror Mage class
- Added additional dependencies

</details>

<details open>
<summary>
1.2.5
</summmary>
<br>
- Disabled some unused code
</details>

<details open>
<summary>
1.2.3
</summmary>
<br>
- Forced orbitals to be removed on game end. Removing Astronomer will cause issues until a proper fix is found
</details>

<details open>
<summary>
1.2.1
</summmary>
<br>
- Reduced scythe knockback
</details>

<details open>
<summary>
1.2.0
</summmary>
<br>
- Changed wording from "Shield" to "Barrier"
</details>

<details open>
<summary>
1.1.0
</summmary>
<br>
- Scythes not cutting bullets has been moved from Harvester to a base trait
- Base scythe speed increased 100 -> 300
- Base scythe damage decreased 55 -> 25
- Base shield speed decreased 200 -> 100
- Fixed issues with cards involving race conditions
- Dark Harvest now gives 50% -> 100% scythe damage and 50% -> 0% scythe speed
- Sharper Scythes now gives 75% -> 50% scythe damage
</details>

<details open>
<summary>
1.0.0
</summmary>
<br>
- Added Astronomer Class
</details>
</details>