# Home

Welcome to the Lestine wiki!

Lestine is Thuleanx's first commercial game project.
It's a mix between a bullet heaven and arpg.
The main concept is consuming enemies to gain their power, similar to the item Head Hunter from Path of Exile.

## Design

Lestine's core pillars are:
1. Randomly generated items
2. Deep character customization
3. Horde-like enemies
4. Fast action combat

Lestine gameplay loop resembles that of Risk of Rain. 
- Spawn into a level
- Fight your way to a teleporter
- Activate and occupy the teleporter
- Kill boss, use teleporter

The main mechanic will be manually consuming enemies to absorb their stats, passives, or even abilities.
It is never disadvantageous to consume an enemy. Extra effects should be immediately felt by the player, and inform them
that they should be more aggressive.

## Spec
- 1000 enemies
This entails both smooth, stutterless spawning of entities and sustaining them. For spawning, we will queue spawn into
an async spawn manager so entities are not guaranteed to immediately spawned. Rendering will be instanced and whenever
possible we will use Unity's Visual Effect Graph for GPU particles instead of particle system.

Recently, I watched an assassin's creed Unity GDC talk going over how they achieved mass (1000+) NPCs. The main takeaway
is LODs, making sure you only insert into structures and discard them since deletion is slow, and using the previous
frame's structure for bump avoidance. LOD is not too relevant to us, but we will use the later two techniques.

## Character
Black serpent sea art

## Stats
* Note: Damage of different attributes are either for flavor or can be buffed directly, but we don't intend to have 
resistances to different damage types

Core stats, all characters have:
Attack
Attack speed
Defense
Max Health
Movement Speed

Core resource:
Health
Barrier
Frenzy charges

Optional stats:
Damage scaling
Evasion
Crit chance
Crit damage
Dodge chance
Health regen
Health degen
Luck

Properties of the attack themselves:
Motion value
Knockback
Proc coefficient

Damage from the player will be significantly different than from enemies, who doesn't have many stacking items.
Enemies, for instance, doesn't crit the majority of the time. So logic for them will be different, and we should split 
the damage pipelines into 2 separate functions.

The rest of the more novel effects can be implemented as status effects, with a tick function. 

## Enemy Buffs
Unlike items, enemy buffs should generally stack multiplicatively or introduce novel effects. Examples includes:
- More movement speed
- Shoots out fireballs in addition to their main attacks
- Drain health from surrounding units
- Enemies explode on death
- Burning Witness | DOT trail where you walk
- Bleed on hit
- Regeneration
- Explode on death
- Split into 3
- Blackhole
- Shoots out exploding blobs on kill
- Gasoline | Ignite nearby enemies when enemies die
- Ukulele | Chain lightning to nearby characters when hit
- Chance to incinerate on screen enemies
- Tesla Coil
- Enemies that hits you are infected, spreading to multiple on death, stacking
- Charged Perforator | Chance of smiting enemies on hit
- Genesis Loop | Continuously drain health, when low will cause you to slow down, invulnerable, and exploding dealling massive damage
- Irradiant Pearl | Increase all stats by 30%
- Planula | Flat heal from incoming damage
- Shatterspleen | Crits bleed, bleed enemies explode on death
- Impale

## Items
This should follow Risk of Rain 2:
- Armor-Piercing round | Deals an additional 20% damage to elites
- Bison Steak | Flat max health increase
- Bustling Fungus | Heal increases after standing still
- Crowbar | Increase initial damage when enemy is full health
- Enemy Drink | Additive movement speed
- Focus Crystal | Increase damage with enemies close to you
- Lens-Maker's Glass | Crit chance
- Consuming enemies grants movespeed and attackspeed
- Repulsion armor | Flat damage reduction
- Soldier's Syringe | Flat attack speed
- Topaz | Temp barrier on kill
- Tougher Times | Chance to block damage

- Atk Missile | Chance to fire homing spears
- Chronobauble | Slow enemies on hit
- Harvester's Scyth | Crit heals
- Flat armor on consuming enemies
- Leeching Seed | Damage heals you
- Old Guillotine | Culling strike
- Fear nearby enemies when falling below certain health threshold, long cooldown
- Predatory instincts | Crits increase attack speed
- Razorwire | Dot around self
- Boost your movement speed until you get hit
- Ignition tank | Increase burn damage
- Boxing Gloves | Chance to knock enemies back on hit
- Hit stun chance

- Reduce all cooldowns
- Aegis | Healing past full grants a temp barrier
- Leaf Clover | Luck + 1, advantage on all throws
- Brilliant Behemoth
- Plasma Chain | Group hit enemies together
- Collosal Knurl | Max health, regen, armor
- Infusion | Increase max health on consume
- Happiest mask | Consumed enemies are now summoned as spectral allies
- N'kuhana's Opinion | Healing is stored as damage and released as projectiles
- Laser Scope | Crits deal additional damage
- Increase effect duration

## Gears
Character-specific upgrade to their equipment / weapon, similar to potentials in BlazeBlue Entropy Effect
- Spear shoots multiple projectiles
- Whirlwind every so often (Bag of Salt)
- Burst mode (like bullet triple taps)
- Auto-dodge one attack on a cooldown
- Homing spears
- Spear pierce
- Spear size
- Stand still and channel your spear in all directions
- Frenzy charges
- Heaven Cracker | Every 4th basic attacks fire a piercing shot
- Shattering Justice | Armor pen on hit
- If holding no weapons, discarded weapons become spectral weapons that fight for you
- Every 4th attack is a spear that pulls enemies into its range
- Increase damage of weapon for each pierce

## Coding and naming standards

I suck at naming things, and don't generally keep to a consistent standard for personal work.
For this project, I will be conforming to:
- [Conventional Commit Cheatsheet](https://gist.github.com/qoomon/5dfcdf8eec66a051ecd85625518cfd13)
outlines how a standard commit should look as well as what is included in descriptions.
Commits should be often and only encompass one feature.

## Tasks
[task_list](./task_list.md)

## Resources

### AI
[Anticipatory Collision Avoidance](https://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter19_Guide_to_Anticipatory_Collision_Avoidance.pdf)
