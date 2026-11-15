# Midnight Pantry

Minigame 6 for GDIM 31.

A cat gets into a pantry at night. Jump between the shelves and kick the red rat. Each kick makes five little rats, so the room gets crowded pretty fast.

## Controls

A/D or the arrow keys to move. Space to jump. Left Shift to kick. R to restart.

## Devlog

### For loops in Update

The whole for loop finishes before that call to Update ends. It does not do one round per frame. For example, the loop that creates five rats makes all five from the same hit. If I put a really slow loop inside Update, the game would have to wait for it, so that frame would lag. To spread a long task across frames, I would need to split the work up or use something like a coroutine.

### Jumping through platforms

While testing, I found that touching the side of a shelf can count as landing. The cat can then jump again even though it is not standing on top of anything. Our collision code only checks whether the object is tagged Ground or Platform. It never checks which side the cat touched, so a side collision also sets isGrounded to true.

There is another limitation: we turn off the cat's whole collider while it moves upward. This lets it go through a platform, but it also ignores other solid objects during that time. A ground check using the contact direction, together with one-way platform collision, would be more reliable. I kept the simple approach from the assignment so this behavior is still easy to test.

## Level and features

The scene is a small pantry with seven shelves at different heights, floorboards, wallpaper, books, plants, a clock, and cupboards. Ground and platforms use separate tagged Tilemaps with TilemapCollider2D components. The Pantry Palette contains the environment tiles used in the level.

Every hit creates five rats, shrinks the red rat, and plays the next explosion color: yellow, red, blue, then yellow again. Small rats rotate around the Z axis so they stay facing the screen. A small counter shows the hits and the next color. R resets the scene.

## Source

Unity starter project, character animations, explosion, and environment sprites supplied with the [GDIM 31 MG6 repository](https://github.com/UCI-GDIM31/MG6).

