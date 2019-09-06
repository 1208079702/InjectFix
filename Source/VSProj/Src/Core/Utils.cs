/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

using System;
using System.Reflection;
using System.Collections.Generic;

namespace IFix.Core
{
    //�����ʹ�ø�������
    public static class Utils
    {
        /// <summary>
        /// �ж�һ�������Ƿ��ܸ�ֵ��һ��delegate����
        /// </summary>
        /// <param name="delegateMethod">delegate������������ͷ��invoke����</param>
        /// <param name="method">����ֵ�ķ���</param>
        /// <returns>�Ƿ��ܸ�ֵ</returns>
        public static bool IsAssignable(MethodInfo delegateMethod, MethodInfo method)
        {
            if (delegateMethod == null || method == null)
            {
                return false;
            }
            if (delegateMethod.ReturnType != method.ReturnType)
            {
                return false;
            }
            ParameterInfo[] lhsParams = delegateMethod.GetParameters();
            ParameterInfo[] rhsParams = method.GetParameters();
            if (lhsParams.Length != rhsParams.Length)
            {
                return false;
            }

            for (int i = 0; i < lhsParams.Length; i++)
            {
                if (lhsParams[i].ParameterType != rhsParams[i].ParameterType
                    || lhsParams[i].IsOut != rhsParams[i].IsOut)
                {
                    return false;
                }
            }

            return true;
        }

        //�������Ļ��棬����������棬ÿ�ζ�����IsAssignableһ������ȡƥ���ǳ���
        static Dictionary<Type, MethodInfo> delegateAdptCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// ��һ��wrapper������ͷ�������ܹ����䵽�ض�delegate�ķ���
        /// </summary>
        /// <param name="obj">wrapper����</param>
        /// <param name="delegateType">delegate����</param>
        /// <param name="perfix">����ǰ׺���ܹ��ų���һЩ���������繹�캯��</param>
        /// <returns></returns>
        public static Delegate TryAdapterToDelegate(object obj, Type delegateType, string perfix)
        {
            MethodInfo method;
            if (!delegateAdptCache.TryGetValue(delegateType, out method))
            {
                MethodInfo delegateMethod = delegateType.GetMethod("Invoke");
                var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance
                    | BindingFlags.DeclaredOnly);
                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name.StartsWith(perfix) && IsAssignable(delegateMethod, methods[i]))
                    {
                        method = methods[i];
                        delegateAdptCache[delegateType] = method;
                    }
                }
            }
            if (method == null)
            {
                return null;
            }
            else
            {
                return Delegate.CreateDelegate(delegateType, obj, method);
            }
        }
    }
}