using System;
using System.Data.Common;
using System.Numerics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

using AForge;
using AForge.Genetic;
using static RubiksCube;

/*
 * Just solve first layer!
 * 
 * What if there were functions that translated into other functions to allow for different perspectives?
 * Such that a front face turn is the same as a left column turn from a different face's perspective.
 * 
 * //public new const int MaxLength = 128;
 * 
 * Do you even need a copy of the cube instance or maybe just a cubeState? But how will the moves be done?
 * 
 * Should the cube be made private?
 * 
 * Does the chromosome need more functions, like for display or returning values?
 * 
 * Error handling for decodechromosome?
 * 
 * RubiksCubeChromosome inherts BinaryChromosome which inherits IChromosome, so all is good.
 * Passed in during GA setup? 
 * passing in original cube state - memory issue if in chromosome, but more convienient
 * 
 * inconsisent variable names (_ vs camelCase)
 * 
 * Do I need to initialise with the functions so the cube has been modified before the fitness function?]
 * 
 * Shuffle cube by default when instancing cube? Make display false then.
 */


//RubiksCube.RotationDirection CW;//= RubiksCube.RotationDirection.Clockwise;
//RubiksCube.RotationDirection ACW;// = RubiksCube.RotationDirection.AntiClockwise;

public class RubiksCube
{
    public char[,,] cubeState;
    private readonly char[,,] SOLVED_CUBE =
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

    private void DisplayTopAndBottomCubeStatesColour(int face)
    {
        string face_name = face == 0 ? "╔═══╦Top╦═══╗" : "╔══Bottom═══╗";
        Console.WriteLine($"                      {face_name}                      ");

        for (int i = 0; i < 3; i++)
        {
            Console.Write($"                      ║ ");
            for (int j = 0; j < 3; j++)
            {
                DisplayColouredCell(cubeState[face, i, j]);
                Console.Write(" ║ ");
            }
            Console.WriteLine();

            if (i < 2) // Check if it's not the last iteration
                Console.WriteLine($"                      ╠═══╬═══╬═══╣                      ");
        }

        Console.WriteLine($"                      ╚═══╩═══╩═══╝                      ");
    }

