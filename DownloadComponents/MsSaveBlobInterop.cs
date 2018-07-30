using System;
using Microsoft.JSInterop;

namespace DownloadComponents
{
    public class MsSaveBlobInterop
    {
        public static void MsSaveBlob(string payload, string filename)
        {
            JSRuntime.Current.InvokeAsync<string>("MsSaveBlob", payload, filename);
        }
    }
}
