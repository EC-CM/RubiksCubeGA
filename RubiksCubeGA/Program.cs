using Accord;
using Accord.Genetic;

namespace RubiksCubeGA
{
    public class Program
    {
        public static void GeneticAlgorithm(
            int populationSize = 100,
            int randomSelectionPortion = 10,
            double mutationRate = 0.1,
            double crossoverRate = 0.3,
            int targetFitness = 20)
        {
            RubiksCube myCube = new RubiksCube();
            myCube.Shuffle(100, true);
            myCube.Display();

            BinaryChromosome firstChromosome = new BinaryChromosome(64);
            RubiksCubeFitnessFunction fitnessFunction = new RubiksCubeFitnessFunction(myCube.cubeState.Clone() as char[,,]);
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

            Console.WriteLine();
            Console.WriteLine("                     Press ENTER to run the genetic algorithm.                     ");
            Console.ReadLine();
            myCube.Display();
            Console.WriteLine();


            while (true)
            {
                epochCount++;

                // Run one epoch of the genetic algorithm
                population.RunEpoch();

                // Get the best individual in the current population
                BinaryChromosome bestChromosome = population.BestChromosome as BinaryChromosome;

                // Get the fitness of the best individual in this epoch
                double currentFitness = fitnessFunction.Evaluate(bestChromosome);

                Console.WriteLine($"                  Epoch {epochCount} | Current Fitness: {currentFitness} | Best Fitness: {bestFitness}"); // take up space with binary instead?

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

                if (bestFitness >= targetFitness)
                {
                    Console.WriteLine($"Target fitness of {targetFitness} reached. Stopping.");
                    if (bestChromosome != null)
                    {
                        fitnessFunction.ResetCube();
                        Console.WriteLine();
                        fitnessFunction.Display(bestChromosome);
                        fitnessFunction.Inspect(bestChromosome);
                        fitnessFunction.ShowPoints(bestChromosome);
                        Console.ReadLine();
                    }
                    break;
                }

                if (epochCount % 1000 == 0) { Console.ReadLine(); } // store pause point in a variable?
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
                mutationRate: 0.2,
                targetFitness: 114); //Current max
        }
    }
}
