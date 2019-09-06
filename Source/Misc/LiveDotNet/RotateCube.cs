/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE.txt' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
 */

using UnityEngine;
using IFix;

//������ʾ�޸Ĵ��������ˢ�µ����
public class RotateCube : MonoBehaviour
{
    public Light theLight;

    [Patch]
    void Update()
    {
        //��ת
        transform.Rotate(Vector3.up * Time.deltaTime * 20);
        //�ı���ɫ
        theLight.color = new Color(Mathf.Sin(Time.time) / 2 + 0.5f, 0, 0, 1);
    }
}
