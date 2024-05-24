using Accord;
using Accord.Genetic;



namespace RubiksCubeGA
{
    // Needed for RotationDirection
    using static RubiksCube;

    public class RubiksCube
    {
        public char[,,] cubeState;
        public readonly char[,,] SOLVED_CUBE =
            {
            { // Top face (White)
                {'W', 'W', 'W'},
                {'W', 'W', 'W'},
                {'W', 'W', 'W'}
            },
            { // Left face (Green)
                {'G', 'G', 'G'},
                {'G', 'G', 'G'},
                {'G', 'G', 'G'}
            },
            { // Front face (Red)
                {'R', 'R', 'R'},
                {'R', 'R', 'R'},
                {'R', 'R', 'R'}
            },
            { // Right face (Blue)
                {'B', 'B', 'B'},
                {'B', 'B', 'B'},
                {'B', 'B', 'B'}
            },
            { // Back face (Orange)
                {'O', 'O', 'O'},
                {'O', 'O', 'O'},
                {'O', 'O', 'O'}
            },
            { // Bottom face (Yellow)
                {'Y', 'Y', 'Y'},
                {'Y', 'Y', 'Y'},
                {'Y', 'Y', 'Y'}
            },
        };

        private readonly char[,,] DEBUG_CUBE =
            {
            { // Top face (White)
                { 'A', 'B', 'C' },
                { 'D', 'E', 'F' },
                { 'G', 'H', 'I' }
            },
            { // Left face (Green)
                { 'J', 'K', 'L' }, // red
                { 'M', 'N', 'O' },
                { 'P', 'Q', 'R' }
            },
            { // Front face (Red)
                { 'S', 'T', 'U' }, // blue
                { 'V', 'W', 'X' },
                { 'Y', 'Z', '0' }

            },
            { // Right face (Blue)
                { '1', '2', '3' }, // orange
                { '4', '5', '6' },
                { '7', '8', '9' }
            },
            { // Back face (Orange)
                { 'a', 'b', 'c' }, // green
                { 'd', 'e', 'f' },
                { 'g', 'h', 'i' }
            },
            { // Bottom face (Yellow)
                { 'j', 'k', 'l' },
                { 'm', 'n', 'o' },
                { 'p', 'q', 'r' }
            },
        };

        public enum RotationDirection
        {
            Clockwise, // Default
            AntiClockwise
        }

        public RubiksCube()
        {
            // Initialise
            cubeState = GetCubeState();
        }

        private char[,,] GetCubeState()
        { // This may need user input of a cube.
            cubeState = SOLVED_CUBE;
            return cubeState;
        }

        private void DisplayTopAndBottomCubeStatesColour(int face, string? contents, char[,,] cube_state)
        {
            string face_name = face == 0 ? "╔═══╦Top╦═══╗" : "╔══Bottom═══╗";
            Console.WriteLine($"                      {face_name}                      ");

            for (int i = 0; i < 3; i++)
            {
                Console.Write($"                      ║ ");
                for (int j = 0; j < 3; j++)
                {
                    DisplayColouredCell(cube_state[face, i, j], contents);
                    Console.Write(" ║ ");
                }
                Console.WriteLine();

                if (i < 2) // Check if it's not the last iteration
                    Console.WriteLine($"                      ╠═══╬═══╬═══╣                      ");
            }

            Console.WriteLine($"                      ╚═══╩═══╩═══╝                      ");
        }

