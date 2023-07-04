# Plane Tactics: A Turn-based Plane Game

## Introduction

![alt text](Screenshots/title.png?raw=true)
Plane Tactics is an experimental turn-based game about aircraft battles. The game puts the player in command of a plane squad, of which contains various types of aircrafts, to fight against enemy aircrafts.  
This game is an experiment to explore the concept of gridless chess-like tatics games, and the plane theme add a further concept of forced movement that depends of previous movement.

## Gameplay Design

![alt text](Screenshots/sampleGameplay.png?raw=true)
The game takes place on a gridless map, where each player/cpu take turns to control their units. For each unit, the player chooses where to go, and either attacks other units or skip attacking. All units must make their move before ending each turn.  

Movement for each unit of the current turn has an effect on the movement of next turn. For example, a unit turning hard right the current turn will not be able to turn hard opposite next turn, and a unit moving at maximum speed the current turn will not be able to slow down to minumim speed next turn.

Attack must only be done after a unit is moved. An attack consist of multiple hits, with each hit may hit or miss the target. The chance of hitting is determined by how accurate a unit is pointed to the target, as well as the distance between the unit and the target.

Each player's plane squad consist of different types of aircraft. The type gives the aircraft different capabilities and weaknesses. The list of aircraft types are as follows:  

| Aircraft Type |                                            Description | 
|--------------:|--------------------------------------------------------|
|        Biplane|        Slow but maneuvable. Has weak attck and defence.|
|  Light Fighter|  All-rounder. Has average movement, attack and defence.|
|  Heavy Fighter|    Has strong attack and defence, but is very sluggish.|
|    Jet Fighter|  Fast but hard to turn. Has average attack and defence.|

## UI

The following section describes the every UI on each stages of the game.

![alt text](Screenshots/selectionUI.png?raw=true)
Unit Selection UI
| Number |               Description |
|-------:|---------------------------|
|       1|    Player's Squad Insignia|
|       2|               Current Turn|
|       3|               Players Unit|
|       4|    Unit's Height Indicator|
|       5|                    Minimap|
|       6| Select Next Available Unit|
|       7|       Proceed to Next Turn|

![alt text](Screenshots/movementUI.png?raw=true)
Movement UI
| Number |                                                       Description |
|-------:|-------------------------------------------------------------------|
|       1| Current Movement Selection (Solid: Available, Outline: Capability)|
|       2|     Next Movement Prediction (Inner: Available, Outer: Capability)|
|       3|                                                        Attack Area|
|       4|                                                Movement Prediction|
|       5|                                            Move to Higher Position|
|       6|                                             Move to Lower Position|
|       7|                                       Cancel the Current Selection|

![alt text](Screenshots/attackUI.png?raw=true)
Attack UI
| Number |                               Description |
|-------:|-------------------------------------------|
|       1| Enemy Unit Selection and Damage Calcuation|
|       2|                     Enemy Unit Information|
|       3|             Enemy Unit Movement Prediction|
|       4|       Skip the Attack Instead of Attacking|
|       5|                  Undo the current movement|


## Discussion and Further Developement Suggestions
Since the game is still in a proof of concept stage, there are still a lot to be done and there are multiple way to improve the gameplay. Here lists a few of the possibilities that may benefit the gameplay.
- Add special movements that differs from the standard movement, that require recharge and/or matching criteria
- Add special attacks that require recharge and/or criteria
- Add enviroment based effect such as fog, air-control etc.
- Add pilots that may affect stats of the aircraft.
- Add ways to spawn new units.
