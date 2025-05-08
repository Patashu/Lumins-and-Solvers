# Lumins and Solvers
A [Lumins and Shades](https://store.steampowered.com/app/653020/Lumins_and_Shades/) solver. Made in Visual Studio 2022 as a C# WinForms app.

For personal use, but you can try it if you want.

Example level:

```
Flattened Pyramid - I
RGbA,11x7
TERRAIN
...g.......
..bW#C7.5R.
.C.#.#W..W#
..g.rYr.r..
##.W.#.##M.
.G6W#M#Wb..
....6.b....
ACTORS
...........
..........0
...........
...........
...........
0..........
...........
```

First line is the level name.

Second line is which lamps start on (lower case for off, upper case for on), then a comma, then the dimensions.

```
TERRAIN
. = floor
# = wall
RGBMYCWA = lamps
rgba = normal switches
1234 = (rgba) hold switches
5678 = (rgba) once switches

ACTORS
0123456789 = lumins (starting lives)
)!@#$%^&*( = shades (starting lives)
~ = star
```

Output is a solution in the form of 'swap to this actor, then make these north/east/south/west movements'. Actors are assigned in reading order starting at 0. Optimizes for most lives saved, then lowest number of steps, then somewhat optimizes for least actor switches (not sure how to guarantee this part is optimal but it's good enough).

Meta mechanics aren't supported yet. My plans are to add

1) in terrain or actors (haven't decided yet), you can add rainbow lamps of radius 1-9

2) a third optional section called GOALS, with . for 'don't care', 0123... for 'must have an actor of lives 0123... or higher here'. When the puzzle is won, GOALS is checked, and if it is not satisfied, forbid going to this state.

if I ever get stuck in Night Sky :B

Oh, and I don't support whatever's after Flattened Pyramid yet either because I haven't gotten there yet.

TODO:
* goals and rainbow statues as described above
* The win condition is not 'RGBA', it's 'turn on all the lights'. For example, a level with only CMY lamps can be won with all on or all off.
* Rainbow statues can hit switches turn 0 and kill actors before a turn is taken. But I'm not sure if they go before or after the first win check, and I'm not sure if their switch hitting is simultaneous with, before, or after normal actors also starting on switches (and if it's not simultaneous, what steps happen in between and how many times).
* I'm not sure what happens if a rainbow statue starts on top of a star or actor.