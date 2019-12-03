using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NJITHook {
    internal class VMT {
        internal unsafe VMT(IntPtr obj) {
            this._cls = (void***)obj;

            this._old_table = *this._cls;

            uint size = 0;

            while (this._old_table[size++] != null)
                ;

            this._new_table = (void**)Marshal.AllocHGlobal((int)(size * IntPtr.Size));

            for (var i = 0; i < size; i++)
                this._new_table[i] = this._old_table[i];

            *this._cls = this._new_table;
        }
        
        unsafe ~VMT() {
            *this._cls = this._old_table;
            Marshal.FreeHGlobal((IntPtr)this._new_table);
        }

        internal unsafe void Hook(uint index, Delegate hook, ref Delegate original, Type delegateType) {
            original = Marshal.GetDelegateForFunctionPointer((IntPtr)this._old_table[index], delegateType);
            
            this._new_table[index] = (void*)Marshal.GetFunctionPointerForDelegate(hook);
        }

        private unsafe void*** _cls;
        private unsafe void** _old_table;
        private unsafe void** _new_table;
    }
}
