#region Usings

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32.SafeHandles;

#endregion

namespace CompositeApplicationFramework.Utility
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     Detects insertion or removal of removable drives.
    ///     Use it in 1 or 2 steps:
    ///     1) Create instance of this class in your project and add handlers for the
    ///     DeviceArrived, DeviceRemoved and QueryRemove events.
    /// </summary>
    public class DriveDetector : IDisposable
    {
        /// <summary>
        ///     The easiest way to use DriveDetector.
        ///     It will create hidden form for processing Windows messages about USB drives
        ///     You do not need to override WndProc in your form.
        /// </summary>
        public DriveDetector(string fileToOpen = null)
        {
            Init(Application.Current.MainWindow, fileToOpen);
        }

        public DriveDetector(Window window, string fileToOpen = null)
        {
            Init(window, fileToOpen);
        }

        /// <summary>
        ///     Gets the value indicating whether the query remove event will be fired.
        /// </summary>
        public bool IsQueryHooked => _deviceNotifyHandle != IntPtr.Zero;

        /// <summary>
        ///     Gets letter of drive which is currently hooked. Empty string if none.
        ///     See also IsQueryHooked.
        /// </summary>
        public string HookedDrive { get; private set; }

        /// <summary>
        ///     Gets the file stream for file which this class opened on a drive to be notified
        ///     about it's removal.
        ///     This will be null unless you specified a file to open (DriveDetector opens root directory of the flash drive)
        /// </summary>
        public FileStream OpenedFile { get; private set; }

        /// <summary>
        ///     Unregister and close the file we may have opened on the removable drive.
        ///     Garbage collector will call this method.
        /// </summary>
        public void Dispose()
        {
            RegisterForDeviceChange(false, null);
        }

        /// <summary>
        ///     Events signalized to the client app.
        ///     Add handlers for these events in your form to be notified of removable device events
        /// </summary>
        public event DriveDetectorEventHandler DeviceArrived;

        public event DriveDetectorEventHandler DeviceRemoved;
        public event DriveDetectorEventHandler QueryRemove;

        private void Init(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var hwndSource = HwndSource.FromHwnd(hwnd);
            hwndSource?.AddHook(WndProc);
            _recipientHandle = hwnd;
        }

        private void Init(Window window, string fileToOpen)
        {
            Init(window);
            _fileToOpen = fileToOpen;
            OpenedFile = null;
            _deviceNotifyHandle = IntPtr.Zero;
            _directoryHandle = IntPtr.Zero; // handle to the root directory of the flash drive which we open 
            HookedDrive = "";
        }

        /// <summary>
        ///     Hooks specified drive to receive a message when it is being removed.
        ///     This can be achieved also by setting e.HookQueryRemove to true in your
        ///     DeviceArrived event handler.
        ///     By default DriveDetector will open the root directory of the flash drive to obtain notification handle
        ///     from Windows (to learn when the drive is about to be removed).
        /// </summary>
        /// <param name="fileOnDrive">
        ///     Drive letter or relative path to a file on the drive which should be
        ///     used to get a handle - required for registering to receive query remove messages.
        ///     If only drive letter is specified (e.g. "D:\\", root directory of the drive will be opened.
        /// </param>
        /// <returns>true if hooked ok, false otherwise</returns>
        public bool EnableQueryRemove(string fileOnDrive)
        {
            if (string.IsNullOrEmpty(fileOnDrive))
            {
                throw new ArgumentException("Drive path must be supplied to register for Query remove.");
            }

            if (fileOnDrive.Length == 2 && fileOnDrive[1] == ':')
            {
                fileOnDrive += '\\'; // append "\\" if only drive letter with ":" was passed in.
            }

            if (_deviceNotifyHandle != IntPtr.Zero)
            {
                // Unregister first...
                RegisterForDeviceChange(false, null);
            }

            if (Path.GetFileName(fileOnDrive).Length == 0 || !File.Exists(fileOnDrive))
            {
                _fileToOpen = null; // use root directory...
            }
            else
            {
                _fileToOpen = fileOnDrive;
            }

            RegisterQuery(Path.GetPathRoot(fileOnDrive));

            return _deviceNotifyHandle != IntPtr.Zero;
        }

        /// <summary>
        ///     Unhooks any currently hooked drive so that the query remove
        ///     message is not generated for it.
        /// </summary>
        public void DisableQueryRemove()
        {
            if (_deviceNotifyHandle != IntPtr.Zero)
            {
                RegisterForDeviceChange(false, null);
            }
        }

        #region WindowProc

        /// <summary>
        ///     Message handler which must be called from client form.
        ///     Processes Windows messages and calls event handlers.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_DEVICECHANGE)
            {
                return IntPtr.Zero;
            }
            // WM_DEVICECHANGE can have several meanings depending on the WParam value...
            int devType;
            char c;
            switch (wParam.ToInt32())
            {
                //
                // New device has just arrived
                //
                case DBT_DEVICEARRIVAL:
                {
                    devType = Marshal.ReadInt32(lParam, 4);
                    if (devType == DBT_DEVTYP_VOLUME)
                    {
                        var vol = (DEV_BROADCAST_VOLUME) Marshal.PtrToStructure(lParam, typeof (DEV_BROADCAST_VOLUME));

                        // Get the drive letter 
                        c = DriveMaskToLetter(vol.dbcv_unitmask);

                        //
                        // Call the client event handler
                        //
                        // We should create copy of the event before testing it and
                        // calling the delegate - if any
                        var tempDeviceArrived = DeviceArrived;
                        if (tempDeviceArrived != null)
                        {
                            var e = new DriveDetectorEventArgs {Drive = c + ":\\"};
                            tempDeviceArrived(this, e);

                            // Register for query remove if requested
                            if (e.HookQueryRemove)
                            {
                                // If something is already hooked, unhook it now
                                if (_deviceNotifyHandle != IntPtr.Zero)
                                {
                                    RegisterForDeviceChange(false, null);
                                }

                                RegisterQuery(c + ":\\");
                            }
                        } // if  has event handler
                    }
                    break;
                }

                //
                // Device is about to be removed
                // Any application can cancel the removal
                //
                case DBT_DEVICEQUERYREMOVE:
                {
                    devType = Marshal.ReadInt32(lParam, 4);
                    if (devType == DBT_DEVTYP_HANDLE)
                    {
                        //
                        // Call the event handler in client
                        //
                        var tempQuery = QueryRemove;
                        if (tempQuery != null)
                        {
                            var e = new DriveDetectorEventArgs {Drive = HookedDrive};
                            // drive which is hooked
                            tempQuery(this, e);

                            // If the client wants to cancel, let Windows know
                            if (e.Cancel)
                            {
                                return (IntPtr) BROADCAST_QUERY_DENY;
                            }
                            // Unregister the notification, this will
                            // close the handle to file or root directory also. 
                            // We have to close it anyway to allow the removal so
                            // even if some other app cancels the removal we would not know about it...                                    
                            RegisterForDeviceChange(false, null); // will also close the mFileOnFlash
                        }
                    }
                    break;
                }

                //
                // Device has been removed
                //
                case DBT_DEVICEREMOVECOMPLETE:
                {
                    devType = Marshal.ReadInt32(lParam, 4);
                    if (devType == DBT_DEVTYP_VOLUME)
                    {
                        devType = Marshal.ReadInt32(lParam, 4);
                        if (devType == DBT_DEVTYP_VOLUME)
                        {
                            var vol =
                                (DEV_BROADCAST_VOLUME) Marshal.PtrToStructure(lParam, typeof (DEV_BROADCAST_VOLUME));
                            c = DriveMaskToLetter(vol.dbcv_unitmask);

                            //
                            // Call the client event handler
                            //
                            var tempDeviceRemoved = DeviceRemoved;
                            if (tempDeviceRemoved != null)
                            {
                                var e = new DriveDetectorEventArgs {Drive = c + ":\\"};
                                tempDeviceRemoved(this, e);
                            }

                            // TODO: we could unregister the notify handle here if we knew it is the right drive which has been just removed
                            //RegisterForDeviceChange(false, null);
                        }
                    }
                    break;
                }
                default:
                {
                    return IntPtr.Zero;
                }
            }
            return IntPtr.Zero;
        }

        #endregion

        #region  Private Area

        /// <summary>
        ///     New: 28.10.2007 - handle to root directory of flash drive which is opened
        ///     for device notification
        /// </summary>
        private IntPtr _directoryHandle = IntPtr.Zero;

        /// <summary>
        ///     Name of the file to try to open on the removable drive for query remove registration
        /// </summary>
        private string _fileToOpen;

        /// <summary>
        ///     Handle to file which we keep opened on the drive if query remove message is required by the client
        /// </summary>
        private IntPtr _deviceNotifyHandle;

        /// <summary>
        ///     Handle of the window which receives messages from Windows. This will be a form.
        /// </summary>
        private IntPtr _recipientHandle;

        // Win32 constants
        // ReSharper disable once InconsistentNaming
        //private const int DBT_DEVTYP_DEVICEINTERFACE = 5;

        // ReSharper disable once InconsistentNaming
        private const int DBT_DEVTYP_HANDLE = 6;

        // ReSharper disable once InconsistentNaming
        private const int BROADCAST_QUERY_DENY = 0x424D5144;

        // ReSharper disable once InconsistentNaming
        private const int WM_DEVICECHANGE = 0x0219;

        // ReSharper disable once InconsistentNaming
        private const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device

        // ReSharper disable once InconsistentNaming
        private const int DBT_DEVICEQUERYREMOVE = 0x8001; // Preparing to remove (any program can disable the removal)

        // ReSharper disable once InconsistentNaming
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 

        // ReSharper disable once InconsistentNaming
        private const int DBT_DEVTYP_VOLUME = 0x00000002; // drive type is logical volume

        /// <summary>
        ///     Registers for receiving the query remove message for a given drive.
        ///     We need to open a handle on that drive and register with this handle.
        ///     Client can specify this file in mFileToOpen or we will open root directory of the drive
        /// </summary>
        /// <param name="drive">drive for which to register. </param>
        private void RegisterQuery(string drive)
        {
            var register = true;

            if (_fileToOpen == null)
            {
                // Change 28.10.2007 - Open the root directory if no file specified - leave mFileToOpen null 
                // If client gave us no file, let's pick one on the drive... 
                //mFileToOpen = GetAnyFile(drive);
                //if (mFileToOpen.Length == 0)
                //    return;     // no file found on the flash drive                
            }
            else
            {
                // Make sure the path in mFileToOpen contains valid drive
                // If there is a drive letter in the path, it may be different from the  actual
                // letter assigned to the drive now. We will cut it off and merge the actual drive 
                // with the rest of the path.
                if (_fileToOpen.Contains(":"))
                {
                    var tmp = _fileToOpen.Substring(3);
                    var root = Path.GetPathRoot(drive);
                    if (root != null) _fileToOpen = Path.Combine(root, tmp);
                }
                else
                {
                    _fileToOpen = Path.Combine(drive, _fileToOpen);
                }
            }

            try
            {
                //mFileOnFlash = new FileStream(mFileToOpen, FileMode.Open);
                // Change 28.10.2007 - Open the root directory 
                OpenedFile = _fileToOpen == null ? null : new FileStream(_fileToOpen, FileMode.Open);
            }
            catch (Exception)
            {
                // just do not register if the file could not be opened
                register = false;
            }

            if (!register) return;
            //RegisterForDeviceChange(true, mFileOnFlash.SafeFileHandle);
            //mCurrentDrive = drive;
            // Change 28.10.2007 - Open the root directory 
            if (OpenedFile == null)
            {
                RegisterForDeviceChange(drive);
            }
            else
            {
                // old version
                RegisterForDeviceChange(true, OpenedFile.SafeFileHandle);
            }

            HookedDrive = drive;
        }

        /// <summary>
        ///     New version which gets the handle automatically for specified directory
        ///     Only for registering! Unregister with the old version of this function...
        /// </summary>
        /// <param name="dirPath">e.g. C:\\dir</param>
        private void RegisterForDeviceChange(string dirPath)
        {
            var handle = Native.OpenDirectory(dirPath);

            if (handle == IntPtr.Zero)
            {
                _deviceNotifyHandle = IntPtr.Zero;
                return;
            }

            _directoryHandle = handle; // save handle for closing it when unregistering

            // Register for handle
            var data = new DEV_BROADCAST_HANDLE
            {
                dbch_devicetype = DBT_DEVTYP_HANDLE,
                dbch_reserved = 0,
                dbch_nameoffset = 0,
                dbch_handle = handle,
                dbch_hdevnotify = (IntPtr) 0
            };

            //data.dbch_data = null;
            //data.dbch_eventguid = 0;

            var size = Marshal.SizeOf(data);

            data.dbch_size = size;

            var buffer = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(data, buffer, true);

            _deviceNotifyHandle = Native.RegisterDeviceNotification(_recipientHandle, buffer, 0);
        }

        /// <summary>
        ///     Registers to be notified when the volume is about to be removed
        ///     This is requierd if you want to get the QUERY REMOVE messages
        /// </summary>
        /// <param name="register">true to register, false to unregister</param>
        /// <param name="fileHandle">handle of a file opened on the removable drive</param>
        private void RegisterForDeviceChange(bool register, SafeFileHandle fileHandle)
        {
            if (register)
            {
                // Register for handle
                var data = new DEV_BROADCAST_HANDLE
                {
                    dbch_devicetype = DBT_DEVTYP_HANDLE,
                    dbch_reserved = 0,
                    dbch_nameoffset = 0,
                    dbch_handle = fileHandle.DangerousGetHandle(),
                    dbch_hdevnotify = (IntPtr) 0
                };
                //data.dbch_data = null;
                //data.dbch_eventguid = 0;
                //Marshal. fileHandle; 
                var size = Marshal.SizeOf(data);

                data.dbch_size = size;

                var buffer = Marshal.AllocHGlobal(size);

                Marshal.StructureToPtr(data, buffer, true);

                _deviceNotifyHandle = Native.RegisterDeviceNotification(_recipientHandle, buffer, 0);
            }
            else
            {
                // close the directory handle
                if (_directoryHandle != IntPtr.Zero)
                {
                    Native.CloseDirectoryHandle(_directoryHandle);
                    //    string er = Marshal.GetLastWin32Error().ToString();
                }

                // unregister
                if (_deviceNotifyHandle != IntPtr.Zero)
                {
                    Native.UnregisterDeviceNotification(_deviceNotifyHandle);
                }

                _deviceNotifyHandle = IntPtr.Zero;

                _directoryHandle = IntPtr.Zero;

                HookedDrive = "";

                if (OpenedFile == null) return;

                OpenedFile.Close();

                OpenedFile = null;
            }
        }

        /// <summary>
        ///     Gets drive letter from a bit mask where bit 0 = A, bit 1 = B etc.
        ///     There can actually be more than one drive in the mask but we
        ///     just use the last one in this case.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static char DriveMaskToLetter(int mask)
        {
            const string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // 1 = A
            // 2 = B
            // 4 = C...
            var cnt = 0;

            var pom = mask/2;

            while (pom != 0)
            {
                // while there is any bit set in the mask
                // shift it to the righ...                
                pom = pom/2;

                cnt++;
            }

            var letter = cnt < drives.Length ? drives[cnt] : '?';

            return letter;
        }

        #endregion
    }

    // Delegate for event handler to handle the device events 
    public delegate void DriveDetectorEventHandler(object sender, DriveDetectorEventArgs e);

    /// <summary>
    ///     Our class for passing in custom arguments to our event handlers
    /// </summary>
    public class DriveDetectorEventArgs : EventArgs
    {
        /// <summary>
        ///     Get/Set the value indicating that the event should be cancelled
        ///     Only in QueryRemove handler.
        /// </summary>
        public bool Cancel;

        /// <summary>
        ///     Drive letter for the device which caused this event
        /// </summary>
        public string Drive;

        /// <summary>
        ///     Set to true in your DeviceArrived event handler if you wish to receive the
        ///     QueryRemove event for this drive.
        /// </summary>
        public bool HookQueryRemove;

        public DriveDetectorEventArgs()
        {
            Cancel = false;
            Drive = "";
            HookQueryRemove = false;
        }
    }

    internal static class Native
    {
        //
        // CreateFile  - MSDN
        // ReSharper disable once InconsistentNaming
        private const uint GENERIC_READ = 0x80000000;
        // ReSharper disable once InconsistentNaming
        private const uint OPEN_EXISTING = 3;
        // ReSharper disable once InconsistentNaming
        private const uint FILE_SHARE_READ = 0x00000001;
        // ReSharper disable once InconsistentNaming
        private const uint FILE_SHARE_WRITE = 0x00000002;
        // ReSharper disable once InconsistentNaming
        private const uint FILE_ATTRIBUTE_NORMAL = 128;
        // ReSharper disable once InconsistentNaming
        private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        // ReSharper disable once InconsistentNaming
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        //   HDEVNOTIFY RegisterDeviceNotification(HANDLE hRecipient,LPVOID NotificationFilter,DWORD Flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

        // should be "static extern unsafe"
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string fileName,
            // file name
            uint desiredAccess,
            // access mode
            uint shareMode,
            // share mode
            uint securityAttributes,
            // Security Attributes
            uint creationDisposition,
            // how to create
            uint flagsAndAttributes,
            // file attributes
            int hTemplateFile // handle to template file
            );

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(
            IntPtr hObject // handle to object
            );

        /// <summary>
        ///     Opens a directory, returns it's handle or zero.
        /// </summary>
        /// <param name="dirPath">path to the directory, e.g. "C:\\dir"</param>
        /// <returns>handle to the directory. Close it with CloseHandle().</returns>
        public static IntPtr OpenDirectory(string dirPath)
        {
            // open the existing file for reading          
            var handle = CreateFile(
                dirPath,
                GENERIC_READ,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                0,
                OPEN_EXISTING,
                FILE_FLAG_BACKUP_SEMANTICS | FILE_ATTRIBUTE_NORMAL,
                0);

            return handle == INVALID_HANDLE_VALUE ? IntPtr.Zero : handle;
        }

        public static bool CloseDirectoryHandle(IntPtr handle)
        {
            return CloseHandle(handle);
        }
    }

    // Structure with information for RegisterDeviceNotification.
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct DEV_BROADCAST_HANDLE
    {
        public int dbch_size;

        public int dbch_devicetype;

        public int dbch_reserved;

        public IntPtr dbch_handle;

        public IntPtr dbch_hdevnotify;

        public Guid dbch_eventguid;

        public long dbch_nameoffset;

        //public byte[] dbch_data[1]; // = new byte[1];
        public byte dbch_data;

        public byte dbch_data1;
    }

    // Struct for parameters of the WM_DEVICECHANGE message
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct DEV_BROADCAST_VOLUME
    {
        public int dbcv_size;

        public int dbcv_devicetype;

        public int dbcv_reserved;

        public int dbcv_unitmask;
    }
}