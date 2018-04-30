using System;
using Microsoft.AspNetCore.Blazor.Browser.Interop;

namespace MarkdownComponent
{
    public class HtmlRendererInterop
    {
        public static string ReplaceInnerHtml(string elementId, string htmlContent) =>
         RegisteredFunction.Invoke<string>("MarkdownComponent.HtmlRendererInterop.ReplaceInnerHtml", elementId, htmlContent);
    }
}
