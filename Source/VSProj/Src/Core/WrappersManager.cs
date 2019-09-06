/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

using System;

namespace IFix.Core
{
    //�ýӿ���ע�����Զ�ʵ��
    public interface WrappersManager
    {
        //����һ��delegate�����anon�ǿվ��Ǳհ�
        Delegate CreateDelegate(Type type, int id, object anon);
        //����һ��interface�Ž���
        AnonymousStorey CreateBridge(int fieldNum, int[] slots, VirtualMachine virtualMachine);
        //����һ��wrapper���󣨻��ɲ��������߼����ã����������wrapper���飩
        object CreateWrapper(int id);
        //��ʼ��wrapper����
        object InitWrapperArray(int len);
    }
}
