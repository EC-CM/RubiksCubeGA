# Rubik's Cube: Genetic Algorithm
This project was created for the Artificial Intelligence module during the second year of my degree, submitted in December 2023.

The cube state and functions are represented with their own class. A 64 bit binary chromosome represents a random sequence of 16 moves, with each four bits representing 1 of 16 possible move types. Due to limitations, the "standing" layer (clockwise/anticlockwise) and the option to not move are not represented.

The fitness function evaluates each face of the cube and gives each a higher score based on the number of squares surrounding the middle square that match its colour. A completed face scores more points and a face with no matches is penalised.

The program works, but it struggles to solve much more than a few shuffles. I have found that shuffling the cube 4 times is good for demonstration purposes.

One improvement may be to change the fitness function to better represent how close a cube is to being solved. Another may be to run two genetic algorithms in parallel and merging populations at some point. Different chromosomes, such as permutation chromosomes, may work also. 


Note:
I did not practice version control through this project, so I have compiled all saved versions in order of modification date to track historical changes. 

