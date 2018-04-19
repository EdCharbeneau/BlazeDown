using System;
using Microsoft.AspNetCore.Blazor.Browser.Interop;

namespace MarkdownComponent
{
    public class ExampleJsInterop
    {
        public static string Prompt(string message)
        {
            return RegisteredFunction.Invoke<string>(
                "MarkdownComponent.ExampleJsInterop.Prompt",
                message);
        }
    }
}
