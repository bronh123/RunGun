This game was created for my CMSC425 Game Programing Class at the University of Maryland. 
This was created in collaboration with Matthew Morrison, Dani Eng-Kohn, Ved Kothavade, and Maverick Durant. 
The goal of the assignment was to experience the game development cycle while creating a viable project. 

Run Gun is a first person horde shooter with roguelike elements and an emphasis on freedom of movement. 
We drew inspiration from games likeVampire Survivors, Risk of Rain 2, Ultrakill, Doom Eternal, and Get to the Orange Door.
The game takes place in one of a few skatepark-like arenas, and the objective is to survive for as long as 
possible as waves of enemies spawn and try to reduce the player’s health to zero. Difficulty will scale with time, 
but the player will also be able to obtain upgrades in the form of pickups that increase their stats (currently health, speed,
strength, jump force, and defense) or strengthen their weapons. These stat increases do not remain between deaths, 
and players start with no upgrades each time they begin a run.

For enemy navigation, we used both a built in system with high engine complexity (NavMesh) and a simpler, custom built system for our use (coroutines)
Both serve their respective purposes well, demonstrating that we understand when to use built in systems, when to roll our own, and how to combine them to make the best possible game
Another relatively complex (but necessary) algorithm that came up was one which found a random point on a Mesh, which is used to spawn enemies on the valid ground as defined by the NavMesh for the level
While not necessarily complex, there were some systems that required us to come up with some creative solutions to implement
All enemies inherit from an EnemyStats class which handles universal qualities shared between all enemy types like taking damage, dying, dropping exp, showing damage effects, and shared stats such as health, speed, and contact damage.
We handled upgrades by creating an Upgrade class that stores information for each upgrade such as an upgrade type enum, title, description, stat modifier value, and quality (a modifier that increases the upgrades effect but has a much rarer chance of appearing). Then, in a class called UpgradeDatabase, we generate a list of all possible upgrades and select 3 stat upgrades and occasionally 2 weapon upgrades whenever the player levels up.


Music and Sounds
Player damage sound was taken from Minecraft.
Picking up exp sound was taken from Minecraft.
Enemy death sound was found on Youtube.
Desert music was taken from Risk of Rain 2.
Forest music was taken from Battle Brothers.
Main menu music was made by a friend of the group.
Some elements of the UI (notably the exp bar) were taken from Fzero-GX.
Models
The Assets for the desert level (though we made the layout ourselves).
The water prefab for the forest map
