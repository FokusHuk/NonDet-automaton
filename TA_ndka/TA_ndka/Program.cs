using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TA_ndka
{
    partial class Program
    {
        static int NodesCount; // количество вершин
        static string[] NKAEndNodes; // конечные вершины
        static string[,] G; // граф, содержащий метки существующих дуг НКА
        static string[] Labels; // метки НКА
        static string[] Indexes; // индексы вершин НКА
        static List<string> MarkedArcs; // помеченные дуги НКА
        
        static List<string> Ciphers; // Шифры состояний ДКА
        static List<List<string>> Paths; // Переходы в ДКА для каждого состояния
        static List<string> DKAEndNodes; // конечные вершины
        static List<string> Suffixes; // Суффиксы минимизированного автомата

        static void Main(string[] args)
        {
            const string path = @"C:\Users\Ilya Axenov\source\repos\TA_ndka\TA_ndka\InputData.txt";            

            Init(path);            
            //ShowGMatrix();
            SetMarkedArcsNumbers();
            ShowMarkedArcsNumbers();

            for (int i = 0; i < NodesCount; i++)
                SetIndex(i, i);

            ShowNKAtable();

            CreateDKAtable();
            SetDKAendNodes();
            ShowDKAtable();

            Minimalize();

            Console.ReadLine();
        }

        private static void Init(string path)
        {
            StreamReader reader = new StreamReader(@path);

            NodesCount = Convert.ToInt32(reader.ReadLine());

            Labels = reader.ReadLine().Split(' ');

            NKAEndNodes = reader.ReadLine().Split(' ');

            G = new string[NodesCount, NodesCount];

            for (int i = 0; i < NodesCount; i++)
                for (int k = 0; k < NodesCount; k++)
                    G[i, k] = " ";

            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(' ');

                if (G[Convert.ToInt32(line[0]), Convert.ToInt32(line[1])] == " ")
                    G[Convert.ToInt32(line[0]), Convert.ToInt32(line[1])] = line[2];
                else
                    G[Convert.ToInt32(line[0]), Convert.ToInt32(line[1])] += " " + line[2];
            }

            Indexes = new string[NodesCount];
            Indexes[0] = "1";
            for (int i = 1; i < NodesCount; i++)
                Indexes[i] = "";
        }

        private static void ShowNKAtable()
        {
            Console.WriteLine("Таблица НКА.");

            Console.Write("Вершина НКА\t");
            Console.Write("Fl\t");
            foreach (string label in Labels)
                Console.Write(label + "\t");
            Console.Write("@\t");
            Console.WriteLine("Индекс");

            for (int i = 0; i < NodesCount; i ++)
            {
                Console.Write("{0, -11}\t", i);

                if (NKAEndNodes.Contains(i.ToString()))
                    Console.Write("1 \t");
                else
                    Console.Write("  \t");

                for (int j = 0; j < Labels.Length; j ++)
                {
                    Console.Write(InNodes(i, Labels[j]) + "\t");
                }

                Console.Write(InNodes(i, "@") + "\t");
                Console.WriteLine("{0, -6}", Indexes[i]);
            }

            Console.WriteLine();
        }

        private static string InNodes(int node, string label = "")
        {
            string inNodes = "";

            for (int j = 0; j < NodesCount; j++)
            {
                if (label == "")
                {
                    if (G[node, j] != " ")
                        inNodes += j + " ";
                }
                else
                {
                    string[] labels = G[node, j].Split(' ');

                    if (labels.Contains(label))
                        inNodes += j + " ";
                }
            }

            if (inNodes != "")
            {
                inNodes = inNodes.Remove(inNodes.Length - 1);
            }

            return inNodes;
        }

        private static string OutNodes(int node)
        {
            string outNodes = "";

            for (int j = 0; j < NodesCount; j++)
            {
                if (G[j, node] != " ")
                    outNodes += j + " ";
            }

            if (outNodes != "")
            {
                outNodes = outNodes.Remove(outNodes.Length - 1);
            }

            return outNodes;
        }

        private static void ShowGMatrix()
        {
            Console.WriteLine("##############################");
            Console.Write("  ");
            for (int i = 0; i < NodesCount; i++)
                Console.Write(i + " ");
            Console.WriteLine();

            for (int i = 0; i < NodesCount; i++)
            {
                Console.Write(i + " ");

                for (int j = 0; j < NodesCount; j++)
                {
                    Console.Write(G[i,j] + " ");
                }

                Console.WriteLine();
            }
            Console.WriteLine("##############################\n");
        }

        private static void SetMarkedArcsNumbers()
        {
            MarkedArcs = new List<string>();
            MarkedArcs.Add("0 0");

            for (int i = 0; i < NodesCount; i++)
                for (int j = 0; j < Labels.Length; j ++)
                {
                    string inNodes = InNodes(i, Labels[j]);

                    if (inNodes == "")
                        continue;

                    string[] inNodesMatrix = inNodes.Split(' ');

                    MarkedArcs.Add("");

                    for (int k = 0; k < inNodesMatrix.Length; k++)
                        MarkedArcs[MarkedArcs.Count - 1] += i + " " + inNodesMatrix[k] + "|";

                    MarkedArcs[MarkedArcs.Count - 1] = MarkedArcs[MarkedArcs.Count - 1].Remove(MarkedArcs[MarkedArcs.Count - 1].Length - 1);                        
                }

            for (int i = 1; i < MarkedArcs.Count; i ++)
            {
                string[] arc = MarkedArcs[i].Split('|')[0].Split(' ');

                string[] inNodes = G[Convert.ToInt32(arc[0]), Convert.ToInt32(arc[1])].Split(' ');

                for (int k = 0; k < inNodes.Length; k ++)
                {
                    if (inNodes[k] != "@")
                    {
                        if (!Labels.Contains(MarkedArcs[i][MarkedArcs[i].Length - 1].ToString()))
                            MarkedArcs[i] += " " + inNodes[k];
                        else
                            MarkedArcs[++i] += " " + inNodes[k];
                    }
                }
            }
        }

        private static void ShowMarkedArcsNumbers()
        {
            Console.WriteLine("Номера помеченных переходов:");
            for (int i = 1; i < MarkedArcs.Count; i++)
            {
                string[] arcs = MarkedArcs[i].Split('|');

                Console.Write("[" + (i + 1) + "] ");
                foreach (string arc in arcs)
                    Console.Write(arc + "; ");

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static void SetIndex(int node, int currentNode, string GPath = "")
        {
            GPath += node;

            string[] outNodes = OutNodes(currentNode).Split(' ');

            if (outNodes.Length == 0 || outNodes.Length == 1 && outNodes[0] == "")
                if (!Indexes[node].Contains("1"))
                {
                    Indexes[node] += "1";
                    return;
                }

            foreach (string on in outNodes)
            {
                List<int> indexes = new List<int>();

                for (int k = 1; k < MarkedArcs.Count; k++)
                    if (MarkedArcs[k].Contains(on + " " + currentNode.ToString()))
                    {
                        indexes.Add(k + 1);
                    }

                if (indexes.Count != 0)
                {
                    foreach (int index in indexes)
                    {
                        if (!Indexes[node].Contains(index.ToString()))
                        {
                            if (Indexes[node] != "")
                                Indexes[node] += "," + index.ToString();
                            else
                                Indexes[node] += index.ToString();
                        }
                    }

                    for (int i = 0; i < indexes.Count; i++)
                    {
                        string[] arcs = MarkedArcs[indexes[i] - 1].Split('|');

                        arcs[arcs.Length - 1] = arcs[arcs.Length - 1].Remove(arcs[arcs.Length - 1].Length - 2);

                        foreach (string arc in arcs)
                        {
                            string[] a = arc.Split(' ');

                            string[] labels = G[Convert.ToInt32(a[0]), Convert.ToInt32(a[1])].Split(' ');

                            if (labels.Contains("@") && !GPath.Contains(on))
                                SetIndex(node, Convert.ToInt32(on), GPath);
                        }
                    }
                }              

                if (indexes.Count == 0 && !GPath.Contains(on))
                    SetIndex(node, Convert.ToInt32(on), GPath);
            }
        }

        private static void ShowDKAtable(bool showSuffixes = false)
        {
            Console.WriteLine("Таблица ДКА.");

            Console.Write("{0, -16} {1, -2}\t", "Шифр", "Fl");
            foreach (string label in Labels)
                Console.Write("{0, -16} ", label);
            if (showSuffixes)
                Console.Write("{0, -16} ", "Suffix");
            Console.WriteLine();

            for (int i = 0; i < Ciphers.Count; i ++)
            {
                Console.Write("{0, -16} ", Ciphers[i]);

                if (DKAEndNodes.Contains(Ciphers[i]))
                {
                    if (Ciphers[i] == "0")
                        Console.Write("0 \t");
                    else
                        Console.Write("{0, -2}\t", DKAEndNodes.IndexOf(Ciphers[i]) + 1);
                }
                else
                    Console.Write("  \t");

                for (int j = 0; j < Labels.Length; j++)
                    Console.Write("{0, -16} ", Paths[i][j]);

                if (showSuffixes)
                    Console.Write(Suffixes[i]);

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static void CreateDKAtable()
        {
            Ciphers = new List<string>();
            Paths = new List<List<string>>();

            SetDKAnode("1", 0);

            List<string> UnrealisedCiphers;

            while ((UnrealisedCiphers = GetUnrealisedCiphers()).Count != 0)
            {
                foreach (string cipher in UnrealisedCiphers)                
                    SetDKAnode(cipher, Ciphers.Count);
            }

            for (int i = 0; i < Paths.Count; i++)
                for (int k = 0; k < Paths[i].Count; k++)
                    if (Paths[i][k] == "")
                        Paths[i][k] = "0";
        }

        private static List<string> GetUnrealisedCiphers()
        {
            List<string> UnrealisedCiphers = new List<string>();

            for (int j = 0; j < Ciphers.Count; j++)
                for (int i = 0; i < Paths[0].Count; i++)
                {
                    if (!Ciphers.Contains(Paths[j][i]) && Paths[j][i] != "")
                        UnrealisedCiphers.Add(Paths[j][i]);
                }

            return UnrealisedCiphers;
        }

        private static void SetDKAnode(string cipher, int index)
        {
            Ciphers.Add(cipher);
            Paths.Add(new List<string>());
            for (int i = 0; i < Labels.Length; i++)
                Paths[index].Add("");

            for (int j = 0; j < Labels.Length; j++)
            {
                for (int i = 1; i < MarkedArcs.Count; i++)
                {
                    string[] nodes = MarkedArcs[i].Split('|');

                    nodes[nodes.Length - 1] = nodes[nodes.Length - 1].Remove(nodes[nodes.Length - 1].Length - 2);

                    for (int f = 0; f < nodes.Length; f++)
                    {
                        string[] node = nodes[f].Split(' ');

                        if (MarkedArcs[i][MarkedArcs[i].Length - 1].ToString() == Labels[j])
                        {
                            bool haveIndex = false;

                            string[] ciphers = cipher.Split(',');
                            string[] indexes = Indexes[Convert.ToInt32(node[0])].Split(',');

                            for (int h = 0; h < indexes.Length; h++)
                                for (int k = 0; k < ciphers.Length; k++)
                                {
                                    if (indexes[h] == ciphers[k])
                                    {
                                        haveIndex = true;
                                        break;
                                    }
                                }

                            if (haveIndex)
                            {
                                if (!Paths[index][j].Contains((i + 1).ToString()))
                                {
                                    if (Paths[index][j] != "")
                                        Paths[index][j] += "," + (i + 1).ToString();
                                    else
                                        Paths[index][j] += (i + 1).ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void SetDKAendNodes()
        {
            DKAEndNodes = new List<string>();

            string[] EndNodesIndexes = new string[NKAEndNodes.Length];

            for (int i = 0; i < NKAEndNodes.Length; i ++)
                EndNodesIndexes[i] = Indexes[Convert.ToInt32(NKAEndNodes[i])];

            for (int i = 0; i < Ciphers.Count; i++)
                for (int j = 0; j < EndNodesIndexes.Length; j++)
                {
                    string[] endIndexes = EndNodesIndexes[j].Split(',');

                    for (int k = 0; k < endIndexes.Length; k++)
                        if (Ciphers[i].Contains(endIndexes[k]))
                            DKAEndNodes.Add(Ciphers[i]);
                }

            for (int i = 0; i < DKAEndNodes.Count; i++)
            {
                int EndNodeIndex = Ciphers.IndexOf(DKAEndNodes[i]);

                for (int j = 0; j < Paths[EndNodeIndex].Count; j++)
                    if (Paths[EndNodeIndex][j] == "" || Paths[EndNodeIndex][j] == "0")
                        Paths[EndNodeIndex][j] = "$";
            }

            bool isErrorEndNodeExist = false;
            foreach (List<string> paths in Paths)
                foreach (string p in paths)
                {
                    if (p == "0")
                        isErrorEndNodeExist = true;
                }

            if (isErrorEndNodeExist)
            {
                Ciphers.Add("0");
                Paths.Add(new List<string>());
                for (int i = 0; i < Labels.Length; i++)
                    Paths[Paths.Count - 1].Add("$");
                DKAEndNodes.Add("0");
            }
        }

        // Новая таблица:
        // 1) удалить повторяющиеся состояния (заменить одним; тем, которое входит в таблицу последний раз)
        // 2) изменить соответствующие переходы в таблице
        // 3) назначить начальные суффиксы "A"
        //
        // Новая таблица:
        // 4) изменить соотвествующие переходы, отмечая их через суффиксы
        // 5) назначить новые суффиксы, при этом повторяющимся строкам присваивается одинаковое значение суффикса
        //
        // Строим новые таблицы, пока кол-во различных суффиксов увеличивается или пока их кол-во не будет равно кол-ву состояний ДКА:
        // 6) изменить соотвествующие переходы, отмечая их через суффиксы
        // Новые суффиксы назначаются тем строкам, которые имеют одинаковые суффиксы:
        // 7) назначить новые суффиксы, при этом повторяющимся строкам присваивается одинаковое значение суффикса
        //
        // 
    }
}
