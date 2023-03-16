![](https://i.imgur.com/IoX3Vu4.png)

<b>This mod is an addition to Jewelcrafting mod that adds neew gems for your items with new soulcatching mechanic.

Mod adds 30+ gems with different effects. All gems acquired from killing monsters and capturing their souls into Soulcatcher Lantern.

</b>

<details>
  <summary><b><span style="color:aqua;font-weight:200;font-size:20px">
    How-To:
</span></b></summary>

#
<details>
  <summary><span style="color:aqua;font-weight:200;font-size:20px">
    New Items and Buildings:
</span></summary>
 <b>In addition to Jewelcrafting Gemcutting table Soulcatcher adds new build piece: Soul Altar. This station will be used in order to convert captured souls into gems</b>

 ![](https://i.imgur.com/NH3fD7m.png)

 ![](https://i.imgur.com/tuvzPTA.png)



<b>Also new item added: Soulcatcher Lantern that will allow you to capture souls of defeated enemies (Craftable in Jewelcrafting Gemcutting table)</b>

![](https://i.imgur.com/dQeve8D.png)

![](https://i.imgur.com/dsVkLas.png)


**Soul Bowl:**

![](https://i.imgur.com/62F6E5r.png)

This building allows you to use souls in order to get some AoE buffs.

![](https://i.imgur.com/nwywPJe.png)

After placing it, in order to use Bowl you would need to take lantern in your hand which has one of 5 souls:

1) Deer (increase plant growup speed)
2) Stone Golem (increase players damage reduction)
3) Greydwarf (heals all players)
4) Boar (regens stamina for all players)
5) Fenring (increases all players damage dealt)

![](https://i.imgur.com/o7AvhH2.png)

Then you can press E multiple tiems in order to add resource (time) to current bowl.

![](https://i.imgur.com/ntgkbvo.png)

In config you can find options for each soul value (heal, stamina regen, +damage, +reduction, growup rate), as well as bowl range (AoE) and soul remaining time


Soul Platform:

![](https://i.imgur.com/qWKNcZO.png)

Just a simple decoration building that allows you to place ANY soul on it, in order to create visual model.
Doesn't give anything and can be used just as home decoration

![](https://i.imgur.com/08k1kKr.png)

![](https://i.imgur.com/KU9jdJf.png)



</details>

#


<details>
  <summary><span style="color:aqua;font-weight:200;font-size:20px">
    Capturing souls:
</span></summary>
 <b>Soul has 10% chance + 1% per creature level to spawn

To capture a soul you need two conditions:

1) Kill enemy
2) At least one Soulcather Lantern **should be in your inventory**

First you kill enemy that is able to be converted into gem:

![](https://i.imgur.com/7R99ztI.png)

Then you should take Soulcatcher Lantern from your inventory and hold right mouse button while looking at soul

![](https://i.imgur.com/HHKZs0d.png)

After few seconds you will successfully capture soul (if you have less than max souls inside your lantern)

![](https://i.imgur.com/NCZsURW.png)

You can check result by simply hovering on your Lantern:

![](https://i.imgur.com/ZZbvdtm.png)

</b>

</details>

#

<details>
  <summary><span style="color:aqua;font-weight:200;font-size:20px">
    Creating your first gem:
</span></summary>
<b>
In order to convert Lantern Souls into gems you need do few things:

1) Build Soul Altar
2) Interact with it

Soul Altar UI will be opened:

![](https://i.imgur.com/mXQeMG2.png)

In order to see all possible soul convertions click on (!) icon:

![](https://i.imgur.com/y1sKAam.png)

After you open Soul Altar UI click on Soulcatcher Lantern in your inventory with Left Mouse Button. It will choose particular lantern as target. For example i will choose this lanter:

![](https://i.imgur.com/iS17Wu8.png)

Result is:

![](https://i.imgur.com/8vaph4n.png)

On top you can see list of your captured souls (Image, current soul amount and in which gem it can be converted)

In bottom side you will see gems itself (GEMS WILL APPEAR ONLY IF YOU HAVE ENOUGH SOULS TO CRAFT THEM).

After click on gem you can select it to see its description / craft time and set it as craft target:

![](https://i.imgur.com/0pqB68R.png)

I choose surtling gem as target. Then you just click "Create" button and craft process will start:

![](https://i.imgur.com/IaeFeN7.png)

After process is done (100%), Soul Altar will create gem on top of it as result: 

![](https://i.imgur.com/bzG88hS.png)


</b>

</details>

#

<details>
  <summary><span style="color:aqua;font-weight:200;font-size:20px">
    Using gems and progressions:
</span></summary>
<b>

After you done previous steps you can use gems and put it in your items (Same as in Jewelcrafting mod).

We got surtling gem from convertion:

![](https://i.imgur.com/XnOVO7f.png)

Which can be used in any weapon:

![](https://i.imgur.com/8FzeFMK.png)

Trying to attack enemy:

![](https://i.imgur.com/eKnYUTe.png)

BOOM

There are 3 gem tiers of each gem (Except Yagluth and Bonemass gems)

You can use 3 same tier gems in order to create new tier of it. It can be done in Gemcutting table:

![](https://i.imgur.com/6gdtRUH.png)

3 tiers: Gem, Ascend Gem, Immortal Gem

Also Soulcatcher adds a small Skill: 

![](https://i.imgur.com/iSfrDGQ.png)

Capturing souls will increase this skill level. Default soul capturing time is 4 seconds, but with each level of this skill you decrease it on 0.02s (100 lvl = 2 seconds capture duration)
Also Soulcatcher increases soul spawn chance by 0.05% per level

</b>

</details>

#

</details>


<details>
  <summary><b><span style="color:aqua;font-weight:200;font-size:20px">
    YAML settings
</span></b></summary>

Location: BepInEx/Config/Jewelcrafting.Sockets_Soulcatcher_KG_JC_Additions.yml

A .yml file that will allow you to edit all gems stats (Synced from Serverside)


```yaml


Deer Soul Power:
  slot: legs
  gem: Deer Soul Gem
  power:
    value: [10, 15, 20, 25, 30]
  unique: None

Neck Soul Power:
  slot: legs
  gem: Neck Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: None

Boar Soul Power:
  slot: all
  gem: Boar Soul Gem
  power:
    value: [3, 5, 7, 9, 11]
  unique: None

Greydwarf Soul Power:
  slot: all
  gem: Greydwarf Soul Gem
  power:
    value: [3, 6, 9, 12, 15]
    cooldown: [60, 50, 45, 40, 35]
  unique: Item

GreydwarfBrute Soul Power:
  slot: all
  gem: GreydwarfBrute Soul Gem
  power:
    value: [0.4, 0.7, 1, 1.3, 1.6]
  unique: Gem

GreydwarfShaman Soul Power:
  slot: all
  gem: GreydwarfShaman Soul Gem
  power:
    value: [26, 25, 24, 23, 20]
  unique: Gem

Troll Soul Power:
  slot: weapon
  gem: Troll Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
    chance: [30, 35, 40, 45, 50]
  unique: None

Skeleton Soul Power:
  slot: all
  gem: Skeleton Soul Gem
  power:
    chance: [7, 9, 11, 13, 15]
    value: [3, 2, 2, 1, 0]
  unique: Gem

Draugr Soul Power:
  slot: shield
  gem: Draugr Soul Gem
  power:
    value: [20, 30, 40, 50, 60]
  unique: Gem

Blob Soul Power:
  slot: legs
  gem: Blob Soul Gem
  power:
    value: [0.3, 0.8, 1.5, 2.2, 2.9]
  unique: None

Leech Soul Power:
  slot: weapon
  gem: Leech Soul Gem
  power:
    value: [1, 1.5, 2, 2.5, 3]
  unique: None

Wraith Soul Power:
  slot: all
  gem: Wraith Soul Gem
  power:
    cooldown: [30, 26, 22, 18, 14]
  unique: Gem

Abomination Soul Power:
  slot: all
  gem: Abomination Soul Gem
  power:
    value: [30, 40, 50, 60, 70]
    cooldown: [30, 28, 26, 24, 22]
  unique: Gem

Wolf Soul Power:
  slot: [head, chest, legs, weapon]
  gem: Wolf Soul Gem
  power:
    value: [9, 12, 15, 18, 21]
  unique: Item

Bat Soul Power:
  slot: weapon
  gem: Bat Soul Gem
  power:
    value: [1, 1.5, 2, 2.5, 3]
  unique: None

Fenring Soul Power:
  slot: weapon
  gem: Fenring Soul Gem
  power:
    value: [1, 2, 3, 4, 5]
  unique: None

Cultist Soul Power:
  slot: head
  gem: Cultist Soul Gem
  power:
    value: [20, 35, 50, 65, 80]
    cooldown: [35, 30, 25, 20, 15]
  unique: Gem

Ulv Soul Power:
  slot: all
  gem: Ulv Soul Gem
  power:
    cooldown: [20, 18, 16, 14, 12]
  unique: Gem

StoneGolem Soul Power:
  slot: all
  gem: StoneGolem Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Gem

Hatchling Soul Power:
  slot: weapon
  gem: Hatchling Soul Gem
  power:
    value: [150, 200, 250, 300, 350]
  unique: Gem

Goblin Soul Power:
  slot: weapon
  gem: Goblin Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Gem

Deathsquito Soul Power:
  slot: weapon
  gem: Deathsquito Soul Gem
  power:
    value: [5, 7, 9, 11, 13]
  unique: Gem

Lox Soul Power:
  slot: [head, chest, legs, weapon, cloak]
  gem: Lox Soul Gem
  power:
    value: [10, 20, 30, 40, 50]
  unique: Gem

GoblinBrute Soul Power:
  slot: [head, chest, legs, cloak]
  gem: GoblinBrute Soul Gem
  power:
    value: [15, 20, 25, 30, 35]
  unique: Gem

GoblinShaman Soul Power:
  slot: [head, chest, legs, cloak]
  gem: GoblinShaman Soul Gem
  power:
    value: [10, 15, 20, 25, 30]
  unique: Gem

TarBlob Soul Power:
  slot: all
  gem: TarBlob Soul Gem
  power:
    duration: [6, 7, 8, 9, 10]
    cooldown: [32, 30, 28, 26, 24]
  unique: Gem

Surtling Soul Power:
  slot: weapon
  gem: Surtling Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
    cooldown: [60, 50, 45, 40, 35]
  unique: Gem

Serpent Soul Power:
  slot: shield
  gem: Serpent Soul Gem
  power:
    value: [25, 50, 75, 100, 125]
  unique: Gem

Hare Soul Power:
  slot: weapon
  gem: Hare Soul Gem
  power:
    value: [4, 8, 12, 16, 20]
  unique: None

Dverger Soul Power:
  slot: all
  gem: Dverger Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Item

Dverger Fire Mage Soul Power:
  slot: all
  gem: Dverger Fire Mage Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Item

Dverger Blood Mage Soul Power:
  slot: all
  gem: Dverger Blood Mage Soul Gem
  power:
    value: [10, 15, 20, 25, 30]
  unique: Item

Dverger Ice Mage Soul Power:
  slot: all
  gem: Dverger Ice Mage Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Item

Tick Soul Power:
  slot: all
  gem: Tick Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Item

Gjall Soul Power:
  slot: all
  gem: Gjall Soul Gem
  power:
    value: [3, 6, 9, 12, 15]
  unique: Item

Seeker Soul Power:
  slot: all
  gem: Seeker Soul Gem
  power:
    value: [5, 10, 15, 20, 25]
  unique: Item

Seeker Brute Soul Power:
  slot: all
  gem: Seeker Brute Soul Gem
  power:
    value: 1
  unique: Gem

Eikthyr Soul Power:
  slot: all
  gem: Eikthyr Soul Gem
  power:
    value: [1, 2, 3, 4, 5]
  unique: Gem

Elder Soul Power:
  slot: all
  gem: Elder Soul Gem
  power:
    value: [25, 24, 23, 22, 21]
  unique: Gem

Bonemass Soul Power:
  slot: all
  gem: Bonemass Soul Gem
  power:
    value: 40
  unique: Gem

Moder Soul Power:
  slot: all
  gem: Moder Soul Gem
  power:
    value: [20, 30, 40, 50, 60]
  unique: Gem

Yagluth Soul Power:
  slot: all
  gem: Yagluth Soul Gem
  power:
    value: 40
  unique: Gem

The Queen Soul Power:
  slot: all
  gem: The Queen Soul Gem
  power:
    value: 20
  unique: Gem


```

</details>

<details>
  <summary><b><span style="color:aqua;font-weight:200;font-size:20px">
    Patchnotes
</span></b></summary>

| Version | Changes                                                                                                                                                                                                                                                                                                                               |
|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1.0     | Mod Released                                                                                                                                                                                                                                                                                                                          |
| 1.1     | Soulcatcher skill now also increases soul spawn chance (0.05% per Soulcatcher level). <br/>Fixed gem balance                                                                                                                                                                                                                          |
| 1.2     | Added new gems: Wraith Gem and Cultist Gem. Fixed few visual effects                                                                                                                                                                                                                                                                  |
| 1.3     | New Gem Tier added: Godlike<br/>Gem icons changed depending on gem tier<br/>Added new time formatting for soul altar craft duration<br/>Lantern now attached as back item same as Hammer (Tool attachment).                                                                                                                           |
| 1.4     | Added new gems: Goblin Brute and Goblin Shaman<br/>Added new mechanic - Lantern Souls combinating : you can now choose one lantern and click with it on another lantern in order to add souls from source to target (takes Fee %, configurable in config)                                                                             |
| 1.5     | New Gem Tier added: Odin's Wrath<br/>Fixed Wraith gem teleport issues                                                                                                                                                                                                                                                                 |
| 1.6     | New Gem Added: Abomination Gem.<br/>Bonemass / Yagluth gem nerfed<br/>Added new Building Piece: Soul Platform<br/>Removed Soul Altar crafting station requirement<br/>Moved Soul Altar into "Crafting" section                                                                                                                        |
| 1.7     | New Gem Added: Leech Gem.<br/>Added new Jewelry: Soul Necklace and Soul Ring.<br/>Bugfixes                                                                                                                                                                                                                                            |
| 1.8     | New Gem Added: Blob Tar Gem.<br/>Gems icon background image changed<br/>Bugfixes<br/>Lantern can now be attached to Item Stand                                                                                                                                                                                                        |
| 1.9     | New Gem Added: Ulv Gem<br/>Some small fixes                                                                                                                                                                                                                                                                                           |
| 2.0     | Ulv Gem now also deals damage to enemy on activation<br/>Small balance changes                                                                                                                                                                                                                                                        |
| 2.1     | Fixed Soul Altar Station gem dupe bug                                                                                                                                                                                                                                                                                                 |
| 2.2     | New Building added: Soul Bowl: allows you to use Deer, Stone Golem, Fenring, Boar, Greydwarf souls and get AoE buffs for players and plants.<br/>Added VFX's for Eikthyr / Bonemass / Yagluth gems<br/>Balance changes (Please remove Jewelcrafting.Sockets_Soulcatcher_KG_JC_Additions.yml in config folder so it will be recreated) |
| 2.2.1   | Removed useless print when Deer Soul Bowl (plant speed) activated                                                                                                                                                                                                                                                                     |
| 2.2.2   | Removed BlobGem sound effect<br/>Fixed Blob Gem sometimes not removing fall damage<br/>Changed Ulv gem damage type from true damage to lightning                                                                                                                                                                                      |
| 2.3     | Added in-game dynamic sprite caching, which will speed up mod load time in x5-6 times<br/>Added new config options for each gem: IsCraftable<br/>Now Possible Convertions UI will automatically update in-game without game restart                                                                                                   |
| 2.4     | New Gem Added: Greydwarf Elite Gem<br/>Now Soul Altar also displays icon of gem its crafting                                                                                                                                                                                                                                          |
| 2.5     | New Gem Added: Greydwarf Shaman Gem<br/>Small balance changes (please remove .yml file or copy-paste YAML settings from this page)                                                                                                                                                                                                    |
| 2.6     | New (admin) item added: Cursed Doll. (Prefab id: Soulcatcher_ValhallaItem)<br/>Cursed Doll is used each item socket try and will prevent your item from destruction on fail<br/>You cannot get this item in game, so its for admin to decide where to add this prefab (Trader, Quests or something else)                              |
| 2.7     | Soul Bowls now cannot be placed inside each other<br/>Bugfixes                                                                                                                                                                                                                                                                        |
| 2.8     | Now you can see Soul Bowl range when placing it<br/>You cannot build structures on top of Soulcatcher Piece objects anymores                                                                                                                                                                                                          |
| 2.9     | Fixed a bug where Soul Altar animation would stop and require player to rejoin area in order to finish crafting                                                                                                                                                                                                                       |
| 3.0     | Fixed Soul Bowl + Soul Platform causing small stutters                                                                                                                                                                                                                                                                                |
| 3.1     | Updated for new Valheim version                                                                                                                                                                                                                                                                                                       |
| 4.0     | Updated for Mistlands<br/>Added gems for each new mistlands creatures<br/>Now you have a chance to spawn "gold" soul that will give you double amount                                                                                                                                                                                 |
| 4.1     | Updated ItemManager so it doesn't throw errors and such...                                                                                                                                                                                                                                                                            |
| 4.2     | Fixed Blob Gem not working and throwing error making character stuck in air                                                                                                                                                                                                                                                           |
| 4.3     | Fixed Tar Blob Gem<br/>Updated ItemDataManager to latest                                                                                                                                                                                                                                                                              |
| 4.4     | Updated for new Valheim patch                                                                                                                                                                                                                                                                                                         |
</details>

**For Questions or Comments, find KG#7777 ![https://i.imgur.com/CPYNjXV.png](https://i.imgur.com/CPYNjXV.png) in the Odin Plus Team Discord:
[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/5gXNxNkUBt)**