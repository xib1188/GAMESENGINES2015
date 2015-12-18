This proyect contains a little game that includes procedural generation.

The aim of the game is to stay alive as long as posible.
You're driving a spaceship inside a fitted slide on where TNT boxes, nuclear drums and help spells are appearing randomly.

Initially two tunnels are created and concatenated. When the spaceship enters inside the second one, the game controller tells to the left tunnel to generate a new tunnel.
This new tunnel will be concatenated with the other and also will be randomly generated. So the game is based in two tunnels that are switching betwen themselves and updating.

The proyect uses particle systems (explosion, spells and menu efects), sounds (to make the game more interesting, collisions/rigidbodies (to detect collisions betwen the ship and objects), mesh generating(vertexs, triangles and uv's).

Algorithms used include a basic path follower, a tunnel generation (generating vertexs and all things necessaries to make a mesh), movement controller.

The movement of the space ship is not very smooth and that is because in order to mantain the spaceship centered on each track inside the tunnel, the path follower algorithm uses a "look at" function and that makes the problem.
I tried to use quaternions slerp functions but because of the calculus decimals it made the spaceship went out the tracks over the time.

The tunnel forms are based on the formulas that are used to make a 3d sphere on the space.
