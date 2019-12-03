﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NJITHook {
    public static class Data {
        public enum CorJitFlag {
            CORJIT_FLG_SPEED_OPT = 0x00000001,
            CORJIT_FLG_SIZE_OPT = 0x00000002,
            CORJIT_FLG_DEBUG_CODE = 0x00000004, // generate "debuggable" code (no code-mangling optimizations)
            CORJIT_FLG_DEBUG_EnC = 0x00000008, // We are in Edit-n-Continue mode
            CORJIT_FLG_DEBUG_INFO = 0x00000010, // generate line and local-var info
            CORJIT_FLG_LOOSE_EXCEPT_ORDER = 0x00000020, // loose exception order
            CORJIT_FLG_TARGET_PENTIUM = 0x00000100,
            CORJIT_FLG_TARGET_PPRO = 0x00000200,
            CORJIT_FLG_TARGET_P4 = 0x00000400,
            CORJIT_FLG_TARGET_BANIAS = 0x00000800,
            CORJIT_FLG_USE_FCOMI = 0x00001000, // Generated code may use fcomi(p) instruction
            CORJIT_FLG_USE_CMOV = 0x00002000, // Generated code may use cmov instruction
            CORJIT_FLG_USE_SSE2 = 0x00004000, // Generated code may use SSE-2 instructions
            CORJIT_FLG_PROF_CALLRET = 0x00010000, // Wrap method calls with probes
            CORJIT_FLG_PROF_ENTERLEAVE = 0x00020000, // Instrument prologues/epilogues
            CORJIT_FLG_PROF_INPROC_ACTIVE_DEPRECATED = 0x00040000,
            // Inprocess debugging active requires different instrumentation
            CORJIT_FLG_PROF_NO_PINVOKE_INLINE = 0x00080000, // Disables PInvoke inlining
            CORJIT_FLG_SKIP_VERIFICATION = 0x00100000,
            // (lazy) skip verification - determined without doing a full resolve. See comment below
            CORJIT_FLG_PREJIT = 0x00200000, // jit or prejit is the execution engine.
            CORJIT_FLG_RELOC = 0x00400000, // Generate relocatable code
            CORJIT_FLG_IMPORT_ONLY = 0x00800000, // Only import the function
            CORJIT_FLG_IL_STUB = 0x01000000, // method is an IL stub
            CORJIT_FLG_PROCSPLIT = 0x02000000, // JIT should separate code into hot and cold sections
            CORJIT_FLG_BBINSTR = 0x04000000, // Collect basic block profile information
            CORJIT_FLG_BBOPT = 0x08000000, // Optimize method based on profile information
            CORJIT_FLG_FRAMED = 0x10000000, // All methods have an EBP frame
            CORJIT_FLG_ALIGN_LOOPS = 0x20000000, // add NOPs before loops to align them at 16 byte boundaries
            CORJIT_FLG_PUBLISH_SECRET_PARAM = 0x40000000,
            // JIT must place stub secret param into local 0.  (used by IL stubs)
        };

        public enum CorInfoCallConv {
            C = 1,
            DEFAULT = 0,
            EXPLICITTHIS = 64,
            FASTCALL = 4,
            FIELD = 6,
            GENERIC = 16,
            HASTHIS = 32,
            LOCAL_SIG = 7,
            MASK = 15,
            NATIVEVARARG = 11,
            PARAMTYPE = 128,
            PROPERTY = 8,
            STDCALL = 2,
            THISCALL = 3,
            VARARG = 5
        }

        public enum CorInfoType : byte {
            BOOL = 2,
            BYREF = 18,
            BYTE = 4,
            CHAR = 3,
            CLASS = 20,
            COUNT = 23,
            DOUBLE = 15,
            FLOAT = 14,
            INT = 8,
            LONG = 10,
            NATIVEINT = 12,
            NATIVEUINT = 13,
            PTR = 17,
            REFANY = 21,
            SHORT = 6,
            STRING = 16,
            UBYTE = 5,
            UINT = 9,
            ULONG = 11,
            UNDEF = 0,
            USHORT = 7,
            VALUECLASS = 19,
            VAR = 22,
            VOID = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CorinfoSigInst32 {
            public uint classInstCount;
            public unsafe IntPtr* classInst;
            public uint methInstCount;
            public unsafe IntPtr* methInst;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CorinfoSigInst64 {
            public uint classInstCount;
            uint dummy;
            public IntPtr* classInst;
            public uint methInstCount;
            uint dummy2;
            public IntPtr* methInst;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CorinfoSigInfo32 {
            public CorInfoCallConv callConv;
            public IntPtr retTypeClass;
            public IntPtr retTypeSigClass;
            public CorInfoType retType;
            public byte flags;
            public ushort numArgs;
            public CorinfoSigInst32 sigInst;
            public IntPtr args;
            public uint token;
            public IntPtr sig;
            public IntPtr scope;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CorMethodInfo64 {
            public IntPtr methodHandle;
            public IntPtr moduleHandle;
            public IntPtr ilCode;
            public uint ilCodeSize;
            public ushort maxStack;
            public ushort EHCount;
            public uint corInfoOptions;
            public CorinfoSigInst64 args;
            public CorinfoSigInst64 locals;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CorMethodInfo32 {
            public IntPtr methodHandle;
            public IntPtr moduleHandle;
            public IntPtr ilCode;
            public uint ilCodeSize;
            public ushort maxStack;
            public ushort EHCount;
            public uint corInfoOptions;
            public CorinfoSigInst32 args;
            public CorinfoSigInst32 locals;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate int CompileMethodDel32(IntPtr thisPtr, IntPtr corJitInfo, CorMethodInfo32* methodInfo, CorJitFlag flags, IntPtr nativeEntry, IntPtr nativeSizeOfCode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate int CompileMethodDel64(IntPtr thisPtr, IntPtr corJitInfo, CorMethodInfo64* methodInfo, CorJitFlag flags, IntPtr nativeEntry, IntPtr nativeSizeOfCode);
    }
}