using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ratchet.Code;

namespace PatchMethod
{
    class Program
    {
        public static int simpleAdd(int a, int b)
        {
            return a + b;
        }

        static void Main(string[] args)
        {
            System.Reflection.Emit.AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName("Patch"), System.Reflection.Emit.AssemblyBuilderAccess.Run, (string)null);
            System.Reflection.Emit.ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("module");
            System.Reflection.Emit.TypeBuilder typeBuilder = moduleBuilder.DefineType("program", System.Reflection.TypeAttributes.Public);
            System.Reflection.Emit.MethodBuilder methodBuilder = typeBuilder.DefineMethod("simpleSub", System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.HideBySig, typeof(int),  new Type[] { typeof(int) , typeof(int) });

            List<MSIL.Instruction> patchedMethod = new List<MSIL.Instruction>();
            foreach (MSIL.Instruction instruction in MSIL.ReadMethod(typeof(Program).GetMethod("simpleAdd")))
            {
                if (instruction.OpCode == System.Reflection.Emit.OpCodes.Add) { instruction.OpCode = System.Reflection.Emit.OpCodes.Sub; }
                patchedMethod.Add(instruction);
            }
            MSIL.EmitMethod(patchedMethod, methodBuilder);

            Type type = typeBuilder.CreateType();
            System.Reflection.MethodInfo pathedMethod = type.GetMethod("simpleSub");
            System.Console.WriteLine("result: " + (int)pathedMethod.Invoke(null, new object[] { 3, 1 }));
        }
    }
}
