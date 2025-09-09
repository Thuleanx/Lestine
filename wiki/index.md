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

## Spec
- 1000 enemies
This entails both smooth, stutterless spawning of entities and sustaining them. For spawning, we will queue spawn into
an async spawn manager so entities are not guaranteed to immediately spawned. Rendering will be instanced and whenever
possible we will use Unity's Visual Effect Graph for GPU particles instead of particle system.

Recently, I watched an assassin's creed Unity GDC talk going over how they achieved mass (1000+) NPCs. The main takeaway
is LODs, making sure you only insert into structures and discard them since deletion is slow, and using the previous
frame's structure for bump avoidance. LOD is not too relevant to us, but we will use the later two techniques.

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
