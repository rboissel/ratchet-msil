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
    class MSIL_inline
    {
        static void inlineParameter(List<MSIL.Instruction> method, object obj, int parameterIndex)
        {
            if (obj == null){ inlineParameter_null(method, parameterIndex); }
            else if (obj is Int32) { inlineParameter_int32(method, (Int32)obj, parameterIndex); }
            else if (obj is Int64) { inlineParameter_int64(method, (Int64)obj, parameterIndex); }
            else
            {
                Type t = obj.GetType();
            }

        }

        static void inlineParameter_null(List<MSIL.Instruction> method, int parameterIndex)
        {
            for (int n = 0; n < method.Count; n++)
            {
                if (method[n].OpCode == System.Reflection.Emit.OpCodes.Ldarg && (int)method[n].Data == parameterIndex)
                {
                    method[n] = new MSIL.Instruction(System.Reflection.Emit.OpCodes.Ldnull, null) { _Offset = method[n]._Offset };
                }
            }
        }

        static void inlineParameter_int32(List<MSIL.Instruction> method, Int32 value, int parameterIndex)
        {
            for (int n = 0; n < method.Count; n++)
            {
                if (method[n].OpCode == System.Reflection.Emit.OpCodes.Ldarg && (int)method[n].Data == parameterIndex)
                {
                    method[n] = new MSIL.Instruction(System.Reflection.Emit.OpCodes.Ldc_I4, value) { _Offset = method[n]._Offset };
                }
            }
        }
        static void inlineParameter_int64(List<MSIL.Instruction> method, Int64 value, int parameterIndex)
        {
            for (int n = 0; n < method.Count; n++)
            {
                if (method[n].OpCode == System.Reflection.Emit.OpCodes.Ldarg && (int)method[n].Data == parameterIndex)
                {
                    method[n] = new MSIL.Instruction(System.Reflection.Emit.OpCodes.Ldc_I8, value) { _Offset = method[n]._Offset };
                }
            }
        }
    }
}
