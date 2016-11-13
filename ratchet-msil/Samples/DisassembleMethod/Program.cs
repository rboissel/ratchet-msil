using System;
using System.Collections.Generic;
using Ratchet.Code;

namespace DisassembleMethod
{
    class Program
    {
        public int simpleAdd(int a, int b)
        {
            return  a + b;
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Simple Add:");
            
            foreach ( MSIL.Instruction instruction in MSIL.ReadMethod(typeof(Program).GetMethod("simpleAdd")))
            {
                Console.WriteLine(instruction.ToString());
            }
        }
    }
}
