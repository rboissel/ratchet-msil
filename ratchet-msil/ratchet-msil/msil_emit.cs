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
    }
}
