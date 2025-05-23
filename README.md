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
* Buried Graveyard mechanics: 'special character' (and maybe 'reset switch' though I think it's just anti-softlock and may never be part of a puzzle.)
* I think 'Process Turn 0 Switch Hitting' should be a checkbox. (My current assumptions are at the start of the puzzle: Overlapping actors frag each other (special character beats character, not sure how rainbow statues/stars overlapping other actors interact as I've never managed to cause it), actors in the wrong light/dark die, actors depress switches, then a (check won/kill actors/depress switches) loop until nothing changes. (There may be an additional 'check won' before the first 'actors depress switches', I am not sure.) (If it's off, it will skip this step.) (Right now I have a partial implementation that makes rainbow statues hold down switches.)
* The win condition is not 'RGBA', it's 'all lightbulbs in the puzzles are on'. For example, a level with only CMY lamps can be won with rgb or RGB. Workaround: Just try solving the level with the initial RGB state flipped and you should get the other set of solutions.
* It would be cool to have a feature where you can describe the overworld, one or more puzzles in that overworld and an end state you need (puzzles solved with X deaths, special character moved to position Y, etc) but it sounds very difficult to spec out, implement and optimize.
* It might be fun to add custom elements, like for example 'lightbulb that is always on' or any sort of logic gate on RGBA state.