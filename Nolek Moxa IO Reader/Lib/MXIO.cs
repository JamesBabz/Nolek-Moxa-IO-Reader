/******************************************************************************************/
/*  Copyright (C) MOXA Inc. All rights reserved.                                          */
/*                                                                                        */
/*  This software is distributed under the terms of the                                   */
/*  MOXA License.  See the file COPYING-MOXA for details.                                 */
/*                                                                                        */
/******************************************************************************************/

/******************************************************************************************/
/*  ---- Important: ----                                                                  */
/*  Windows CE platform not contain a definition for 'Pack', you will get CS0117 error.   */
/*  define preprocessor symbols before first token in file in windows CE platform.        */
/******************************************************************************************/
//#define _NET_WINCE
//==========================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Nolek_Moxa_IO_Reader.Lib
{
    class MXIO_CS
    {
        /********************************************************************************************/
        /*                                                                                          */
        /*                      MXIO.NET Library 1.8.0.0 - Function Prototypes                      */
        /*                      Copyright (C) 2010-2013 MOXA Technologies Co., Ltd.                 */
        /*                                                                                          */
        /********************************************************************************************/
        //Active Tag Struct Define
        // ========================
        public const int SUPPORT_MAX_SLOT = 16;
        public const int SUPPORT_MAX_CHANNEL = 64;
        public const int SupportMaxChOfBit = SUPPORT_MAX_CHANNEL >> 3;
        //
#if _NET_WINCE
        [StructLayout(LayoutKind.Explicit, Size = 4)]
#else
        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
#endif
        public struct _ANALOG_VAL
        {
            [FieldOffset(0)]
            public UInt32 iVal;

            [FieldOffset(0)]
            public float fVal;

            [FieldOffset(0)]
            public byte BytVal_0;
            [FieldOffset(1)]
            public byte BytVal_1;
            [FieldOffset(2)]
            public byte BytVal_2;
            [FieldOffset(3)]
            public byte BytVal_3;
        }

        //V1.2 OPC Tag DATA Struct
#if _NET_WINCE
        [StructLayout(LayoutKind.Sequential)]
#else
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
        public struct _IOLOGIKSTRUCT
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] BytMagic;
            public UInt16 wVersion;         // struct define of version 1.0.0
            public UInt16 wLength;
            public UInt16 wHWID;            // for user to know which API to Read/write
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] dwSrcIP;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] BytSrcMAC;
            public byte BytMsgType;         // for AP side to known what kind of Message return
            public UInt16 wMsgSubType;
            //------------------------
            // tag timestamp
            public UInt16 wYear;
            public byte BytMonth;
            public byte BytDay;
            public byte BytHour;
            public byte BytMin;
            public byte BytSec;
            public UInt16 wMSec;
            //-------------------------
            public byte BytLastSlot;        //add to notice the last slot, Range 0-16, 0=>myself only
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUPPORT_MAX_SLOT)]
            public byte[] BytLastCh;
            //-------------------------
            // support up to 16 slots and 64 channels //size:5408 bytes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUPPORT_MAX_SLOT * SUPPORT_MAX_CHANNEL)]
            public byte[] BytChType;    // channel I/O type
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUPPORT_MAX_SLOT)]
            public UInt16[] wSlotID;    // Slot Module ID
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUPPORT_MAX_SLOT * SupportMaxChOfBit)]
            public byte[] BytChNumber;  // bitwised¡A1=Enable¡A0=Disable
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUPPORT_MAX_SLOT * SUPPORT_MAX_CHANNEL)] //
            public _ANALOG_VAL[] dwChValue;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUPPORT_MAX_SLOT * SupportMaxChOfBit)]
            public byte[] BytChLocked;  // bitwised, 0=free, 1=locked
        }
        //
#if _NET_WINCE
        [StructLayout(LayoutKind.Sequential)]
#else
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
        public struct _ACTDEV_IO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] BytSrcMAC;
            public Int32 iHandle;
        }
        //
#if _NET_WINCE
        [StructLayout(LayoutKind.Sequential)]
