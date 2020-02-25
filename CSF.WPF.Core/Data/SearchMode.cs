using System;

namespace CSF.WPF.Core.Data
{
    [Flags]
    public enum SearchMode
    {
        None = 0,
        Label = 1 << 0,
        Value = 1 << 1,
        Extra = 1 << 2,
        Regex = 1 << 3,
        Full = 1 << 4,
        IgnoreCase = 1 << 5
    }
}
