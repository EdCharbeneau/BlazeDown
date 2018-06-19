using System;
using Microsoft.AspNetCore.Blazor.Browser.Interop;

namespace DownloadComponents
{
    public class MsSaveBlobInterop
    {
        public static void MsSaveBlob(string payload, string filename)
        {
            RegisteredFunction.Invoke<string>("DownloadComponents.MsSaveBlobInterop.MsSaveBlob",
                payload, filename);
        }
    }
}
