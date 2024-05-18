using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace CaYaTSKMGR
{
    public class RegistryWatcher : IDisposable
    {
        private readonly string _registryKeyPath;
        private readonly RegistryKey _registryKey;
        private readonly IntPtr _hRegistryKey;
        private readonly IntPtr _hEvent;
        private IntPtr _hThread;
        private bool _disposed = false;
        private Dictionary<string, object> _previousValues;

        public event EventHandler<RegistryChangedEventArgs> RegistryChanged;

        public RegistryWatcher(string registryKeyPath)
        {
            _registryKeyPath = registryKeyPath;
            _registryKey = Registry.CurrentUser.OpenSubKey(registryKeyPath, true);
            //_registryKey = Registry.LocalMachine.OpenSubKey(registryKeyPath, true);
            //_registryKey = Registry.ClassesRoot.OpenSubKey(registryKeyPath, true);
            //_registryKey = Registry.CurrentConfig.OpenSubKey(registryKeyPath, true);
            //_registryKey = Registry.Users.OpenSubKey(registryKeyPath, true);
            _hRegistryKey = _registryKey.Handle.DangerousGetHandle(); // SafeRegistryHandle'dan IntPtr'a dönüştürme
            _hEvent = CreateEvent(IntPtr.Zero, false, false, null);
            _previousValues = new Dictionary<string, object>();
            LoadPreviousValues();
            StartWatching();
        }

        private void LoadPreviousValues()
        {
            foreach (string valueName in _registryKey.GetValueNames())
            {
                _previousValues[valueName] = _registryKey.GetValue(valueName);
            }
        }

        public void StartWatching()
        {
            if (_hThread == IntPtr.Zero)
            {
                _hThread = CreateThread(IntPtr.Zero, 0, WatchRegistry, IntPtr.Zero, 0, IntPtr.Zero);
            }
        }

        public void StopWatching()
        {
            if (_hThread != IntPtr.Zero)
            {
                TerminateThread(_hThread, 0);
                _hThread = IntPtr.Zero;
            }
        }

        private void WatchRegistry()
        {
            while (!_disposed)
            {
                RegNotifyChangeKeyValue(_hRegistryKey, true, REG_NOTIFY_CHANGE_NAME | REG_NOTIFY_CHANGE_LAST_SET, _hEvent, true);
                WaitForSingleObject(_hEvent, INFINITE);
                CheckForChanges();
            }
        }

        private void CheckForChanges()
        {
            var currentValues = new Dictionary<string, object>();
            foreach (string valueName in _registryKey.GetValueNames())
            {
                currentValues[valueName] = _registryKey.GetValue(valueName);
            }

            foreach (var value in currentValues)
            {
                if (!_previousValues.ContainsKey(value.Key) || !_previousValues[value.Key].Equals(value.Value))
                {
                    OnRegistryChanged(new RegistryChangedEventArgs(_registryKeyPath, value.Key, value.Value));
                }
            }

            foreach (var value in _previousValues)
            {
                if (!currentValues.ContainsKey(value.Key))
                {
                    OnRegistryChanged(new RegistryChangedEventArgs(_registryKeyPath, value.Key, null));
                }
            }

            _previousValues = currentValues;
        }

        protected virtual void OnRegistryChanged(RegistryChangedEventArgs e)
        {
            RegistryChanged?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                StopWatching();
                _registryKey?.Close();
                _disposed = true;
            }
        }

        // PInvoke declarations
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree, uint dwNotifyFilter, IntPtr hEvent, bool fAsynchronous);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, ThreadStart lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        private const uint REG_NOTIFY_CHANGE_NAME = 0x1;
        private const uint REG_NOTIFY_CHANGE_LAST_SET = 0x4;
        private const uint INFINITE = 0xFFFFFFFF;
    }

    public class RegistryChangedEventArgs : EventArgs
    {
        public string RegistryKeyPath { get; }
        public string ValueName { get; }
        public object Value { get; }

        public RegistryChangedEventArgs(string registryKeyPath, string valueName, object value)
        {
            RegistryKeyPath = registryKeyPath;
            ValueName = valueName;
            Value = value;
        }
    }
}
