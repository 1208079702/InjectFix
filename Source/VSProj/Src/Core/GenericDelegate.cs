/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//ifix������հ�����Ահ���Ӧ��delegate����������
//�����е������ԭ���Ǹ��ط������ñհ����޸�ʱ���˱հ�����ʱ�ᱨ�Ҳ����������Ĵ���
//�����������ͨ��CustomBridage���������⣬���Ǻܶ�ʱ���û��޷�Ԥ֪���������
//�������Ϊ�˼������������Ӱ�죺��������������4�����Ҿ�Ϊ�������ͣ�
//�޷���ֵ���߷���ֵ���������ͣ������ܹ�������ͨ�����ͣ��Զ�������������

namespace IFix.Core
{
    internal class GenericDelegateFactory
    {
        //�޷���ֵ���ͷ���
        static MethodInfo[] genericAction = null;
        //�з���ֵ���ͷ���
        static MethodInfo[] genericFunc = null;

        //����delegate�������������Ļ���
        static Dictionary<Type, Func<GenericDelegate, Delegate>> genericDelegateCreatorCache
            = new Dictionary<Type, Func<GenericDelegate, Delegate>>();

        //Prevent unity il2cpp code stripping
        static void PreventStripping(object obj)
        {
            if (obj != null)
            {
                var gd = new GenericDelegate(null, -1, null);
                gd.Action();
                gd.Action(obj);
                gd.Action(obj, obj);
                gd.Action(obj, obj, obj);
                gd.Action(obj, obj, obj, obj);

                gd.Func<object>();
                gd.Func<object, object>(obj);
                gd.Func<object, object, object>(obj, obj);
                gd.Func<object, object, object, object>(obj, obj, obj);
                gd.Func<object, object, object, object, object>(obj, obj, obj, obj);
            }
        }

        internal static Delegate Create(Type delegateType, VirtualMachine virtualMachine, int methodId, object anonObj)
        {
            Func<GenericDelegate, Delegate> genericDelegateCreator;
            if (!genericDelegateCreatorCache.TryGetValue(delegateType, out genericDelegateCreator))
            {
                //������ͷ�������δ��ʼ��
                if (genericAction == null)
                {
                    PreventStripping(null);
                    var methods = typeof(GenericDelegate).GetMethods(BindingFlags.Instance | BindingFlags.Public
                        | BindingFlags.DeclaredOnly);
                    genericAction = methods.Where(m => m.Name == "Action").OrderBy(m => m.GetParameters().Length)
                        .ToArray();
                    genericFunc = methods.Where(m => m.Name == "Func").OrderBy(m => m.GetParameters().Length).ToArray();
                }

                MethodInfo delegateMethod = delegateType.GetMethod("Invoke");

                var parameters = delegateMethod.GetParameters();
                if ((delegateMethod.ReturnType.IsValueType && delegateMethod.ReturnType != typeof(void)) 
                    || parameters.Length > 4
                    || parameters.Any(p => p.ParameterType.IsValueType || p.ParameterType.IsByRef)
                    )
                {
                    //�������֧�ֵķ�Χ��������һ����Զ���ؿյĹ�����
                    genericDelegateCreator = (x) => null;
                }
                else
                {
                    if (delegateMethod.ReturnType == typeof(void) && parameters.Length == 0)
                    {
                        //���޲��޷���ֵ���⴦��
                        var methodInfo = genericAction[0];
                        genericDelegateCreator = (o) => Delegate.CreateDelegate(delegateType, o, methodInfo);
                    }
                    else
                    {
                        //���ݲ�������������ֵ�ҵ�����ʵ��
                        var typeArgs = parameters.Select(pinfo => pinfo.ParameterType);
                        MethodInfo genericMethodInfo = null;
                        if (delegateMethod.ReturnType == typeof(void))
                        {
                            genericMethodInfo = genericAction[parameters.Length];
                        }
                        else
                        {
                            genericMethodInfo = genericFunc[parameters.Length];
                            //������з���ֵ����Ҫ���Ϸ���ֵ��Ϊ����ʵ��
                            typeArgs = typeArgs.Concat(new Type[] { delegateMethod.ReturnType });
                        }
                        //ʵ�������ͷ���
                        var methodInfo = genericMethodInfo.MakeGenericMethod(typeArgs.ToArray());
                        //������
                        genericDelegateCreator = (o) => Delegate.CreateDelegate(delegateType, o, methodInfo);
                    }
                }
                //���湹�������´ε���ֱ�ӷ���
                genericDelegateCreatorCache[delegateType] = genericDelegateCreator;
            }
            //����delegate
            return genericDelegateCreator(new GenericDelegate(virtualMachine, methodId, anonObj));
        }
    }

