using System;

namespace System.Runtime.ConstrainedExecution
{
    [Serializable]
    public enum Consistency
    {
        MayCorruptProcess,
        MayCorruptAppDomain,
        MayCorruptInstance,
        WillNotCorruptState
    }
}