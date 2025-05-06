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

Output is a solution in the form of 'swap to this actor, then make these north/east/south/west movements'. Actors are assigned in reading order starting at 0.

Meta mechanics aren't supported yet and I didn't test hold switches/fallen orchard/anything past flattened pyramid yet.