    public void Display(bool colour = true, bool line = true)
    {
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
                            DisplayColouredCell(cubeState[face, row, col]);
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
                DisplayTopAndBottomCubeStatesColour(0);

                Console.WriteLine(" ╔═══Left════╗ ╔═══Front═══╗ ╔═══Right═══╗ ╔═══Back════╗ ");

                for (int row = 0; row < 3; row++)
                {
                    Console.Write(" ║ ");
                    for (int face = 1; face < 5; face++)
                    {
                        for (int col = 0; col < 3; col++)
                        {
                            DisplayColouredCell(cubeState[face, row, col]);
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


                DisplayTopAndBottomCubeStatesColour(5);

            }
        }
        else
        { // Currently no basic, but in a line option.

            Console.WriteLine("                     ╔═══╦Top╦═══╗                     ");
            Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                cubeState[0, 0, 0], cubeState[0, 0, 1], cubeState[0, 0, 2]);
            Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
            Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                cubeState[0, 1, 0], cubeState[0, 1, 1], cubeState[0, 1, 2]);
            Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
            Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                cubeState[0, 2, 0], cubeState[0, 2, 1], cubeState[0, 2, 2]);
            Console.WriteLine("                     ╚═══╩═══╩═══╝                     ");
            Console.WriteLine();
            Console.WriteLine("╔═══Left════╗ ╔═══Front═══╗ ╔═══Right═══╗ ╔═══Back════╗");
            Console.WriteLine("║ {0} ║ {1} ║ {2} ║ ║ {3} ║ {4} ║ {5} ║ ║ {6} ║ {7} ║ {8} ║ ║ {9} ║ {10} ║ {11} ║",
                cubeState[1, 0, 0], cubeState[1, 0, 1], cubeState[1, 0, 2],
                cubeState[2, 0, 0], cubeState[2, 0, 1], cubeState[2, 0, 2],
                cubeState[3, 0, 0], cubeState[3, 0, 1], cubeState[3, 0, 2],
                cubeState[4, 0, 0], cubeState[4, 0, 1], cubeState[4, 0, 2]);
            Console.WriteLine("╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣");
            Console.WriteLine("║ {0} ║ {1} ║ {2} ║ ║ {3} ║ {4} ║ {5} ║ ║ {6} ║ {7} ║ {8} ║ ║ {9} ║ {10} ║ {11} ║",
                cubeState[1, 1, 0], cubeState[1, 1, 1], cubeState[1, 1, 2],
                cubeState[2, 1, 0], cubeState[2, 1, 1], cubeState[2, 1, 2],
                cubeState[3, 1, 0], cubeState[3, 1, 1], cubeState[3, 1, 2],
                cubeState[4, 1, 0], cubeState[4, 1, 1], cubeState[4, 1, 2]);
            Console.WriteLine("╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣ ╠═══╬═══╬═══╣");
            Console.WriteLine("║ {0} ║ {1} ║ {2} ║ ║ {3} ║ {4} ║ {5} ║ ║ {6} ║ {7} ║ {8} ║ ║ {9} ║ {10} ║ {11} ║",
                cubeState[1, 2, 0], cubeState[1, 2, 1], cubeState[1, 2, 2],
                cubeState[2, 2, 0], cubeState[2, 2, 1], cubeState[2, 2, 2],
                cubeState[3, 2, 0], cubeState[3, 2, 1], cubeState[3, 2, 2],
                cubeState[4, 2, 0], cubeState[4, 2, 1], cubeState[4, 2, 2]);
            Console.WriteLine("╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝ ╚═══╩═══╩═══╝");
            Console.WriteLine();
            Console.WriteLine("                     ╔══Bottom═══╗                     ");
            Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                cubeState[5, 0, 0], cubeState[5, 0, 1], cubeState[5, 0, 2]);
            Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
            Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                cubeState[5, 1, 0], cubeState[5, 1, 1], cubeState[5, 1, 2]);
            Console.WriteLine("                     ╠═══╬═══╬═══╣                     ");
            Console.WriteLine("                     ║ {0} ║ {1} ║ {2} ║                     ",
                cubeState[5, 2, 0], cubeState[5, 2, 1], cubeState[5, 2, 2]);
            Console.WriteLine("                     ╚═══╩═══╩═══╝                     ");

        }

    }

    private void DisplayColouredCell(char cell)
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

        Console.Write("█");

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

    }

    // THERE IS ROOM TO OPTIMISE LATER
    private void RotateRow2(int row, RotationDirection d = default)
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

public class RubiksCubeChromosome : BinaryChromosome
{
    /// <summary>
    /// This custom chromosome allows for manipulation of a cloned cube within this class.
    /// </summary>

    public RubiksCube rubiksCube;
    public readonly char[,,] originalCubeState;
    private string hex_value;
    private Dictionary<char, Action> moves;

    private Random RandomGenerator;

    public RubiksCubeChromosome(RubiksCube originalCube) : base(64)
    {
        // Create a "deep" copy of the cube to prevent modifications to the original
        rubiksCube = originalCube;
        originalCubeState = (char[,,])originalCube.cubeState.Clone();

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

        // Initialise random object
        RandomGenerator = new Random();

        // Decode binary and use to action moves
        hex_value = DecodeChromosome();
        ApplyMoves();
    }

    public string DecodeChromosome()
    { // Decodes into a format where each move can be represented by a single hex digit
        hex_value = Convert.ToUInt64(this.ToString(), 2).ToString("X");
        return hex_value;
    }

    private void ApplyMoves()
    { // Executes a sequence of Rubik's Cube moves, based on each identifier
        foreach (char hex_digit in hex_value)
        {
            if (moves.TryGetValue(hex_digit, out var move))
            {
                move.Invoke();
            }
            else { throw new Exception("[ERROR] Invalid digit - no matching move."); }
        }
    }

    public bool Probability(double p = 0.5)
    {
        double roll = RandomGenerator.NextDouble();
        if (roll <= p) { return true; }
        else { return false; }
    }


