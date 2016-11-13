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
    static class MSIL_patcher
    {
        static public void PatchToken(List<MSIL.Instruction> opcodes, MSIL.Resolver Resolver)
        {
            foreach (MSIL.Instruction opcode in opcodes)
            {
                if (opcode.OpCode.FlowControl == System.Reflection.Emit.FlowControl.Call)
                {
                    object data = Resolver.ResolveMethod((int)((MSIL.MetadataToken)opcode.Data).Token);
                    if (data != null) { opcode._Data = data; }
                }
                if (opcode.OpCode.OperandType == System.Reflection.Emit.OperandType.InlineField)
                {
                    object data = Resolver.ResolveField((int)((MSIL.MetadataToken)opcode.Data).Token);
                    if (data != null) { opcode._Data = data; }
                }
                if (opcode.OpCode == System.Reflection.Emit.OpCodes.Ldftn)
                {
                    object data = Resolver.ResolveMethod((int)((MSIL.MetadataToken)opcode.Data).Token);
                    if (data != null) { opcode._Data = data; }
                }
                if (opcode.OpCode == System.Reflection.Emit.OpCodes.Ldstr)
                {
                    object data = Resolver.ResolveString((int)((MSIL.MetadataToken)opcode.Data).Token);
                    if (data != null) { opcode._Data = data; }
                }
                if (opcode.OpCode == System.Reflection.Emit.OpCodes.Ldtoken)
                {
                    // Keep the token
                }
                if (opcode.OpCode == System.Reflection.Emit.OpCodes.Newarr ||
                    opcode.OpCode == System.Reflection.Emit.OpCodes.Constrained)
                {
                    object data = Resolver.ResolveType((int)((MSIL.MetadataToken)opcode.Data).Token);
                    if (data != null) { opcode._Data = data; }
                }
            }
        }

        static public void PatchLocals(List<MSIL.Instruction> opcodes, MSIL.Resolver resolver)
        {
            foreach (MSIL.Instruction opcode in opcodes)
            {
                if (opcode.OpCode == System.Reflection.Emit.OpCodes.Ldloc_0) { object local = resolver.ResolveLocal(0); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Ldloc; } }
                else if (opcode.OpCode == System.Reflection.Emit.OpCodes.Ldloc_1) { object local = resolver.ResolveLocal(1); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Ldloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Ldloc_2) { object local = resolver.ResolveLocal(2); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Ldloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Ldloc_3) { object local = resolver.ResolveLocal(3); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Ldloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Ldloc) { object local = resolver.ResolveLocal((int)opcode._Data); if (local != null) { opcode._Data = local; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Ldloc_S) { object local = resolver.ResolveLocal((byte)opcode._Data); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Ldloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Stloc_0) { object local = resolver.ResolveLocal(0); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Stloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Stloc_1) { object local = resolver.ResolveLocal(1); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Stloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Stloc_2) { object local = resolver.ResolveLocal(2); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Stloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Stloc_3) { object local = resolver.ResolveLocal(3); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Stloc; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Stloc) { object local = resolver.ResolveLocal((int)opcode._Data); if (local != null) { opcode._Data = local; } }
                else if(opcode.OpCode == System.Reflection.Emit.OpCodes.Stloc_S) { object local = resolver.ResolveLocal((byte)opcode._Data); if (local != null) { opcode._Data = local; opcode._OpCode = System.Reflection.Emit.OpCodes.Stloc; } }
            }
        }

        static public void PatchJump(List<MSIL.Instruction> opcodes)
        {
            for (int n = 0; n < opcodes.Count; n++)
            {
                if (opcodes[n].OpCode.FlowControl == System.Reflection.Emit.FlowControl.Branch ||
                    opcodes[n].OpCode.FlowControl == System.Reflection.Emit.FlowControl.Cond_Branch)
                {
                    if (opcodes[n]._Data is List<Int32>)
                    {
                        List<Int32> offsets = (List<Int32>)opcodes[n]._Data;
                        List<MSIL.Instruction> jumps = new List<MSIL.Instruction>();
                        opcodes[n]._Data = null;
                        for (int l = 0; l < offsets.Count; l++)
                        {
                            for (int x = 0; x < opcodes.Count; x++)
                            {
                                if (offsets[l] == opcodes[x]._Offset) { jumps.Add(opcodes[x]); break; }
                            }
                        }
                        if (offsets.Count != jumps.Count)
                        {
                            throw new InvalidProgramException("Invalid switch labels");
                        }
                        opcodes[n]._Data = jumps;
                    }
                    else
                    {
                        int offset = (int)opcodes[n]._Data;
                        opcodes[n]._Data = null;
                        for (int x = 0; x < opcodes.Count; x++)
                        {
                            if (offset == opcodes[x]._Offset) { opcodes[n]._Data = opcodes[x]; break; }
                        }
                        if (opcodes[n]._Data == null)
                        {
                            throw new InvalidProgramException("Invalid jump: '" + offset.ToString("X") + "'");
                        }
                    }
                }
            }
        }
    }
}