#else
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
        public struct _MODULE_LIST
        {
            public UInt16 nModuleID;    //Module ID of device
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] szModuleIP;   //Save IP address of device
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] szMAC;        //Save MAC address of device
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] szModuleIP1;  //Save 2nd IP address of device
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] szMAC1;       //Save 2nd MAC address of device
            public byte bytLanUse;      //0 -> Lan0, 1 -> Lan1
        }

        public delegate void pfnCALLBACK(IntPtr bytData, UInt16 wSize);

        public delegate void pfnTagDataCALLBACK(_IOLOGIKSTRUCT[] ios, UInt16[] wMutex);
        /*************************************************/
        /*                                               */
        /*     RS232 & RS485  Module Connect Command     */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXSIO_OpenCommport(byte[] szCommport, UInt32 dwBaudrate, byte bytDataFormat, UInt32 dwTimeout, Int32[] hCommport);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXSIO_CloseCommport(Int32 hCommport);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXSIO_Connect(Int32 hCommport, byte bytUnitID, byte bytTransmissionMode, Int32[] hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXSIO_Disconnect(Int32 hConnection);

        /**********************************************/
        /*                                            */
        /*     Ethernet Module Connect Command        */
        /*                                            */
        /**********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Init();

        [DllImport("MXIO_NET.dll")]
        public static extern void MXEIO_Exit();

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Connect(byte[] szIP, UInt16 wPort, UInt32 dwTimeOut, Int32[] hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Disconnect(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_CheckConnection(Int32 hConnection, UInt32 dwTimeOut, byte[] bytStatus);

        /***********************************************/
        /*                                             */
        /*              General Command                */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_GetDllVersion();

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_GetDllBuildDate();

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_GetModuleType(Int32 hConnection, byte bytSlot, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ReadFirmwareRevision(Int32 hConnection, byte[] bytRevision);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ReadFirmwareDate(Int32 hConnection, UInt16[] wDate);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_Restart(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_Reset(Int32 hConnection);

        /***********************************************/
        /*                                             */
        /*              Modbus Command                 */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ReadCoils(Int32 hConnection, byte bytCoilType, UInt16 wStartCoil, UInt16 wCount, byte[] bytCoils);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_WriteCoils(Int32 hConnection, UInt16 wStartCoil, UInt16 wCount, byte[] bytCoils);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ReadRegs(Int32 hConnection, byte bytRegisterType, UInt16 wStartRegister, UInt16 wCount, UInt16[] wRegister);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_WriteRegs(Int32 hConnection, UInt16 wStartRegister, UInt16 wCount, UInt16[] wRegister);

        /***********************************************/
        /*                                             */
        /*           Digital Input Command             */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DI_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI_Read(Int32 hConnection, byte bytSlot, byte bytChannel, byte[] bytValue);

        /***********************************************/
        /*                                             */
        /*          Digital Output Command             */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Writes(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Read(Int32 hConnection, byte bytSlot, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Write(Int32 hConnection, byte bytSlot, byte bytChannel, byte bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_GetSafeValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_SetSafeValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_GetSafeValue(Int32 hConnection, byte bytSlot, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_SetSafeValue(Int32 hConnection, byte bytSlot, byte bytChannel, byte bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_GetSafeValues_W(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValues);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_SetSafeValues_W(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValues);

        /***********************************************/
        /*                                             */
        /*           Analog Input Command              */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int AI_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI_Read(Int32 hConnection, byte bytSlot, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI_ReadRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*           Analog Output Command             */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int AO_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_Writes(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_Read(Int32 hConnection, byte bytSlot, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_Write(Int32 hConnection, byte bytSlot, byte bytChannel, double dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_WriteRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_ReadRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_WriteRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16 wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_GetSafeValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_SetSafeValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_GetSafeValue(Int32 hConnection, byte bytSlot, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_SetSafeValue(Int32 hConnection, byte bytSlot, byte bytChannel, double dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_GetSafeRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_SetSafeRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_GetSafeRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO_SetSafeRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16 wValue);

        /***********************************************/
        /*                                             */
        /*                RTD Command                  */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int RTD_Read(Int32 hConnection, byte bytSlot, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD_ReadRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*   ioLogik 2000 RTD command                  */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ResetMin(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ResetMax(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetChannelStatus(Int32 hConnection, byte bytChannel, byte[] bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetChannelStatus(Int32 hConnection, byte bytChannel, byte bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetEngUnit(Int32 hConnection, byte bytChannel, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetEngUnit(Int32 hConnection, byte bytChannel, UInt16 wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetSensorType(Int32 hConnection, byte bytChannel, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetSensorType(Int32 hConnection, byte bytChannel, UInt16 wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetMathPar(Int32 hConnection, byte bytChannel, UInt16[] wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetMathPar(Int32 hConnection, byte bytChannel, UInt16 wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_GetMathPars(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetMathPars(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMinRaw(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMaxRaw(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMin(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMax(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetChnAvg(Int32 hConnection, byte bytChannel, byte[] bytChnIdx, byte bytChCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int RTD2K_SetChnDev(Int32 hConnection, byte bytChannel, byte bytChMinued, byte bytChSub);

        /***********************************************/
        /*                                             */
        /*                TC Command                   */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int TC_Read(Int32 hConnection, byte bytSlot, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC_ReadRaw(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*   ioLogik 2000 TC command                   */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadRaw(Int32 hConnection, byte bytChannel, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ResetMin(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ResetMax(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetChannelStatus(Int32 hConnection, byte bytChannel, byte[] bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetChannelStatus(Int32 hConnection, byte bytChannel, byte bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetEngUnit(Int32 hConnection, byte bytChannel, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetEngUnit(Int32 hConnection, byte bytChannel, UInt16 wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetSensorType(Int32 hConnection, byte bytChannel, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetSensorType(Int32 hConnection, byte bytChannel, UInt16 wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetMathPar(Int32 hConnection, byte bytChannel, UInt16[] wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetMathPar(Int32 hConnection, byte bytChannel, UInt16 wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_GetMathPars(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetMathPars(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMathPar);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMinRaw(Int32 hConnection, byte bytChannel, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMaxRaw(Int32 hConnection, byte bytChannel, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMin(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMax(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetChnAvg(Int32 hConnection, byte bytChannel, byte[] bytChnIdx, byte bytChCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int TC2K_SetChnDev(Int32 hConnection, byte bytChannel, byte bytChMinued, byte bytChSub);

        /***********************************************/
        /*                                             */
        /*    ioLogik 2000 special Module command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int Module2K_GetSafeStatus(Int32 hConnection, UInt16[] wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Module2K_ClearSafeStatus(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int Module2K_GetInternalReg(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Module2K_SetInternalReg(Int32 hConnection, byte bytChannel, UInt16 wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Module2K_GetInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Module2K_SetInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*   ioLogik 4000 special Adapter command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int Adp4K_ReadStatus(Int32 hConnection, UInt16[] wBusStatus, UInt16[] wFPStatus, UInt16[] wEWStatus, UInt16[] wESStatus, UInt16[] wECStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Adp4K_ClearStatus(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int Adp4K_ReadFirmwareRevision(Int32 hConnection, UInt16[] wRevision);

        [DllImport("MXIO_NET.dll")]
        public static extern int Adp4K_ReadFirmwareDate(Int32 hConnection, UInt16[] wDate);

        [DllImport("MXIO_NET.dll")]
        public static extern int Adp4K_ReadSlotAmount(Int32 hConnection, UInt16[] wAmount);

        [DllImport("MXIO_NET.dll")]
        public static extern int Adp4K_ReadAlarmedSlot(Int32 hConnection, UInt32[] dwAlarm);

        /***********************************************/
        /*                                             */
        /*    ioLogik 4000 Digital output command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DO4K_GetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO4K_SetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO4K_GetSafeAction(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO4K_SetSafeAction(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16 wAction);

        /***********************************************/
        /*                                             */
        /*    ioLogik 4000 Analog output command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int AO4K_GetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO4K_SetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO4K_GetSafeAction(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO4K_SetSafeAction(Int32 hConnection, byte bytSlot, byte bytChannel, UInt16 wAction);

        /***********************************************/
        /*                                             */
        /*    ioLogik 2000 Digital I/O  command        */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DIO2K_GetIOMode(Int32 hConnection, byte bytChannel, byte[] bytMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DIO2K_SetIOMode(Int32 hConnection, byte bytChannel, byte bytMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DIO2K_GetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DIO2K_SetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwMode);

        /***********************************************/
        /*                                             */
        /*    ioLogik 2000 Digital Input command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_GetMode(Int32 hConnection, byte bytChannel, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_SetMode(Int32 hConnection, byte bytChannel, UInt16 wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_GetFilter(Int32 hConnection, byte bytChannel, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI2K_SetFilter(Int32 hConnection, byte bytChannel, UInt16 wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwMode);

        /***********************************************/
        /*                                             */
        /*    ioLogik 2000 Digital output command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_GetMode(Int32 hConnection, byte bytChannel, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_SetMode(Int32 hConnection, byte bytChannel, UInt16 wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_GetPowerOnValue(Int32 hConnection, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_SetPowerOnValue(Int32 hConnection, byte bytChannel, byte bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_GetPowerOnSeqDelaytimes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO2K_SetPowerOnSeqDelaytimes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*       ioLogik 2000 Relay Count & Reset      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_GetResetTime(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_TotalCntRead(Int32 hConnection, byte bytChannel, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_TotalCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_CurrentCntRead(Int32 hConnection, byte bytChannel, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_CurrentCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_ResetCnt(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int RLY2K_ResetCnts(Int32 hConnection, byte bytStartChannel, byte bytCount);

        /***********************************************/
        /*                                             */
        /*       ioLogik 2000 Counter command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_Clears(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_Read(Int32 hConnection, byte bytChannel, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_Clear(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_ClearOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetOverflow(Int32 hConnection, byte bytChannel, byte[] bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_ClearOverflow(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetFilter(Int32 hConnection, byte bytChannel, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetFilter(Int32 hConnection, byte bytChannel, UInt16 wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetStartStatus(Int32 hConnection, byte bytChannel, byte[] bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetStartStatus(Int32 hConnection, byte bytChannel, byte bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetTriggerTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetTriggerTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetTriggerType(Int32 hConnection, byte bytChannel, byte[] bytType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetTriggerType(Int32 hConnection, byte bytChannel, byte bytType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetPowerOnValue(Int32 hConnection, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetPowerOnValue(Int32 hConnection, byte bytChannel, byte bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetSafeValue(Int32 hConnection, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetSafeValue(Int32 hConnection, byte bytChannel, byte bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_GetTriggerTypeWord(Int32 hConnection, byte bytChannel, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int Cnt2K_SetTriggerTypeWord(Int32 hConnection, byte bytChannel, UInt16 wType);

        /***********************************************/
        /*                                             */
        /*     ioLogik 2000 Pulse Output command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetSignalWidths(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetSignalWidths(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetSignalWidth(Int32 hConnection, byte bytChannel, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetSignalWidth(Int32 hConnection, byte bytChannel, UInt16 wHiWidth, UInt16 wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetSignalWidths32(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwHiWidth, UInt32[] dwLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetSignalWidths32(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwHiWidth, UInt32[] dwLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetSignalWidth32(Int32 hConnection, byte bytChannel, UInt32[] dwHiWidth, UInt32[] dwLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetSignalWidth32(Int32 hConnection, byte bytChannel, UInt32 dwHiWidth, UInt32 dwLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwOutputCounts);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwOutputCounts);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetOutputCount(Int32 hConnection, byte bytChannel, UInt32[] dwOutputCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetOutputCount(Int32 hConnection, byte bytChannel, UInt32 dwOutputCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetStartStatus(Int32 hConnection, byte bytChannel, byte[] bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetStartStatus(Int32 hConnection, byte bytChannel, byte bytStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetPowerOnValue(Int32 hConnection, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetPowerOnValue(Int32 hConnection, byte bytChannel, byte bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_GetSafeValue(Int32 hConnection, byte bytChannel, byte[] bytValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int Pulse2K_SetSafeValue(Int32 hConnection, byte bytChannel, byte bytValue);

        /***********************************************/
        /*                                             */
        /*     ioLogik 2000 Analog Input command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMin(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMinRaw(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ResetMin(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMax(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ReadMaxRaw(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_ResetMax(Int32 hConnection, byte bytChannel);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_SetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_GetRange(Int32 hConnection, byte bytChannel, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_SetRange(Int32 hConnection, byte bytChannel, UInt16 wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_GetChannelStatus(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_SetChannelStatus(Int32 hConnection, byte bytChannel, UInt16 wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI2K_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*    ioLogik 2000 Analog output command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_SetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_GetRange(Int32 hConnection, byte bytChannel, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_SetRange(Int32 hConnection, byte bytChannel, UInt16 wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_GetPowerOnValue(Int32 hConnection, byte bytChannel, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_SetPowerOnValue(Int32 hConnection, byte bytChannel, double dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_GetPowerOnRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_SetPowerOnRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_GetPowerOnRaw(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int AO2K_SetPowerOnRaw(Int32 hConnection, byte bytChannel, UInt16 wValue);

        /***********************************************/
        /*                                             */
        /*  ioLogik 2000 Click & Go Logic Command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int Logic2K_GetStartStatus(Int32 hConnection, UInt16[] wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int Logic2K_SetStartStatus(Int32 hConnection, UInt16 wStatus);

        /***********************************************/
        /*                                             */
        /*       ioLogik 2000 Message Command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int Message2K_Start(int iProtocol, UInt16 wPort, pfnCALLBACK iProcAddress);

        [DllImport("MXIO_NET.dll")]
        public static extern int Message2K_Stop(int iProtocol);

        /***********************************************/
        /*                                             */
        /*   ioLogik 4200 special Adapter command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E42_GetInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_SetInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_GetIOMapMode(Int32 hConnection, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_SetIOMapMode(Int32 hConnection, UInt16 wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_ReadStatus(Int32 hConnection, UInt16[] wState, UInt16[] wLastErrorCode);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_ClearStatus(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_ReadSlotAmount(Int32 hConnection, UInt16[] wAmount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_GetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_SetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_GetPowerOnValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_SetPowerOnValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_Message_Start(int iProtocol, UInt16 wPort, pfnCALLBACK iProcAddress);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_Message_Stop(int iProtocol);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_Logic_GetStartStatus(Int32 hConnection, UInt16[] wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_Logic_SetStartStatus(Int32 hConnection, UInt16 wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_ReadFirmwareRevision(Int32 hConnection, byte[] bytRevision);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_ReadFirmwareDate(Int32 hConnection, UInt16[] wDate);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DI_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_Writes(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AI_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AI_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_Writes(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_WriteRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_GetFaultValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_AO_SetFaultValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_RTD_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_RTD_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_TC_Reads(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_TC_ReadRaws(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_GetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_SetSafeActions(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32 wAction);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_GetFaultValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_SetFaultValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_GetPowerOnValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_DO_SetPowerOnValues(Int32 hConnection, byte bytSlot, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_Modbus_List(Int32 hConnection, byte[] FilePath);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_RTD_GetEngUnit(Int32 hConnection, byte bytSlot, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_RTD_SetEngUnit(Int32 hConnection, byte bytSlot, UInt16 wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_RTD_GetSensorType(Int32 hConnection, byte bytSlot, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_RTD_SetSensorType(Int32 hConnection, byte bytSlot, UInt16 wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_TC_GetEngUnit(Int32 hConnection, byte bytSlot, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_TC_SetEngUnit(Int32 hConnection, byte bytSlot, UInt16 wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_TC_GetSensorType(Int32 hConnection, byte bytSlot, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_TC_SetSensorType(Int32 hConnection, byte bytSlot, UInt16 wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_GetWorkInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E42_SetWorkInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /**********************************************/
        /*                                            */
        /*     Ethernet Module Connect Command        */
        /*                                            */
        /**********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_W5K_Connect(byte[] szIP, UInt16 wPort, UInt32 dwTimeOut, Int32[] hConnection, byte[] szMACAddr);

        /***********************************************/
        /*                                             */
        /*    ioLogik 5000 Digital Input command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        /***********************************************/
        /*                                             */
        /*       ioLogik 5000 Counter command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_Clears(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_ClearOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_SetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_GetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_SetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwMode);

        /***********************************************/
        /*                                             */
        /*    ioLogik 5000 Digital output command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_Writes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        /***********************************************/
        /*                                             */
        /*     ioLogik 5000 Pulse Output command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_GetSignalWidths32(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwHiWidth, UInt32[] dwLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_SetSignalWidths32(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwHiWidth, UInt32[] dwLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_GetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwOutputCounts);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_SetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwOutputCounts);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        /***********************************************/
        /*                                             */
        /*    ioLogik 5000 Digital I/O  command        */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DIO_GetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DIO_SetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwMode);

        /***********************************************/
        /*                                             */
        /*     ioLogik 5000 Analog Input command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_SetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*       ioLogik 5000 Relay Count & Reset      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_RLY_GetResetTime(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_RLY_TotalCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_RLY_CurrentCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_RLY_ResetCnts(Int32 hConnection, byte bytStartChannel, byte bytCount);

        /***********************************************/
        /*                                             */
        /*  ioLogik 5000 Click & Go Logic Command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Logic_GetStartStatus(Int32 hConnection, UInt16[] wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Logic_SetStartStatus(Int32 hConnection, UInt16 wStatus);

        /***********************************************/
        /*                                             */
        /*       ioLogik 5000 Message Command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Message_Start(int iProtocol, UInt16 wPort, pfnCALLBACK iProcAddress);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Message_Stop(int iProtocol);

        /***********************************************/
        /*                                             */
        /*    ioLogik 5000 special Module command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_SetInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetSafeStatus(Int32 hConnection, UInt16[] wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_ClearSafeStatus(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetCallerID(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_SetCallerID(Int32 hConnection, byte bytChannel, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetGprsSignal(Int32 hConnection, UInt16[] wSignal);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_ListOpcDevices(byte[] szIP, UInt32 dwTimeOut, UInt16[] wDeviceCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetOpcDevicesInfo(byte[] szIP, UInt32 dwTimeOut, UInt16 wDeviceCount, byte[] szDeviceInfo);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetOpcHostName(byte[] szIP, UInt32 dwTimeOut, byte[] szAliasName);

        /**********************************************/
        /*                                            */
        /*     Ethernet Module Connect Command        */
        /*                                            */
        /**********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_E1K_Connect(byte[] szIP, UInt16 wPort, UInt32 dwTimeOut, Int32[] hConnection, byte[] szPassword);

        /***********************************************/
        /*                                             */
        /*    ioLogik 1000 Digital Input command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DI_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DI_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DI_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DI_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DI_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        /***********************************************/
        /*                                             */
        /*       ioLogik 1000 Counter command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_Clears(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_ClearOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wFilter);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_SetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_GetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Cnt_SetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwMode);

        /***********************************************/
        /*                                             */
        /*    ioLogik 1000 Digital output command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_Writes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_GetSafeValues_W(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_SetSafeValues_W(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_GetPowerOnSeqDelaytimes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DO_SetPowerOnSeqDelaytimes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*     ioLogik 1000 Pulse Output command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_GetSignalWidths(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_SetSignalWidths(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_GetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwOutputCounts);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_SetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwOutputCounts);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_Pulse_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        /***********************************************/
        /*                                             */
        /*    ioLogik 1000 Digital I/O  command        */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DIO_GetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode);

        /***********************************************/
        /*                                             */
        /*     ioLogik 1000 Analog Input command       */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AI_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*       ioLogik 1000 Relay Count & Reset      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RLY_TotalCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        /***********************************************/
        /*                                             */
        /*    ioLogik 1000 special Module command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_GetSafeStatus(Int32 hConnection, UInt16[] wStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_ClearSafeStatus(Int32 hConnection);

        /***********************************************/
        /*                                             */
        /*    ioLogik 4200 special Module command      */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E42_ClearSafeStatus(Int32 hConnection);

        /***********************************************/
        /*                                             */
        /*  ioLogik W5000+E1200 special Module command */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_ReadStatus(int hConnection, UInt16[] wState, UInt16[] wLastErrorCode);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Exp_Reconnect(Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Exp_Status(int hConnection, UInt16[] wState);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_ReadSlotAmount(Int32 hConnection, UInt16[] wAmount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_ReadLastSlotIndex(Int32 hConnection, UInt16[] wAmount);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Modbus_List(Int32 hConnection, byte[] FilePath);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DIO_GetIOModes_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwMode, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_Reads_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_Writes_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DO_GetModes_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_Reads_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_DI_GetModes_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wMode, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_Reads_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Cnt_Clears_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_GetStartStatuses_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_Pulse_SetStartStatuses_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_Reads_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadRaws_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMins_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMinRaws_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMaxs_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_ReadMaxRaws_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_AI_GetRanges_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_VC_Reads_Ex(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue, byte bytSlot);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_GetWorkInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int W5K_SetWorkInternalRegs(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /***********************************************/
        /*                                             */
        /*  ioLogik TC special Module command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_GetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_SetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_GetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_SetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_TC_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        /***********************************************/
        /*                                             */
        /*  ioLogik RTD special Module command         */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwStatus);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_GetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_SetEngUnits(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wEngUnit);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_GetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_SetSensorTypes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wSensorType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_RTD_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        /***********************************************/
        /*                                             */
        /*  ioLogik AO special Module command          */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_Writes(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_WriteRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_GetSafeRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_SetSafeRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_SetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wRange);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_GetPowerOnRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_SetPowerOnRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_GetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_AO_SetChannelStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);
        /***************************************************************/
        /*                                                             */
        /*   ioLogik Special Module command for fix modbus address     */
        /*                                                             */
        /***************************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ReadCoils_Ex(Int32 hConnection, byte bytCoilType, UInt16 wStartCoil, UInt16 wCount, byte[] bytCoils);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_WriteCoils_Ex(Int32 hConnection, UInt16 wStartCoil, UInt16 wCount, byte[] bytCoils);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ReadRegs_Ex(Int32 hConnection, byte bytRegisterType, UInt16 wStartRegister, UInt16 wCount, UInt16[] wRegister);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_WriteRegs_Ex(Int32 hConnection, UInt16 wStartRegister, UInt16 wCount, UInt16[] wRegister);

        /***************************************************************/
        /*                                                             */
        /*   ioLogik Special Module command for Active Tag             */
        /*                                                             */
        /***************************************************************/
        /* MXIO_Init_ActiveTag() will cause exception. Remove it. */
        //[DllImport("MXIO_NET.dll")]
        //public static extern int MXIO_Init_ActiveTag( UInt16 wDataPort, UInt16 wCmdPort, UInt32 dwToleranceTimeout, UInt32 dwCmdTimeout, pfnTagDataCALLBACK iProcAddress, UInt16 wSize);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_Init_ActiveTag_Ex(UInt16 wDataPort, UInt16 wCmdPort, UInt32 dwToleranceTimeout, UInt32 dwCmdTimeout, pfnCALLBACK iProcAddress, UInt16 wSize);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ListDevs_ActiveTag(UInt16[] wDeviceCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_GetDevsInfo_ActiveTag(UInt16 wDeviceCount, byte[] szDeviceInfo);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_Close_ActiveTag();

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_Connect_ActiveTag(UInt32 dwTimeOut, Int32[] hConnection, byte[] szMACAddr, UInt16 wPort, byte[] szPassword);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_RelLock_ActiveTag();

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_GetSubType(Int32 hConnection, byte bytSlot, UInt32[] dwSubType);

        [DllImport("MXIO_NET.dll")]
        public static extern int E1K_DIO_SetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwMode);

        /***************************************************************/
        /*                                                             */
        /*   ioLogik General command for change duplicate IP address   */
        /*                                                             */
        /***************************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_AutoSearch(UInt32 nType, UInt32 nRetryCount, UInt32 nTimeout, UInt32[] nNumber, byte[] struML);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ListIF(UInt16[] wDeviceCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_GetIFInfo(UInt16 wIFCount, byte[] szIFInfo);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_SelectIF(UInt16 wIFCount, byte[] szIFInfo, UInt32 dwIFIndex);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_ChangeDupIP(byte[] szIP, UInt16 wPort, byte[] szMACAddr, UInt32 dwTimeOut, byte[] szPassword, byte[] szNewIP);

        /*************************************************/
        /*                                               */
        /*                  Overriding Function          */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXSIO_Connect(Int32 hCommport, byte bytUnitID, byte bytTransmissionMode, out Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Connect(byte[] szIP, UInt16 wPort, UInt32 dwTimeOut, out Int32 hConnection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_W5K_Connect(byte[] szIP, UInt16 wPort, UInt32 dwTimeOut, out Int32 hConnection, byte[] szMACAddr);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_E1K_Connect(byte[] szIP, UInt16 wPort, UInt32 dwTimeOut, out Int32 hConnection, byte[] szPassword);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXIO_Connect_ActiveTag(UInt32 dwTimeOut, out Int32 hConnection, byte[] szMACAddr, UInt16 wPort, byte[] szPassword);

        /*************************************************/
        /*                                               */
        /*      ioLogik R1000 Digital I/O command        */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DIO_GetIOModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        /*************************************************/
        /*                                               */
        /*      ioLogik R1000 Digital Input command      */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DI_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DI_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DI_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DI_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DI_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);
        /*************************************************/
        /*                                               */
        /*         ioLogik R1000 Counter command          */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_Clears(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_ClearOverflows(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_SetFilters(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_SetTriggerTypeWords(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_GetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Cnt_SetSaveStatusesOnPowerFail(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);
        /*************************************************/
        /*                                               */
        /*      ioLogik R1000 Digital output command     */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_Writes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_GetSafeValues_W(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_SetSafeValues_W(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_GetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_SetModes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_GetPowerOnSeqDelaytimes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_DO_SetPowerOnSeqDelaytimes(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /*************************************************/
        /*                                               */
        /*         ioLogik R1000 Relay Count & Reset      */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_RLY_TotalCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_RLY_CurrentCntReads(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_RLY_ResetCnts(Int32 hConnection, byte bytStartChannel, byte bytCount);

        /*************************************************/
        /*                                               */
        /*       ioLogik R1000 Pulse Output command       */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_GetSignalWidths(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_SetSignalWidths(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wHiWidth, UInt16[] wLoWidth);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_GetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_SetOutputCounts(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_GetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_SetStartStatuses(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32[] dwValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_Pulse_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt32 dwValue);

        /*************************************************/
        /*                                               */
        /*       ioLogik R1000 Analog Input command      */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ReadMins(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ReadMinRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ResetMins(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ReadMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ReadMaxRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_ResetMaxs(Int32 hConnection, byte bytStartChannel, byte bytCount);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AI_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        /*************************************************/
        /*                                               */
        /*      ioLogik R1000 Analog output command       */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_Reads(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_Writes(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_ReadRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_WriteRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_GetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_SetSafeValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_GetSafeRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_SetSafeRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_GetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_SetRanges(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_GetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_SetPowerOnValues(Int32 hConnection, byte bytStartChannel, byte bytCount, double[] dValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_GetPowerOnRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_AO_SetPowerOnRaws(Int32 hConnection, byte bytStartChannel, byte bytCount, UInt16[] wValue);
        /*************************************************/
        /*                                               */
        /*      ioLogik R1000 special Module command     */
        /*                                               */
        /*************************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_GetSafeStatus(Int32 hConnection, UInt16[] wValue);

        [DllImport("MXIO_NET.dll")]
        public static extern int R1K_ClearSafeStatus(Int32 hConnection);

        /*************************************************/
        /*                                               */
        /*                  Error Code                   */
        /*                                               */
        /*************************************************/
        public const int MXIO_OK = 0;

        public const int ILLEGAL_FUNCTION = 1001;
        public const int ILLEGAL_DATA_ADDRESS = 1002;
        public const int ILLEGAL_DATA_VALUE = 1003;
        public const int SLAVE_DEVICE_FAILURE = 1004;
        public const int SLAVE_DEVICE_BUSY = 1006;

        public const int EIO_TIME_OUT = 2001;
        public const int EIO_INIT_SOCKETS_FAIL = 2002;
        public const int EIO_CREATING_SOCKET_ERROR = 2003;
        public const int EIO_RESPONSE_BAD = 2004;
        public const int EIO_SOCKET_DISCONNECT = 2005;
        public const int PROTOCOL_TYPE_ERROR = 2006;
        public const int EIO_PASSWORD_INCORRECT = 2007;

        public const int SIO_OPEN_FAIL = 3001;
        public const int SIO_TIME_OUT = 3002;
        public const int SIO_CLOSE_FAIL = 3003;
        public const int SIO_PURGE_COMM_FAIL = 3004;
        public const int SIO_FLUSH_FILE_BUFFERS_FAIL = 3005;
        public const int SIO_GET_COMM_STATE_FAIL = 3006;
        public const int SIO_SET_COMM_STATE_FAIL = 3007;
        public const int SIO_SETUP_COMM_FAIL = 3008;
        public const int SIO_SET_COMM_TIME_OUT_FAIL = 3009;
        public const int SIO_CLEAR_COMM_FAIL = 3010;
        public const int SIO_RESPONSE_BAD = 3011;
        public const int SIO_TRANSMISSION_MODE_ERROR = 3012;
        public const int SIO_BAUDRATE_NOT_SUPPORT = 3013;

        public const int PRODUCT_NOT_SUPPORT = 4001;
        public const int HANDLE_ERROR = 4002;
        public const int SLOT_OUT_OF_RANGE = 4003;
        public const int CHANNEL_OUT_OF_RANGE = 4004;
        public const int COIL_TYPE_ERROR = 4005;
        public const int REGISTER_TYPE_ERROR = 4006;
        public const int FUNCTION_NOT_SUPPORT = 4007;
        public const int OUTPUT_VALUE_OUT_OF_RANGE = 4008;
        public const int INPUT_VALUE_OUT_OF_RANGE = 4009;
        public const int SLOT_NOT_EXIST = 4010;
        public const int FIRMWARE_NOT_SUPPORT = 4011;
        public const int CREATE_MUTEX_FAIL = 4012;
        public const int ENUM_NET_INTERFACE_FAIL = 5000;
        public const int ADD_INFO_TABLE_FAIL = 5001;
        public const int UNKNOWN_NET_INTERFACE_FAIL = 5002;
        public const int TABLE_NET_INTERFACE_FAIL = 5003;

        /*************************************************/
        /*                                               */
        /*              Data Format Setting              */
        /*                                               */
        /*************************************************/
        /* Data length define   */
        public const int BIT_5 = 0x00;
        public const int BIT_6 = 0x01;
        public const int BIT_7 = 0x02;
        public const int BIT_8 = 0x03;

        /* Stop bits define */
        public const int STOP_1 = 0x00;
        public const int STOP_2 = 0x04;

        /* Parity define    */
        public const int P_EVEN = 0x18;
        public const int P_ODD = 0x08;
        public const int P_SPC = 0x38;
        public const int P_MRK = 0x28;
        public const int P_NONE = 0x00;

        /*************************************************/
        /*                                               */
        /*        Modbus Transmission Mode Setting       */
        /*                                               */
        /*************************************************/
        public const int MODBUS_RTU = 0x0;
        public const int MODBUS_ASCII = 0x1;

        /*************************************************/
        /*                                               */
        /*            Check Connection Status            */
        /*                                               */
        /*************************************************/
        public const int CHECK_CONNECTION_OK = 0x0;
        public const int CHECK_CONNECTION_FAIL = 0x1;
        public const int CHECK_CONNECTION_TIME_OUT = 0x2;

        /*************************************************/
        /*                                               */
        /*            Modbus Coil Type Setting           */
        /*                                               */
        /*************************************************/
        public const int COIL_INPUT = 0x2;
        public const int COIL_OUTPUT = 0x1;

        /*************************************************/
        /*                                               */
        /*            Modbus Coil Type Setting           */
        /*                                               */
        /*************************************************/
        public const int REGISTER_INPUT = 0x4;
        public const int REGISTER_OUTPUT = 0x3;

        /*************************************************/
        /*                                               */
        /*             ioLogik 4000 Bus Status           */
        /*                                               */
        /*************************************************/
        public const int NORMAL_OPERATION = 0x0;
        public const int BUS_STANDYBY = 0x1;
        public const int BUS_COMMUNICATION_FAULT = 0x2;
        public const int SLOT_CONFIGURATION_FAILED = 0x3;
        public const int NO_EXPANSION_SLOT = 0x4;

        /*************************************************/
        /*                                               */
        /*        ioLogik 4000 Field Power Status        */
        /*                                               */
        /*************************************************/
        public const int FIELD_POWER_ON = 0x0;
        public const int FIELD_POWER_OFF = 0x1;

        /*************************************************/
        /*                                               */
        /*         ioLogik 4000 Watchdog Status          */
        /*                                               */
        /*************************************************/
        public const int WATCHDOG_NO_ERROR = 0x0;
        public const int WATCHDOG_ERROR = 0x1;

        /*************************************************/
        /*                                               */
        /*   ioLogik 4000 Modbus Error Setup Status      */
        /*                                               */
        /*************************************************/
        public const int SETUP_NO_ERROR = 0x0;
        public const int SETUP_ERROR = 0x1;

        /*************************************************/
        /*                                               */
        /*   ioLogik 4000 Modbus Error Check Status      */
        /*                                               */
        /*************************************************/
        public const int CHECKSUM_NO_ERROR = 0x0;
        public const int CHECKSUM_ERROR = 0x1;

        /*************************************************/
        /*                                               */
        /*         ioLogik 4000 AO Safe Action           */
        /*                                               */
        /*************************************************/
        public const int SAFE_VALUE = 0x00;     //Suport AO & DO module
        public const int HOLD_LAST_STATE = 0x01;//Suport AO & DO module
        public const int LOW_LIMIT = 0x02;      //Only suport AO module
        public const int HIGH_LIMIT = 0x03;     //Only suport AO module

        /*************************************************/
        /*                                               */
        /*               Protocol Type                   */
        /*                                               */
        /*************************************************/
        public const int PROTOCOL_TCP = 0x01;
        public const int PROTOCOL_UDP = 0x02;
        /*************************************************/
        /*                                               */
        /*               AI Channel Type                 */
        /*                                               */
        /*************************************************/
        public const int M2K_AI_RANGE_150mv = 0;
        public const int M2K_AI_RANGE_500mv = 1;
        public const int M2K_AI_RANGE_5V = 2;
        public const int M2K_AI_RANGE_10V = 3;
        public const int M2K_AI_RANGE_0_20mA = 4;
        public const int M2K_AI_RANGE_4_20mA = 5;
        public const int M2K_AI_RANGE_0_150mv = 6;
        public const int M2K_AI_RANGE_0_500mv = 7;
        public const int M2K_AI_RANGE_0_5V = 8;
        public const int M2K_AI_RANGE_0_10V = 9;
        /*************************************************/
        /*                                               */
        /*               W53xx BUS STATE                 */
        /*                                               */
        /*************************************************/
        public const int BUS_INIT = 0x00;
        public const int BUS_INIT_NET = 0x01;
        public const int BUS_INIT_FAULT = 0x02;
        public const int BUS_IO = 0x03;
        public const int BUS_IO_FAULT = 0x04;
        public const int BUS_IDLE = 0x05;
        /*************************************************/
        /*                                               */
        /*      W53xx Expansion Module Error Code        */
        /*                                               */
        /*************************************************/
        public const int EXPASION_NO_ERROR = 0x00;
        public const int EXPASION_COMMUNICATION_ERROR = 0x01;
        public const int EXPASION_MODBUS_ERROR = 0x02;
        public const int EXPASION_PENDING_ERROR = 0x03;
        public const int EXPASION_COMBINATION_ERROR = 0x04;
        public const int EXPASION_OUT_OF_MEMORY_ERROR = 0x05;
        /*************************************************/
        /*                                               */
        /*         W53xx Expansion Module State          */
        /*                                               */
        /*************************************************/
        public const int EXPASION_OFFLINE = 0x00;
        public const int EXPASION_ONLINE = 0x01;
        public const int EXPASION_UNMATCH_TYPE = 0x02;
        public const int EXPASION_RETURN_MODBUS_EXCEPTION = 0X03;
        public const int EXPASION_NETWORK_ERROR = 0X04;
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- nModuleType      */
        /*                                               */
        /*************************************************/
        public const int ACTTAG_E2210_ID = 0x2210;
        public const int ACTTAG_E2210_V2_ID = 0x2211;
        public const int ACTTAG_E2212_ID = 0x2212;
        public const int ACTTAG_E2214_ID = 0x2214;
        public const int ACTTAG_E2240_ID = 0x2240;
        public const int ACTTAG_E2240_V2_ID = 0x2241;
        public const int ACTTAG_E2242_ID = 0x2242;
        public const int ACTTAG_E2260_ID = 0x2260;
        public const int ACTTAG_E2262_ID = 0x2262;

        public const int ACTTAG_E1210_ID = 0xE210;
        public const int ACTTAG_E1211_ID = 0xE211;
        public const int ACTTAG_E1212_ID = 0xE212;
        public const int ACTTAG_E1214_ID = 0xE214;
        public const int ACTTAG_E1240_ID = 0xE240;
        public const int ACTTAG_E1241_ID = 0xE241;
        public const int ACTTAG_E1242_ID = 0xE242;
        public const int ACTTAG_E1260_ID = 0xE260;
        public const int ACTTAG_E1262_ID = 0xE262;
        public const int ACTTAG_E1261_ID = 0x5A;

        public const int ACTTAG_E2210T_ID = 0x150;
        public const int ACTTAG_E2212T_ID = 0x151;
        public const int ACTTAG_E2214T_ID = 0x152;
        public const int ACTTAG_E2240T_ID = 0x153;
        public const int ACTTAG_E2242T_ID = 0x154;
        public const int ACTTAG_E2260T_ID = 0x155;
        public const int ACTTAG_E2262T_ID = 0x156;

        public const int ACTTAG_E1210T_ID = 0x50;
        public const int ACTTAG_E1211T_ID = 0x51;
        public const int ACTTAG_E1212T_ID = 0x52;
        public const int ACTTAG_E1214T_ID = 0x53;
        public const int ACTTAG_E1240T_ID = 0x54;
        public const int ACTTAG_E1241T_ID = 0x55;
        public const int ACTTAG_E1242T_ID = 0x56;
        public const int ACTTAG_E1260T_ID = 0x57;
        public const int ACTTAG_E1262T_ID = 0x58;
        public const int ACTTAG_E1261T_ID = 0x59;

        public const int ACTTAG_W5340_ID = 0x5340;
        public const int ACTTAG_W5312_ID = 0x5312;
        public const int ACTTAG_E4200_ID = 0x4200;

        public const int ACTTAG_W5340TSLOT_ID = 0x100;
        public const int ACTTAG_W5312TSLOT_ID = 0x101;
        public const int ACTTAG_W5340SLOT_ID = 0x102;
        public const int ACTTAG_W5312SLOT_ID = 0x103;
        public const int ACTTAG_W5341SLOT_ID = 0x104;
        public const int ACTTAG_W5341TSLOT_ID = 0x105;

        public const int ACTTAG_W5340_HSDPA_ID = 0x106;
        public const int ACTTAG_W5312_HSDPA_ID = 0x107;
        public const int ACTTAG_W5341_HSDPA_ID = 0x108;
        public const int ACTTAG_W5340T_HSDPA_ID = 0x109;
        public const int ACTTAG_W5312T_HSDPA_ID = 0x10A;
        public const int ACTTAG_W5341T_HSDPA_ID = 0x10B;
        public const int ACTTAG_W5340_HSDPA_JPN_ID = 0x10C;
        public const int ACTTAG_W5340T_HSDPA_JPN_ID = 0x10E;
        public const int ACTTAG_W5312_HSDPA_JPN_ID = 0x10D;
        public const int ACTTAG_W5312T_HSDPA_JPN_ID = 0x10F;
        public const int ACTTAG_W5340_HSDPA_JPS_ID = 0x110;
        public const int ACTTAG_W5340T_HSDPA_JPS_ID = 0x112;
        public const int ACTTAG_W5312_HSDPA_JPS_ID = 0x111;
        public const int ACTTAG_W5312T_HSDPA_JPS_ID = 0x113;
        public const int ACTTAG_W5340_AP_ID = 0x114;

        public const int ACTTAG_IOPAC_8020_T_ID = 0x180;

        public const int ACTTAG_IOPAC_8020_5_RJ45_C_T_ID = 0x181;
        public const int ACTTAG_IOPAC_8020_5_M12_C_T_ID = 0x182;
        public const int ACTTAG_IOPAC_8020_9_RJ45_C_T_ID = 0x183;
        public const int ACTTAG_IOPAC_8020_9_M12_C_T_ID = 0x184;
        public const int ACTTAG_IOPAC_8020_5_RJ45_IEC_T_ID = 0x185;
        public const int ACTTAG_IOPAC_8020_5_M12_IEC_T_ID = 0x186;
        public const int ACTTAG_IOPAC_8020_9_RJ45_IEC_T_ID = 0x187;
        public const int ACTTAG_IOPAC_8020_9_M12_IEC_T_ID = 0x188;
        public const int ACTTAG_IOPAC_8020_5_RJ45_C_ID = 0x201;
        public const int ACTTAG_IOPAC_8020_5_M12_C_ID = 0x202;
        public const int ACTTAG_IOPAC_8020_9_RJ45_C_ID = 0x203;
        public const int ACTTAG_IOPAC_8020_9_M12_C_ID = 0x204;
        public const int ACTTAG_IOPAC_8020_5_RJ45_IEC_ID = 0x205;
        public const int ACTTAG_IOPAC_8020_5_M12_IEC_ID = 0x206;
        public const int ACTTAG_IOPAC_8020_9_RJ45_IEC_ID = 0x207;
        public const int ACTTAG_IOPAC_8020_9_M12_IEC_ID = 0x208;

        public const int ACTTAG_E1510_ID = 0x220;
        public const int ACTTAG_E1510_T_ID = 0x221;
        public const int ACTTAG_E1512_ID = 0x222;
        public const int ACTTAG_E1512_T_ID = 0x223;
        public const int ACTTAG_E1261H_ID = 0x340;
        public const int ACTTAG_E1261H_T_ID = 0x341;
        public const int ACTTAG_E1263H_ID = 0x342;
        public const int ACTTAG_E1263H_T_ID = 0x343;

        public const int ACTTAG_E1213_ID = 0x5B;
        public const int ACTTAG_E1213T_ID = 0x5C;

        public const int ACTTAG_W5348_GPRS_C_ID = 0x115;
        public const int ACTTAG_W5348_HSDPA_C_ID = 0x116;
        public const int ACTTAG_W5348_GPRS_IEC_ID = 0x117;
        public const int ACTTAG_W5348_HSDPA_IEC_ID = 0x118;
        public const int ACTTAG_W5348_GPRS_C_T_ID = 0x119;
        public const int ACTTAG_W5348_HSDPA_C_T_ID = 0x11A;
        public const int ACTTAG_W5348_GPRS_IEC_T_ID = 0x11B;
        public const int ACTTAG_W5348_HSDPA_IEC_T_ID = 0x11C;

        public const int ACTTAG_E1510_CT_T_ID = 0x224;
        public const int ACTTAG_E1512_CT_T_ID = 0x225;
        public const int ACTTAG_E1213_CT_ID = 0x5D;
        public const int ACTTAG_E1213_CT_T_ID = 0x5E;
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- BytMsgType       */
        /*                                               */
        /*************************************************/
        public const int ACTTAG_MSG_POWER_ON = 1;
        public const int ACTTAG_MSG_HEARTBEAT = 2;
        public const int ACTTAG_MSG_CONFIG = 3;
        public const int ACTTAG_MSG_IO_STATUS = 4;
        public const int ACTTAG_MSG_SYSTEM = 5;
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- BytMsgSubType    */
        /*                                               */
        /*************************************************/
        //under ACTTAG_MSG_SYSTEM
        public const int ACTTAG_MSG_SUB_HEARTBEAT_TIMEOUT = 1;
        public const int ACTTAG_MSG_SUB_READWRITE_TIMEOUT = 2;
        public const int ACTTAG_MSG_SUB_CLIENT_DISCONNECT = 3;
        public const int ACTTAG_MSG_SUB_SERVER_DISCONNECT = 4;

        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- BytChType        */
        /*                                               */
        /*************************************************/
        // 0~9=DIO mode
        public const int ACTTAG_DI_DI = 0;
        public const int ACTTAG_DO_DO = 1;
        public const int ACTTAG_DI_CNT = 2;
        public const int ACTTAG_DO_PULSE = 3;
        public const int ACTTAG_DI_DI_DISABLE = 4;
        public const int ACTTAG_DO_DO_DISABLE = 5;
        public const int ACTTAG_RLY_DO = 6;
        public const int ACTTAG_RLY_PULSE = 7;

        // 10~49=AIO range
        public const int ACTTAG_AI_DISABLE = 10;
        //AI 11
        public const int ACTTAG_AI_150_150MV = 11;
        public const int ACTTAG_AI_500_500MV = 12;
        public const int ACTTAG_AI_5_5V = 13;
        public const int ACTTAG_AI_10_10V = 14;
        public const int ACTTAG_AI_0_20MA = 15;
        public const int ACTTAG_AI_4_20MA = 16;
        public const int ACTTAG_AI_0_150MV = 17;
        public const int ACTTAG_AI_0_500MV = 18;
        public const int ACTTAG_AI_0_5V = 19;
        public const int ACTTAG_AI_0_10V = 20;
        //AO
        public const int ACTTAG_AO_DISABLE = 30;
        public const int ACTTAG_AO_0_10V = 31;
        public const int ACTTAG_AO_4_20MA = 32;
        public const int ACTTAG_AO_0_20MA = 33;
        public const int ACTTAG_AO_10_10V = 34;
        public const int ACTTAG_AO_0_5V = 35;

        // 50~99=TC
        public const int ACTTAG_TC_DISABLE = 50;
        //Celsius (C)
        public const int ACTTAG_TC_Type_K_C = 51;
        public const int ACTTAG_TC_Type_J_C = 52;
        public const int ACTTAG_TC_Type_T_C = 53;
        public const int ACTTAG_TC_Type_B_C = 54;
        public const int ACTTAG_TC_Type_R_C = 55;
        public const int ACTTAG_TC_Type_S_C = 56;
        public const int ACTTAG_TC_Type_E_C = 57;
        public const int ACTTAG_TC_Type_N_C = 58;
        public const int ACTTAG_TC_Type_L_C = 59;
        public const int ACTTAG_TC_Type_U_C = 60;
        public const int ACTTAG_TC_Type_C_C = 61;
        public const int ACTTAG_TC_Type_D_C = 62;

        //Fahrenheit (F)
        public const int ACTTAG_TC_Type_K_F = 71;
        public const int ACTTAG_TC_Type_J_F = 72;
        public const int ACTTAG_TC_Type_T_F = 73;
        public const int ACTTAG_TC_Type_B_F = 74;
        public const int ACTTAG_TC_Type_R_F = 75;
        public const int ACTTAG_TC_Type_S_F = 76;
        public const int ACTTAG_TC_Type_E_F = 77;
        public const int ACTTAG_TC_Type_N_F = 78;
        public const int ACTTAG_TC_Type_L_F = 79;
        public const int ACTTAG_TC_Type_U_F = 80;
        public const int ACTTAG_TC_Type_C_F = 81;
        public const int ACTTAG_TC_Type_D_F = 82;

        public const int ACTTAG_TC_VOLTAGE_78_126MV = 91;
        public const int ACTTAG_TC_VOLTAGE_39_062MV = 92;
        public const int ACTTAG_TC_VOLTAGE_19_532MV = 93;
        public const int ACTTAG_TC_VOLTAGE_78MV = 94;
        public const int ACTTAG_TC_VOLTAGE_32_7MV = 95;
        public const int ACTTAG_TC_VOLTAGE_65_5MV = 96;

        // 100~149=RTD
        public const int ACTTAG_RTD_DISABLE = 100;
        //Celsius (C)
        public const int ACTTAG_RTD_PT50_C = 101;
        public const int ACTTAG_RTD_PT100_C = 102;
        public const int ACTTAG_RTD_PT200_C = 103;
        public const int ACTTAG_RTD_PT500_C = 104;
        public const int ACTTAG_RTD_PT1000_C = 105;
        public const int ACTTAG_RTD_JPT100_C = 106;
        public const int ACTTAG_RTD_JPT200_C = 107;
        public const int ACTTAG_RTD_JPT500_C = 108;
        public const int ACTTAG_RTD_JPT1000_C = 109;
        public const int ACTTAG_RTD_NI100_C = 110;
        public const int ACTTAG_RTD_NI200_C = 111;
        public const int ACTTAG_RTD_NI500_C = 112;
        public const int ACTTAG_RTD_NI1000_C = 113;
        public const int ACTTAG_RTD_NI120_C = 114;
        public const int ACTTAG_RTD_CU10_C = 115;

        //Fahrenheit (F)
        public const int ACTTAG_RTD_PT50_F = 121;
        public const int ACTTAG_RTD_PT100_F = 122;
        public const int ACTTAG_RTD_PT200_F = 123;
        public const int ACTTAG_RTD_PT500_F = 124;
        public const int ACTTAG_RTD_PT1000_F = 125;
        public const int ACTTAG_RTD_JPT100_F = 126;
        public const int ACTTAG_RTD_JPT200_F = 127;
        public const int ACTTAG_RTD_JPT500_F = 128;
        public const int ACTTAG_RTD_JPT1000_F = 129;
        public const int ACTTAG_RTD_NI100_F = 130;
        public const int ACTTAG_RTD_NI200_F = 131;
        public const int ACTTAG_RTD_NI500_F = 132;
        public const int ACTTAG_RTD_NI1000_F = 133;
        public const int ACTTAG_RTD_NI120_F = 134;
        public const int ACTTAG_RTD_CU10_F = 135;

        public const int ACTTAG_RTD_RESISTANCE_1_310_ = 141;
        public const int ACTTAG_RTD_RESISTANCE_1_620_ = 142;
        public const int ACTTAG_RTD_RESISTANCE_1_1250_ = 143;
        public const int ACTTAG_RTD_RESISTANCE_1_2200_ = 144;
        public const int ACTTAG_RTD_RESISTANCE_1_2000_ = 145;
        public const int ACTTAG_RTD_RESISTANCE_1_327_ = 146;

        //Virtual channel
        public const int ACTTAG_VIRTUAL_CH_AVG_C = 201;
        public const int ACTTAG_VIRTUAL_CH_DIF_C = 202;
        public const int ACTTAG_VIRTUAL_CH_AVG_F = 203;
        public const int ACTTAG_VIRTUAL_CH_DIF_F = 204;
        public const int ACTTAG_VIRTUAL_CH_MCONNECT = 205;  //slot expansion module connection
        public const int ACTTAG_VIRTUAL_CH_DISABLE = 206;
        public const int ACTTAG_VIRTUAL_CH_VALUE = 207;

        public const int ACTTAG_SYSTEM_CONNECTION = 255;  //SYSTEM CONNECTION TAG
        /*************************************************/
        /*                                               */
        /*         Module Sub Type Define                */
        /*                                               */
        /*************************************************/
        public const int TYPE_GPRS = 1;
        public const int TYPE_HSDPA = 2;
        //
        public const int TYPE_WIDE_TEMP = 1;
        /*************************************************/
        /*                                               */
        /*         Module Series Define                  */
        /*                                               */
        /*************************************************/
        public const int E4000_SERIES = 1;
        public const int E2000_SERIES = 2;
        public const int E4200_SERIES = 4;
        public const int E1200_SERIES = 8;
        public const int W5000_SERIES = 16;
        public const int E1500_SERIES = 64;
        public const int AOPC_SERVER = 256;
        /*************************************************/
        /*                                               */
        /*         ActiveTag Struct Index Define         */
        /*                                               */
        /*************************************************/
        public const int ACTTAG_INDEX_MODEL = 8;
        public const int ACTTAG_INDEX_IP = 10;
        public const int ACTTAG_INDEX_MAC = 14;
        public const int ACTTAG_INDEX_MSGTYPE = 20;
        public const int ACTTAG_INDEX_MSGSUBTYPE = 21;
        public const int ACTTAG_INDEX_TIME_YEAR = 23;
        public const int ACTTAG_INDEX_TIME_MONTH = 25;
        public const int ACTTAG_INDEX_TIME_DAY = 26;
        public const int ACTTAG_INDEX_TIME_HOUR = 27;
        public const int ACTTAG_INDEX_TIME_MIN = 28;
        public const int ACTTAG_INDEX_TIME_SEC = 29;
        public const int ACTTAG_INDEX_TIME_MSEC = 30;
        public const int ACTTAG_INDEX_LASTSLOT = 32;
        public const int ACTTAG_INDEX_LASTCH_OFSLOT = 33;    //16 slots
        public const int ACTTAG_INDEX_CHTYPE_OFSLOT = 49;    //size: 16*64 = 1024
        public const int ACTTAG_INDEX_ID_OFSLOT = 1073;      //size: 16*2  = 32
        public const int ACTTAG_INDEX_CHABLE_OFSLOT = 1105;  //size: 16*(64/8)  = 128, bitwised
        public const int ACTTAG_INDEX_CHVALUE_OFSLOT = 1233; //size: 16*64  = 1024*4 = 4096
        public const int ACTTAG_INDEX_CHLOCK_OFSLOT = 5329;  //size: 16*(64/8)  = 128, bitwised
        //
        public const int MX_IF_ONE_IF_SIZE = 282;  //
        public const int MX_IF_INDEX_NUM = 0;    //SIZE:4  (DWORD)
        public const int MX_IF_INDEX_IP = 4;    //SIZE:16 (STRING)
        public const int MX_IF_INDEX_MAC = 20;   //SIZE:6
        public const int MX_IF_INDEX_DESC = 26;   //SIZE:256(STRING)
        //---------------------------------------------------------------------------
        public const int MX_ML_MODULE_LIST_SIZE = 47;
        public const int MX_ML_INDEX_WHWID = 0;
        public const int MX_ML_INDEX_SZIP0 = 2;    //SIZE:16 (STRING)
        public const int MX_ML_INDEX_SZMAC0 = 18;   //SIZE:6 (STRING)
        public const int MX_ML_INDEX_SZIP1 = 24;   //SIZE:16 (STRING)
        public const int MX_ML_INDEX_SZMAC1 = 40;   //SIZE:6 (STRING)
        public const int MX_ML_INDEX_BYTLANUSE = 46;
        //---------------------------------------------------------------------------
        /*************************************************/
        /*                                               */
        /*                  Error Code                   */
        /*                                               */
        /*************************************************/
        //[Flags]
        public enum MXIO_ErrorCode : int
        {
            MXIO_OK = 0,
            ILLEGAL_FUNCTION = 1001,
            ILLEGAL_DATA_ADDRESS = 1002,
            ILLEGAL_DATA_VALUE = 1003,
            SLAVE_DEVICE_FAILURE = 1004,
            SLAVE_DEVICE_BUSY = 1006,

            EIO_TIME_OUT = 2001,
            EIO_INIT_SOCKETS_FAIL = 2002,
            EIO_CREATING_SOCKET_ERROR = 2003,
            EIO_RESPONSE_BAD = 2004,
            EIO_SOCKET_DISCONNECT = 2005,
            PROTOCOL_TYPE_ERROR = 2006,
            EIO_PASSWORD_INCORRECT = 2007,

            SIO_OPEN_FAIL = 3001,
            SIO_TIME_OUT = 3002,
            SIO_CLOSE_FAIL = 3003,
            SIO_PURGE_COMM_FAIL = 3004,
            SIO_FLUSH_FILE_BUFFERS_FAIL = 3005,
            SIO_GET_COMM_STATE_FAIL = 3006,
            SIO_SET_COMM_STATE_FAIL = 3007,
            SIO_SETUP_COMM_FAIL = 3008,
            SIO_SET_COMM_TIME_OUT_FAIL = 3009,
            SIO_CLEAR_COMM_FAIL = 3010,
            SIO_RESPONSE_BAD = 3011,
            SIO_TRANSMISSION_MODE_ERROR = 3012,
            SIO_BAUDRATE_NOT_SUPPORT = 3013,

            PRODUCT_NOT_SUPPORT = 4001,
            HANDLE_ERROR = 4002,
            SLOT_OUT_OF_RANGE = 4003,
            CHANNEL_OUT_OF_RANGE = 4004,
            COIL_TYPE_ERROR = 4005,
            REGISTER_TYPE_ERROR = 4006,
            FUNCTION_NOT_SUPPORT = 4007,
            OUTPUT_VALUE_OUT_OF_RANGE = 4008,
            INPUT_VALUE_OUT_OF_RANGE = 4009,
            SLOT_NOT_EXIST = 4010,
            FIRMWARE_NOT_SUPPORT = 4011,
            CREATE_MUTEX_FAIL = 4012,
            ENUM_NET_INTERFACE_FAIL = 5000,
            ADD_INFO_TABLE_FAIL = 5001,
            UNKNOWN_NET_INTERFACE_FAIL = 5002,
            TABLE_NET_INTERFACE_FAIL = 5003
        };
        /*************************************************/
        /*                                               */
        /*              Data Format Setting              */
        /*                                               */
        /*************************************************/
        /* Data length define   */
        [Flags]
        public enum MXIO_DataLength : int
        {
            BIT_5 = 0x00,
            BIT_6 = 0x01,
            BIT_7 = 0x02,
            BIT_8 = 0x03
        };
        /* Stop bits define */
        [Flags]
        public enum MXIO_StopBit : int
        {
            STOP_1 = 0x00,
            STOP_2 = 0x04
        };
        /* Parity define    */
        [Flags]
        public enum MXIO_Parity : int
        {
            P_EVEN = 0x18,
            P_ODD = 0x08,
            P_SPC = 0x38,
            P_MRK = 0x28,
            P_NONE = 0x00
        };
        /*************************************************/
        /*                                               */
        /*        Modbus Transmission Mode Setting       */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ModbusTransModeSetting : int
        {
            MODBUS_RTU = 0x0,
            MODBUS_ASCII = 0x1,
        };
        /*************************************************/
        /*                                               */
        /*            Check Connection Status            */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_CheckConnectinStatus : int
        {
            CHECK_CONNECTION_OK = 0x0,
            CHECK_CONNECTION_FAIL = 0x1,
            CHECK_CONNECTION_TIME_OUT = 0x2
        };
        /*************************************************/
        /*                                               */
        /*            Modbus Coil Type Setting           */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_CoilTypeSetting : int
        {
            COIL_INPUT = 0x2,
            COIL_OUTPUT = 0x1
        };
        /*************************************************/
        /*                                               */
        /*            Modbus Register Type Setting       */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_RegisterTypeSetting : int
        {
            REGISTER_INPUT = 0x4,
            REGISTER_OUTPUT = 0x3
        };
        /*************************************************/
        /*                                               */
        /*             ioLogik 4000 Bus Status           */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_E4KBusStatus : int
        {
            NORMAL_OPERATION = 0x0,
            BUS_STANDYBY = 0x1,
            BUS_COMMUNICATION_FAULT = 0x2,
            SLOT_CONFIGURATION_FAILED = 0x3,
            NO_EXPANSION_SLOT = 0x4
        };
        /*************************************************/
        /*                                               */
        /*        ioLogik 4000 Field Power Status        */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_E4KFieldPowerStatus : int
        {
            FIELD_POWER_ON = 0x0,
            FIELD_POWER_OFF = 0x1,
        };
        /*************************************************/
        /*                                               */
        /*         ioLogik 4000 Watchdog Status          */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_E4KWatchdogStatus : int
        {
            WATCHDOG_NO_ERROR = 0x0,
            WATCHDOG_ERROR = 0x1
        };
        /*************************************************/
        /*                                               */
        /*   ioLogik 4000 Modbus Error Setup Status      */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_E4KModbusErrorSetupStatus : int
        {
            SETUP_NO_ERROR = 0x0,
            SETUP_ERROR = 0x1
        };
        /*************************************************/
        /*                                               */
        /*   ioLogik 4000 Modbus Error Check Status      */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_E4KModbusErrorCheckStatus : int
        {
            CHECKSUM_NO_ERROR = 0x0,
            CHECKSUM_ERROR = 0x1
        };

        /*************************************************/
        /*                                               */
        /*         ioLogik 4000 AO Safe Action           */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_E4KAOSafeAction : int
        {
            SAFE_VALUE = 0x00,     //Suport AO & DO module
            HOLD_LAST_STATE = 0x01,//Suport AO & DO module
            LOW_LIMIT = 0x02,      //Only suport AO module
            HIGH_LIMIT = 0x03      //Only suport AO module
        };
        /*************************************************/
        /*                                               */
        /*               Protocol Type                   */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ProtocolType : int
        {
            PROTOCOL_TCP = 0x01,
            PROTOCOL_UDP = 0x02
        };
        /*************************************************/
        /*                                               */
        /*               AI Channel Type                 */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_AIChannelType : int
        {
            M2K_AI_RANGE_150mv = 0,
            M2K_AI_RANGE_500mv = 1,
            M2K_AI_RANGE_5V = 2,
            M2K_AI_RANGE_10V = 3,
            M2K_AI_RANGE_0_20mA = 4,
            M2K_AI_RANGE_4_20mA = 5,
            M2K_AI_RANGE_0_150mv = 6,
            M2K_AI_RANGE_0_500mv = 7,
            M2K_AI_RANGE_0_5V = 8,
            M2K_AI_RANGE_0_10V = 9
        };
        /*************************************************/
        /*                                               */
        /*               W53xx BUS STATE                 */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_W53xxBusState : int
        {
            BUS_INIT = 0x00,
            BUS_INIT_NET = 0x01,
            BUS_INIT_FAULT = 0x02,
            BUS_IO = 0x03,
            BUS_IO_FAULT = 0x04,
            BUS_IDLE = 0x05
        };
        /*************************************************/
        /*                                               */
        /*      W53xx Expansion Module Error Code        */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_W53xxExpModuleErrorCode : int
        {
            EXPASION_NO_ERROR = 0x00,
            EXPASION_COMMUNICATION_ERROR = 0x01,
            EXPASION_MODBUS_ERROR = 0x02,
            EXPASION_PENDING_ERROR = 0x03,
            EXPASION_COMBINATION_ERROR = 0x04,
            EXPASION_OUT_OF_MEMORY_ERROR = 0x05
        };
        /*************************************************/
        /*                                               */
        /*         W53xx Expansion Module State          */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_W53xxExpModuleState : int
        {
            EXPASION_OFFLINE = 0x00,
            EXPASION_ONLINE = 0x01,
            EXPASION_UNMATCH_TYPE = 0x02,
            EXPASION_RETURN_MODBUS_EXCEPTION = 0X03,
            EXPASION_NETWORK_ERROR = 0X04
        };
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- nModuleType      */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ActiveTagModuleType : int
        {
            ACTTAG_E2210_ID = 0x2210,
            ACTTAG_E2210_V2_ID = 0x2211,
            ACTTAG_E2212_ID = 0x2212,
            ACTTAG_E2214_ID = 0x2214,
            ACTTAG_E2240_ID = 0x2240,
            ACTTAG_E2240_V2_ID = 0x2241,
            ACTTAG_E2242_ID = 0x2242,
            ACTTAG_E2260_ID = 0x2260,
            ACTTAG_E2262_ID = 0x2262,

            ACTTAG_E1210_ID = 0xE210,
            ACTTAG_E1211_ID = 0xE211,
            ACTTAG_E1212_ID = 0xE212,
            ACTTAG_E1214_ID = 0xE214,
            ACTTAG_E1240_ID = 0xE240,
            ACTTAG_E1241_ID = 0xE241,
            ACTTAG_E1242_ID = 0xE242,
            ACTTAG_E1260_ID = 0xE260,
            ACTTAG_E1262_ID = 0xE262,
            ACTTAG_E1261_ID = 0x5A,

            ACTTAG_E2210T_ID = 0x150,
            ACTTAG_E2212T_ID = 0x151,
            ACTTAG_E2214T_ID = 0x152,
            ACTTAG_E2240T_ID = 0x153,
            ACTTAG_E2242T_ID = 0x154,
            ACTTAG_E2260T_ID = 0x155,
            ACTTAG_E2262T_ID = 0x156,

            ACTTAG_E1210T_ID = 0x50,
            ACTTAG_E1211T_ID = 0x51,
            ACTTAG_E1212T_ID = 0x52,
            ACTTAG_E1214T_ID = 0x53,
            ACTTAG_E1240T_ID = 0x54,
            ACTTAG_E1241T_ID = 0x55,
            ACTTAG_E1242T_ID = 0x56,
            ACTTAG_E1260T_ID = 0x57,
            ACTTAG_E1262T_ID = 0x58,
            ACTTAG_E1261T_ID = 0x59,

            ACTTAG_W5340_ID = 0x5340,
            ACTTAG_W5312_ID = 0x5312,
            ACTTAG_E4200_ID = 0x4200,

            ACTTAG_W5340TSLOT_ID = 0x100,
            ACTTAG_W5312TSLOT_ID = 0x101,
            ACTTAG_W5340SLOT_ID = 0x102,
            ACTTAG_W5312SLOT_ID = 0x103,
            ACTTAG_W5341SLOT_ID = 0x104,
            ACTTAG_W5341TSLOT_ID = 0x105,

            ACTTAG_W5340_HSDPA_ID = 0x106,
            ACTTAG_W5312_HSDPA_ID = 0x107,
            ACTTAG_W5341_HSDPA_ID = 0x108,
            ACTTAG_W5340T_HSDPA_ID = 0x109,
            ACTTAG_W5312T_HSDPA_ID = 0x10A,
            ACTTAG_W5341T_HSDPA_ID = 0x10B,
            ACTTAG_W5340_HSDPA_JPN_ID = 0x10C,
            ACTTAG_W5340T_HSDPA_JPN_ID = 0x10E,
            ACTTAG_W5312_HSDPA_JPN_ID = 0x10D,
            ACTTAG_W5312T_HSDPA_JPN_ID = 0x10F,
            ACTTAG_W5340_HSDPA_JPS_ID = 0x110,
            ACTTAG_W5340T_HSDPA_JPS_ID = 0x112,
            ACTTAG_W5312_HSDPA_JPS_ID = 0x111,
            ACTTAG_W5312T_HSDPA_JPS_ID = 0x113,
            ACTTAG_W5340_AP_ID = 0x114,

            ACTTAG_IOPAC_8020_T_ID = 0x180,

            ACTTAG_IOPAC_8020_5_RJ45_C_T_ID = 0x181,
            ACTTAG_IOPAC_8020_5_M12_C_T_ID = 0x182,
            ACTTAG_IOPAC_8020_9_RJ45_C_T_ID = 0x183,
            ACTTAG_IOPAC_8020_9_M12_C_T_ID = 0x184,
            ACTTAG_IOPAC_8020_5_RJ45_IEC_T_ID = 0x185,
            ACTTAG_IOPAC_8020_5_M12_IEC_T_ID = 0x186,
            ACTTAG_IOPAC_8020_9_RJ45_IEC_T_ID = 0x187,
            ACTTAG_IOPAC_8020_9_M12_IEC_T_ID = 0x188,
            ACTTAG_IOPAC_8020_5_RJ45_C_ID = 0x201,
            ACTTAG_IOPAC_8020_5_M12_C_ID = 0x202,
            ACTTAG_IOPAC_8020_9_RJ45_C_ID = 0x203,
            ACTTAG_IOPAC_8020_9_M12_C_ID = 0x204,
            ACTTAG_IOPAC_8020_5_RJ45_IEC_ID = 0x205,
            ACTTAG_IOPAC_8020_5_M12_IEC_ID = 0x206,
            ACTTAG_IOPAC_8020_9_RJ45_IEC_ID = 0x207,
            ACTTAG_IOPAC_8020_9_M12_IEC_ID = 0x208,

            ACTTAG_E1510_ID = 0x220,
            ACTTAG_E1510_T_ID = 0x221,
            ACTTAG_E1512_ID = 0x222,
            ACTTAG_E1512_T_ID = 0x223,
            ACTTAG_E1261H_ID = 0x340,
            ACTTAG_E1261H_T_ID = 0x341,
            ACTTAG_E1263H_ID = 0x342,
            ACTTAG_E1263H_T_ID = 0x343,

            ACTTAG_E1213_ID = 0x5B,
            ACTTAG_E1213T_ID = 0x5C,

            ACTTAG_W5348_GPRS_C_ID = 0x115,
            ACTTAG_W5348_HSDPA_C_ID = 0x116,
            ACTTAG_W5348_GPRS_IEC_ID = 0x117,
            ACTTAG_W5348_HSDPA_IEC_ID = 0x118,
            ACTTAG_W5348_GPRS_C_T_ID = 0x119,
            ACTTAG_W5348_HSDPA_C_T_ID = 0x11A,
            ACTTAG_W5348_GPRS_IEC_T_ID = 0x11B,
            ACTTAG_W5348_HSDPA_IEC_T_ID = 0x11C,

            ACTTAG_E1510_CT_T_ID = 0x224,
            ACTTAG_E1512_CT_T_ID = 0x225,
            ACTTAG_E1213_CT_ID = 0x5D,
            ACTTAG_E1213_CT_T_ID = 0x5E

        };
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- BytMsgType       */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ActiveTagMsgType : int
        {
            ACTTAG_MSG_POWER_ON = 1,
            ACTTAG_MSG_HEARTBEAT = 2,
            ACTTAG_MSG_CONFIG = 3,
            ACTTAG_MSG_IO_STATUS = 4,
            ACTTAG_MSG_SYSTEM = 5
        };
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- BytMsgSubType    */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ActiveTagMsgSubType : int
        {
            //under ACTTAG_MSG_SYSTEM
            ACTTAG_MSG_SUB_HEARTBEAT_TIMEOUT = 1,
            ACTTAG_MSG_SUB_READWRITE_TIMEOUT = 2,
            ACTTAG_MSG_SUB_CLIENT_DISCONNECT = 3,
            ACTTAG_MSG_SUB_SERVER_DISCONNECT = 4
        };
        /*************************************************/
        /*                                               */
        /*         Active Tag Define -- BytChType        */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ActiveTagChType : int
        {
            // 0~9=DIO mode
            ACTTAG_DI_DI = 0,
            ACTTAG_DO_DO = 1,
            ACTTAG_DI_CNT = 2,
            ACTTAG_DO_PULSE = 3,
            ACTTAG_DI_DI_DISABLE = 4,
            ACTTAG_DO_DO_DISABLE = 5,
            ACTTAG_RLY_DO = 6,
            ACTTAG_RLY_PULSE = 7,

            // 10~49=AIO range
            ACTTAG_AI_DISABLE = 10,
            //AI 11
            ACTTAG_AI_150_150MV = 11,
            ACTTAG_AI_500_500MV = 12,
            ACTTAG_AI_5_5V = 13,
            ACTTAG_AI_10_10V = 14,
            ACTTAG_AI_0_20MA = 15,
            ACTTAG_AI_4_20MA = 16,
            ACTTAG_AI_0_150MV = 17,
            ACTTAG_AI_0_500MV = 18,
            ACTTAG_AI_0_5V = 19,
            ACTTAG_AI_0_10V = 20,
            //AO
            ACTTAG_AO_DISABLE = 30,
            ACTTAG_AO_0_10V = 31,
            ACTTAG_AO_4_20MA = 32,
            ACTTAG_AO_0_20MA = 33,
            ACTTAG_AO_10_10V = 34,
            ACTTAG_AO_0_5V = 35,

            // 50~99=TC
            ACTTAG_TC_DISABLE = 50,
            //Celsius (C)
            ACTTAG_TC_Type_K_C = 51,
            ACTTAG_TC_Type_J_C = 52,
            ACTTAG_TC_Type_T_C = 53,
            ACTTAG_TC_Type_B_C = 54,
            ACTTAG_TC_Type_R_C = 55,
            ACTTAG_TC_Type_S_C = 56,
            ACTTAG_TC_Type_E_C = 57,
            ACTTAG_TC_Type_N_C = 58,
            ACTTAG_TC_Type_L_C = 59,
            ACTTAG_TC_Type_U_C = 60,
            ACTTAG_TC_Type_C_C = 61,
            ACTTAG_TC_Type_D_C = 62,

            //Fahrenheit (F)
            ACTTAG_TC_Type_K_F = 71,
            ACTTAG_TC_Type_J_F = 72,
            ACTTAG_TC_Type_T_F = 73,
            ACTTAG_TC_Type_B_F = 74,
            ACTTAG_TC_Type_R_F = 75,
            ACTTAG_TC_Type_S_F = 76,
            ACTTAG_TC_Type_E_F = 77,
            ACTTAG_TC_Type_N_F = 78,
            ACTTAG_TC_Type_L_F = 79,
            ACTTAG_TC_Type_U_F = 80,
            ACTTAG_TC_Type_C_F = 81,
            ACTTAG_TC_Type_D_F = 82,

            ACTTAG_TC_VOLTAGE_78_126MV = 91,
            ACTTAG_TC_VOLTAGE_39_062MV = 92,
            ACTTAG_TC_VOLTAGE_19_532MV = 93,
            ACTTAG_TC_VOLTAGE_78MV = 94,
            ACTTAG_TC_VOLTAGE_32_7MV = 95,
            ACTTAG_TC_VOLTAGE_65_5MV = 96,

            // 100~149=RTD
            ACTTAG_RTD_DISABLE = 100,
            //Celsius (C)
            ACTTAG_RTD_PT50_C = 101,
            ACTTAG_RTD_PT100_C = 102,
            ACTTAG_RTD_PT200_C = 103,
            ACTTAG_RTD_PT500_C = 104,
            ACTTAG_RTD_PT1000_C = 105,
            ACTTAG_RTD_JPT100_C = 106,
            ACTTAG_RTD_JPT200_C = 107,
            ACTTAG_RTD_JPT500_C = 108,
            ACTTAG_RTD_JPT1000_C = 109,
            ACTTAG_RTD_NI100_C = 110,
            ACTTAG_RTD_NI200_C = 111,
            ACTTAG_RTD_NI500_C = 112,
            ACTTAG_RTD_NI1000_C = 113,
            ACTTAG_RTD_NI120_C = 114,
            ACTTAG_RTD_CU10_C = 115,

            //Fahrenheit (F)
            ACTTAG_RTD_PT50_F = 121,
            ACTTAG_RTD_PT100_F = 122,
            ACTTAG_RTD_PT200_F = 123,
            ACTTAG_RTD_PT500_F = 124,
            ACTTAG_RTD_PT1000_F = 125,
            ACTTAG_RTD_JPT100_F = 126,
            ACTTAG_RTD_JPT200_F = 127,
            ACTTAG_RTD_JPT500_F = 128,
            ACTTAG_RTD_JPT1000_F = 129,
            ACTTAG_RTD_NI100_F = 130,
            ACTTAG_RTD_NI200_F = 131,
            ACTTAG_RTD_NI500_F = 132,
            ACTTAG_RTD_NI1000_F = 133,
            ACTTAG_RTD_NI120_F = 134,
            ACTTAG_RTD_CU10_F = 135,

            ACTTAG_RTD_RESISTANCE_1_310_ = 141,
            ACTTAG_RTD_RESISTANCE_1_620_ = 142,
            ACTTAG_RTD_RESISTANCE_1_1250_ = 143,
            ACTTAG_RTD_RESISTANCE_1_2200_ = 144,
            ACTTAG_RTD_RESISTANCE_1_2000_ = 145,
            ACTTAG_RTD_RESISTANCE_1_327_ = 146,

            //Virtual channel
            ACTTAG_VIRTUAL_CH_AVG_C = 201,
            ACTTAG_VIRTUAL_CH_DIF_C = 202,
            ACTTAG_VIRTUAL_CH_AVG_F = 203,
            ACTTAG_VIRTUAL_CH_DIF_F = 204,
            ACTTAG_VIRTUAL_CH_MCONNECT = 205,  //slot expansion module connection
            ACTTAG_VIRTUAL_CH_DISABLE = 206,  //W5340+E1000 Virtual channel disable
            ACTTAG_VIRTUAL_CH_VALUE = 207,  //W5340+E1000 Virtual channel

            ACTTAG_SYSTEM_CONNECTION = 255   //SYSTEM CONNECTION TAG
        };
        /*************************************************/
        /*                                               */
        /*         Module Series Define                  */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_AutoSearchType : int
        {
            E4000_SERIES = 1,
            E2000_SERIES = 2,
            E4200_SERIES = 4,
            E1200_SERIES = 8,
            W5000_SERIES = 16,
            E1500_SERIES = 64,
            AOPC_SERVER = 256
        };
        /*************************************************/
        /*                                               */
        /*         ActiveTag Struct Index Define         */
        /*                                               */
        /*************************************************/
        [Flags]
        public enum MXIO_ActiveTagStructIndex : int
        {
            ACTTAG_INDEX_MODEL = 8,
            ACTTAG_INDEX_IP = 10,
            ACTTAG_INDEX_MAC = 14,
            ACTTAG_INDEX_MSGTYPE = 20,
            ACTTAG_INDEX_MSGSUBTYPE = 21,
            ACTTAG_INDEX_TIME_YEAR = 23,
            ACTTAG_INDEX_TIME_MONTH = 25,
            ACTTAG_INDEX_TIME_DAY = 26,
            ACTTAG_INDEX_TIME_HOUR = 27,
            ACTTAG_INDEX_TIME_MIN = 28,
            ACTTAG_INDEX_TIME_SEC = 29,
            ACTTAG_INDEX_TIME_MSEC = 30,
            ACTTAG_INDEX_LASTSLOT = 32,
            ACTTAG_INDEX_LASTCH_OFSLOT = 33,    //16 slots
            ACTTAG_INDEX_CHTYPE_OFSLOT = 49,    //size: 16*64 = 1024
            ACTTAG_INDEX_ID_OFSLOT = 1073,      //size: 16*2  = 32
            ACTTAG_INDEX_CHABLE_OFSLOT = 1105,  //size: 16*(64/8)  = 128, bitwised
            ACTTAG_INDEX_CHVALUE_OFSLOT = 1233, //size: 16*64  = 1024*4 = 4096
            ACTTAG_INDEX_CHLOCK_OFSLOT = 5329,  //size: 16*(64/8)  = 128, bitwised
        };
        [Flags]
        public enum MXIO_IFDataIndex : int
        {
            //
            MX_IF_ONE_IF_SIZE = 282,  //
            MX_IF_INDEX_NUM = 0,    //SIZE:4  (DWORD)
            MX_IF_INDEX_IP = 4,    //SIZE:16 (STRING)
            MX_IF_INDEX_MAC = 20,   //SIZE:6
            MX_IF_INDEX_DESC = 26    //SIZE:256(STRING)
        };
        [Flags]
        public enum MXIO_ModuleDataIndex : int
        {
            //---------------------------------------------------------------------------
            MX_ML_MODULE_LIST_SIZE = 47,
            MX_ML_INDEX_WHWID = 0,
            MX_ML_INDEX_SZIP0 = 2,    //SIZE:16 (STRING)
            MX_ML_INDEX_SZMAC0 = 18,   //SIZE:6 (STRING)
            MX_ML_INDEX_SZIP1 = 24,   //SIZE:16 (STRING)
            MX_ML_INDEX_SZMAC1 = 40,   //SIZE:6 (STRING)
            MX_ML_INDEX_BYTLANUSE = 46
        };
        //---------------------------------------------------------------------------
    }
}