    public void Inspect()
    { // Provides feedback on attributes of instance and class
        Console.WriteLine($"" +
            $"Generated Binary:  {this.ToString()}" + Environment.NewLine +
            $"Decoded Hex Value: {hex_value}" + Environment.NewLine +
            $"Decimal Value:     {this.Value}" + Environment.NewLine +
            $"Maximum Value:     {this.MaxValue}" + Environment.NewLine +
            $"Length:            {this.Length}/{RubiksCubeChromosome.MaxLength} bits");
    }




}

public class RubiksCubeSolvingFitnessFunction : IFitnessFunction
{
    public double fitness;
    private char[,,]? originalCubeState;

    public RubiksCubeSolvingFitnessFunction()
    {
        fitness = 0;
    }

    public double Evaluate(IChromosome chromosome)
    {
        if (chromosome is RubiksCubeChromosome rubiksCubeChromosome)
        {
            // Initialise original cube state globally
            originalCubeState = rubiksCubeChromosome.originalCubeState;

            // Limit focus
            fitness = Eval(rubiksCubeChromosome.rubiksCube.cubeState);

            return fitness;

            //rubiksCubeChromosome.DecodeChromosome();
            //rubiksCubeChromosome.ApplyMoves();

        }

        /* Note: It may be better to copy the original/initial state into the chromosome
        // so only one parameter is needed when instancing.


        // Rank the fitness of the new cube state
        // old state compared to new state

        // LAYER
        // if the new cube state has a completed layer, it is considered perfect
        // the more of the same colour on a single face, the better
        // penalties?

        // WHOLE CUBE
        // Count of cubies in correct position and orientation
        // Count of layers completed
        // Uniform colour distribution - surely this is a good thing?
        // Adjacent matching colours
        // Move efficiency - penalise unnecessary moves
        */


        return fitness;
    }
    private double Eval(char[,,] cubeState)
    {
        // Goal: Many of the same colour (matching the middle) on each face.

        char target_colour = cubeState[0, 1, 1];
        //Console.WriteLine($"Targetting {target_colour}.");

        int face = 0;
        int count = 0;
        foreach (char cell in cubeState)
        {
            if (cell == target_colour) { fitness += 1; }

            count++;
            if (count % 9 == 0)
            { // Moved onto a new face - RESET
                face++;
                if (face < 6)
                {
                    target_colour = cubeState[face, 1, 1]; // Middle square
                    //Console.WriteLine($"Targetting {target_colour}.");
                }
            }

        }

        return fitness;
    }

    private double Eval_NOTWORKING(char[,,] cubeState)
    {
        // Just looking for completed faces

        // need a baseline to compare to - fitness of shuffled cube
        // consider adjacent cells - all blue may not be the right blue

        string face = "";

        foreach (char cell in cubeState)
        {
            // if face string is empty OR the cell matches the first
            if (face.Length == 0 || cell == face[0])
            {
                Console.WriteLine($"FLAG: {face}, {cell}");
                face += cell;

                if (face.Length > 8)
                {
                    Console.WriteLine("RESET");
                    if (face == new string(face[0], 9)) // ex "GGGGGGGG" == "G"*8
                    {
                        fitness += 50; // Every complete face awards 50 points.
                    }
                    face = ""; // Reset
                }
            }
            else { face = ""; Console.WriteLine("FULL RESET"); }
        }

        return fitness;
    }

}


