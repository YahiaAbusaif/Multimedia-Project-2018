﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace multimedia
{
    class Node
    {
        public int number;
        public string data;
        public string code;
        public Node leftChild, rightChild;

        public Node(string data, int number)
        {
            this.data = data;
            this.number = number;
            this.code = "";
            this.leftChild = null;
            this.rightChild = null;

        }

        public Node(Node leftChild, Node rightChild)
        {
            this.leftChild = leftChild;
            this.rightChild = rightChild;
            this.data = leftChild.data + rightChild.data; //for debug >>can remove it
            this.number = leftChild.number + rightChild.number;
        }
    }

    class Huffman
    {
        static public int numberofextend; //no of extend huffman
        static private IList<Node> huffmanlist; //include all letter with thier code
        static private IList<int> dict;
        static public IList<Node> Gethuffman() //tested
        {//get copy of list
            IList<Node> newlist = new List<Node>();

            for (int i = 0; i < huffmanlist.Count; i++)
            {
                Node tempNode = huffmanlist[i];
                newlist.Add(tempNode);
            }
            return newlist;
        }
        static public void build(Dictionary<string, int> input/*IList<string> input, IList<int> array*/) //tested
        {//give them array of string & array of number of each string
            IList<Node> list = new List<Node>();
            numberofextend = 0;
            dict = new List<int>();
            huffmanlist = new List<Node>();
            for (int i = 0; i < input.Count; i++)
            {
                int value = input.ToList()[i].Value; //array[i]
                string key = input.ToList()[i].Key; //input[i]
                dict.Add(value);
                if (value != 0)
                    list.Add(new Node(key, value));
            }

            //build the tree
            Stack<Node> stack = GetSortedStack(list);
            while (stack.Count > 1)
            {
                //move last 2 least number
                Node leftChild = stack.Pop();
                Node rightChild = stack.Pop();
                //repalce them with the sum of them 
                Node parentNode = new Node(leftChild, rightChild);
                stack.Push(parentNode);
                //sort again
                stack = GetSortedStack(stack.ToList<Node>());
            }
            if (stack.Count == 1)
            {
                Node parentNode1 = stack.Pop();
                GenerateCode(parentNode1);
            }
            for (int i = 0; i < huffmanlist.Count; i++)
            {
                for (int j = i + 1; j < huffmanlist.Count; j++)
                {
                    if (huffmanlist[i].number < huffmanlist[j].number)
                    {
                        Node tempNode = huffmanlist[j];
                        huffmanlist[j] = huffmanlist[i];
                        huffmanlist[i] = tempNode;
                    }
                }
            }

        }

        public static Stack<Node> GetSortedStack(IList<Node> list) //tested
        {//sort thd probability
            //sort the nodes
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].number < list[j].number)
                    {
                        Node tempNode = list[j];
                        list[j] = list[i];
                        list[i] = tempNode;
                    }
                }
            }
            //make new array for store the new value 
            Stack<Node> stack = new Stack<Node>();
            for (int j = 0; j < list.Count; j++)
                stack.Push(list[j]);
            return stack;
        }

        public static void GenerateCode(Node parentNode) //tested
        { //after build tree , apply this function on the parent node to get the code for each symple
            if (parentNode == null)
                return;
            else if (parentNode.leftChild == null)
            {
                huffmanlist.Add(parentNode);
            }
            else
            {
                parentNode.leftChild.code = parentNode.code + "0";
                parentNode.rightChild.code = parentNode.code + "1";
                GenerateCode(parentNode.leftChild);
                GenerateCode(parentNode.rightChild);
            }

        }

        public static long callength() //must be less than the original size
        { //this number will increase if the gab between the no of rebeat letter decrease
            long res = (18 * dict.Count());//size of dict
            for (int i = 0; i < huffmanlist.Count; i++)
            {
                res += (huffmanlist[i].number * huffmanlist[i].code.Length);
            }
            return res;
        }

        public static string codeData(string input)  //given data ,output code
        {
            string res = "";
            for (int i = 0; i < dict.Count; i++)
            {
                int x = dict[i];
                for (int j = 17; j > -1; j--)
                {
                    if (((1 << j) & x) != 0)
                    {
                        res += '1';
                        x ^= (1 << j);
                    }
                    else
                        res += '0';
                }
            }

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < huffmanlist.Count; j++)
                {
                    if (input[i] == huffmanlist[j].data[0])
                    {
                        res += huffmanlist[j].code;
                        break;
                    }
                }
            }
            return res;
        }

        public static string DecodeData(string input, IList<string> GeneralDict) //given code ,output data
        {
            int x = 0, length = 0;
            //IList<int> mynum = new List<int>();
            Dictionary<string, int> GeneralDictwithMynum = new Dictionary<string, int>();
            for (int i = 0; i < GeneralDict.Count; i++)
            {
                GeneralDictwithMynum.Add(GeneralDict[i], 0);
                int y = 0;
                for (int j = 0; j < 18; j++)
                {
                    y |= (((input[x++] - '0') << (17 - j)));
                }
                GeneralDictwithMynum[GeneralDict[i]] = y;
                length += y;
                //mynum.Add(y);
            }
            //Huffman.build(GeneralDict, mynum);
            Huffman.build(GeneralDictwithMynum);
            string res = "", curr = "";
            while (res.Length < length)
            {
                if (x > input.Length)
                    return res;
                //add current code
                curr += input[x++];
                //search on current code on the list 
                for (int j = 0; j < huffmanlist.Count; j++)
                {
                    if (curr == huffmanlist[j].code) //if found then add data to result and search on the next symple 
                    {
                        res += huffmanlist[j].data;
                        curr = "";
                        break;
                    }
                }

            }
            return res;
        }

        public static void extendhuffman()//extend huffman code 
        {
            if (numberofextend == null)
                return;
            numberofextend++;
            IList<Node> newlist = new List<Node>();
            //build new list
            for (int i = 0; i < huffmanlist.Count; i++)
            {
                for (int j = i; j < huffmanlist.Count; j++)
                {
                    newlist.Add(new Node(huffmanlist[i].data + huffmanlist[j].data, huffmanlist[i].number * huffmanlist[j].number));
                }
            }


            //build the tree
            Stack<Node> stack = GetSortedStack(newlist);
            while (stack.Count > 1)
            {
                //move last 2 least number
                Node leftChild = stack.Pop();
                Node rightChild = stack.Pop();
                //repalce them with the sum of them 
                Node parentNode = new Node(leftChild, rightChild);
                stack.Push(parentNode);
                //sort again
                stack = GetSortedStack(stack.ToList<Node>());
            }

            Node parentNode1 = stack.Pop();
            huffmanlist = new List<Node>();
            GenerateCode(parentNode1);


            //for debug
            for (int i = 0; i < huffmanlist.Count; i++)
            {
                for (int j = i + 1; j < huffmanlist.Count; j++)
                {
                    if (huffmanlist[i].number < huffmanlist[j].number)
                    {
                        Node tempNode = huffmanlist[j];
                        huffmanlist[j] = huffmanlist[i];
                        huffmanlist[i] = tempNode;
                    }
                }
            }

        }




        public static IList<char> applyhuffman(IList<char> input)
        {
            string[] x = { "0000", "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1111" };
            Dictionary<string, int> currlist = new Dictionary<string, int>();
            for (int i = 0; i < 16; i++)
            {
                currlist.Add(x[i], 0);
            }
            IList<char> res = new List<char>();
            string curr = "";
            for (int i = 0; i < input.Count; i++)
            {
                curr += input[i];
                if (curr.Length == 4)
                {
                    currlist[curr]++;
                    curr = "";
                }
            }
            Huffman.build(currlist);
            /*
            if (Huffman.callength() > input.Count)
            {
                res.Add('0');
                for (int i = 0; i < input.Count; i++)
                    res.Add(input[i]);
            }
            res.Add('1');*/
            for (int i = 0; i < dict.Count; i++)
            {
                int y = dict[i];
                for (int j = 17; j > -1; j--)
                {
                    if (((1 << j) & y) != 0)
                    {
                        res.Add('1');
                        y ^= (1 << j);
                    }
                    else
                        res.Add('0');
                }
            }
            for (int i = 0; i < input.Count; i++)
            {
                curr += input[i];
                if (curr.Length == 4)
                {
                    for (int j = 0; j < huffmanlist.Count; j++)
                    {
                        if (huffmanlist[j].data == curr)
                        {
                            for (int k = 0; k < huffmanlist[j].code.Length; j++)
                            {
                                res.Add(huffmanlist[j].code[k]);
                            }
                        }
                    }
                    curr = "";
                }
            }
            return res;
        }
    }


    class letter
    {
        public double upper;
        public double lower;
        public string data;
        public letter(string data, double up, double low)
        {
            this.data = data;
            this.upper = up;
            this.lower = low;

        }
    }

    class arthmitc
    {
        static public IList<letter> arthmitclist;
        static public int number;

        public static void Main(IList<string> input, IList<int> array) //given array of string & array of number of each string
        {
            arthmitclist = new List<letter>();
            int sum = 0;
            for (int i = 0; i < array.Count; i++)
            {
                sum += array[i];
            }
            number = sum;
            double curr = 0.0;
            for (int i = 0; i < array.Count; i++)
            {
                arthmitclist.Add(new letter(input[i], (curr + array[i]) / sum, curr / sum));
                curr += array[i];

            }
        }

        public static string buildbinary(string input, IList<int> number)
        {
            string res = "";
            IList<Double> x = new List<Double>();
            for (int i = 0; i < arthmitclist.Count; i++)
            {
                if (arthmitclist[i].data[0] == input[input.Length - 1])
                {
                    for (int j = 7; j > -1; j--)
                    {
                        if (i > (1 << j))
                        {
                            res += '1';
                            i -= (1 << j);
                        }
                        else
                            res += '0';
                    }
                    break;
                }
            }
            for (int i = 0; i < number.Count; i++)
            {
                int u = number[i];
                for (int j = 17; j > -1; j--)
                {
                    if (((1 << j) & u) != 0)
                    {
                        res += '1';
                        u ^= (1 << j);
                    }
                    else
                    {
                        res += '0';
                    }
                }
            }
            x = codeData(input);
            for (int i = 0; i < x.Count; i++)
            {
                res += arthmitc.doubletobinary(x[i]);
            }
            return res;
        }

        public static string buildstring(string input, IList<string> gene)
        {
            string res = "";
            int x = 0;
            int end = 0;
            for (; x < 8; x++)
            {
                end += (input[x] - '0') << (7 - x);
            }
            IList<int> mynum = new List<int>();
            for (int i = 0; i < gene.Count; i++)
            {
                int y = 0;
                for (int j = 0; j < 18; j++)
                {
                    y |= (((input[x++] - '0') << (17 - j)));
                }
                mynum.Add(y);
            }
            arthmitc.Main(gene, mynum);
            //string send = arthmitclist[end].data;

            while (x < input.Length)
            {
                string curr = "";
                for (int i = 0; i < 64; i++)
                    curr += input[x++];
                res += arthmitc.decodeData(arthmitc.Binarytodouble(curr));
            }
            return res;
        }

        public static IList<double> codeData(string input) //given data ,output code
        {
            IList<double> list = new List<double>();
            IList<letter> artlist = new List<letter>();
            for (int i = 0; i < arthmitclist.Count; i++)
            {
                artlist.Add(new letter(arthmitclist[i].data, arthmitclist[i].upper, arthmitclist[i].lower));
            }
            double up = 1, down = 0;
            char end = input[input.Length - 1]; //need to change
            string curr = "";
            for (int i = 0; i < input.Length; i++)
            {
                curr += input[i];
                for (int j = 0; j < artlist.Count; j++)
                {
                    if (artlist[j].data == curr)
                    {
                        curr = "";
                        up = artlist[j].upper;
                        down = artlist[j].lower;
                        double ratio = up - down;
                        if (ratio < 0.000000001) //end of coding  need to change
                        {
                            list.Add((up + down) / 2.0);
                            up = 1;
                            down = 0;
                            artlist.Clear();
                            for (int k = 0; k < arthmitclist.Count; k++)
                            {
                                artlist.Add(new letter(arthmitclist[k].data, arthmitclist[k].upper, arthmitclist[k].lower));
                            }
                            break;
                        }
                        for (int k = 0; k < arthmitclist.Count; k++)
                        {
                            artlist[k].lower = arthmitclist[k].lower * ratio + down;
                            artlist[k].upper = arthmitclist[k].upper * ratio + down;
                        }
                        break;
                    }
                }
            }
            return list;
        }

        public static string decodeData(double input/*,string end*/) //given double & the last symple in the text ,output data  for each double
        {
            if (input == null /*|| end == null*/)
                return null;

            string res = "";
            IList<letter> artlist = new List<letter>();
            for (int i = 0; i < arthmitclist.Count; i++)
            {
                artlist.Add(new letter(arthmitclist[i].data, arthmitclist[i].upper, arthmitclist[i].lower));
            }

            while (true)
            {

                for (int j = 0; j < artlist.Count; j++)
                {
                    if (input < artlist[j].upper && input > artlist[j].lower)
                    {
                        res += artlist[j].data;
                        double up = artlist[j].upper;
                        double down = artlist[j].lower;
                        double ratio = up - down;
                        if (ratio < 0.000000001)  //end of coding  need to change 
                        {
                            return res;
                        }
                        for (int k = 0; k < arthmitclist.Count; k++)
                        {
                            artlist[k].lower = arthmitclist[k].lower * ratio + down;
                            artlist[k].upper = arthmitclist[k].upper * ratio + down;
                        }
                    }
                }
            }
        }

        public static string doubletobinary(double x)
        {
            long m = BitConverter.DoubleToInt64Bits(x);
            string str = Convert.ToString(m, 2);
            return str;
        }

        public static double Binarytodouble(string str)
        {
            long n = Convert.ToInt64(str, 2);
            double x = BitConverter.Int64BitsToDouble(n);
            return x;
        }

    }


    class lzw
    {
        public static IList<char> LetterDict;

        public static void Main(IList<char> input) //give them array of char to initil dict
        {
            //fill letterdict with all letter in the data set
            LetterDict = new List<char>();
            for (int i = 0; i < input.Count; i++)
            {
                LetterDict.Add(input[i]);
            }

        }

        public static IList<int> Coding(string input) //given string , output code
        {
            IList<int> mylist = new List<int>();
            IList<string> Dict = new List<string>();
            for (int i = 0; i < LetterDict.Count; i++)
                Dict.Add(LetterDict[i].ToString());
            string curr = input[0] + "";
            int last = 0, x = 1;
            while (curr.Length > 0)
            {
                while (true)
                {
                    bool test = true;
                    for (int j = 0; j < Dict.Count; j++)
                    {
                        if (curr == Dict[j])
                        {
                            last = j;
                            test = false;
                            break;
                        }
                    }
                    if (test)
                        break;
                    if (x < input.Length)
                        curr += input[x++];
                    else
                        break;
                }
                mylist.Add(last);
                if (Dict.Count < 130000) //max 16 bit
                    Dict.Add(curr);
                string temp = "";
                int o = Dict[last].Length;
                while (o < curr.Length)
                    temp += curr[o++];
                curr = temp;
            }
            return mylist;
        }


        public static string deCoding(IList<int> input) //given code , output string
        {
            string res = "";
            IList<string> Dict = new List<string>();
            for (int i = 0; i < LetterDict.Count; i++)
                Dict.Add(LetterDict[i].ToString());
            string last = Dict[input[0]];
            res += last;

            for (int i = 1; i < input.Count; i++)
            {
                if (input[i] >= Dict.Count)
                    input[i]++;
                res += Dict[input[i]];
                if (Dict.Count < 130000)
                {
                    Dict.Add(last + Dict[input[i]][0]);
                    last = Dict[input[i]];
                }
            }
            return res;
        }


        public static IList<char> convertbinary(IList<int> input) //convert int list to binary code
        {
            IList<char> res = new List<char>();
            for (int i = 0; i < input.Count; i++)
            {
                int x = input[i];
                for (int j = 15; j > -1; j--)
                {
                    if ((x & (1 << j)) != 0)
                        res.Add('1');
                    else
                        res.Add('0');
                }
            }

            return res;
        }

        public static IList<int> convertint(IList<char> input) //convert code binary to int
        {
            IList<int> res = new List<int>();
            int curr = 0, test = 0;
            for (int i = 0; i < input.Count; i++)
            {
                if (test == 16)
                {
                    res.Add(curr);
                    curr = 0;
                    test = 0;
                }
                if (input[i] == '1')
                {
                    curr += 1 << (15 - test);
                }
                test++;
            }
            res.Add(curr);
            return res;
        }

    }




}
