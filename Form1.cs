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
            //GoButton_Click(this, new EventArgs());
#endif
        }

        static int[][] directions = new int[][] { [-1, 0], [0, 1], [0, -1], [1, 0] };
        static char[] directionLabels = new char[] { 'w', 's', 'n', 'e' };

        private void GoButton_Click(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";

            try
            {
                var initialState = new GameState(LevelDataTextBox.Text);

                //At this point, rainbow statues spawned in should press buttons. This may instantly win the puzzle.
                //TODO: Rainbow statues can hit switches turn 0, cause an instant win and kill actors before a turn is taken.
                //But I'm not sure if they go before or after the first win check,
                //and I'm not sure if their switch hitting is simultaneous with, before, or after normal actors also starting on switches
                //(and if it's not simultaneous, what steps happen in between and how many times).
                //Also, I'm not sure what happens if a rainbow statue starts on top of a star or actor.
                //TODO: Mildly duplicated logic.
                for (int i = 0; i < initialState.Actors.Length; ++i)
                {
                    if (initialState.Actors[i].type == ActorTypes.RainbowStatue)
                    {
                        initialState.PressButton(initialState.Actors[i].x, initialState.Actors[i].y);
                    }
                }
                var somethingHappened = true;
                var won = false;
                while (somethingHappened)
                {
                    somethingHappened = false;
                    won = won || (initialState.Red && initialState.Blue && initialState.Green && initialState.Anti);
                    somethingHappened = initialState.LightAllActorsAndUnpressButtons();
                }
                if (won)
                {
                    OutputTextBox.Text = "It's already solved!";
                    return;
                }

                var mobileActorCount = 0;
                for (int i = 0; i < initialState.Actors.Length; ++i)
                {
                    if (initialState.Actors[i].IsMobile())
                    {
                        mobileActorCount += 1;
                    }
                }
                for (var i = 0; i <= mobileActorCount; ++i)
                {
                    var result = Solve(initialState, i);
                    if (result)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = ex.ToString();
            }
        }

        bool Solve(GameState initialState, int deathTolerance)
        {
            var outputText = "";

            Dictionary<string, GameState> seenStates = new Dictionary<string, GameState>();
            List<GameState> novelStates = new List<GameState>();

            //0) put the initial state in old moves and new moves.
            seenStates[initialState.ToString()] = initialState;
            novelStates.Add(initialState);

            //1) pop a novel state. (if we can't, we're done.)
            while (novelStates.Count > 0)
            {
                var currentState = novelStates[0];
                novelStates.RemoveAt(0);

                //2) in a loop, try all possible moves. (to try a move, pick an alive mobile actor and a cardinal direction and attempt to move in that direction.)
                for (int i_old = 0; i_old < currentState.Actors.Length; ++i_old)
                {
                    var who_moved_dummy = currentState.WhoMoved >= 0 ? currentState.WhoMoved : 0;
                    var i = (i_old + who_moved_dummy) % currentState.Actors.Length;
                    var actor = currentState.Actors[i];
                    if (!(actor.lives >= 0 && actor.IsMobile()))
                    {
                        continue;
                    }

                    for (int d = 0; d < directions.Length; ++d)
                    {
                        var direction = directions[d];
                        var newPosition = new int[] { actor.x + direction[0], actor.y + direction[1] };

                        //3a) if there is a non-star actor or obstacle in that direction or it's oob, nevermind.
                        if (!currentState.inBounds(newPosition[0], newPosition[1]))
                        {
                            continue;
                        }
                        var destinationTile = currentState.Tiles[newPosition[0], newPosition[1]];
                        if (destinationTile is Obstacle)
                        {
                            continue;
                        }
                        var actorBlocked = false;
                        foreach (var otherActor in currentState.Actors)
                        {
                            if (otherActor.lives >= 0 && otherActor.type != ActorTypes.Star && otherActor.x == newPosition[0] && otherActor.y == newPosition[1])
                            {
                                actorBlocked = true;
                                break;
                            }
                        }
                        if (actorBlocked)
                        {
                            continue;
                        }

                        //3b) otherwise, move the actor to that tile and collect the star there. calculate lighting. if THAT actor loses a live, process it.
                        //unpress the old button. if the actor is still alive, press the new button.
                        //(!) check won state, calculate lighting, attempt to kill all actors (which unpresses buttons). if any died, GOTO (!).
                        var newState = (GameState)currentState.Clone();
                        var old_x = actor.x;
                        var old_y = actor.y;
                        //because Actor is a struct, I have to always access it inside of the array.
                        var tileUnderActor = newState.Tiles[actor.x, actor.y];
                        newState.Actors[i].x = newPosition[0];
                        newState.Actors[i].y = newPosition[1];
                        newState.ActorCollectsStar(i);
                        newState.LightActor(i);
                        newState.UnpressButton(old_x, old_y);
                        if (newState.Actors[i].lives >= 0)
                        {
                            newState.PressButton(newPosition[0], newPosition[1]);
                        }
                        var somethingHappened = true;
                        var won = false;
                        while (somethingHappened)
                        {
                            somethingHappened = false;
                            won = won || (newState.Red && newState.Blue && newState.Green && newState.Anti);
                            somethingHappened = newState.LightAllActorsAndUnpressButtons();
                        }

                        //3c) if we still have enough lumins+shades alive, check if it's in the dictionary of old moves. if it is, nevermind.
                        //if it isn't, add it to old moves and new moves.
                        var actorsDied = 0;
                        foreach (var newActor in newState.Actors)
                        {
                            if (newActor.IsMobile() && newActor.lives < 0)
                            {
                                actorsDied += 1;
                            }
                        }
                        if (actorsDied > deathTolerance)
                        {
                            continue;
                        }

                        var key = newState.ToString();
                        if (seenStates.ContainsKey(key))
                        {
                            continue;
                        }
                        newState.DescendedFrom = currentState;
                        newState.WhoMoved = i;
                        newState.HowMoved = directionLabels[d];
                        seenStates[key] = newState;
                        //Check goals.
                        if (won)
                        {
                            var goals_satisfied = true;
                            foreach (var goal in newState.Goals.Keys)
                            {
                                var goal_value = newState.Goals[goal];
                                if (newState.Actors.Any(a => a.x == goal.Item1 && a.y == goal.Item2 && a.IsMobile() && a.lives >= goal_value))
                                {
                                    continue;
                                }
                                else
                                {
                                    goals_satisfied = false;
                                    break;
                                }
                            }
                            if (!goals_satisfied)
                            {
                                continue; //Should be in seen states but not novel states since we cannot make more moves from it.
                            }
                        }
                        //3d) if it's won (and goals were satisfied), report replay to the user.
                        if (won)
                        {
                            var result = "";
                            var replayState = newState;
                            var lastWhoMoved = -1;
                            List<GameState> replayParts = new List<GameState>();
                            while (replayState != null && replayState.WhoMoved >= 0)
                            {
                                replayParts.Insert(0, replayState);
                                replayState = replayState.DescendedFrom;
                            }
                            foreach (var replayPart in replayParts)
                            {
                                var newPart = "";
                                if (lastWhoMoved != replayPart.WhoMoved)
                                {
                                    lastWhoMoved = replayPart.WhoMoved;
                                    newPart += lastWhoMoved.ToString();
                                }
                                newPart += replayPart.HowMoved.ToString();
                                result += newPart;
                            }
                            result += Environment.NewLine + "Surviving actors:" + Environment.NewLine;
                            foreach (var survivingActor in newState.Actors.Where(x => x.IsMobile() && x.lives >= 0))
                            {
                                result += (survivingActor.type == ActorTypes.Lumin ? "Lumin " : "Shade ")
                                    + survivingActor.lives + " at (" + survivingActor.x + "," + survivingActor.y + ")"
                                    + " standing on " + newState.Tiles[survivingActor.x, survivingActor.y].PrettyPrint()
                                    + Environment.NewLine;
                            }
                            if (OutputTextBox.Text.Contains(result) || outputText.Contains(result))
                            {
                                continue;
                            }
                            outputText += result + Environment.NewLine;
                            if (!AllDistinctSolutionsCheckBox.Checked)
                            {
                                OutputTextBox.Text += outputText;
                                return true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //3e) If it wasn't won, we can add it to novelStates and continue searching from here.
                        novelStates.Add(newState);
                    }
                }
            }
            if (outputText == "")
            {
                outputText = deathTolerance + " deaths: Checked " + seenStates.Count + " states and found no new solution, sorry." + Environment.NewLine + Environment.NewLine;
            }
            OutputTextBox.Text += outputText;
            return false;
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
            RainbowStatue,
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

            public virtual string PrettyPrint()
            {
                return "Ground";
            }
        }

        public class LSButton : Tile
        {
            public LSButton(Colours color, ButtonTypes type)
            {
                this.color = color;
                this.type = type;
            }
            public Colours color;
            public ButtonTypes type;

            public override string ToString()
            {
                return color.ToString() + type.ToString();
            }

            public override string PrettyPrint()
            {
                var result = color.ToString() + " ";
                switch (type)
                {
                    case ButtonTypes.Normal: result += "Button"; break;
                    case ButtonTypes.Once: result += "Once Button"; break;
                    case ButtonTypes.Hold: result += "Hold Button"; break;
                }
                return result;
            }
        }

        public class Obstacle : Tile
        {
            public Obstacle(LampColours lampColor)
            {
                this.lampColor = lampColor;
            }
            public LampColours lampColor;

            public override string ToString()
            {
                return lampColor.ToString();
            }

            public override string PrettyPrint()
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
            public ActorTypes type;
            public int x;
            public int y;
            public int lives;

            public bool IsMobile()
            {
                return type == ActorTypes.Lumin || type == ActorTypes.Shade;
            }
            public override string ToString()
            {
                if (lives < 0)
                {
                    return type.ToString() + lives.ToString();
                }
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
            public Dictionary<(int, int), int> Goals;
            public bool Red;
            public bool Green;
            public bool Blue;
            public bool Anti;
            public GameState? DescendedFrom = null;
            public int WhoMoved = -1;
            public char HowMoved = '\0';

            public GameState(GameState old)
            {
                this.Red = old.Red;
                this.Green = old.Green;
                this.Blue = old.Blue;
                this.Anti = old.Anti;
                this.TouchedOnces = new HashSet<(int, int)>(old.TouchedOnces);
                this.Actors = (Actor[])old.Actors.Clone();
                this.Tiles = old.Tiles;
                this.Goals = old.Goals;
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
                //then check for optional GOALS
                Goals = new Dictionary<(int, int), int>();
                if (lines.Length > (4 + dimensions[1]*2) && lines[4 + dimensions[1] * 2].StartsWith("GOALS"))
                {
                    for (int i = 0; i < dimensions[1]; ++i)
                    {
                        var goals = lines[5 + dimensions[1]*2 + i].ToCharArray();
                        for (int j = 0; j < dimensions[0]; ++j)
                        {
                            var goal = goals[j];
                            if (goal != '.')
                            {
                                Goals[(j, i)] = (int)(goal - '0');
                            }
                        }
                    }
                }

                Actors = tempActors.ToArray();
                TouchedOnces = new HashSet<(int, int)>();
            }

            public object Clone()
            {
                return new GameState(this);
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
                    case 'W': return new Obstacle(LampColours.White);
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
                    case 'q': return new Actor(ActorTypes.RainbowStatue, x, y, 0);
                    case 'w': return new Actor(ActorTypes.RainbowStatue, x, y, 1);
                    case 'e': return new Actor(ActorTypes.RainbowStatue, x, y, 2);
                    case 'r': return new Actor(ActorTypes.RainbowStatue, x, y, 3);
                    case 't': return new Actor(ActorTypes.RainbowStatue, x, y, 4);
                    case 'y': return new Actor(ActorTypes.RainbowStatue, x, y, 5);
                    case 'u': return new Actor(ActorTypes.RainbowStatue, x, y, 6);
                    case 'i': return new Actor(ActorTypes.RainbowStatue, x, y, 7);
                    case 'o': return new Actor(ActorTypes.RainbowStatue, x, y, 8);
                    case 'p': return new Actor(ActorTypes.RainbowStatue, x, y, 9);
                    default: throw new InvalidOperationException("Unrecognized actor: " + c);
                }
            }

            public bool ActorCollectsStar(int i)
            {
                for (int j = 0; j < Actors.Length; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (Actors[j].type == ActorTypes.Star && Actors[j].lives >= 0 && Actors[i].x == Actors[j].x && Actors[i].y == Actors[j].y)
                    {
                        //ig stars with extra lives give them all at once?
                        //it's either that or it loses one life and you can step on it again but that seems less natural
                        Actors[i].lives += (Actors[j].lives + 1);
                        Actors[j].lives = -1;
                        return true;
                    }
                }
                return false;
            }

            public bool LightActor(int i)
            {
                if (Actors[i].lives < 0 || !Actors[i].IsMobile())
                {
                    return false;
                }

                //1-2) First, calculate if the actor i is in light or darkness:
                //1) If Anti, check the surrounding 3x3 for Anti. If yes, we're in darkness.
                //2) Check the surrounding 3x3 for non-Anti. For each lamp found, if it's on, we're in light.
                //3) If we're a lumin and we're in light, return false.
                //4) If we're a shade and we're in darkness, return false.
                //5) If we have lives, subtract one, flip lumin/shade and return false.
                //6) If we have no lives, die (-1) and return true.
                var lit = false;

                for (var x = Actors[i].x - 1; x <= Actors[i].x + 1; ++x)
                {
                    for (var y = Actors[i].y - 1; y <= Actors[i].y + 1; ++y)
                    {
                        if (!inBounds(x, y))
                        {
                            continue;
                        }
                        var tile = Tiles[x, y];
                        if (tile is Obstacle obstacle)
                        {
                            //anti-lamps actually anti-light and it takes precedence
                            if (obstacle.lampColor == LampColours.Anti)
                            {
                                if (LampLit(obstacle.lampColor))
                                {
                                    lit = false;
                                    goto ready;
                                }
                            }
                            else
                            {
                                lit |= LampLit(obstacle.lampColor);
                            }
                        }
                    }
                }

                //also check for rainbow statues (note that rainbow is overriden by anti-light)
                foreach (var statue in Actors.Where(x => x.type == ActorTypes.RainbowStatue))
                {
                    var kingsDistance = Math.Max(Math.Abs(statue.x - Actors[i].x), Math.Abs(statue.y - Actors[i].y));
                    if ((statue.lives + 1) >= kingsDistance)
                    {
                        return false;
                    }
                }

                ready:

                //NEATNESS: I always hate writing 'doubled code' like this
                if (Actors[i].type == ActorTypes.Lumin)
                {
                    if (lit)
                    {
                        return false;
                    }
                    Actors[i].lives -= 1;
                    if (Actors[i].lives >= 0)
                    {
                        Actors[i].type = ActorTypes.Shade;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (!lit)
                    {
                        return false;
                    }
                    Actors[i].lives -= 1;
                    if (Actors[i].lives >= 0)
                    {
                        Actors[i].type = ActorTypes.Lumin;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            bool LampLit(LampColours color)
            {
                switch (color)
                {
                    case LampColours.Red: return Red;
                    case LampColours.Green: return Green;
                    case LampColours.Blue: return Blue;
                    case LampColours.Anti: return !Anti;
                    case LampColours.Yellow: return Red == Green;
                    case LampColours.Cyan: return Green == Blue;
                    case LampColours.Magenta: return Blue == Red;
                    case LampColours.White:
                    {
                        //NEATNESS: is there a more elegant way to do this?
                        if (Red)
                        {
                            return Green == Blue;
                        }
                        else
                        {
                            return Green != Blue;
                        }
                    }
                    default: return false;
                }
            }

            void ActuateButton(Colours color)
            {
                switch (color)
                {
                    case Colours.Red: Red = !Red; break;
                    case Colours.Green: Green = !Green; break;
                    case Colours.Blue: Blue = !Blue; break;
                    case Colours.Anti: Anti = !Anti; break;
                }
            }

            public bool UnpressButton(int x, int y)
            {
                var tile = Tiles[x, y];
                if (tile is LSButton b && b.type == ButtonTypes.Hold)
                {
                    ActuateButton(b.color);
                    return true;
                }
                return false;
            }

            public bool PressButton(int x, int y)
            {
                var tile = Tiles[x, y];
                if (tile is LSButton b)
                {
                    switch (b.type)
                    {
                        case ButtonTypes.Normal:
                            ActuateButton(b.color);
                            return true;
                        case ButtonTypes.Hold:
                            ActuateButton(b.color);
                            return true;
                        case ButtonTypes.Once:
                            if (!TouchedOnces.Contains((x, y)))
                            {
                                TouchedOnces.Add((x, y));
                                ActuateButton(b.color);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                    }
                }
                return false;
            }

            public bool LightAllActorsAndUnpressButtons()
            {
                var deadActorIndexes = new List<int>();
                for (var i = 0; i < Actors.Length; ++i)
                {
                    if (LightActor(i))
                    {
                        deadActorIndexes.Add(i);
                    }
                }
                foreach (var index in deadActorIndexes)
                {
                    UnpressButton(Actors[index].x, Actors[index].y);
                }
                return deadActorIndexes.Count > 0;
            }

            public bool inBounds(int x, int y)
            {
                return x >= 0 && y >= 0 && x < Tiles.GetLength(0) && y < Tiles.GetLength(1);
            }

            public override string ToString()
            {
                var result = Red.ToString() + Green.ToString() + Blue.ToString() + Anti.ToString();
                var sortedActors = Actors.ToList();
                sortedActors.Sort();
                foreach (var actor in sortedActors)
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
