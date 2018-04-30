Blazor.registerFunction('MarkdownComponent.HtmlRendererInterop.ReplaceInnerHtml', function (elementId, innerHtml) {
   
    let el = document.getElementById(elementId);
    if (el) { el.innerHTML = innerHtml; }
    
});