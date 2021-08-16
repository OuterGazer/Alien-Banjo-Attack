# Alien-Banjo-Attack
A Space-Invaders-like clone created with the XNA Framework as part of Rob Mile's "The Yellow Book" second semester coursework.

The players commands an accordeon able to shoot musical notes in order to fence off the evil banjo attackers spawning on screen.
Player moves the accordion with the arrow keys and shoots with spacebar.
Additional controls are P to pause/unpause and G to save and quit the game (when starting again it will automatically load the saved state).

To install the game simply donwload the zip, extract and run setup.exe.
Afterwards you need to download and install the XNA 4.0 redistributable to get the necessary libraries in your system (included in repository).

The game features all extra-credit implementations:
- Explosion animations
- Sound effects
- "Strum fire": the Deadly Strummer (green banjo) shoots musical notes aimed at the player's current position at the moment of shooting.

I implemented the following as extra from the original assignament:
- Pressing P pauses and unpauses the game.
- Backgound music.
- A limitation to the player shooting. Now the player shoots every fraction of time instead of 1 missile per frame. Enemy spawning has been adjusted accordingly not to overwhelm the player.
