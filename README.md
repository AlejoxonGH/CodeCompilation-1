# CODE COLLECTION I



itch.io link: https://alejoxon.itch.io/code-collection-1



ESPAÑOL / SPANISH 🇪🇸:



Esta es una recopilación de proyectos elaborados durante la cursada de mi carrera.


Se compone de 6 proyectos:

-Runner: Un auto-runner que aplica algoritmos básicos del desarrollo de juegos que son vitales para respetar la filosofía de diseño de software conocida como "S.O.L.I.D.". Dichos algoritmos son: MVC (para el player), Factory y Object Pool (para las balas y obsaculos), Screen Manager (para el menú de pausa), y Strategy (para los tipos de enemigo aéreo).

-BulletHell: Utiliza varias funciones de LinQ, librería de C# para el manejo de colecciones (Select, Where, Any, Zip, OfType, First, SkipWhile, Concat, SelectMany, OrderBy, ToArray). También usa un método de tipo generador para crear colecciones, y Tuplas para agrupar propiedades de distinto tipo.

-LotOfBoids: Un programa que corre por su cuenta. En este hay un cazador que persigue a un enorme número de presas (boids). El cazador se puede cansar, y si no ve a ninguna de las presas, patrulla; las presas se mueven en grupo, evaden al cazador y buscan pildoras de comida. El cazador está programado con un maquina de estados que funciona por eventos (Event FSM), las presas utilizan Flocking, todos esquivan obstaculos (obstacle avoiding), detectan la distancia al resto mediante una grilla (grid) y se mueven usando Steering.

-Maze: Un laberinto pequeño que tienes que atravesar sin que te atrapen los enemigos. Los enemigos utilizan Pathfinding (A*) para ir hacia el jugador y Field Of View para detectarlo. Al ser detectado, todos los enemigos son alertados de la posición del jugador.

-Red vs Blue: Dos equipos (Rojo y Azul) deben llegar a un objetivo verde guiados por los clicks del mouse. Cada equipo tiene un Lider y 5 minions. Los minions utilizan todo el tiempo Leader Following para mantenerse cerca de su lider, los líderes van hacia el objetivo marcado por el click del mouse correspondiente y si no hay ojetivo se quedan quietos. Al detectar a un miembro del bando enemigo, tanto Lider como minions lo atacarán, y si tienen poca vida restante huirán a una zona segura. Todas las entidades utilizan una maquina de estados (Event FSM), esquivan obstaculos, detectan enemigos mediante FOV, deciden un camino con "Theta*", y se mueven usando Steering.

-Sudoku Solver: Simplemente un generador y resolvedor de tableros de sudoku que funciona utilizando recursión. 
Ningún arte usado es de mi propiedad.



Controles (generales):


WASD - ESPACIO - Click Izq. - Click Der. - Esc



===============================================================



ENGLISH / INGLÉS 🇬🇧:



This is a compilation of projects developed during my video game career studies.


It's composed of 6 projects:

-Runner: An auto-runner that applies basic algorythms for game developing which are vital for respecting the software design philosophy known as "S.O.L.I.D.". Said algorythms are: MVC (for the player), Factory and Object Pool (for bullets and obstacles), Screen Manager (for the pause menu), and Strategy (for the aereal enemy types).

-BulletHell: It utilizes various funtions from LinQ, a C# library for collection managing (Select, Where, Any, Zip, OfType, First, SkipWhile, Concat, SelectMany, OrderBy, ToArray). Also uses a Generator method to create collections, and Tuples to group properties of different type.

-LotOfBoids: A game that runs on it's own. In which theres a Hunter that pursuits an enormous number of preys (Boids). The hunter can grow tired and rest, if it doesn't see any preys, it patrols; the preys move in packs, they evade the hunter and search for food pills. The hunter is coded using a Finite State Machine (FSM) that works through Events, the preys use the Flocking algorythm, all entities Avoid Obstacles, detect the distance between each other through a Grid, and move themselves using a Steering system.

-Maze: A tiny maze which you have to traverse as a square without being reached by enemies. The enemies use Pathfinding (A*) to reach the player and "Field Of View" detection to locate them. When detected, all enemies are alerted of the player's position.

-Red vs Blue: Two teams (Red and Blue) must reach to a green goal guided by the clicks of your mouse. Each team has a leader and 5 minions. The minions use the "Leader Following" behaviour all the time they aren't attacking or fleeing to keep close to their leader, while leaders move to the goal marked by the matching mouse click, if there's no goal they stand still. When detecting a member of the enemy team, both leaders and minions will attack them, and if they're low on health points they'll flee to a safe zone. All entities use an Event State Machine, Obstacle Avoiding, detect enemies via FOV, find their shortest path through "Theta*", and move using Steering.

-Sudoku Solver: Simply a sudoku generator and solver that works using recursion.
None of the used art is of my property.



Controls (generic):


WASD - SPACE - Left Click - Right Click - Esc 
