using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Pacman.Simulator;
using System.Diagnostics;


public class Evolve {

    static int popsize = 30;
    static int arraysize  = 10;
    static int elite = popsize / 2;
    static int evaluationRepetitions = 1;
    static double[][] population = new double[popsize][arraysize];
    static int generations = 200;
    static double[] fitness = new double[popsize];

    public static void main (String[] args) {
        for (int i = 0; i < population.Length; i++) {
            population[i] = randomIndividual ();
        }
        double lastBestFitness = 0;
        for (int i = 0; i < generations; i++) {
            oneMoreGeneration ();
            if (fitness[0] > lastBestFitness) {
                lastBestFitness = fitness[0];
            }
            Console.Write ("Gen %d fitness %.4f\n", i, fitness[0]);
        }
    }

    public static double[] randomIndividual () {
        Random random = new Random ();
        double[] individual = new double[arraysize];
        for (int i = 0; i < individual.Length; i++) {
            individual[i] = random.Next();
        }
        return individual;
    }

    public static void oneMoreGeneration () {
        for (int i = 0; i < elite; i++) {
            evaluate (i);
        }
        for (int i = elite; i < population.Length; i++) {
            Array.Copy(population[i - elite], 0, population[i], 0, arraysize);
            mutate(population[i]);
            evaluate (i);
        }
        sortPopulationByFitness ();
    }

    private static void mutate (double[] individual) {
        Random random = new Random ();
        for (int i = 0; i < individual.Length; i++) {
            individual[i] += random.Next() * 0.1;
        }
    }

    private static void evaluate (int which) {
        fitness[which] = 0;
        for (int i = 0; i < evaluationRepetitions; i++) {
            fitness[which] += evaluate(population[which]);
        }
        fitness[which] = fitness[which] / evaluationRepetitions;
    }

    private static double evaluate (double[] individual) {
        // here goes a fitness function!
        return 0;
    }


    private static void sortPopulationByFitness () {
        for (int i = 0; i < population.Length; i++) {
            for (int j = i + 1; j < population.Length; j++) {
                if (fitness[i] < fitness[j]) {
                    double cache = fitness[i];
                    fitness[i] = fitness[j];
                    fitness[j] = cache;
                    double[] gcache = population[i];
                    population[i] = population[j];
                    population[j] = gcache;
                }
            }
        }
    }
}
