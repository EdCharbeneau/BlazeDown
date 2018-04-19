using System;
using Microsoft.AspNetCore.Blazor.Browser.Interop;

namespace MarkdownComponent
{
    public class HtmlRendererInterop
    {
        public static string RenderMarkdownAsHtml(string htmlContent)
        {
            return RegisteredFunction.Invoke<string>(
                "MarkdownComponent.HtmlRendererInterop.RenderMarkdown",
                htmlContent);
        }
    }
}