        public void Display(bool colour = true, bool line = true, string? contents = "█", char[,,]? cube_state = null) //
        {
            // Allow for calling using other states
            if (cube_state == null) { cube_state = cubeState; }

            if (colour)
            {
                if (line)
                {
                    Console.WriteLine(" ╔═══╦Top╦═══╗ ╔═══Left════╗ ╔═══Front═══╗ ╔═══Right═══╗ ╔═══Back════╗ ╔══Bottom═══╗ ");

                    for (int row = 0; row < 3; row++)
                    {
                        Console.Write(" ║ ");
                        for (int face = 0; face < 6; face++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                DisplayColouredCell(cube_state[face, row, col], contents);
                                Console.Write(" ║ ");
                            }
                            if (face < 5)
                            { Console.Write("║ "); } // Check if it's not the last face

                        }
                        Console.WriteLine();
                        if (row < 2)
                        { Console.WriteLine(" ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ "); } // Check if it's not the last iteration

                    }
                    Console.WriteLine(" ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ");
                }
                else
                {
                    DisplayTopAndBottomCubeStatesColour(0, contents, cube_state);

                    Console.WriteLine(" ╔═══Left════╗ ╔═══Front═══╗ ╔═══Right═══╗ ╔═══Back════╗ ");

                    for (int row = 0; row < 3; row++)
                    {
                        Console.Write(" ║ ");
                        for (int face = 1; face < 5; face++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                DisplayColouredCell(cube_state[face, row, col], contents);
                                Console.Write(" ║ ");
                            }
                            if (face < 4)
                            { Console.Write("║ "); } // Check if it's not the last face

                        }
                        Console.WriteLine();
                        if (row < 2)
                        { Console.WriteLine(" ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ "); } // Check if it's not the last iteration

                    }
                    Console.WriteLine(" ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ");


                    DisplayTopAndBottomCubeStatesColour(5, contents, cube_state);

                }
            }
            else
            { // Currently no basic, but in a line option.

                Console.WriteLine("                     ╔═══╦Top╦═══╗                     ");
                Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                    cube_state[0, 0, 0], cube_state[0, 0, 1], cube_state[0, 0, 2]);
                Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
                Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                    cube_state[0, 1, 0], cube_state[0, 1, 1], cube_state[0, 1, 2]);
                Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
                Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                    cube_state[0, 2, 0], cube_state[0, 2, 1], cube_state[0, 2, 2]);
                Console.WriteLine("                     ╚═══╩═══╩═══╝                     ");
                Console.WriteLine();
                Console.WriteLine("╔═══Left════╗ ╔═══Front═══╗ ╔═══Right═══╗ ╔═══Back════╗");
                Console.WriteLine("║ {0} ║ {1} ║ {2} ║ ║ {3} ║ {4} ║ {5} ║ ║ {6} ║ {7} ║ {8} ║ ║ {9} ║ {10} ║ {11} ║",
                    cube_state[1, 0, 0], cube_state[1, 0, 1], cube_state[1, 0, 2],
                    cube_state[2, 0, 0], cube_state[2, 0, 1], cube_state[2, 0, 2],
                    cube_state[3, 0, 0], cube_state[3, 0, 1], cube_state[3, 0, 2],
                    cube_state[4, 0, 0], cube_state[4, 0, 1], cube_state[4, 0, 2]);
                Console.WriteLine("╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣");
                Console.WriteLine("║ {0} ║ {1} ║ {2} ║ ║ {3} ║ {4} ║ {5} ║ ║ {6} ║ {7} ║ {8} ║ ║ {9} ║ {10} ║ {11} ║",
                    cube_state[1, 1, 0], cube_state[1, 1, 1], cube_state[1, 1, 2],
                    cube_state[2, 1, 0], cube_state[2, 1, 1], cube_state[2, 1, 2],
                    cube_state[3, 1, 0], cube_state[3, 1, 1], cube_state[3, 1, 2],
                    cube_state[4, 1, 0], cube_state[4, 1, 1], cube_state[4, 1, 2]);
                Console.WriteLine("╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣");
                Console.WriteLine("║ {0} ║ {1} ║ {2} ║ ║ {3} ║ {4} ║ {5} ║ ║ {6} ║ {7} ║ {8} ║ ║ {9} ║ {10} ║ {11} ║",
                    cube_state[1, 2, 0], cube_state[1, 2, 1], cube_state[1, 2, 2],
                    cube_state[2, 2, 0], cube_state[2, 2, 1], cube_state[2, 2, 2],
                    cube_state[3, 2, 0], cube_state[3, 2, 1], cube_state[3, 2, 2],
                    cube_state[4, 2, 0], cube_state[4, 2, 1], cube_state[4, 2, 2]);
                Console.WriteLine("╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝");
                Console.WriteLine();
                Console.WriteLine("                     ╔══Bottom═══╗                     ");
                Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                    cube_state[5, 0, 0], cube_state[5, 0, 1], cube_state[5, 0, 2]);
                Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
                Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                    cube_state[5, 1, 0], cube_state[5, 1, 1], cube_state[5, 1, 2]);
                Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
                Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                    cube_state[5, 2, 0], cube_state[5, 2, 1], cube_state[5, 2, 2]);
                Console.WriteLine("                     ╚═══╩═══╩═══╝                     ");

            }

        }
        //"█"
        private void DisplayColouredCell(char cell, string? contents)
        {
            ConsoleColor originalColour = Console.ForegroundColor;

            switch (cell)
            {
                case 'W': Console.ForegroundColor = ConsoleColor.White; break;
                case 'G': Console.ForegroundColor = ConsoleColor.Green; break;
                case 'R': Console.ForegroundColor = ConsoleColor.Red; break;
                case 'B': Console.ForegroundColor = ConsoleColor.Blue; break;
                case 'O': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case 'Y': Console.ForegroundColor = ConsoleColor.Yellow; break;
                default: Console.ForegroundColor = originalColour; break;
            }

            if (contents != null) { Console.Write(contents); }
            else { Console.Write(cell); }

            Console.ForegroundColor = originalColour;
        }

        public void RotateRow(int row, RotationDirection d = default)
        {
            char[] temp_row_a;
            char[] temp_row_b;

            switch (d)
            {
                case RotationDirection.Clockwise:
                    temp_row_a = GetCubeRow(1, row); // Backup row of left face
                    UpdateCubeRow(1, row, GetCubeRow(2, row)); // Replace row of left face with row of front face
                    temp_row_b = GetCubeRow(4, row); // Backup row of back face
                    UpdateCubeRow(4, row, temp_row_a); // Replace row of back face with previously stored row of left face
                    temp_row_a = GetCubeRow(3, row); // Backup row of right face
                    UpdateCubeRow(3, row, temp_row_b); // Replace row of right face with previously stored row of back face
                    UpdateCubeRow(2, row, temp_row_a); // Replace row of front face with previously stored row of right face
                    break;

                case RotationDirection.AntiClockwise:
                    temp_row_a = GetCubeRow(1, row); // Backup row of left face
                    UpdateCubeRow(1, row, GetCubeRow(4, row)); // Replace row of left face with row of back face
                    temp_row_b = GetCubeRow(2, row); // Backup row of front face
                    UpdateCubeRow(2, row, temp_row_a); // Replace row of front face with previously stored row of left face
                    temp_row_a = GetCubeRow(3, row); // Backup row of right face
                    UpdateCubeRow(3, row, temp_row_b); // Replace row of right face with previously stored row of front face
                    UpdateCubeRow(4, row, temp_row_a); // Replace row of back face with previously stored row of right face
                    break;

                default:
                    throw new ArgumentException("Invalid direction specified. Argument must be RotationDirection.Clockwise or RotationDirection.AntiClockwise.");
            }

        }

        public void RotateCol(int column, RotationDirection d = default)
        {
            char[] temp_column_a;
            char[] temp_column_b;

            switch (d)
            {
                case RotationDirection.Clockwise:
                    temp_column_a = GetCubeCol(0, column); // Backup column of top face
                    UpdateCubeCol(0, column, GetCubeCol(4, column)); // Replace column of top face with column of back face
                    temp_column_b = GetCubeCol(2, column); // Backup column of front face
                    UpdateCubeCol(2, column, temp_column_a); // Replace column of front face with previously stored column of top face
                    temp_column_a = GetCubeCol(5, column); // Backup column of bottom face
                    UpdateCubeCol(5, column, temp_column_b); // Replace column of bottom face with previously stored column of front face
                    UpdateCubeCol(4, column, temp_column_a); // Replace column of back face with previously stored column of bottom face
                    break;

                case RotationDirection.AntiClockwise:
                    temp_column_a = GetCubeCol(0, column); // Backup column of top face
                    UpdateCubeCol(0, column, GetCubeCol(2, column)); // Replace column of top face with column of front face
                    temp_column_b = GetCubeCol(4, column); // Backup column of back face
                    UpdateCubeCol(4, column, temp_column_a); // Replace column of back face with previously stored column of top face
                    temp_column_a = GetCubeCol(5, column); // Backup column of bottom face
                    UpdateCubeCol(5, column, temp_column_b); // Replace column of bottom face with previously stored column of back face
                    UpdateCubeCol(2, column, temp_column_a); // Replace column of front face with previously stored column of bottom face
                    break;
                default:
                    throw new ArgumentException("Invalid direction specified. Argument must be RotationDirection.Clockwise or RotationDirection.AntiClockwise.");
            }
        }

        public void RotateFrontFace(RotationDirection d = default)
        {
            char[] temp_slice_a;
            char[] temp_slice_b;

            switch (d)
            {
                case RotationDirection.Clockwise:
                    temp_slice_a = GetCubeRow(0, 2); // Backup bottom row of top face
                    UpdateCubeRow(0, 2, GetCubeCol(1, 2)); // Replace bottom row of top face with right column of left face
                    temp_slice_b = GetCubeCol(3, 0); // Backup left column of right face
                    UpdateCubeCol(3, 0, temp_slice_a); // Replace left column of right face with previously stored bottom row of top face
                    temp_slice_a = GetCubeRow(5, 0); // Backup top row of bottom face
                    UpdateCubeRow(5, 0, temp_slice_b); // Replace top row of bottom face with previously stored left column of right face
                    UpdateCubeCol(1, 2, temp_slice_a); // Replace right column of left face with previously stored top row of bottom face
                    break;

                case RotationDirection.AntiClockwise:
                    temp_slice_a = GetCubeRow(0, 2); // Backup bottom row of top face
                    UpdateCubeRow(0, 2, GetCubeCol(3, 0)); // Replace bottom row of top face with left column of right face
                    temp_slice_b = GetCubeCol(1, 2); // Backup right column of left face
                    UpdateCubeCol(1, 2, temp_slice_a); // Replace right column of left face with previously stored bottom row of top face
                    temp_slice_a = GetCubeRow(5, 0); // Backup top row of bottom face
                    UpdateCubeRow(5, 0, temp_slice_b); // Replace top row of bottom face with previously stored right column of left face
                    UpdateCubeCol(3, 0, temp_slice_a); // Replace bottom row of top face with previously stored top row of bottom face
                    break;
                default:
                    throw new ArgumentException("Invalid direction specified. Argument must be RotationDirection.Clockwise or RotationDirection.AntiClockwise.");
            }
        }

        // List of pairs?

        public void RotateBackFace(RotationDirection d = default)
        {
            char[] temp_slice_a;
            char[] temp_slice_b;

            switch (d)
            {
                case RotationDirection.Clockwise:
                    temp_slice_a = GetCubeRow(0, 0); // Backup top row of top face
                    UpdateCubeRow(0, 0, GetCubeCol(1, 0)); // Replace top row of top face with left column of left face
                    temp_slice_b = GetCubeCol(3, 2); // Backup right column of right face
                    UpdateCubeCol(3, 2, temp_slice_a); // Replace right column of right face with previously stored top row of top face
                    temp_slice_a = GetCubeRow(5, 2); // Backup bottom row of bottom face
                    UpdateCubeRow(5, 2, temp_slice_b); // Replace bottom row of bottom face with previously stored right column of right face 
                    UpdateCubeCol(1, 0, temp_slice_a); // Replace left column of left face with prevously stored bottom row of bottom face
                    break;

                case RotationDirection.AntiClockwise:
                    temp_slice_a = GetCubeRow(0, 0); // Backup top row of top face
                    UpdateCubeRow(0, 0, GetCubeCol(3, 2)); // Replace top row of top face with right row of right face 
                    temp_slice_b = GetCubeCol(1, 0); // Backup left column of left face
                    UpdateCubeCol(1, 0, temp_slice_a); // Replace left column of left face with previously stored top row of top face
                    temp_slice_a = GetCubeRow(5, 2); // Backup bottom row of bottom face
                    UpdateCubeRow(5, 2, temp_slice_b); // Replace bottom row of bottom face with previously stored left column of left face
                    UpdateCubeCol(3, 2, temp_slice_a); // Replace right column of right face previously stored bottom row of bottom face
                    break;
                default:
                    throw new ArgumentException("Invalid direction specified. Argument must be RotationDirection.Clockwise or RotationDirection.AntiClockwise.");
            }
        }

        private char[] GetCubeRow(int faceIndex, int rowIndex)
        {
            char[] row = new char[3];
            for (int i = 0; i < 3; i++)
            {
                row[i] = cubeState[faceIndex, rowIndex, i];
            }
            return row;
        }

        private void UpdateCubeRow(int faceIndex, int rowIndex, char[] row)
        {
            for (int i = 0; i < 3; i++)
            {
                cubeState[faceIndex, rowIndex, i] = row[i];
            }
        }

        private char[] GetCubeCol(int faceIndex, int colIndex)
        {
            char[] column = new char[3];
            for (int i = 0; i < 3; i++)
            {
                column[i] = cubeState[faceIndex, i, colIndex];
            }
            return column;
        }

        private void UpdateCubeCol(int faceIndex, int colIndex, char[] column)
        {
            for (int i = 0; i < 3; i++)
            {
                cubeState[faceIndex, i, colIndex] = column[i];
            }

        }

        public void Shuffle(int moves = 50, bool showShuffling = false)
        {
            if (showShuffling)
            {
                Display();
                Console.WriteLine(Environment.NewLine +
                    "                                PRESS ENTER TO SHUFFLE                                 " +
                    Environment.NewLine);
                Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("_____________________________________ SHUFFLING ____________________________________");
                Console.WriteLine();
            }

            Random random = new Random();
            int which_function;
            int which_row_or_col;
            RubiksCube.RotationDirection which_direction;
            string direction_name;

            for (int n = 0; n < moves + 1; n++)
            {
                which_function = random.Next(0, 4);
                which_row_or_col = random.Next(0, 3);
                which_direction = (random.Next(0, 2) == 0 ? RubiksCube.RotationDirection.Clockwise : RubiksCube.RotationDirection.AntiClockwise);
                direction_name = (which_direction == RubiksCube.RotationDirection.Clockwise) ? "clockwise" : "anti-clockwise";

                switch (which_function)
                {
                    case 0:
                        RotateRow(which_row_or_col, which_direction);
                        if (showShuffling) { Console.Write($"{Environment.NewLine}                            [{n}] Rotating row {which_row_or_col} {direction_name}"); }
                        break;
                    case 1:
                        RotateCol(which_row_or_col, which_direction);
                        if (showShuffling) { Console.Write($"{Environment.NewLine}                            [{n}] Rotating column {which_row_or_col} {direction_name}"); }
                        break;
                    case 2:
                        RotateFrontFace(which_direction);
                        if (showShuffling) { Console.Write($"{Environment.NewLine}                            [{n}] Rotating front face {direction_name}"); }
                        break;
                    case 3:
                        RotateBackFace(which_direction);
                        if (showShuffling) { Console.Write($"{Environment.NewLine}                            [{n}] Rotating back face {direction_name}"); }
                        break;
                    default: break;
                }

                if (showShuffling)
                {
                    Console.WriteLine();
                    Display();
                }
            }
            Console.WriteLine("____________________________________________________________________________________");
            Console.WriteLine();

        }

        // THERE IS ROOM TO OPTIMISE LATER
        private void RotateRow_ForLaterOptimisation(int row, RotationDirection d = default)
        {
            char[] temp_row_a;
            char[] temp_row_b;
            int a, b;

            switch (d)
            {
                case RotationDirection.Clockwise:
                    a = 2; b = 4;
                    break;

                case RotationDirection.AntiClockwise:
                    a = 4; b = 2;
                    break;

                default:
                    throw new ArgumentException("Invalid direction specified. Argument must be RotationDirection.Clockwise or RotationDirection.AntiClockwise.");
            }

            temp_row_a = GetCubeRow(1, row);
            UpdateCubeRow(1, row, GetCubeRow(a, row));
            temp_row_b = GetCubeRow(b, row);
            UpdateCubeRow(b, row, temp_row_a);
            temp_row_a = GetCubeRow(3, row);
            UpdateCubeRow(3, row, temp_row_b);
            UpdateCubeRow(a, row, temp_row_a);

        }
    }




    public class RubiksCubeFitnessFunction : IFitnessFunction
    {
        /// <summary>
        /// 
        /// 
        /// NOTES: 
        /// * Reset cube state before or after fitness check?
        ///         // Before means first run is redundant. After could mean you run code on the wrong state. Need to think about it.
        /// * Hardcode in face colour or still check colour each time?
        /// * Use state or configuration instead of cubeState?
        /// * Instance cube in or out of constructor?
        /// </summary>

        private readonly char[,,] originalCubeState;
        private RubiksCube rubiksCube = new RubiksCube();
        private Dictionary<char, Action> moves;

        private int population_count;
        private string points = "";

        public RubiksCubeFitnessFunction(char[,,] originalCubeState)
        {
            // Create a new cube "deep copy" to test out moves on
            rubiksCube.cubeState = (char[,,])originalCubeState.Clone();

            // Store original cubeState to revert back to later
            this.originalCubeState = (char[,,])originalCubeState.Clone();

            // Initialize the dictionary, definining all possible moves
            moves = new Dictionary<char, Action>
        {
            {'0', () => rubiksCube.RotateRow(0, RotationDirection.Clockwise)},
            {'1', () => rubiksCube.RotateRow(1, RotationDirection.Clockwise)},
            {'2', () => rubiksCube.RotateRow(2, RotationDirection.Clockwise)},
            {'3', () => rubiksCube.RotateCol(0, RotationDirection.Clockwise)},
            {'4', () => rubiksCube.RotateCol(1, RotationDirection.Clockwise)},
            {'5', () => rubiksCube.RotateCol(2, RotationDirection.Clockwise)},
            {'6', () => rubiksCube.RotateFrontFace(RotationDirection.Clockwise)},
            {'7', () => rubiksCube.RotateBackFace(RotationDirection.Clockwise)},
            {'8', () => rubiksCube.RotateRow(0, RotationDirection.AntiClockwise)},
            {'9', () => rubiksCube.RotateRow(1, RotationDirection.AntiClockwise)},
            {'A', () => rubiksCube.RotateRow(2, RotationDirection.AntiClockwise)},
            {'B', () => rubiksCube.RotateCol(0, RotationDirection.AntiClockwise)},
            {'C', () => rubiksCube.RotateCol(1, RotationDirection.AntiClockwise)},
            {'D', () => rubiksCube.RotateCol(2, RotationDirection.AntiClockwise)},
            {'E', () => rubiksCube.RotateFrontFace(RotationDirection.AntiClockwise)},
            {'F', () => rubiksCube.RotateBackFace(RotationDirection.AntiClockwise)}
        };

            population_count = 0;
        }

        public double Evaluate(IChromosome chromosome)
        {

            // RESET BEFORE EACH FITNESS CHECK
            double fitness = 0;
            ResetCube();
            points = "";

            population_count++;
            // RESET CUBE STATE (Before each fitness check)


            //Console.WriteLine($"{population_count}: {chromosome}"); // another pause to show this, also number them?
            Decode(chromosome);

            // FITNESS GOAL: Reward for how complete each face is
            //               1. How many cells match the middle square colour? -> +1pt/cell
            //               2. Is the face complete? If yes -> +10pts
            //               3. If no cells match -> -1pt


            // Target colour of middle square on first face
            char target_colour = rubiksCube.cubeState[0, 1, 1];

            int face = 0;
            int count = 0;
            int face_outOf9 = 0;

            // Iterate through all 54 cells on cube
            foreach (char cell in rubiksCube.cubeState)
            {
                // Reward colours matching middle of face
                if (cell == target_colour)
                {
                    //Console.WriteLine($"[+1] FACE {face}: Matching cell found.");
                    points += $"[+1] FACE {face}: Cell {count%9+1} match.{Environment.NewLine}";
                    fitness += 1;
                    face_outOf9 += 1;
                }

                count++;
                if (count % 9 == 0)
                { // Moved onto a new face - RESET

                    // Penalise a face with no matching cells
                    if (face_outOf9 == 1)
                    {
                        //Console.WriteLine($"[-1] FACE {face}: No matching cells.");
                        points += $"[-1] FACE {face}: No matching cells.{Environment.NewLine}";
                        fitness--;
                    }

                    // Reward a complete face
                    if (face_outOf9 == 9)
                    {
                        points += $"[+10] FACE {face}: Complete!.{Environment.NewLine}";
                        fitness += 10;
                    }

                    face_outOf9 = 0;
                    face++;

                    if (face < 6)
                    {
                        // Target colour of middle square on NEXT face
                        target_colour = rubiksCube.cubeState[face, 1, 1];
                    }

                }

            }

            return fitness;

        }

        private void Decode(IChromosome chromosome)
        { // Executes a sequence of Rubik's Cube moves, based on each identifier
            foreach (char hex_digit in BinaryToHex(chromosome))
            {
                if (moves.TryGetValue(hex_digit, out var move))
                {
                    move.Invoke();
                }
                else { throw new Exception("[MoveERROR] Invalid move selected - check chromosome and hex."); }
            }
            //return rubiksCube.cubeState;
        }

        public string BinaryToHex(IChromosome chromosome)
        {
            return Convert.ToUInt64(chromosome.ToString(), 2).ToString("X");
        }

        public void Display(BinaryChromosome chromosome)
        {
            Decode(chromosome);
            rubiksCube.Display();
        }

        public void Inspect(BinaryChromosome chromosome, bool extended=false)
        { // Provides feedback on attributes of instance and class

            Console.WriteLine($"" +
                $" [{BinaryToHex(chromosome)}] {chromosome.ToString()}{Environment.NewLine}");

            if (extended)
            {
                Console.WriteLine(
                    $"Generated Binary:  {chromosome.ToString()}" + Environment.NewLine +
                    $"Decoded Hex Value: {BinaryToHex(chromosome)}" + Environment.NewLine +
                    $"Decimal Value:     {chromosome.Value}" + Environment.NewLine +
                    $"Maximum Value:     {chromosome.MaxValue}" + Environment.NewLine +
                    $"Length:            {chromosome.Length}/{BinaryChromosome.MaxLength} bits" + Environment.NewLine);
            }

            // move descriptions
        }

        public void ShowPoints(BinaryChromosome chromosome)
        {
            points = "";
            Evaluate(chromosome);
            Console.WriteLine(points);
            population_count--;
            points = "";
        }

        public void ResetCube()
        {
            rubiksCube.cubeState = (char[,,])originalCubeState.Clone();
        }


    }


    
}
