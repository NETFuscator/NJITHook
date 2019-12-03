using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NJITHook {
    public class JITHook {
        public unsafe JITHook(Delegate hook, Version version) {
            VMT vmt;

            RuntimeHelpers.PrepareDelegate(hook);

            foreach (var method in typeof(JITHook).GetMethods())
                RuntimeHelpers.PrepareMethod(method.MethodHandle);

            switch (version) {
                case Version.CORJit:
                    vmt = new VMT(GetJit_CorJit());
                    break;

                case Version.CLRJit:
                    vmt = new VMT(GetJit_ClrJit());
                    break;

                default:
                    throw new NotImplementedException();
            }

            Delegate compileMethodDelegate = null;

            if (IntPtr.Size == 4) {
                vmt.Hook(0, hook, ref compileMethodDelegate, typeof(Data.CompileMethodDel32));
                this._compileMethod32 = (Data.CompileMethodDel32)compileMethodDelegate;
            }
            else {
                vmt.Hook(0, hook, ref compileMethodDelegate, typeof(Data.CompileMethodDel64));
                this._compileMethod64 = (Data.CompileMethodDel64)compileMethodDelegate;
            }

            this._vmt = vmt;
        }

        public enum Version {
            CORJit,
            CLRJit
        }

        public unsafe int CompileMethod(IntPtr thisPtr, IntPtr corJitInfo, Data.CorMethodInfo32* methodInfo, Data.CorJitFlag flags, IntPtr nativeEntry, IntPtr nativeSizeOfCode) {
            return this._compileMethod32(thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode);
        }

        public unsafe int CompileMethod(IntPtr thisPtr, IntPtr corJitInfo, Data.CorMethodInfo64* methodInfo, Data.CorJitFlag flags, IntPtr nativeEntry, IntPtr nativeSizeOfCode) {
            return this._compileMethod64(thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode);
        }

        [DllImport("corjit.dll", EntryPoint = "getJit")]
        private static extern IntPtr GetJit_CorJit();

        [DllImport("clrjit.dll", EntryPoint = "getJit")]
        private static extern IntPtr GetJit_ClrJit();

        private Data.CompileMethodDel32 _compileMethod32;
        private Data.CompileMethodDel64 _compileMethod64;

        private VMT _vmt;
    }
}
