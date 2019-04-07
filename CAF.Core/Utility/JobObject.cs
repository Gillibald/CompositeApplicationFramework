#region Usings

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CompositeApplicationFramework.Base;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class JobObject : DisposableBase
    {
        // ReSharper disable once InconsistentNaming
        private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;
        private IntPtr _handle;

        public JobObject()
        {
            _handle = CreateJobObject(IntPtr.Zero, null);

            var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION {LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE};

            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION {BasicLimitInformation = info};

            var length = Marshal.SizeOf(typeof (JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            var extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (
                !SetInformationJobObject(
                    _handle,
                    JobObjectInfoType.ExtendedLimitInformation,
                    extendedInfoPtr,
                    (uint) length))
            {
                throw new Exception($"Unable to set information.  Error: {Marshal.GetLastWin32Error()}");
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateJobObject(IntPtr a, string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetInformationJobObject(
            IntPtr hJob,
            JobObjectInfoType infoType,
            IntPtr lpJobObjectInfo,
            uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        protected override void DisposeHandler()
        {
            Close();
        }

        public void Close()
        {
            CloseHandle(_handle);
            _handle = IntPtr.Zero;
        }

        public bool AddProcess(IntPtr processHandle)
        {
            return AssignProcessToJobObject(_handle, processHandle);
        }

        public bool AddProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return AddProcess(process.Handle);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    #region Helper classes

    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    internal struct IO_COUNTERS
    {
        public ulong ReadOperationCount;

        public ulong WriteOperationCount;

        public ulong OtherOperationCount;

        public ulong ReadTransferCount;

        public ulong WriteTransferCount;

        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;

        public long PerJobUserTimeLimit;

        public uint LimitFlags;

        public UIntPtr MinimumWorkingSetSize;

        public UIntPtr MaximumWorkingSetSize;

        public uint ActiveProcessLimit;

        public UIntPtr Affinity;

        public uint PriorityClass;

        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct SECURITY_ATTRIBUTES
    {
        public uint nLength;

        public IntPtr lpSecurityDescriptor;

        public int bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;

        public IO_COUNTERS IoInfo;

        public UIntPtr ProcessMemoryLimit;

        public UIntPtr JobMemoryLimit;

        public UIntPtr PeakProcessMemoryUsed;

        public UIntPtr PeakJobMemoryUsed;
    }

    public enum JobObjectInfoType
    {
        AssociateCompletionPortInformation = 7,

        BasicLimitInformation = 2,

        // ReSharper disable once InconsistentNaming
        BasicUIRestrictions = 4,

        EndOfJobTimeInformation = 6,

        ExtendedLimitInformation = 9,

        SecurityLimitInformation = 5,

        GroupInformation = 11
    }

    #endregion
}