public class Program
{
    static void GA(
        int populationSize = 1000,
        int randomSelectionPortion = 10,
        int targetFitness = 20,
        double mutationRate = 0.3,
        double crossoverRate = 0.9)
    {
        RubiksCube myCube = new RubiksCube();
        //myCube.Shuffle(100, false);
        myCube.Display();

        RubiksCubeChromosome firstChromosome = new RubiksCubeChromosome(myCube);
        //firstChromosome.Inspect();
        Console.WriteLine(firstChromosome.ToString());

        RubiksCubeChromosome secondChromosome = new RubiksCubeChromosome(myCube);
        Console.WriteLine(secondChromosome.ToString());

        RubiksCubeSolvingFitnessFunction fitnessFunction = new RubiksCubeSolvingFitnessFunction();
        EliteSelection selectionMethod = new EliteSelection();
        // mutation and crossover?

        Population population = new Population(
            populationSize,
            firstChromosome,
            fitnessFunction,
            selectionMethod);

        population.MutationRate = mutationRate;
        population.CrossoverRate = crossoverRate;

        // Number of the population that will be selected to breed, regardless of fitness
        population.RandomSelectionPortion = randomSelectionPortion;

        int epochCount = 0;
        double bestFitness = 0;

        Console.WriteLine("                     Press ENTER to run the genetic algorithm.");
        Console.ReadLine();
        myCube.Display();
        Console.WriteLine();


        while (true)
        {
            epochCount++;

            // Run one epoch of the genetic algorithm
            population.RunEpoch();

            // Get the best individual in the current population
            RubiksCubeChromosome bestChromosome = population.BestChromosome as RubiksCubeChromosome;

            // Get the fitness of the best individual in this epoch
            double currentFitness = fitnessFunction.Evaluate(bestChromosome);

            Console.WriteLine($"                  Epoch {epochCount} | Current Fitness: {currentFitness} | Best Fitness: {bestFitness}");

            if (currentFitness > bestFitness)
            {
                bestFitness = currentFitness;
                if (bestChromosome != null)
                {
                    bestChromosome.rubiksCube.Display();
                    Console.ReadLine();
                }
            }

            if (bestFitness >= targetFitness)
            {
                Console.WriteLine($"Target fitness of {targetFitness} reached. Stopping.");
                if (bestChromosome != null)
                {
                    bestChromosome.rubiksCube.Display();
                    Console.ReadLine();
                }
                break;
            }

            if (epochCount % 5000 == 0) { Console.ReadLine(); }
        }
    }

    static void Main(string[] args)
    {
        GA();

        /*
        RubiksCube myCube = new RubiksCube();
        myCube.Display();
        RubiksCubeChromosome firstChromosome = new RubiksCubeChromosome(myCube);
        firstChromosome.Inspect();
        firstChromosome.rubiksCube.Display();
        RubiksCubeSolvingFitnessFunction fitnessFunction = new RubiksCubeSolvingFitnessFunction();
        Console.WriteLine(fitnessFunction.Evaluate(firstChromosome));
        */

    }


}

// NEED TO WORK OUT WHY IT ALWAY SEEMS TO SHUFFLE

/*
Bit Representation 1 - Leading bit represents direction.

RotateRow:

    [0] - 0000 - RotateRow(0, C)
    [1] - 0001 - RotateRow(1, C)
    [2] - 0010 - RotateRow(2, C)
    [8] - 1000 - RotateRow(0, AC)
    [9] - 1001 - RotateRow(1, AC)
    [A] - 1010 - RotateRow(2, AC)

RotateCol:

    [3] - 0011 - RotateCol(0, C)
    [4] - 0100 - RotateCol(1, C)
    [5] - 0101 - RotateCol(2, C)
    [B] - 1011 - RotateCol(0, AC)
    [C] - 1100 - RotateCol(1, AC)
    [D] - 1101 - RotateCol(2, AC)

RotateFrontFace:

    [6] - 0110 - RotateFrontFace(C)
    [E] - 1110 - RotateFrontFace(AC)

RotateBackFace:

    [7] - 0111 - RotateBackFace(C)
    [F] - 1111 - RotateBackFace(AC)

 */

/*

Bit Representation 2 - Moves are in order

RotateRow:

    [0] - 0000 - RotateRow(0, C)
    [1] - 0001 - RotateRow(1, C)
    [2] - 0010 - RotateRow(2, C)
    [3] - 0011 - RotateRow(0, AC)
    [4] - 0100 - RotateRow(1, AC)
    [5] - 0101 - RotateRow(2, AC)

RotateCol:

    [6] - 0110 - RotateCol(0, C)
    [7] - 0111 - RotateCol(1, C)
    [8] - 1000 - RotateCol(2, C)
    [9] - 1001 - RotateCol(0, AC)
    [A] - 1010 - RotateCol(1, AC)
    [B] - 1011 - RotateCol(2, AC)

RotateFrontFace:

    [C] - 1100 - RotateFrontFace(C)
    [D] - 1101 - RotateFrontFace(AC)

RotateBackFace:

    [E] - 1110 - RotateBackFace(C)
    [F] - 1111 - RotateBackFace(AC)


 */

