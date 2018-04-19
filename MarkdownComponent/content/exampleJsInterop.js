// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

Blazor.registerFunction('MarkdownComponent.ExampleJsInterop.Prompt', function (message) {
   
    let el = document.getElementById("markdown-component");
    if (el) { el.innerHTML = message; }
    else {
        console.log("HTML not rendered");
    }
});
