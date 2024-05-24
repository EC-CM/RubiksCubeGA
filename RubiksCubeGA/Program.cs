using Accord;
using Accord.Genetic;

namespace RubiksCubeGA
{
    public class SharedFunctions
    {
        public static string CentreMessage(string s, char c = ' ', int width = 85, string justify = "centre")
        {
            if (justify == "centre")
            {
                int padding = (width - s.Length) / 2;
                return s.PadLeft(padding + s.Length, c).PadRight(width, c);
            }
            else if (justify == "left")
            {
                return new string(' ', 22) + s;
            }
            else { return s; }

        }
    }

    public class Program : SharedFunctions
    {
        public static void GeneticAlgorithm(
            int populationSize = 100,



            int randomSelectionPortion = 10,
            double mutationRate = 0.1,
            double crossoverRate = 0.3,
            int targetFitness = 60)
        {
            RubiksCube myCube = new RubiksCube();
            myCube.Shuffle(4, true);
            Console.WriteLine(Environment.NewLine + CentreMessage(" Starting Cube ") + Environment.NewLine);
            myCube.Display();

            BinaryChromosome firstChromosome = new BinaryChromosome(64);
            RubiksCubeFitnessFunction fitnessFunction = new RubiksCubeFitnessFunction(myCube.cubeState.Clone() as char[,,]); //null);
            EliteSelection selectionMethod = new EliteSelection();

            Population population = new Population(
                populationSize,
                firstChromosome,
                fitnessFunction,
                selectionMethod);

            population.MutationRate = mutationRate; // Percentage of chromosomes that will undergo bit mutation
            population.CrossoverRate = crossoverRate;
            population.RandomSelectionPortion = randomSelectionPortion; //Number of the population that will be selected to breed, regardless of fitness


            int epochCount = 0;
            double bestFitness = 0;

            Console.WriteLine(Environment.NewLine + CentreMessage(" Press ENTER to run the genetic algorithm. ") + Environment.NewLine);
            Console.ReadLine();
            Console.WriteLine();


            while (true)
            {
                epochCount++;

                // Run one epoch of the genetic algorithm
                population.RunEpoch();

                // Get the best individual in the current population
                BinaryChromosome? bestChromosome = population.BestChromosome as BinaryChromosome;

                // Get the fitness of the best individual in this epoch
                double currentFitness = fitnessFunction.Evaluate(bestChromosome);

                Console.WriteLine(CentreMessage($"Epoch {epochCount,4} | Current Fitness: {currentFitness} | Best Fitness: {bestFitness}")); // take up space with binary instead?

                if (currentFitness > bestFitness)
                {
                    bestFitness = currentFitness;
                    if (bestChromosome != null)
                    { // mix all into one
                        fitnessFunction.ResetCube();
                        Console.WriteLine();
                        fitnessFunction.Display(bestChromosome);
                        fitnessFunction.Inspect(bestChromosome);
                        fitnessFunction.ShowPoints(bestChromosome);
                        Console.ReadLine();
                    }
                }

                if (currentFitness >= targetFitness)
                {
                    Console.WriteLine(CentreMessage($"Target fitness of {targetFitness} reached. Stopping."));
                    // TODO: Display moves in a readable form.
                    break;
                }

                // Increase mutation rate to compensate
                if (epochCount % 5000 == 0 && population.MutationRate <= 0.9)  // Population limits it to 1.0 automatically, but this prevents messages
                { // Should this be done after x same fitness values instead of every x epochs?
                    Console.WriteLine(CentreMessage($"Increasing mutation rate from {population.MutationRate:F3} to {population.MutationRate + 0.1:F3} and forcing mutations."));
                    population.MutationRate += 0.1;

                    for (int i = 0; i < population.Size; i++)
                    {
                        for (int n = 0; n < 1; n++)
                        { // Force all chromosomes to mutate. Optionally do multiple successive mutations.
                            //(population[i] as BinaryChromosome)?.Mutate();
                        }
                    }
                }

                if (epochCount % 1000 == 0) { Console.Read(); } // store pause point in a variable?
            }

            while (Console.ReadKey(true).Key == ConsoleKey.Enter)
            {
                // Prevent program from terminating unless something is entered
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("____ _  _ ___  _ _  _ . ____    ____ _  _ ___  ____    ____ ____ _    _  _ ____ ____ ");
            Console.WriteLine("|__/ |  | |__] | |_/  ' [__     |    |  | |__] |___    [__  |  | |    |  | |___ |__/ ");
            Console.WriteLine("|  \\ |__| |__] | | \\_   ___]    |___ |__| |__] |___    ___] |__| |___  \\/  |___ |  \\ ");
            Console.WriteLine();
            Console.WriteLine("      ____ ____ _  _ ____ ___ _ ____    ____ _    ____ ____ ____ _ ___ _  _ _  _     ");
            Console.WriteLine("      | __ |___ |\\ | |___  |  | |       |__| |    | __ |  | |__/ |  |  |__| |\\/|     ");
            Console.WriteLine("      |__] |___ | \\| |___  |  | |___    |  | |___ |__] |__| |  \\ |  |  |  | |  |     ");
            Console.WriteLine();
            GeneticAlgorithm(
                mutationRate: 0.2);
        }
    }
}
