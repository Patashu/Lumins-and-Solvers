using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using static System.Net.Mime.MediaTypeNames;
using static Lumins_and_Solvers.Form1;

namespace Lumins_and_Solvers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            LevelDataTextBox.Text = @"Flattened Pyramid - I
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
...........";
            GoButton_Click(null, new EventArgs());
#endif
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            OutputTextBox.Text = new GameState(LevelDataTextBox.Text).ToString();
        }

        public enum Colours
        {
            Red,
            Green,
            Blue,
            Anti,
        }

        public enum ButtonTypes
        {
            Normal,
            Once,
            Hold,
        }

        public enum ActorTypes
        {
            Lumin,
            Shade,
            Star,
        }

        public enum LampColours
        {
            Wall,
            Red,
            Green,
            Blue,
            Magenta,
            Yellow,
            Cyan,
            White,
            Anti,
        }

        public class Tile
        {
            public Tile()
            {
            }

            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }

            public override string ToString()
            {
                return ".";
            }
        }

        public class LSButton : Tile
        {
            public LSButton(Colours color, ButtonTypes type)
            {
                this.color = color;
                this.type = type;
            }
            Colours color;
            ButtonTypes type;

            public override string ToString()
            {
                return color.ToString() + type.ToString();
            }
        }

        public class Obstacle : Tile
        {
            public Obstacle(LampColours lampColor)
            {
                this.lampColor = lampColor;
            }
            LampColours lampColor;

            public override string ToString()
            {
                return lampColor.ToString();
            }
        }

        public struct Actor : IComparable<Actor>
        {
            public Actor(ActorTypes type, int x, int y, int lives)
            {
                this.type = type;
                this.x = x;
                this.y = y;
                this.lives = lives;
            }
            ActorTypes type;
            int x;
            int y;
            int lives;

            public override string ToString()
            {
                return type.ToString() + x.ToString() + y.ToString() + lives.ToString();
            }

            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }

            int IComparable<Actor>.CompareTo(Actor other)
            {
                if (other.type != this.type)
                {
                    return this.type.CompareTo(other.type);
                }
                if (other.lives != this.lives)
                {
                    return this.lives.CompareTo(other.lives);
                }
                if (other.x != this.x)
                {
                    return this.x.CompareTo(other.x);
                }
                return this.y.CompareTo(other.y);
            }
        }

        public class GameState : ICloneable
        {
            public Tile[,] Tiles;
            public Actor[] Actors;
            public HashSet<(int, int)> TouchedOnces;
            bool Red;
            bool Green;
            bool Blue;
            bool Anti;

            private GameState()
            {

            }

            static Tile CharToTile(char c)
            {
                switch (c)
                {
                    case '.': return new Tile();
                    case '#': return new Obstacle(LampColours.Wall);
                    case 'R': return new Obstacle(LampColours.Red);
                    case 'G': return new Obstacle(LampColours.Green);
                    case 'B': return new Obstacle(LampColours.Blue);
                    case 'C': return new Obstacle(LampColours.Cyan);
                    case 'Y': return new Obstacle(LampColours.Yellow);
                    case 'M': return new Obstacle(LampColours.Magenta);
                    case 'W': return new Obstacle(LampColours.Magenta);
                    case 'A': return new Obstacle(LampColours.Anti);
                    case 'r': return new LSButton(Colours.Red, ButtonTypes.Normal);
                    case 'g': return new LSButton(Colours.Green, ButtonTypes.Normal);
                    case 'b': return new LSButton(Colours.Blue, ButtonTypes.Normal);
                    case 'a': return new LSButton(Colours.Anti, ButtonTypes.Normal);
                    case '1': return new LSButton(Colours.Red, ButtonTypes.Hold);
                    case '2': return new LSButton(Colours.Green, ButtonTypes.Hold);
                    case '3': return new LSButton(Colours.Blue, ButtonTypes.Hold);
                    case '4': return new LSButton(Colours.Anti, ButtonTypes.Hold);
                    case '5': return new LSButton(Colours.Red, ButtonTypes.Once);
                    case '6': return new LSButton(Colours.Green, ButtonTypes.Once);
                    case '7': return new LSButton(Colours.Blue, ButtonTypes.Once);
                    case '8': return new LSButton(Colours.Anti, ButtonTypes.Once);
                    default: throw new InvalidOperationException("Unrecognized tile: " + c);
                }
            }

            static Actor CharToActor(char c, int x, int y)
            {
                switch (c)
                {
                    case '0': return new Actor(ActorTypes.Lumin, x, y, 0);
                    case '1': return new Actor(ActorTypes.Lumin, x, y, 1);
                    case '2': return new Actor(ActorTypes.Lumin, x, y, 2);
                    case '3': return new Actor(ActorTypes.Lumin, x, y, 3);
                    case '4': return new Actor(ActorTypes.Lumin, x, y, 4);
                    case '5': return new Actor(ActorTypes.Lumin, x, y, 5);
                    case '6': return new Actor(ActorTypes.Lumin, x, y, 6);
                    case '7': return new Actor(ActorTypes.Lumin, x, y, 7);
                    case '8': return new Actor(ActorTypes.Lumin, x, y, 8);
                    case '9': return new Actor(ActorTypes.Lumin, x, y, 9);
                    case ')': return new Actor(ActorTypes.Shade, x, y, 0);
                    case '!': return new Actor(ActorTypes.Shade, x, y, 1);
                    case '@': return new Actor(ActorTypes.Shade, x, y, 2);
                    case '#': return new Actor(ActorTypes.Shade, x, y, 3);
                    case '$': return new Actor(ActorTypes.Shade, x, y, 4);
                    case '%': return new Actor(ActorTypes.Shade, x, y, 5);
                    case '^': return new Actor(ActorTypes.Shade, x, y, 6);
                    case '&': return new Actor(ActorTypes.Shade, x, y, 7);
                    case '*': return new Actor(ActorTypes.Shade, x, y, 8);
                    case '(': return new Actor(ActorTypes.Shade, x, y, 9);
                    case '~': return new Actor(ActorTypes.Star, x, y, 0);
                    default: throw new InvalidOperationException("Unrecognized actor: " + c);
                }
            }

            public GameState(string levelData)
            {
                var lines = levelData.Split(new[] { '\n' });
                //ignore lines[0], it's just the name
                var preamble = lines[1].Split(',');
                var initialLights = preamble[0];
                Red = Char.IsUpper(initialLights[0]);
                Green = Char.IsUpper(initialLights[1]);
                Blue = Char.IsUpper(initialLights[2]);
                Anti = Char.IsUpper(initialLights[3]);
                var dimensions = preamble[1].Split('x').Select(x => int.Parse(x)).ToArray();
                Tiles = new Tile[dimensions[0], dimensions[1]];
                //lines[2] is TERRAIN
                for (int i = 0; i < dimensions[1]; ++i)
                {
                    var tiles = lines[3 + i].ToCharArray();
                    for (int j = 0; j < dimensions[0]; ++j)
                    {
                        Tiles[j, i] = CharToTile(tiles[j]);
                    }
                }
                //lines[3]+dimensions[1] is ACTORS
                var tempActors = new List<Actor>();
                for (int i = 0; i < dimensions[1]; ++i)
                {
                    var actors = lines[4 + dimensions[1] + i].ToCharArray();
                    for (int j = 0; j < dimensions[0]; ++j)
                    {
                        var actor = actors[j];
                        if (actor != '.')
                        {
                            tempActors.Add(CharToActor(actor, j, i));
                        }
                    }
                }
                Actors = tempActors.ToArray();
                TouchedOnces = new HashSet<(int, int)>();
            }

            public object Clone()
            {
                var result = new GameState();
                result.Red = Red;
                result.Green = Green;
                result.Blue = Blue;
                result.Anti = Anti;
                result.TouchedOnces = new HashSet<(int, int)>(TouchedOnces);
                result.Actors = (Actor[])Actors.Clone();
                result.Tiles = Tiles;
                return result;
            }

            public override string ToString()
            {
                var result = Red.ToString() + Green.ToString() + Blue.ToString() + Anti.ToString();
                foreach (var actor in Actors)
                {
                    result += actor.ToString();
                }
                foreach (var pos in TouchedOnces)
                {
                    result += pos.ToString();
                }
                return result;
            }
        }
    }
}
