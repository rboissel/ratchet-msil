/*                                                                           *
 * Copyright © 2016, Raphaël Boissel                                         *
 * Permission is hereby granted, free of charge, to any person obtaining     *
 * a copy of this software and associated documentation files, to deal in    *
 * the Software without restriction, including without limitation the        *
 * rights to use, copy, modify, merge, publish, distribute, sublicense,      *
 * and/or sell copies of the Software, and to permit persons to whom the     *
 * Software is furnished to do so, subject to the following conditions:      *
 *                                                                           *
 * - The above copyright notice and this permission notice shall be          *
 *   included in all copies or substantial portions of the Software.         *
 * - The Software is provided "as is", without warranty of any kind,         *
 *   express or implied, including but not limited to the warranties of      *
 *   merchantability, fitness for a particular purpose and noninfringement.  *
 *   In no event shall the authors or copyright holders. be liable for any   *
 *   claim, damages or other liability, whether in an action of contract,    *
 *   tort or otherwise, arising from, out of or in connection with the       *
 *   software or the use or other dealings in the Software.                  *
 * - Except as contained in this notice, the name of Raphaël Boissel shall   *
 *   not be used in advertising or otherwise to promote the sale, use or     *
 *   other dealings in this Software without prior written authorization     *
 *   from Raphaël Boissel.                                                   *
 *                                                                           */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ratchet.Code
{
    class MSIL_emit
    {
        public static void emit(System.Reflection.Emit.OpCode OpCode, object Data, System.Reflection.Emit.ILGenerator Generator)
        {
            if (OpCode.FlowControl == System.Reflection.Emit.FlowControl.Branch || OpCode.FlowControl == System.Reflection.Emit.FlowControl.Cond_Branch)
            {
                if (Data is System.Reflection.Emit.Label)
                {
                    Generator.Emit(OpCode, ((System.Reflection.Emit.Label)Data));
                }
                else
                {
                    throw new Exception("Invalid data. The target of a branch has to substituted by a label before beeing emitted");
                }
            }
            else if (OpCode.FlowControl == System.Reflection.Emit.FlowControl.Call)
            {
                Generator.EmitCall(OpCode, Data as System.Reflection.MethodInfo, null);
            }
            else
            {
                if (Data is System.Reflection.LocalVariableInfo)
                {
                    System.Reflection.LocalVariableInfo info = Data as System.Reflection.LocalVariableInfo;

                    if (OpCode == System.Reflection.Emit.OpCodes.Ldloc ||
                        OpCode == System.Reflection.Emit.OpCodes.Ldloc_S ||
                        OpCode == System.Reflection.Emit.OpCodes.Ldloc_0 ||
                        OpCode == System.Reflection.Emit.OpCodes.Ldloc_1 ||
                        OpCode == System.Reflection.Emit.OpCodes.Ldloc_2 ||
                        OpCode == System.Reflection.Emit.OpCodes.Ldloc_3)
                    {
                        switch (info.LocalIndex)
                        {
                            case 0: Generator.Emit(System.Reflection.Emit.OpCodes.Ldloc_0); break;
                            case 1: Generator.Emit(System.Reflection.Emit.OpCodes.Ldloc_1); break;
                            case 2: Generator.Emit(System.Reflection.Emit.OpCodes.Ldloc_2); break;
                            case 3: Generator.Emit(System.Reflection.Emit.OpCodes.Ldloc_3); break;
                            default:
                                if (info.LocalIndex < 256)
                                {
                                    Generator.Emit(System.Reflection.Emit.OpCodes.Ldloc_S, (byte)info.LocalIndex);
                                }
                                else
                                {
                                    Generator.Emit(System.Reflection.Emit.OpCodes.Ldloc, info.LocalIndex);
                                }
                                break;
                        }
                    }
                    else if (OpCode == System.Reflection.Emit.OpCodes.Stloc ||
                             OpCode == System.Reflection.Emit.OpCodes.Stloc_S ||
                             OpCode == System.Reflection.Emit.OpCodes.Stloc_0 ||
                             OpCode == System.Reflection.Emit.OpCodes.Stloc_1 ||
                             OpCode == System.Reflection.Emit.OpCodes.Stloc_2 ||
                             OpCode == System.Reflection.Emit.OpCodes.Stloc_3)
                    {
                        switch (info.LocalIndex)
                        {
                            case 0: Generator.Emit(System.Reflection.Emit.OpCodes.Stloc_0); break;
                            case 1: Generator.Emit(System.Reflection.Emit.OpCodes.Stloc_1); break;
                            case 2: Generator.Emit(System.Reflection.Emit.OpCodes.Stloc_2); break;
                            case 3: Generator.Emit(System.Reflection.Emit.OpCodes.Stloc_3); break;
                            default:
                                if (info.LocalIndex < 256)
                                {
                                    Generator.Emit(System.Reflection.Emit.OpCodes.Stloc_S, (byte)info.LocalIndex);
                                }
                                else
                                {
                                    Generator.Emit(System.Reflection.Emit.OpCodes.Stloc, info.LocalIndex);
                                }
                                break;
                        }
                    }
                    else
                    {
                        throw new InvalidProgramException("Invalid OpCode and Data combinaison: " + OpCode.ToString() + " " + Data.ToString());
                    }
                }
                else if (OpCode == System.Reflection.Emit.OpCodes.Ldarg_0 ||
                         OpCode == System.Reflection.Emit.OpCodes.Ldarg_1 ||
                         OpCode == System.Reflection.Emit.OpCodes.Ldarg_2 ||
                         OpCode == System.Reflection.Emit.OpCodes.Ldarg_3 ||
                         OpCode == System.Reflection.Emit.OpCodes.Ldarg_S)
                {
                    int argIndx = (int)Data;
                    switch (argIndx)
                    {
                        case 0: Generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); break;
                        case 1: Generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_1); break;
                        case 2: Generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_2); break;
                        case 3: Generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_3); break;
                        default:
                            if (argIndx < 256)
                            {
                                Generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_S, (byte)argIndx);
                            }
                            else
                            {
                                Generator.Emit(System.Reflection.Emit.OpCodes.Ldarg, argIndx);
                            }
                            break;
                    }
                }
                else
                {
                    if (Data == null) { Generator.Emit(OpCode); }
                    else if (Data is int) { Generator.Emit(OpCode, (int)Data); }
                    else if (Data is byte) { Generator.Emit(OpCode, (byte)Data); }
                    else if (Data is double) { Generator.Emit(OpCode, (double)Data); }
                    else if (Data is float) { Generator.Emit(OpCode, (float)Data); }
                    else if (Data is FieldInfo) { Generator.Emit(OpCode, (FieldInfo)Data); }
                    else if (Data is long) { Generator.Emit(OpCode, (long)Data); }
                    else if (Data is Type) { Generator.Emit(OpCode, (Type)Data); }
                    else if (Data is ConstructorInfo) { Generator.Emit(OpCode, (ConstructorInfo)Data); }
                    else
                    {
                        throw new InvalidProgramException("Invalid OpCode and Data combinaison: " + OpCode.ToString() + " " + Data.ToString());
                    }
                }
            }
        }

        static public void emitMethod(List<MSIL.Instruction> Instructions, System.Reflection.Emit.ILGenerator ILGenerator)
        {
            Instructions = new List<MSIL.Instruction>(Instructions);
            Dictionary<int, System.Reflection.Emit.Label> labels = new Dictionary<int, System.Reflection.Emit.Label>();
            Dictionary<int, System.Reflection.LocalVariableInfo> locals = new Dictionary<int, System.Reflection.LocalVariableInfo>();

            {
                HashSet<int> uniqueOffset = new HashSet<int>();
                for (int n = 0; n < Instructions.Count; n++)
                {
                    if (Instructions[n] == null) { continue; }
                    int offset = Instructions[n].Offset;
                    if (uniqueOffset.Contains(offset))
                    {
                        Dictionary<int, MSIL.Instruction> instructionRemap = new Dictionary<int, MSIL.Instruction>();
                        // Conflicting offset we need to patch all of them
                        for (n = 0; n < Instructions.Count; n++)
                        {
                            int hash = Instructions[n].GetHashCode();
                            Instructions[n] = new MSIL.Instruction(Instructions[n].OpCode, Instructions[n].Data) { _Offset = n };
                            instructionRemap.Add(hash, Instructions[n]);
                        }
                        for (n = 0; n < Instructions.Count; n++)
                        {
                            if (Instructions[n].Data != null && (Instructions[n].Data is MSIL.Instruction))
                            {
                                Instructions[n].Data = instructionRemap[(Instructions[n].Data as MSIL.Instruction).GetHashCode()];
                            }
                        }
                        break;
                    }
                    uniqueOffset.Add(offset);
                }
            }

            for (int n = 0; n < Instructions.Count; n++)
            {
                if (Instructions[n] == null) { continue; }
                if (Instructions[n].Data != null && (Instructions[n].Data is MSIL.Instruction))
                {
                    int labelPos = (Instructions[n].Data as MSIL.Instruction).Offset;
                    System.Reflection.Emit.Label label;
                    if (!labels.TryGetValue(labelPos, out label))
                    {
                        label = ILGenerator.DefineLabel();
                        labels.Add(labelPos, label);
                    }
                    Instructions[n] = new MSIL.Instruction(Instructions[n].OpCode, label) { _Offset = Instructions[n]._Offset };
                }
                else if (Instructions[n].Data != null && (Instructions[n].Data is System.Reflection.LocalVariableInfo))
                {
                    int localIndex = (Instructions[n].Data as System.Reflection.LocalVariableInfo).LocalIndex;
                    System.Reflection.LocalVariableInfo local;
                    if (!locals.TryGetValue(localIndex, out local))
                    {
                        local = ILGenerator.DeclareLocal((Instructions[n].Data as System.Reflection.LocalVariableInfo).LocalType, (Instructions[n].Data as System.Reflection.LocalVariableInfo).IsPinned);
                        locals.Add(localIndex, local);
                    }
                    Instructions[n] = new MSIL.Instruction(Instructions[n].OpCode, local) { _Offset = Instructions[n]._Offset };
                }
            }

            for (int n = 0; n < Instructions.Count; n++)
            {
                System.Reflection.Emit.Label label;
                if (labels.TryGetValue(Instructions[n].Offset, out label))
                {
                    ILGenerator.MarkLabel(label);
                }
                Instructions[n].Emit(ILGenerator);
            }
        }
    }
}