    //����������
    internal class GenericDelegate
    {
        //ָ������������
        VirtualMachine virtualMachine;

        //���������id
        int methodId;

        //�󶨵���������
        object anonObj;

        //Ԥ���㣬�Ƿ�Ҫ��anonObj push�ı�־δ
        bool pushSelf;

        //Ԥ���㣬�����anonObj����������Ҫ+1
        int extraArgNum;

        internal GenericDelegate(VirtualMachine virtualMachine, int methodId, object anonObj)
        {
            this.virtualMachine = virtualMachine;
            this.methodId = methodId;
            this.anonObj = anonObj;
            pushSelf = anonObj != null;
            extraArgNum = pushSelf ? 1 : 0;
        }

        public void Action()
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            virtualMachine.Execute(methodId, ref call, extraArgNum);
        }

        public void Action<T1>(T1 p1)
            where T1 : class
        {
            //����call����
            Call call = Call.Begin();
            if (pushSelf)
            {
                //����а󶨵���������push
                call.PushObject(anonObj);
            }
            //push��һ������
            call.PushObject(p1);
            //����ָ��id�����������
            virtualMachine.Execute(methodId, ref call, 1 + extraArgNum);
        }

        public void Action<T1, T2>(T1 p1, T2 p2) 
            where T1 : class
            where T2 : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            call.PushObject(p2);
            virtualMachine.Execute(methodId, ref call, 2 + extraArgNum);
        }

        public void Action<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            call.PushObject(p2);
            call.PushObject(p3);
            virtualMachine.Execute(methodId, ref call, 3 + extraArgNum);
        }

        public void Action<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            call.PushObject(p2);
            call.PushObject(p3);
            call.PushObject(p4);
            virtualMachine.Execute(methodId, ref call, 4 + extraArgNum);
        }

        public TResult Func<TResult>()
            where TResult : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            virtualMachine.Execute(methodId, ref call, extraArgNum);
            return (TResult)call.GetObject();
        }

        public TResult Func<T1, TResult>(T1 p1)
            where T1 : class
            where TResult : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            virtualMachine.Execute(methodId, ref call, 1 + extraArgNum);
            //��ջ�ϻ�ȡ���
            return (TResult)call.GetObject();
        }

        public TResult Func<T1, T2, TResult>(T1 p1, T2 p2)
            where T1 : class
            where T2 : class
            where TResult : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            call.PushObject(p2);
            virtualMachine.Execute(methodId, ref call, 2 + extraArgNum);
            return (TResult)call.GetObject();
        }

        public TResult Func<T1, T2, T3, TResult>(T1 p1, T2 p2, T3 p3)
            where T1 : class
            where T2 : class
            where T3 : class
            where TResult : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            call.PushObject(p2);
            call.PushObject(p3);
            virtualMachine.Execute(methodId, ref call, 3 + extraArgNum);
            return (TResult)call.GetObject();
        }

        public TResult Func<T1, T2, T3, T4, TResult>(T1 p1, T2 p2, T3 p3, T4 p4)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where TResult : class
        {
            Call call = Call.Begin();
            if (pushSelf)
            {
                call.PushObject(anonObj);
            }
            call.PushObject(p1);
            call.PushObject(p2);
            call.PushObject(p3);
            call.PushObject(p4);
            virtualMachine.Execute(methodId, ref call, 4 + extraArgNum);
            return (TResult)call.GetObject();
        }
    }
}
