using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    #region Constants
    public const int WTS_CURRENT_SESSION = -1;
    #endregion

    #region Dll Imports
    [DllImport("Kernel32.dll")]
    static extern bool ProcessIdToSessionId(
        [MarshalAs(UnmanagedType.U4)] int iProcessId,
        [MarshalAs(UnmanagedType.U4)] ref int iCount);

    [DllImport("wtsapi32.dll")]
    static extern int WTSEnumerateSessions(
        IntPtr pServer,
        [MarshalAs(UnmanagedType.U4)] int iReserved,
        [MarshalAs(UnmanagedType.U4)] int iVersion,
        ref IntPtr pSessionInfo,
        [MarshalAs(UnmanagedType.U4)] ref int iCount);

    [DllImport("Wtsapi32.dll")]
    public static extern bool WTSQuerySessionInformation(
        System.IntPtr pServer,
        int iSessionID,
        WTS_INFO_CLASS oInfoClass,
        out System.IntPtr pBuffer,
        out uint iBytesReturned);

    #endregion

    #region Structures
    //Structure for Terminal Service Client IP Address
    [StructLayout(LayoutKind.Sequential)]
    private struct WTS_CLIENT_ADDRESS
    {
        public int iAddressFamily;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] bAddress;
    }

    //Structure for Terminal Service Session Info
    [StructLayout(LayoutKind.Sequential)]
    private struct WTS_SESSION_INFO
    {
        public int iSessionID;
        [MarshalAs(UnmanagedType.LPStr)]
        public string sWinsWorkstationName;
        public WTS_CONNECTSTATE_CLASS oState;
    }

    
    #endregion

    #region Enumurations
    public enum WTS_CONNECTSTATE_CLASS
    {
        WTSActive,
        WTSConnected,
        WTSConnectQuery,
        WTSShadow,
        WTSDisconnected,
        WTSIdle,
        WTSListen,
        WTSReset,
        WTSDown,
        WTSInit
    }

    public enum WTS_INFO_CLASS
    {
        WTSInitialProgram,
        WTSApplicationName,
        WTSWorkingDirectory,
        WTSOEMId,
        WTSSessionId,
        WTSUserName,
        WTSWinStationName,
        WTSDomainName,
        WTSConnectState,
        WTSClientBuildNumber,
        WTSClientName,
        WTSClientDirectory,
        WTSClientProductId,
        WTSClientHardwareId,
        WTSClientAddress,
        WTSClientDisplay,
        WTSClientProtocolType,
        WTSIdleTime,
        WTSLogonTime,
        WTSIncomingBytes,
        WTSOutgoingBytes,
        WTSIncomingFrames,
        WTSOutgoingFrames,
        WTSClientInfo,
        WTSSessionInfo,
        WTSConfigInfo,
        WTSValidationInfo,
        WTSSessionAddressV4,
        WTSIsRemoteSession
    }
    #endregion

    enum ExitCode : int
    {
        Success = 0,
        NoSession = 1,
        NoIP = 2
    }


    static void Main(string[] args)
    {

        
        IntPtr pServer = IntPtr.Zero;
        string sIPAddress = string.Empty;
        int processID = Process.GetCurrentProcess().Id;
        
        int sessionID = -9999;

        ProcessIdToSessionId(processID, ref sessionID);

        WTS_CLIENT_ADDRESS oClientAddres = new WTS_CLIENT_ADDRESS();

        IntPtr pSessionInfo = IntPtr.Zero;


        int iDataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
        int iCurrent = (int)pSessionInfo;

        uint iReturned = 0;
                //Get the IP address of the Terminal Services User
                IntPtr pAddress = IntPtr.Zero;
                if (WTSQuerySessionInformation(pServer,
        sessionID, WTS_INFO_CLASS.WTSClientAddress,
        out pAddress, out iReturned) == true)
                {
                    oClientAddres = (WTS_CLIENT_ADDRESS)Marshal.PtrToStructure
                (pAddress, oClientAddres.GetType());
                    sIPAddress = oClientAddres.bAddress[2] + "." +
            oClientAddres.bAddress[3] + "." + oClientAddres.bAddress[4]
            + "." + oClientAddres.bAddress[5];
                }

                if (sessionID != -9999)
                {
                    if (sIPAddress != null)
                    {
                        Console.WriteLine(sIPAddress);
                        Environment.Exit((int)ExitCode.Success);
                    }
                    else
                    {
                        Console.WriteLine("Warning: No IP Retrieved");
                        Environment.Exit((int)ExitCode.NoIP);
                    }

                }
                else
                {
                    Console.WriteLine("Error: Couldn't Get Session ID");
                    Environment.Exit((int)ExitCode.NoSession);
                }
    }

}