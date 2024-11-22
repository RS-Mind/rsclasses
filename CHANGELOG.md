### Patch Notes:

#### 2.2.0
- Moved the backend to Unity
- Added custom card models for Astronomer and Mirror Mage cards
- Faster Barriers gives less barrier speed (100% -> 50%). Removed block cooldown penalty
- Gravity Well orbital speed increase reduced (100% -> 50%)
- Dual Shields renamed to Double Barriers
- Twin Scythes gun damage penalty reduced (-50% -> -25%)
- Reduced Harvester scythe speed bonus (100% -> 60%). Due to backend changes, the bonus no longer appplies to barrier speed
- Harvest Sickle rarity reduced (Exotic -> Uncommon)
- Fixed an issue with scythe damage scaling with life steal. Moved the effect to Dark Harvest
- Changed the descriptions on Mirror Mage to be more descriptive
- Mirror Mage rarity increased (Common -> Uncommon)
- Reflection Replacement health penalty increased (-30% -> -35%)
- Shatter Fracture size increase reduced (50% -> 30%)
- Might have increased the orbital size bonus of Domain Extension. It may have been in 2.0.0, idk

#### 2.1.0
- Harvester now causes scythes to deal bonus damage based on your lifesteal. Scythe speed lowered to compensate
- Shield Spikes rarity increased to Rare. Damage calculation altered and generally decreased
- Reflection Replacement now decreases health by 30%. Cooldown of Reflection Replacement increased to 3 seconds
- Swapped the block cooldown and echo effect between Guardian and Perfect Guard
- Fracture damage now scales with gun damage
- Adjusted the barrier size increase of Perfect Guard. Fixed the name of the stat
- Removed reflection cooldown penalty from Shatter. Reduced duration increase to 1s. You can now get multiple copies of the card.
- Adjusted barrier positions, causing them to form 2 sets of barriers, rather than evenly spacing. Overall coverage is identical to previous behavior
- Added colors to class tags (the text at the bottom right of the cards
- Fixed an issue with Mirror Mage and bullet spread

#### 2.0.0
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

#### 1.2.5
- Disabled some unused code

#### 1.2.3
- Forced orbitals to be removed on game end. Removing Astronomer will cause issues until a proper fix is found

#### 1.2.1
- Reduced scythe knockback

#### 1.2.0
- Changed wording from "Shield" to "Barrier"

#### 1.1.0
- Scythes not cutting bullets has been moved from Harvester to a base trait
- Base scythe speed increased 100 -> 300
- Base scythe damage decreased 55 -> 25
- Base shield speed decreased 200 -> 100
- Fixed issues with cards involving race conditions
- Dark Harvest now gives 50% -> 100% scythe damage and 50% -> 0% scythe speed
- Sharper Scythes now gives 75% -> 50% scythe damage

#### 1.0.0
- Added Astronomer Class