/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE.txt' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
 */

using IFix.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IFix.Test
{
    public class PerfTest
    {
        //��׼���ԣ��շ�������
        static void Base()
        {
            int LOOPS = 10000000;
            var virtualMachine = SimpleVirtualMachineBuilder.CreateVirtualMachine(LOOPS);

            for (int i = 0; i < 3; i++)
            {
                var sw = Stopwatch.StartNew();
                Call call = Call.Begin();
                virtualMachine.Execute(1, ref call, 0);
                Call.End(ref call);
                sw.Stop();
                Console.WriteLine("Base " + i + "  : " + (LOOPS / (int)sw.Elapsed.TotalMilliseconds * 1000) + "\r\n");
            }
        }

        //ͨ��Call�������add�������÷����߼����£�SimpleVirtualMachineBuilderͨ��Ӳ����ָ����
        //int add(int a, int b)
        //{
        //    return a + b;
        //}
        //ԭ������ͨ�����ַ�ʽ�������������
        static void SafeCall()
        {
            int LOOPS = 10000000;
            var virtualMachine = SimpleVirtualMachineBuilder.CreateVirtualMachine(LOOPS);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < LOOPS; i++)
            {
                Call call = Call.Begin();
                call.PushInt32(4);
                call.PushInt32(6);
                virtualMachine.Execute(0, ref call, 2);
                Call.End(ref call);
                call.GetInt32();
            }
            Console.WriteLine("SafeCall " + "  : " + (LOOPS / (int)sw.Elapsed.TotalMilliseconds * 1000) + "\r\n");
        }

        //ֱ��ͨ��ָ�����ջ������add����
        //������ڲ������������ͨ�����ַ�ʽ
        unsafe static void UnsafeCall()
        {
            IntPtr nativePointer = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(Value)
                * VirtualMachine.MAX_EVALUATION_STACK_SIZE);
            Value* evaluationStackPointer = (Value*)nativePointer.ToPointer();
            object[] managedStack = new object[VirtualMachine.MAX_EVALUATION_STACK_SIZE];

            int LOOPS = 10000000;
            var virtualMachine = SimpleVirtualMachineBuilder.CreateVirtualMachine(LOOPS);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < LOOPS; i++)
            {
                evaluationStackPointer->Value1 = 10;
                evaluationStackPointer->Type = IFix.Core.ValueType.Integer;

                (evaluationStackPointer + 1)->Value1 = 20;
                (evaluationStackPointer + 1)->Type = IFix.Core.ValueType.Integer;

                virtualMachine.Execute(0, evaluationStackPointer, managedStack, evaluationStackPointer, 2);
            }
            Console.WriteLine("UnsafeCall " + "  : " + (LOOPS / (int)sw.Elapsed.TotalMilliseconds * 1000) + "\r\n");

            System.Runtime.InteropServices.Marshal.FreeHGlobal(nativePointer);
        }

        public static void Main(string[] args)
        {
            Base();
            SafeCall();
            UnsafeCall();
            Console.Read();
        }
    }
}
