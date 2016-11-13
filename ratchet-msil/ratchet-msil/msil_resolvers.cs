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
    internal class MSIL_ModuleResolver : MSIL.Resolver
    {
        System.Reflection.Module _Module;
        public MSIL_ModuleResolver(System.Reflection.Module Module) { _Module = Module; }
        public override System.Reflection.FieldInfo ResolveField(int Metadatatoken)
        {
            return _Module.ResolveField(Metadatatoken);
        }
        public override System.Reflection.MethodBase ResolveMethod(int Metadatatoken)
        {
            return _Module.ResolveMethod(Metadatatoken);
        }
        public override Type ResolveType(int Metadatatoken)
        {
            return _Module.ResolveType(Metadatatoken);
        }
        public override string ResolveString(int Metadatatoken)
        {
            return _Module.ResolveString(Metadatatoken);
        }
    }

    internal class MSIL_MethodResolver : MSIL_ModuleResolver
    {
        Dictionary<int, System.Reflection.LocalVariableInfo> _locals = new Dictionary<int, System.Reflection.LocalVariableInfo>();

        public MSIL_MethodResolver(System.Reflection.MethodBase Method) : base(Method.DeclaringType.Module)
        {
            System.Reflection.MethodBody body = Method.GetMethodBody();
            if (Method.GetMethodBody() != null)
            {
                foreach (System.Reflection.LocalVariableInfo localsBody in body.LocalVariables)
                {
                    System.Reflection.LocalVariableInfo _WAR_local = localsBody;
                    _locals.Add(_WAR_local.LocalIndex, _WAR_local);
                }
            }
        }

        public override LocalVariableInfo ResolveLocal(int LocalIndex)
        {
            System.Reflection.LocalVariableInfo local;
            if (_locals.TryGetValue(LocalIndex, out local)) { return local; }
            return null;
        }
    }
}
