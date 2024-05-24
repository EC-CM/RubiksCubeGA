/*

	static void GA(
		int populationSize=100,
		int randomSelectionPortion=10, 
		int targetFitness=20,
		double mutationRate=0.1,
		double crossoverRate=0.3)
	{
		RubiksCube myCube = new RubiksCube();
		myCube.Shuffle(100, true);
		myCube.Display();

		BinaryChromosome firstChromosome = new BinaryChromosome(64); // deal with dynamic bit depth later
		RubiksCubeFitnessFunction fitnessFunction = new RubiksCubeFitnessFunction(myCube.cubeState.Clone() as char[,,]); //.Clone() as char[,,] stops main cube being modified
		EliteSelection selectionMethod = new EliteSelection();

		Population population = new Population(
			populationSize,
			firstChromosome,
			fitnessFunction,
			selectionMethod);
		myCube.Display();

		population.MutationRate = mutationRate; // Percentage of chromosomes that will undergo bit mutation
		population.CrossoverRate = crossoverRate;
		population.RandomSelectionPortion = randomSelectionPortion; //Number of the population that will be selected to breed, regardless of fitness

		int epochCount = 0;
		double bestFitness = 0;

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



 */


/*
public class RubiksCubeSolvingFitnessFunction : IFitnessFunction
{
	public double fitness = 0;
	//private char[,,]? originalCubeState;

	public RubiksCubeSolvingFitnessFunction(){}

	/*
	public double Evaluate(IChromosome chromosome)
	{
		//Console.WriteLine($"Evaluate: {chromosome.ToString()}");
		fitness = 0; // Reset fitness upon each evaluation

		// Initialise original cube state globally
		//originalCubeState = rubiksCubeChromosome.originalCubeState;

		// Limit focus
		fitness = Eval(rubiksCubeChromosome.rubiksCube.cubeState);

		return fitness; 
	}


	private double Evaluate(IChromosome chromosome)
	{
		if (chromosome is RubiksCubeChromosome rubiksCubeChromosome)
		{
			// Goal: Many of the same colour (matching the middle) on each face.

			char target_colour = rubiksCubeChromosome[0, 1, 1];
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

		return 0.0;
	}
}
*/