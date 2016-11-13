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
    static class MSIL_dasm
    {
        static Dictionary<string, System.Reflection.Emit.OpCode> opcodes_names = new Dictionary<string, System.Reflection.Emit.OpCode>();
        static Dictionary<short, System.Reflection.Emit.OpCode> opcodes_bytes = new Dictionary<short, System.Reflection.Emit.OpCode>();


        static MSIL_dasm()
        {
            foreach (System.Reflection.FieldInfo _ in typeof(System.Reflection.Emit.OpCodes).GetFields())
            {
                if (_.IsStatic && _.IsPublic)
                {
                    System.Reflection.Emit.OpCode opcode = (System.Reflection.Emit.OpCode)_.GetValue(null);
                    opcodes_names[opcode.Name] = opcode;
                    opcodes_bytes[opcode.Value] = opcode;
                }
            }
        }

        internal static System.Reflection.Emit.OpCode GetOpCodeFromValue(short OpCode)
        {
            return opcodes_bytes[OpCode];
        }

        internal static MSIL.Instruction GetOpCodeInfo(byte[] bytecode, ref int offset)
        {
            if (offset < 0 || offset >= bytecode.Length)
                return null;
            short Key = bytecode[offset];
            if (Key == 0xFF || Key == 0xFE) { Key *= 0x100; offset++; Key += bytecode[offset]; }
            offset++;
            if (!opcodes_bytes.ContainsKey(Key))
            {
                return null;
            }
            System.Reflection.Emit.OpCode Code = opcodes_bytes[Key];
            try
            {
                switch (Code.OperandType)
                {
                    case System.Reflection.Emit.OperandType.InlineNone: return new MSIL.Instruction(Code, null);
                    case System.Reflection.Emit.OperandType.InlineMethod: offset += 4; return new MSIL.Instruction(Code, new MSIL.MetadataToken(System.BitConverter.ToUInt32(bytecode, offset - 4)));
                    case System.Reflection.Emit.OperandType.InlineI8: offset += 8; return new MSIL.Instruction(Code, System.BitConverter.ToUInt64(bytecode, offset - 8));
                    case System.Reflection.Emit.OperandType.InlineI: offset += 4; return new MSIL.Instruction(Code, System.BitConverter.ToUInt32(bytecode, offset - 4));
                    case System.Reflection.Emit.OperandType.InlineField: offset += 4; return new MSIL.Instruction(Code, new MSIL.MetadataToken(System.BitConverter.ToUInt32(bytecode, offset - 4)));
                    case System.Reflection.Emit.OperandType.InlineBrTarget:
                        offset += 4;
                        int target = System.BitConverter.ToInt32(bytecode, offset - 4);
                        return new MSIL.Instruction(Code, offset + target);
                    case System.Reflection.Emit.OperandType.InlineString: offset += 4; return new MSIL.Instruction(Code, new MSIL.MetadataToken(System.BitConverter.ToUInt32(bytecode, offset - 4)));
                    case System.Reflection.Emit.OperandType.InlineSwitch:
                        {
                            offset += 4;
                            UInt32 count = System.BitConverter.ToUInt32(bytecode, offset - 4);
                            List<Int32> jump = new List<Int32>();
                            while (count > 0)
                            {
                                offset += 4;
                                jump.Add(System.BitConverter.ToInt32(bytecode, offset - 4));
                                count--;
                            }
                            for (int label = 0; label < jump.Count; label++)
                            {
                                jump[label] += offset;
                            }
                            return new MSIL.Instruction(Code, jump);
                        }
                    case System.Reflection.Emit.OperandType.InlineType: offset += 4; return new MSIL.Instruction(Code, new MSIL.MetadataToken(System.BitConverter.ToUInt32(bytecode, offset - 4)));
                    case System.Reflection.Emit.OperandType.InlineR: offset += 8; return new MSIL.Instruction(Code, System.BitConverter.ToDouble(bytecode, offset - 8));
                    case System.Reflection.Emit.OperandType.InlineVar: offset += 2; return new MSIL.Instruction(Code, System.BitConverter.ToUInt16(bytecode, offset - 2));
                    case System.Reflection.Emit.OperandType.InlineSig: offset += 4; return new MSIL.Instruction(Code, new MSIL.MetadataToken(System.BitConverter.ToUInt32(bytecode, offset - 4)));
                    case System.Reflection.Emit.OperandType.InlineTok: offset += 4; return new MSIL.Instruction(Code, new MSIL.MetadataToken(System.BitConverter.ToUInt32(bytecode, offset - 4)));
                    case System.Reflection.Emit.OperandType.ShortInlineBrTarget:
                        offset += 1;
                        int ShortOffSet = (sbyte)unchecked(bytecode[offset - 1]);
                        return new MSIL.Instruction(Code, offset + ShortOffSet);
                    case System.Reflection.Emit.OperandType.ShortInlineI: offset += 1; return new MSIL.Instruction(Code, bytecode[offset - 1]);
                    case System.Reflection.Emit.OperandType.ShortInlineR: offset += 4; return new MSIL.Instruction(Code, System.BitConverter.ToSingle(bytecode, offset - 4));
                    case System.Reflection.Emit.OperandType.ShortInlineVar: offset += 1; return new MSIL.Instruction(Code, bytecode[offset - 1]);
                    default:
                        throw new InvalidProgramException();
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }


            return null;
        }
    }
}
