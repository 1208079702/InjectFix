/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

namespace IFix.Core
{
    public enum Code
    {
        Ldstr,
        Ldelem_I4,
        Clt,
        Ldelema,
        Ldc_I4,
        Throw,
        Ble,
        Clt_Un,
        Ldobj,
        //Calli,
        Blt,
        Ldc_I8,
        Conv_R_Un,
        No,
        Newanon,
        Conv_Ovf_I4,
        Stelem_I8,
        Callvirtvirt,
        Refanyval,
        Ldftn,
        Stind_I,
        Conv_Ovf_U1,
        Conv_Ovf_I1_Un,
        Stobj,
        Box,
        Ldelem_U2,
        Rethrow,
        Conv_Ovf_I2,
        Call,
        Stelem_R4,
        Stelem_I,
        Unbox,
        Conv_Ovf_I_Un,
        Ldsflda,
        Ldelem_I2,
        Add,
        Neg,
        Conv_I1,
        Ldlen,
        CallExtern,
        Ldelem_I8,
        Div_Un,
        Ret,
        Ldloca,
        Beq,
        Stind_I8,
        Bgt,
        Ldelem_I1,
        Ldind_Ref,
        Ldvirtftn2,
        Conv_I4,
        Initobj,
        Br,
        Blt_Un,
        Volatile,
        Unbox_Any,
        Rem_Un,
        Ldelem_U1,
        Shr_Un,
        Localloc,
        Conv_Ovf_U4_Un,
        Not,
        Add_Ovf,
        Break,
        Stind_I2,
        Sub_Ovf,
        Ckfinite,
        Sub,
        Shl,
        Cgt_Un,
        Stind_R8,
        Add_Ovf_Un,
        Jmp,
        Stloc,
        Switch,
        Conv_I2,
        Conv_Ovf_I,
        Mkrefany,
        Ldelem_R4,
        Conv_Ovf_I8_Un,
        Initblk,
        Ldind_I,
        Cpblk,
        Ldtoken,
        Conv_R8,
        Ldind_U2,
        Ldloc,
        Stind_Ref,
        Ldind_I8,
        Ldarga,
        Ldnull,
        Conv_Ovf_U8,
        Isinst,
        Ldc_R4,
        Stind_I4,
        Stelem_Any,
        Bge,
        Ldind_I4,
        Refanytype,
        Ceq,
        Stelem_I1,
        Conv_U,
        Conv_U1,
        Bne_Un,
        Ldfld,
        Rem,
        Ldelem_Ref,
        Sub_Ovf_Un,
        Dup,
        Endfinally,
        Stelem_Ref,
        Ldelem_R8,
        Conv_Ovf_U2_Un,
        Ldelem_I,
        Ldsfld,
        Castclass,
        Arglist,
        Stelem_I4,
        Stind_R4,
        Ldtype, // custom
        Ldc_R8,
        Conv_I,
        Newarr,
        Stfld,
        Conv_Ovf_I4_Un,
        Conv_Ovf_U,
        Constrained,
        Ldflda,
        Shr,

        //Pseudo instruction
        StackSpace,
        Leave,
        Cpobj,
        Conv_Ovf_U4,
        Conv_U2,
        Bgt_Un,
        Ldind_R4,
        Ble_Un,
        Stsfld,
        Brtrue,
        Ldind_U4,
        Unaligned,
        Brfalse,
        Ldind_U1,
        Stind_I1,
        Ldelem_U4,
        Ldelem_Any,
        Conv_I8,
        Conv_R4,
        Stelem_I2,
        Ldarg,
        Endfilter,
        Conv_Ovf_U8_Un,
        Mul_Ovf_Un,
        Cgt,
        Div,
        Mul,
        And,
        Starg,
        Mul_Ovf,
        Nop,
        Conv_Ovf_U2,
        Ldind_I1,
        Tail,
        Or,
        Ldvirtftn,
        Callvirt,
        Sizeof,
        Xor,
        Ldind_I2,
        Readonly,
        Stelem_R8,
        Conv_Ovf_I2_Un,
        Conv_Ovf_U1_Un,
        Conv_U4,
        Bge_Un,
        Newobj,
        Pop,
        Ldind_R8,
        Conv_Ovf_I8,
        Conv_Ovf_I1,
        Conv_Ovf_U_Un,
        Conv_U8,
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct Instruction
    {
        /// <summary>
        /// 指令MAGIC
        /// </summary>
        public const ulong INSTRUCTION_FORMAT_MAGIC = 5059304422653741615;

        /// <summary>
        /// 当前指令
        /// </summary>
        public Code Code;

        /// <summary>
        /// 操作数
        /// </summary>
        public int Operand;
    }

    public enum ExceptionHandlerType
    {
        Catch = 0,
        Filter = 1,
        Finally = 2,
        Fault = 4
    }

    public sealed class ExceptionHandler
    {
        public System.Type CatchType;
        public int CatchTypeId;
        public int HandlerEnd;
        public int HandlerStart;
        public ExceptionHandlerType HandlerType;
        public int TryEnd;
        public int TryStart;
    }
}