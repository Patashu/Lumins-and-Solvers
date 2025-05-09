# Lumins and Solvers
A [Lumins and Shades](https://store.steampowered.com/app/653020/Lumins_and_Shades/) solver. Made in Visual Studio 2022 as a C# WinForms app.

For personal use, but you can try it if you want.

Example levels:

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

Night Sky - F2
rGBA,6x8
TERRAIN
.....2
...B..
.Ar###
...4b.
#..R..
#.##..
#..G.#
......
ACTORS
..0...
.....)
......
......
......
......
......
..~...
GOALS
......
......
......
......
......
......
..0...
......
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
qwertyuiop = rainbow statues (radius 1...10)
~ = star

GOALS (optional)
. = don't care
0123456789 = a lumin or shade with at least that large a number must end here
```

Output is a solution in the form of 'swap to this actor, then make these north/east/south/west movements'. Actors are assigned in reading order starting at 0. Optimizes for most lives saved, then lowest number of steps, then somewhat optimizes for least actor switches (not sure how to guarantee this part is optimal but it's good enough).

TODO:
* Any mechanics beyond Night Sky H.
* The win condition is not 'RGBA', it's 'turn on all the lights' (or more specifically, 'no light exists that is off'). For example, a level with only CMY lamps can be won with all on or all off. Workaround: Just try solving the level with the initial RGB state flipped and you should get the other set of solutions.
* Rainbow statues can hit switches turn 0 and kill actors before a turn is taken. But I'm not sure if they go before or after the first win check, and I'm not sure if their switch hitting is simultaneous with, before, or after normal actors also starting on switches (and if it's not simultaneous, what steps happen in between and how many times). I'm also not sure what happens if a rainbow statue starts on top of a star or actor.
* 'all distinct solutions' mode where it keeps searching after the first solution found