using Accord;
using Accord.Genetic;

namespace RubiksCubeGA
{
    /*
    public class RubiksCubeChromosome : BinaryChromosome
    {
        /// <summary>
        /// Inherits BinaryChromosome to generate a random binary value
        /// The binary value is turned into a hex value instead
        /// The hex value is turned into a sequence of cube rotational moves
        /// These moves are applied to a copy of the cube, to see how it changes
        /// This new cube state is what each chromosome represents in the population
        /// </summary>

        private RubiksCube rubiksCube;
        public char[,,] cube_state;
        public readonly char[,,] originalCubeState; // Might be unneccessary
        private Dictionary<char, Action> moves;

        public RubiksCubeChromosome(char[,,] originalCubeState) : base(64)
        {
            // Create a new cube copy to test out moves on
            rubiksCube = new RubiksCube();
            rubiksCube.cubeState = originalCubeState;

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

            Decode();
        }

        public void Decode()
        {
            ApplyMoves();
            //rubiksCube.Display(); // Add a method to display the cube state
        }

        public string BinaryToHex()
        { // Decodes into a format where each move can be represented by a single hex digit
            return Convert.ToUInt64(this.ToString(), 2).ToString("X");
        }

        private void ApplyMoves()
        { // Executes a sequence of Rubik's Cube moves, based on each identifier
            foreach (char hex_digit in BinaryToHex())
            {
                if (moves.TryGetValue(hex_digit, out var move))
                {
                    move.Invoke();
                }
                else { throw new Exception("[ERROR] Invalid digit - no matching move."); }
            }
        }

        public void Display() { rubiksCube.Display(); }

        public void Inspect()
        { // Provides feedback on attributes of instance and class
            Console.WriteLine($"" +
                $"Generated Binary:  {this.ToString()}" + Environment.NewLine +
                $"Decoded Hex Value: {BinaryToHex()}" + Environment.NewLine +
                $"Decimal Value:     {this.Value}" + Environment.NewLine +
                $"Maximum Value:     {this.MaxValue}" + Environment.NewLine +
                $"Length:            {this.Length}/{RubiksCubeChromosome.MaxLength} bits");
        }

        //public Exit() { } // Update cube state upon terminating?
    }

    public class RubiksCubeFitnessFunction_TEMPCUBE : IFitnessFunction
    {
        private readonly char[,,] originalCubeState;
        public char[,,] newCubeState;

        public RubiksCubeFitnessFunction_TEMPCUBE(char[,,] originalCubeState)
        {
            this.originalCubeState = originalCubeState;
        }

        public double Evaluate(IChromosome chromosome)
        {
            newCubeState = Decode(chromosome);
            // FITNESS LOGIC HERE
            return 0.0; // return fitness;
        }

        private char[,,] Decode(IChromosome chromosome)
        { // Executes a sequence of Rubik's Cube moves, based on each identifier

            RubiksCube tempCube = new RubiksCube();
            tempCube.cubeState = originalCubeState;

            switch (BinaryToHex(chromosome))
            {
                case "0": tempCube.RotateRow(0, RotationDirection.Clockwise); break;
                case "1": tempCube.RotateRow(1, RotationDirection.Clockwise); break;
                case "2": tempCube.RotateRow(2, RotationDirection.Clockwise); break;
                case "3": tempCube.RotateCol(0, RotationDirection.Clockwise); break;
                case "4": tempCube.RotateCol(1, RotationDirection.Clockwise); break;
                case "5": tempCube.RotateCol(2, RotationDirection.Clockwise); break;
                case "6": tempCube.RotateFrontFace(RotationDirection.Clockwise); break;
                case "7": tempCube.RotateBackFace(RotationDirection.Clockwise); break;
                case "8": tempCube.RotateRow(0, RotationDirection.AntiClockwise); break;
                case "9": tempCube.RotateRow(1, RotationDirection.AntiClockwise); break;
                case "A": tempCube.RotateRow(2, RotationDirection.AntiClockwise); break;
                case "B": tempCube.RotateCol(0, RotationDirection.AntiClockwise); break;
                case "C": tempCube.RotateCol(1, RotationDirection.AntiClockwise); break;
                case "D": tempCube.RotateCol(2, RotationDirection.AntiClockwise); break;
                case "E": tempCube.RotateFrontFace(RotationDirection.AntiClockwise); break;
                case "F": tempCube.RotateBackFace(RotationDirection.AntiClockwise); break;
                default: Console.WriteLine("[MoveERROR] Invalid chromosome or hex digit."); break;
            }
            return tempCube.cubeState; // This also deletes the temp cube (maybe look into dispose?)
        }

        public string BinaryToHex(IChromosome chromosome)
        {
            return Convert.ToUInt64(this.ToString(), 2).ToString("X");
        }

        public void Display()
        {
            //rubiksCube.Display(); // cannot do a display on this as cube gets destroyed
            // would have to use original myCube and pass in the state -> myCube.Display(cubeState);
            // Same issue as other fitness function - you need to decode and run on cube 
            // except there is no cube this time, but I was going to make a temp one for that one anyway
        }

    }

    */
}