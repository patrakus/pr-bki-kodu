using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorytm_genetyczny
{
    class TSP
    {
        List<int[]> oldGeneration;                  // tablica przechowująca obecną populacje , która ma w sobie informacje o kolejności miast
        List<int[]> newGeneration;                  // nowa generacja po selekcji, krzyżowaniu i mutacji
        Random random;
        int population;
        int[] theBestGenom;                         // najlepsze zestawienie miast
        int theBestValue;                           // najkrótsza znaleziona droga
        readonly int[][] distanceMatrix;            // macierz zawierająca odległości między miastami
        int[] fitness;                              // tablica zawierająca informacje o totalnej długości drogi między miastami z każdego z osobników populacji


        public TSP(int population, int[][] distanceMatrix)
        {
            oldGeneration = new List<int[]>();
            newGeneration = new List<int[]>();

            this.population = population;
            random = new Random();
            theBestGenom = new int[distanceMatrix[0].Length];
            this.distanceMatrix = distanceMatrix;

            generatePopulation();

            fitness = new int[population];
        }

        /*
         * Generuje pierwszą populacje z losowymi indeksami do każdego z miast
         */
        void generatePopulation()
        {
            int[] kopia = new int[distanceMatrix.Length];
            int[] indeksy = new int[distanceMatrix.Length];

            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                kopia[i] = i;
            }

            List<int> indeksList = new List<int>(kopia);

            for (int i = 0; i < population; i++)
            {
                indeksy = new int[distanceMatrix.Length];
                for (int j = 0; j < distanceMatrix.Length; j++)
                {
                    int indeksWLiscie = random.Next(0, indeksList.Count);

                    indeksy[j] = indeksList[indeksWLiscie];

                    indeksList.Remove(indeksy[j]);
                }
                oldGeneration.Add(indeksy);

                indeksList.AddRange(kopia);
            }

        }

        // oblicza drogi wszystkich miast
        void calculatFitness()
        {
            fitness = new int[population];

            for (int i = 0; i < population; i++)
            {
                for (int j = 0; j < distanceMatrix.Length; j++)
                {
                    if (j < (distanceMatrix.Length - 1))
                    {
                        /*
                         * Tutaj najpierw się odwołujemy do indeksu w liście a potem do indeksu w tablicy int
                         */
                        int k = oldGeneration[i][j];
                        int l = oldGeneration[i][j + 1];

                        fitness[i] += distanceMatrix[k][l];
                    }
                    else
                    {
                        int k = oldGeneration[i][j];
                        int l = oldGeneration[i][0];

                        fitness[i] += distanceMatrix[k][l];
                    }
                }
            }
        }

        // https://www.tutorialspoint.com/genetic_algorithms/genetic_algorithms_parent_selection.htm
        // Tournament Selection
        int TournamentSelection(int participants)
        {
            int[] selecteds = new int[participants];

            for (int i = 0; i < participants; i++)
            {
                selecteds[i] = random.Next(0, oldGeneration.Count);
            }

            int lepszyOsobnik = selecteds[0];

            for (int i = 1; i < participants; i++)
            {
                if (fitness[lepszyOsobnik] > fitness[selecteds[i]])
                {
                    lepszyOsobnik = selecteds[i];
                }
            }

            return lepszyOsobnik;
        }

        // https://www.tutorialspoint.com/genetic_algorithms/genetic_algorithms_parent_selection.htm
        // Roulette Wheel Selection
        int roulette()
        {
            double[] tablicaRuletki = new double[oldGeneration.Count];
            //int[] indeksyDOOminięcia = new int[oldGeneration.Count];
            double sumaWag = 0;

            List<int> indices = new List<int>();

            for (int i = 0; i < tablicaRuletki.Length; i++)
            {
                indices.Add(i);
            }

            foreach (int dopasowanie in fitness)
            {
                sumaWag += dopasowanie;
            }

            for (int i = 0; i < tablicaRuletki.Length; i++)
            {
                tablicaRuletki[i] = fitness[i] / sumaWag;
            }


            // zamiana wartości wag. Najwieksza waga zostanie zamieniona z najmniejszą i tak dalej
            while (indices.Count != 0)
            {
                int indexForMax = FindMaxInArrayOfValues(indices, tablicaRuletki);
                int indexForMin = FindMinInArrayOfValues(indices, tablicaRuletki);

                Swap(ref tablicaRuletki[indexForMax], ref tablicaRuletki[indexForMin]);

                indices.Remove(indexForMax);
                indices.Remove(indexForMin);
            }

            // kręcimy kołem i losujemy

            double randomValue = random.NextDouble();

            for (int i = 0; i < tablicaRuletki.Length; i++)
            {
                randomValue -= tablicaRuletki[i];
                if (randomValue <= 0)
                {
                    return i;
                }
            }

            // for error information
            return -1;
        }

        // kryżowanie
        // https://www.youtube.com/watch?v=c2ft8AG8JKE
        // http://www.rubicite.com/Tutorials/GeneticAlgorithms/CrossoverOperators/PMXCrossoverOperator.aspx/
        void PMX(int[] parentA, int[]parentB)
        {

            int startPoint = random.Next(0, parentA.Length);
            int endPoint = random.Next(0, parentA.Length);

            if (endPoint < startPoint)
            {
                Swap(ref endPoint, ref startPoint);
            }

            //kreacja pierwszego dziecka

            int[] child1 = new int[parentA.Length];
            InitArrayWithValue(-1, child1);

            CopySegment(parentA, child1, startPoint, endPoint);
            CorrectChild(child1, parentA, parentB, startPoint, endPoint);
            CopyTheRest(child1, parentB);

            //Kreacja drugiego dziecka

            int[] child2 = new int[parentB.Length];
            InitArrayWithValue(-1, child2);
            CopySegment(parentB, child2, startPoint, endPoint);
            CorrectChild(child2, parentB, parentA, startPoint, endPoint);
            CopyTheRest(child2, parentA);

            newGeneration.Add(child1);
            newGeneration.Add(child2);

        }

        #region Mutation
        // https://www.tutorialspoint.com/genetic_algorithms/genetic_algorithms_mutation.htm
        void InverseMutation(int index, float szansaNaMutacje)
        {
            if (random.NextDouble() <= szansaNaMutacje)
            {
                int startPoint = random.Next(0, newGeneration[index].Length);
                int endPoint = random.Next(0, newGeneration[index].Length);

                // swap if endPoint is less than startPoint
                if (endPoint < startPoint)
                {
                    Swap(ref endPoint, ref startPoint);
                }

                for (int i = startPoint, j = endPoint; i <=j; i++, j--)
                {
                    Swap(ref newGeneration[index][i], ref newGeneration[index][j]);
                }
            }
        }
        // https://www.tutorialspoint.com/genetic_algorithms/genetic_algorithms_mutation.htm
        void SwapMutation(int index, float szansaNaMutacje)
        {
            if (random.NextDouble() <= szansaNaMutacje)
            {
                int pointOne = random.Next(0, newGeneration[index].Length);
                int pointTwo = random.Next(0, newGeneration[index].Length);

                Swap(ref newGeneration[index][pointOne], ref newGeneration[index][pointTwo]);
            }
        }

        #endregion

        /*
         *  iteriationCount - Ilość pokoleń jakich program wygeneruje w czasie swojego działania
         *  W tym miejscu wywołyłane są wszystkie metody algorytmu genetycznego i w tym samym miejscy dzieje się symulacja w celu
         *  znalezienia najlepszego miasta
         */
        public void Simulate(int iteriationCount)
        {

            for (int counter = 0; counter < iteriationCount; counter++)
            {
                calculatFitness();
                FindTheBest();


                int indexForParent1, indexForParent2;

                //var watch = System.Diagnostics.Stopwatch.StartNew();

                do
                {
                    indexForParent1 = TournamentSelection(5);
                    indexForParent2 = roulette();

                    // zakres między 0 a 1
                    // prawdopodobieństwo w jakim skrzyżują się 2 rodzice
                    float probabilityOfCrossOver = 0.8f;

                    if (random.NextDouble() <= probabilityOfCrossOver)
                    {
                        PMX(oldGeneration[indexForParent1], oldGeneration[indexForParent2]);
                        
                    }
                    else
                    {
                        // jak się nie skrzyżują no to rodzice są po prostu kopiowani
                        newGeneration.Add(oldGeneration[indexForParent1]);
                        newGeneration.Add(oldGeneration[indexForParent2]);
                    }


                } while (newGeneration.Count != oldGeneration.Count);

                //watch.Stop();
                //Console.WriteLine("Time for generating newGeneration: " + watch.ElapsedMilliseconds);
                //Console.ReadLine();

                // zakres między 0 do 1
                // szansa na wystąpienie mutacji u danego osobniga w populacji
                float probalityOfMutation = 0.05f;

                for (int i = 0; i < newGeneration.Count; i++)
                {
                    InverseMutation(i, probalityOfMutation);
                    SwapMutation(i, probalityOfMutation);
                }

                // zamiana starej generacji na nową
                Swap(ref newGeneration, ref oldGeneration);
                // czyszczenie listy która już zawiera informacje o starym pokoleniu
                newGeneration.Clear();

                //Console.WriteLine("OK");
            }


        }

        // znajduje obecnie najlepszego osobnika z najkrótszą drogą
        void FindTheBest()
        {
            int probablyTheBest = fitness[0];
            int index = 0;

            for (int i = 0; i < fitness.Length; i++)
            {
                if (probablyTheBest > fitness[i])
                {
                    probablyTheBest = fitness[i];
                    index = i;
                }
            }

            theBestGenom = oldGeneration[index];
            theBestValue = fitness[index];
        }

        // Helper methods
        private void Swap<T>(ref T variableA, ref T variableB)
        {
            T temporary = variableA;
            variableA = variableB;
            variableB = temporary;
        }

        private int FindMaxInArrayOfValues(List<int> indices, double[] arrayOfValues)
        {
            int index = 0;
            double max = arrayOfValues[0];

            for (int i = 1; i < fitness.Length; i++)
            {
                if (max <= arrayOfValues[i] && indices.Contains(i))
                {
                    max = arrayOfValues[i];
                    index = i;
                }
            }

            return index;
        }

        private int FindMinInArrayOfValues(List<int> indices, double[] arrayOfValues)
        {
            int index = 0;
            double min = arrayOfValues[0];

            for (int i = 1; i < fitness.Length; i++)
            {
                if (min >= arrayOfValues[i] && indices.Contains(i))
                {
                    min = arrayOfValues[i];
                    index = i;
                }
            }

            return index;
        }

        // Kopiuje wartości od rodzica A
        private void CopySegment(int[] parent, int[] child, int startPoint, int endPoint)
        {
            for (int i = startPoint;  i <= endPoint ; i++)
            {
                child[i] = parent[i];
            }
        }

        // Zapewnia poprawne przekopwioane elementów z rodzica a i b. Jest to otrzebne w celu uniknięcia duplikatów.
        // Gdyby tego nie było to duplikaty mogłyby zapełnić tablicę po jakimś czasie i tym samym byłaby informacja tylko o 1 mieście
        void CorrectChild(int[] child, int[] parentA, int[] parentB, int startPoint, int endPoint)
        {
            for (int i = startPoint; i <= endPoint; i++)
            {

                if (!ContainInArray(parentB[i], child))
                {
                    int valueToCopy = parentB[i];

                    int index = SearchForIndex(parentA[i], parentB);

                    while(index >= startPoint && index <= endPoint)
                    {
                        index = SearchForIndex(parentA[index], parentB);
                    }

                    child[index] = valueToCopy;
                }
            }
        }

        bool ContainInArray(int value, int[] array)
        {
            for (int j = 0; j < array.Length; j++)
            {
                if (value == array[j])
                {
                    return true;
                }
            }

            return false;
        }

        int SearchForIndex(int value, int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (value == array[i])
                {
                    return i;
                }
            }

            return -1;
        }

        void InitArrayWithValue(int value, int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        // kopiuje pozostałe wartości z rodzicza B do dziecka
        void CopyTheRest(int[] child, int[] parentB)
        {
            for (int i = 0; i < child.Length; i++)
            {
                if (child[i] == -1)
                {
                    child[i] = parentB[i];
                }
            }
        }

        public override string ToString()
        {
            string array = "";

            for (int i = 0; i < theBestGenom.Length; i++)
            {
                if( i < theBestGenom.Length -1)
                {
                    array += theBestGenom[i].ToString() + "-";
                }
                else
                {
                    array += theBestGenom[i].ToString();
                }
                
            }

            return array + " " + theBestValue.ToString();
        }
    }
}