using HtmlAgilityPack;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownComponent
{
    public class RawHtml : BlazorComponent
    {
        public string HtmlContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            if (HtmlContent == null) return;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(HtmlContent);

            var htmlBody = htmlDoc.DocumentNode;
            Decend(htmlBody, builder);
        }

        private void Decend(HtmlNode ds, RenderTreeBuilder b)
        {
            foreach (var nNode in ds.ChildNodes)
            {
                if (nNode.HasChildNodes)
                {
                    if (nNode.NodeType == HtmlNodeType.Element)
                        b.OpenElement(0, nNode.Name);
                    if (nNode.HasAttributes) Attributes(nNode, b);
                    Decend(nNode, b);
                    b.CloseElement();
                }
                else
                {
                    if (nNode.NodeType == HtmlNodeType.Text)
                    {
                        b.AddContent(0, nNode.InnerText);
                    }
                }
            }
        }

        private void Attributes(HtmlNode n, RenderTreeBuilder b)
        {
            foreach (var a in n.Attributes)
            {
                b.AddAttribute(0, a.Name, a.Value);
            }
        }

    }
}
