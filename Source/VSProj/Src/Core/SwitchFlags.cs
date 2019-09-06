/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE.txt' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
 */

using System;

namespace IFix
{
    //�л�������ִ��
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class InterpretAttribute : Attribute
    {
    }

    //ֱ����Ҫ���ɲ����ķ����ϴ��ǩ
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchAttribute : Attribute
    {
    }

    //�����ֶ�ָ��Ҫ����delegate����Ҫ���ڱհ�����interface������������﷨�ǣ����Ž�
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomBridgeAttribute : Attribute
    {

    }
}