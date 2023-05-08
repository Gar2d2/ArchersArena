# ArchersArena

## Table of Contents:
1. [Project about](#project-about)
2. [Main menu](#main-menu)
3. [Gameplay](#gameplay)
    - [Movement](#movement)
        - [Mouse and keyboard](#mouse-and-keyboard)
        - [Gamepad](#gamepad)
    - [Mechanics](#mechanics)
        - [Shooting](#shooting)
        - [Moving](#moving)
4. [Ending game](#ending-game)
## Project about:
This game is a 2D single screen multiplayer game that can be played with up to 4 friends on 4 different devices.
Game is splitted between rounds, round ends when there is only one player alive. Each player have four lives, game ends when there is only one player with lives left.
There is possibility to join game alone, but its only for previev purposes, game requires at least two players playing on two different controllers (can be played on mouse and keyboard)
![image](https://user-images.githubusercontent.com/56792313/236643562-6fc5ee2a-914e-4ddb-9e6e-2e558044f7e0.png)


## Main menu

The Main menu is first what we can see when we launch this game. While in menu, players with connected controllers (needs to be connected before lunching game)
can press A to join, player that uses mouse and keyboard needs to press spacebar in the same purpose.
After pressing mentioned buttons, square with color and player indicator should appear in the middle of the screen.
When players are connected (in this case four of them), screen in menu should look like this:
![image](https://user-images.githubusercontent.com/56792313/236643814-dfd3c446-a11e-4590-957e-0f092149ef32.png)
After pressing "Start game", fight begins.

## Gameplay
### Movement
#### Mouse and keyboard
A and D - Walk,

Spacebar - Jump,

Hold left mouse button - Start aiming,

Mouse - Aim,

Release left mouse button - Release arrow


#### Gamepad
Left stick - Walk,

A button - Jump, 

Hold right trigger - Start aiming,

Right stick - Aim,

Release right trigger - Release arrow

## Mechanics
#### Shooting
The main ability, or at least the one which provides victory is the ability to shoot arrows. After pressing fire button, the red indicating arrow will apear. This indicator shows in which direction arrow will fly after releasing the fire button.

![image](https://user-images.githubusercontent.com/56792313/236648178-dbe1c0b4-e636-44d1-b798-c438d43b764d.png)

In order to shoot, player needs to have at least one arrow in the quiver, if doesn't, nothing will happen. Arrows are deadly only before hitting the first target other than jumping platform, after that, they can be picked to a player quiver simply by colliding with them

#### Moving 
Apart from the basic movement which is moving left, right and jumping, player can use jumping platforms to enhance his movement. After stepping on one of those platforms, player will be ejected in the direction platform is pointing. Its worth to mention that same will happen with arrow so ricochets kills are possible.

## Ending game
Each player have fixed amount of health, indicated by his own widget in the top part of the screen.
![image](https://user-images.githubusercontent.com/56792313/236648417-812d28fe-8b14-496a-afdd-b20372267abd.png)

When player dies, he looses a heart. When only one player is alive, round ends. At the beginning of the next round all players with at least one life left gets respawned. When only one player can be respawned, game ends and the winner is determinated.
