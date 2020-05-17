//Title     -   Hack Assembler
//Date      -   2020-05-17
//Author    -   Fabian de Alwis Gunasekare
//Version   -   1


using System;
using System.Collections.Generic;
using System.IO;

namespace Assmbler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                Console.WriteLine("Usage: assembler [path to .asm file]");
            else
            {
                Parser parser = new Parser();
                parser.FileIO(args[0]);
            }
        }
    }

    //**********************************************************************************************
    //**********************************************************************************************
    //**********************************************************************************************
    public class Parser
    {
        public void FileIO(string path)
        {
            try
            {
                string name = null;

                using (StreamReader file = new StreamReader(path))
                {
                    name = (file.BaseStream as FileStream).Name;
                    name = name.Split(".")[0] + ".hack";

                    int status = readCommand(file);
                    if (status == -1)
                        Console.WriteLine("Assembly Aborted.");
                    else
                        Console.WriteLine("Assembly Completed.");
                }

                Code code = new Code();
                List<string> assemblyCode = code.GetAssemblyCode();

                using (StreamWriter file = new StreamWriter(name))
                {
                    foreach (string assemblyCommand in assemblyCode)
                        file.WriteLine(assemblyCommand);

                    Console.WriteLine($"File {name} created.");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private int readCommand(StreamReader file)
        {
            string inputCommand;
            int commandTypeValue = 0;
            int inputCommandNumber = 1;
            int assemblyCommandNumber = 0;
            bool firstPass = true;

            Code code = new Code();
            SymbolTable symbolTable = new SymbolTable();
            symbolTable.SymbolTableInitialize();

            while (true)
            {
                while ((inputCommand = file.ReadLine()) != null)
                {
                    //commandTypeValue = -1 invalid input
                    //commandTypeValue = 0 A command
                    //commandTypeValue = 1 C command
                    //commandTypeValue = 2 L command
                    //commandTypeValue = 3 whitespace/comment
                    commandTypeValue = commandType(inputCommand);

                    switch (commandTypeValue)
                    {
                        case -1:
                            Console.WriteLine($"Invalid input at {inputCommandNumber} : {inputCommandCleaned}");
                            return -1;

                        case 0:
                            if (!firstPass)
                            {
                                Console.WriteLine($"A : {inputCommandNumber} : {inputCommandCleaned}");
                                if (code.ACommand(inputCommandCleaned) != 0)
                                {
                                    Console.WriteLine($"Invalid input at {inputCommandNumber} : {inputCommandCleaned}");
                                    return -1;
                                }
                            }
                            ++inputCommandNumber;
                            ++assemblyCommandNumber;
                            break;

                        case 1:
                            if (!firstPass)
                            {
                                Console.WriteLine($"C : {inputCommandNumber} : {inputCommandCleaned}");
                                if (code.CCommand(inputCommandCleaned) != 0)
                                {
                                    Console.WriteLine($"Invalid input at {inputCommandNumber} : {inputCommandCleaned}");
                                    return -1;
                                }
                            }
                            ++inputCommandNumber;
                            ++assemblyCommandNumber;
                            break;

                        case 2:
                            if (firstPass)
                            {
                                Console.WriteLine($"L : {inputCommandNumber} : {inputCommandCleaned}");
                                symbolTable.AddSymbol(inputCommandCleaned, assemblyCommandNumber);
                            }
                            ++inputCommandNumber;
                            break;

                        case 3:
                            break;

                        default:
                            Console.WriteLine($"ERR : {inputCommandNumber} : {inputCommandCleaned}");
                            return -1;
                    }
                }
                if (firstPass)
                {
                    firstPass = false;
                    file.BaseStream.Position = 0;
                    file.DiscardBufferedData();
                    inputCommandNumber = 1;
                    assemblyCommandNumber = 0;
                    Console.WriteLine("First pass completed.");
                }
                else
                    return 0;
            }
        }

        private int commandType(string inputCommand)
        {
            inputCommandCleaned = inputCommand.Replace(" ", "").Split("//")[0];

            if (inputCommandCleaned.Length == 0)
                return 3;
            else if (inputCommandCleaned[0] == '@')
                return 0;
            else if (inputCommandCleaned.Contains("=") || inputCommand.Contains(";"))
                return 1;
            else if (inputCommandCleaned.Contains("(") && inputCommand.Contains(")"))
                return 2;
            else
                return -1;
        }
        private string inputCommandCleaned;
    }

    //**********************************************************************************************
    //**********************************************************************************************
    //**********************************************************************************************
    public class Code
    {
        public int ACommand(string inputCommand)
        {
            int addressValue = 0;
            SymbolTable symbolTable = new SymbolTable();

            inputCommand = inputCommand.Split("@")[1];
            if (!int.TryParse(inputCommand, out addressValue))
                addressValue = int.Parse(symbolTable.RetriveAddress(inputCommand));

            if (addressValue > 32767)
                return -1;

            string binaryValue = "0" + Convert.ToString(addressValue, 2).PadLeft(15, '0');
            assemblyCode.Add(binaryValue);

            return 0;
        }

        public int CCommand(string inputCommand)
        {
            Dictionary<string, string> compMnemonic = new Dictionary<string, string>();
            compMnemonic.Add("0", "0101010");
            compMnemonic.Add("1", "0111111");
            compMnemonic.Add("-1", "0111010");
            compMnemonic.Add("D", "0001100");
            compMnemonic.Add("A", "0110000");
            compMnemonic.Add("!D", "0001101");
            compMnemonic.Add("!A", "0110001");
            compMnemonic.Add("-D", "0001111");
            compMnemonic.Add("-A", "0110011");
            compMnemonic.Add("D+1", "0011111");
            compMnemonic.Add("A+1", "0110111");
            compMnemonic.Add("D-1", "0001110");
            compMnemonic.Add("A-1", "0110010");
            compMnemonic.Add("D+A", "0000010");
            compMnemonic.Add("D-A", "0010011");
            compMnemonic.Add("A-D", "0000111");
            compMnemonic.Add("D&A", "0000000");
            compMnemonic.Add("D|A", "0010101");
            compMnemonic.Add("M", "1110000");
            compMnemonic.Add("!M", "1110001");
            compMnemonic.Add("-M", "1110011");
            compMnemonic.Add("M+1", "1110111");
            compMnemonic.Add("M-1", "1110010");
            compMnemonic.Add("D+M", "1000010");
            compMnemonic.Add("D-M", "1010011");
            compMnemonic.Add("M-D", "1000111");
            compMnemonic.Add("D&M", "1000000");
            compMnemonic.Add("D|M", "1010101");

            Dictionary<string, string> destMnemonic = new Dictionary<string, string>();
            destMnemonic.Add("M", "001");
            destMnemonic.Add("D", "010");
            destMnemonic.Add("MD", "011");
            destMnemonic.Add("A", "100");
            destMnemonic.Add("AM", "101");
            destMnemonic.Add("AD", "110");
            destMnemonic.Add("AMD", "111");

            Dictionary<string, string> jumpMnemonic = new Dictionary<string, string>();
            jumpMnemonic.Add("JGT", "001");
            jumpMnemonic.Add("JEQ", "010");
            jumpMnemonic.Add("JGE", "011");
            jumpMnemonic.Add("JLT", "100");
            jumpMnemonic.Add("JNE", "101");
            jumpMnemonic.Add("JLE", "110");
            jumpMnemonic.Add("JMP", "111");


            if (inputCommand.Contains("=") && inputCommand.Contains(";"))
            {
                string dest = inputCommand.Split("=")[0];
                string comp = inputCommand.Split("=")[1].Split(";")[0];
                string jump = inputCommand.Split("=")[1].Split(";")[1];

                if (!destMnemonic.ContainsKey(dest) || !compMnemonic.ContainsKey(comp) || !jumpMnemonic.ContainsKey(jump))
                    return -1;

                string assemblyCommand = "111" + compMnemonic[comp] + destMnemonic[dest] + jumpMnemonic[jump];
                assemblyCode.Add(assemblyCommand);

            }
            else if (inputCommand.Contains("="))
            {
                string dest = inputCommand.Split("=")[0];
                string comp = inputCommand.Split("=")[1];

                if (!destMnemonic.ContainsKey(dest) || !compMnemonic.ContainsKey(comp))
                    return -1;

                string assemblyCommand = "111" + compMnemonic[comp] + destMnemonic[dest] + "000";
                assemblyCode.Add(assemblyCommand);
            }
            else
            {
                string comp = inputCommand.Split(";")[0];
                string jump = inputCommand.Split(";")[1];

                if (!compMnemonic.ContainsKey(comp) || !jumpMnemonic.ContainsKey(jump))
                    return -1;

                string assemblyCommand = "111" + compMnemonic[comp] + "000" + jumpMnemonic[jump];
                assemblyCode.Add(assemblyCommand);
            }
            return 0;
        }

        public List<string> GetAssemblyCode()
        {
            return assemblyCode;
        }
        private static List<string> assemblyCode = new List<string>();
    }

    public class SymbolTable
    {
        public void SymbolTableInitialize()
        {
            symbolTable.Add("SP", "0");
            symbolTable.Add("LCL", "1");
            symbolTable.Add("ARG", "2");
            symbolTable.Add("THIS", "3");
            symbolTable.Add("THAT", "4");
            symbolTable.Add("R0", "0");
            symbolTable.Add("R1", "1");
            symbolTable.Add("R2", "2");
            symbolTable.Add("R3", "3");
            symbolTable.Add("R4", "4");
            symbolTable.Add("R5", "5");
            symbolTable.Add("R6", "6");
            symbolTable.Add("R7", "7");
            symbolTable.Add("R8", "8");
            symbolTable.Add("R9", "9");
            symbolTable.Add("R10", "10");
            symbolTable.Add("R11", "11");
            symbolTable.Add("R12", "12");
            symbolTable.Add("R13", "13");
            symbolTable.Add("R14", "14");
            symbolTable.Add("R15", "15");
            symbolTable.Add("SCREEN", "16384");
            symbolTable.Add("KBD", "24576");
        }

        public void AddSymbol(string inputCommandCleaned, int assemblyCommandNumber)
        {
            inputCommandCleaned = inputCommandCleaned.Replace("(", "").Replace(")", "");
            symbolTable.Add(inputCommandCleaned, assemblyCommandNumber.ToString());
        }

        internal string RetriveAddress(string inputCommand)
        {
            string address;
            if (!symbolTable.TryGetValue(inputCommand, out address))
            {
                AddSymbol(inputCommand, variableMemoryLocation);
                ++variableMemoryLocation;
            }

            return symbolTable[inputCommand];
        }

        private static Dictionary<string, string> symbolTable = new Dictionary<string, string>();
        private static int variableMemoryLocation = 16;
    }
}
