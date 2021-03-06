﻿/*                                                                           *
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
    /// <summary>
    /// Tools for MSIL manipulation.
    /// </summary>
    public static class MSIL
    {
        public abstract class Resolver
        {
            public abstract Type ResolveType(int Metatoken);
            public abstract System.Reflection.MethodBase ResolveMethod(int Metatoken);
            public abstract System.Reflection.FieldInfo ResolveField(int Metatoken);
            public abstract string ResolveString(int Metatoken);
            public virtual System.Reflection.LocalVariableInfo ResolveLocal(int LocalIndex) { return null; }
        }

        /// <summary>
        /// Represent an MSIL Instruction incuding the opcode and the data associated with it.
        /// the data is stored as an object type use 'is' and 'as' to manipulate it.
        /// </summary>
        public class Instruction
        {
            internal System.Reflection.Emit.OpCode _OpCode = System.Reflection.Emit.OpCodes.Unaligned;
            internal object _Data = null;
            internal int _Offset = -1;
 
            /// <summary>
            /// Get the offset of the instruction in the source bytecode. If unknown it is set to -1.
            /// </summary>
            public int Offset { get { return _Offset; } }
            /// <summary>
            /// Get the OpCode associated to this instruction.
            /// </summary>
            public System.Reflection.Emit.OpCode OpCode { get { return _OpCode; } set { _OpCode = value; } }
            /// <summary>
            /// Get the Data associated to this instruction. NULL if the opcode doesn't have any data associated with it.
            /// </summary>
            public object Data { get { return _Data; } set { _Data = value; } }
    
            public Instruction(System.Reflection.Emit.OpCode OpCode, object Data)
            { _Data = Data; _OpCode = OpCode; }
            public override string ToString()
            {
                if (_Data == null) { return _OpCode.Name; }
                else
                {
                    if (_OpCode.FlowControl == System.Reflection.Emit.FlowControl.Branch && (_Data is Instruction)) { return _OpCode.Name + " " + (_Data as Instruction)._Offset; }
                    if (_OpCode.FlowControl == System.Reflection.Emit.FlowControl.Cond_Branch && (_Data is Instruction)) { return _OpCode.Name + " " + (_Data as Instruction)._Offset; }
                    if (_OpCode.FlowControl == System.Reflection.Emit.FlowControl.Call && (_Data is MethodBase)) { return _OpCode.Name + " " + (_Data as MethodBase).Name; }
                    return _OpCode.Name + " " + _Data.ToString();
                }
            }


            /// <summary>
            /// Emit this instruction into a pre-existing ILGenerator
            /// </summary>
            /// <param name="ILGenerator">The target ILGenerator</param>
            public void Emit(System.Reflection.Emit.ILGenerator ILGenerator)
            {
                MSIL_emit.emit(_OpCode, _Data, ILGenerator);
            }
        }

        public class MetadataToken
        {
            uint _Token = 0;
            public MetadataToken(uint Token) { _Token = Token; }
            public uint ID { get { unchecked { return (_Token & 0x00FFFFFF); } } }
            public uint Token { get { return _Token; } }
        }

        /// <summary>
        /// Disassemble the bytecode of the specified method or constructor
        /// </summary>
        /// <param name="MethodBase">The method to disassemble</param>
        /// <param name="Resolver">A resolver class used while disassembling the code.
        /// Every token resolution and local variable resolution will be made with this resolver</param>
        /// <returns>The list of intruction contained in the method</returns>
        static public List<Instruction> ReadMethod(System.Reflection.MethodBase MethodBase, Resolver Resolver)
        {
            System.Reflection.MethodBody methodBody = MethodBase.GetMethodBody();
            if (methodBody == null) { return new List<Instruction>(); }
            return ReadMethod(methodBody.GetILAsByteArray(), Resolver);
        }

        /// <summary>
        /// Disassemble the bytecode of the specified method or constructor
        /// </summary>
        /// <param name="MethodBase">The method to disassemble</param>
        /// <returns>The list of intruction contained in the method</returns>
        static public List<Instruction> ReadMethod(System.Reflection.MethodBase MethodBase)
        {
            Resolver resolver = new MSIL_MethodResolver(MethodBase);
            return ReadMethod(MethodBase, resolver);
        }

        /// <summary>
        /// Disassemble the bytecode specified in MethodByteCode. The bytecode must be a valid
        /// MSIL bytecode.
        /// </summary>
        /// <param name="MethodByteCode">A byte array containing the bytecode</param>
        /// <param name="Resolver">A resolver class used while disassembling the code.
        /// Every token resolution and local variable resolution will be made with this resolver</param>
        /// <returns>The list of intruction contained in the method bytecode</returns>
        static public List<Instruction> ReadMethod(byte[] MethodByteCode, Resolver Resolver)
        {
            List<Instruction> opcodes = new List<Instruction>();
            if (MethodByteCode == null) { return new List<Instruction>(); }

            int offset = 0;
            while (offset < MethodByteCode.Length)
            {
                int oldoffset = offset;
                Instruction opcode = MSIL_dasm.GetOpCodeInfo(MethodByteCode, ref offset);
                opcode._Offset = oldoffset;
                opcodes.Add(opcode);
            }
            MSIL_patcher.PatchToken(opcodes, Resolver);
            MSIL_patcher.PatchJump(opcodes);
            MSIL_patcher.PatchLocals(opcodes, Resolver);
            MSIL_patcher.PatchLdArgs(opcodes);
            return opcodes;
        }

        /// <summary>
        /// Write the specified instruction list into the method builder. There is no need to manage labels or locals they will be created by this method.
        /// </summary>
        /// <param name="Instructions">The list of instruction to emit</param>
        /// <param name="MethodBuilder">The Method builder recieving the instructions</param>
        static public void EmitMethod(List<Instruction> Instructions, System.Reflection.Emit.MethodBuilder MethodBuilder)
        {
            MSIL_emit.emitMethod(Instructions, MethodBuilder.GetILGenerator());
        }


        /// <summary>
        /// Write the specified instruction list into the method builder. There is no need to manage labels or locals they will be created by this method.
        /// </summary>
        /// <param name="Instructions">The list of instruction to emit</param>
        /// <param name="ILGenerator">The ILGenerator recieving the instructions</param>
        static public void EmitMethod(List<Instruction> Instructions, System.Reflection.Emit.ILGenerator ILGenerator)
        {
            MSIL_emit.emitMethod(Instructions, ILGenerator);
        }
    }
}
