using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_ndka
{
    partial class Program
    {
        // Минимизация автомата

        private static void Minimalize()
        {            
            Console.WriteLine("МИНИМИЗАЦИЯ");

            DeleteRepeatedStates();
            //CutStates();
            SortStates();
            SetStartedSuffixes();
            ShowDKAtable(true);

            List<int> repeatedStates;

            while ((repeatedStates = GetRepeatedStates()).Count != 0)
            {
                ChangeTransitions();
                ClearUnrepeatedStates();
                SetSuffixes(repeatedStates);
                ShowDKAtable(true);
            }
        }

        private static void DeleteRepeatedStates()
        {
            for (int i = 0; i < Paths.Count; i++)
            {
                for (int j = i + 1; j < Paths.Count; j++)
                {
                    bool areEquals = true;

                    for (int k = 0; k < Labels.Length; k++)
                        if (Paths[i][k] != Paths[j][k])
                        {
                            areEquals = false;
                            break;
                        }

                    if (areEquals && Paths[i][0] != "$" && !DKAEndNodes.Contains(Ciphers[j]) && !DKAEndNodes.Contains(Ciphers[i]))
                    {
                        Console.WriteLine("Найдены эквивалентные состояния: {0} и {1}\n", Ciphers[i], Ciphers[j]);

                        Paths[i].Clear();
                        Paths[i] = null;

                        for (int k = 0; k < Paths.Count; k++)
                            for (int f = 0; f < Labels.Length; f++)
                            {
                                if (Paths[k] == null)
                                    continue;

                                if (Paths[k][f].Contains(Ciphers[i]))
                                    Paths[k][f] = Paths[k][f].Replace(Ciphers[i], Ciphers[j]);
                            }

                        for (int k = 0; k < Ciphers.Count; k++)
                            if (k != i && Ciphers[k].Contains(Ciphers[i]))
                                Ciphers[k] = Ciphers[k].Replace(Ciphers[i], Ciphers[j]);

                        break;
                    }
                }
            }

            for (int i = 0; i < Paths.Count; i++)
                if (Paths[i] == null)
                {
                    Paths.Remove(Paths[i]);
                    Ciphers.Remove(Ciphers[i]);
                }

        }

        private static void CutStates()
        {
            for (int k = 0; k < Ciphers.Count; k++)
                Ciphers[k] = Ciphers[k].Split(',')[0];

            for (int i = 0; i < Paths.Count; i++)
                for (int j = 0; j < Labels.Length; j++)
                    if (Paths[i][j] != "$")
                    {
                        Paths[i][j] = Paths[i][j].Split(',')[0];
                    }
        }

        private static void SortStates()
        {
            for (int i = Ciphers.Count - 1; i >= 0; i--)
            {
                if (DKAEndNodes.Contains(Ciphers[i]))
                {
                    Ciphers.Remove(Ciphers[i]);
                    Paths.Remove(Paths[i]);
                }
            }

            string tempCipher;
            List<string> tempPath;

            for (int i = 0; i < Ciphers.Count; i++)
                for (int j = i + 1; j < Ciphers.Count; j++)
                    if (Convert.ToInt32(Ciphers[i].Split(',')[0]) > Convert.ToInt32(Ciphers[j].Split(',')[0]))
                    {
                        tempCipher = Ciphers[i];
                        Ciphers[i] = Ciphers[j];
                        Ciphers[j] = tempCipher;

                        tempPath = Paths[i];
                        Paths[i] = Paths[j];
                        Paths[j] = tempPath;
                    }

            for (int i = 0; i < DKAEndNodes.Count; i++)
                for (int j = i + 1; j < DKAEndNodes.Count; j++)
                    if (Convert.ToInt32(DKAEndNodes[i]) > Convert.ToInt32(DKAEndNodes[j]))
                    {
                        string tempNode = DKAEndNodes[i];
                        DKAEndNodes[i] = DKAEndNodes[j];
                        DKAEndNodes[j] = tempNode;
                    }

            DKAEndNodes.Add(DKAEndNodes[0]);
            DKAEndNodes.RemoveAt(0);

            Ciphers.AddRange(DKAEndNodes);
            for (int i = 0; i < DKAEndNodes.Count; i++)
            {
                Paths.Add(new List<string>());

                for (int k = 0; k < Labels.Length; k++)
                    Paths[Paths.Count - 1].Add("$");
            }
        }

        private static void SetStartedSuffixes()
        {
            string[] baseSuffixes = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "M", "N", "O", "P", "Q", "R", "S", "T" };

            int endFlagIndex = 1;

            Suffixes = new List<string>();
            for (int j = 0; j < Ciphers.Count; j++)
            {
                if (DKAEndNodes.Contains(Ciphers[j]))
                    Suffixes.Add(baseSuffixes[endFlagIndex++]);
                else
                    Suffixes.Add(baseSuffixes[0]);
            }
        }

        private static void ChangeTransitions()
        {
            for (int i = 0; i < Paths.Count; i++)
                for (int j = 0; j < Labels.Length; j++)
                {
                    if (Paths[i][j] != "$" && Paths[i][j] != "")
                    {
                        if (Paths[i][j].Contains("-"))
                            Paths[i][j] = Paths[i][j].Remove(Paths[i][j].IndexOf("-"));
                        Paths[i][j] += "-" + Suffixes[Ciphers.IndexOf(Paths[i][j])];
                    }
                }
        }

        private static void ClearUnrepeatedStates()
        {
            for (int i = 0; i < Suffixes.Count; i++)
            {
                bool stateRepeated = false;

                for (int j = 0; j < Suffixes.Count; j++)
                    if (i != j && Suffixes[i] == Suffixes[j])
                    {
                        stateRepeated = true;
                        break;
                    }

                if (!stateRepeated)
                {
                    for (int k = 0; k < Labels.Length; k++)
                        Paths[i][k] = "";
                }
            }
        }

        private static List<int> GetRepeatedStates()
        {
            List<int> repeatedStates = new List<int>();

            for (int i = 0; i < Ciphers.Count; i++)
                for (int j = i + 1; j < Ciphers.Count; j++)
                    if (Suffixes[i] == Suffixes[j])
                    {
                        if (!repeatedStates.Contains(i))
                            repeatedStates.Add(i);
                        if (!repeatedStates.Contains(j))
                            repeatedStates.Add(j);
                    }

            return repeatedStates;
        }

        private static void SetSuffixes(List<int> repeatedStates)
        {
            int freeIndex = 1;
            bool areEquals = false;

            foreach (int i in repeatedStates)
            {
                for (int j = 0; j < i; j++)
                {
                    areEquals = true;

                    for (int k = 0; k < Labels.Length; k++)
                        if (Paths[i][k].Remove(0, Paths[i][k].IndexOf("-") + 1) != Paths[j][k].Remove(0, Paths[j][k].IndexOf("-") + 1))
                        {
                            areEquals = false;
                            break;
                        }

                    if (areEquals)
                    {
                        Suffixes[i] = Suffixes[j];
                        break;
                    }
                }

                if (!areEquals)
                {
                    Suffixes[i] += freeIndex;
                    freeIndex++;
                }
            }
        }
    }
}
