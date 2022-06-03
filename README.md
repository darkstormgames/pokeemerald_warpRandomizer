# Pokémon Emerald Warp Randomizer

## Disclaimer
As of a while back, this project is nothing more than a project to showcase how to do a complete warp randomization of Pokémon Emerald.
If you just want to create a randomized ROM, go check out the mod-downloads channel at [Pointcrow's Discord Server](https://discord.com/invite/pointcrow)



This is a randomizer for nearly all warps in Pokémon Emerald, written in C#.

To play with your own Seeds, you'll need to build the game yourself ([Take a look at the original decomp project for more infos on how to build pokeemerald.gba](https://github.com/pret/pokeemerald)), but I will provide a few .ips-Patches for everyone to use in the releases.

## Available Options
### Game Options
These are to fix up certain aspects of the game to make it playable and avoid hardlocks.
 - Enable Teleport/Fly everywhere (as the name implies, this enables the field moves Teleport and Fly on every map)
 - Disable auto-walking in elite four rooms (this avoids you walking into the void, when entering an elite four room)
 - Receive HM Waterfall after defeating Juan 
 - Receive all HMs after receiving the Pokedex (This also enables you to use all HMs from the very beginning)

### Gym Options
These change scripts or warps in gyms for either more challenge or to fix some bugs.
 - Randomize interior of Lavaridge Gym
 - Disable Petalburg Gym Events (This disables all events, so you can just battle your dad on first entry)
 - Randomize interior of Mossdeep Gym
 - Don't randomize ladder in Sootopolis Gym (This will affect the entire seed to be completely different)

### Event/Map Options (aka: Easy Mode)
These just make the game easier by disabling certain scripts.
 - Remove stair guard in Devon Corp 1F (Just to avoid a possible softlock)
 - Disable walk to Gym in Petalburg (Disables the tutorial boy in Petalburg for some extra warps)
 - Connect Sootopolis City (Adds some tiles in Sootopolis City, so you can just walk from the left to the right part)


## Excluded warps
These warps will not be randomized.
 - Pokemon Center 2F (Escalator to 2F is still randomized. 2F is just not accessible)
 - Everything on Battle Frontier Island (Except Poke-Center, Mart & Artisan Cave)
 - Battle Tent interiors
 - Shoal Cave
 - Littleroot (Tutorial can be played in full)
 - Safari Zone (entrance is randomized)
 - SS Tidal interior
 - Harbor interiors (Entrances are still randomized. Harbor interiors can't be accessed)
 - Mossdeep City Game Corner interior
 - Sootopolis Mystery Events House interior
 - Entrance to Island Cave|Ancient Tomb
 - Scripted Exits: (Scripted rooms may not be entered, I'm not sure...)
    - Battle Colosseum
    - Inside of Truck
    - Lilycove City Department Store Elevator
    - Record Corner
    - Secret Bases
    - Terra Cave exit (entrances are also locked. Groudon can be reached randomly.)
    - Trade Center
    - Marine Cave exit (entrances are also locked. Kyogre can be reached randomly.)
    - Union Room
 - Gym interiors:
    - Lavaridge Town Gym
    - Mossdeep City Gym
    - Petalburg City Gym
    - Sootopolis Ladder is still randomized (except you turn it off)

You can see all enabled/disabled warps by checking the warpMap.json in the Resources folder.
