# BlazeDown

Blazor + Markdown = BlazeDown!

## This is an experiment built on top of an experiment to kick the tires of **Blazor**. 

**The initial version of BlazeDown was written using Blazor 0.2.** Much has changed in just a few months time and as a result BlazeDown has seen some updates to. This article was updated to reflect how BlazeDown was built using 0.5.1. _The previous version is available here_

This is a proof of concept using [Blazor](https://blogs.msdn.microsoft.com/webdev/2018/07/25/blazor-0-5-0-experimental-release-now-available/) to create an online Markdown editor.

This experiment is in no way intended to be a product, or example of best practices. It's here because it *it's possible* and that's all.

## Client Side C#

BlazeDown was built using Blazor, a client side **experimental framework** for building native web applications using .NET and C#. That's right, this app was created with .NET and is running natively in the browser using [WebAssembly](https://blogs.msdn.microsoft.com/webdev/2018/02/06/blazor-experimental-project/).

Thanks to Blazor the app requires no plugins, this is because the code is compiled to WebAssembly, something your browser understands. Nearly everything you see here was written in .NET using C# with a few exceptions. Since Blazor is in the experimental phase *(I can't stress that enough)*, some small workarounds are required for certain features.

## Building BlazeDown

This experiment was to test out Blazor and see how easy (or difficult) it was to use a .NET Standard library on the client. The beauty of BlazeDown is that writing a Markdown parser was completely unnecessary because one already existed for .NET.

## The Markdown Processor

BlazeDown takes advantage of the .NET ecosystem. It uses the [MarkDig](https://www.nuget.org/packages/Markdig/) an extensible Markdown processor for .NET. Since MarkDig is compatible with .NET Standard 1.1+ it worked flawlessly with Blazor. Having the freedom to reuse existing .NET libraries on the client is in my opinion what makes Blazor an interesting option for developers.

Utilizing MarkDig in Blazor followed the standard procedure of grabbing the package from [NuGet](https://www.nuget.org). Once the package was installed, MarkDig is initialize just as it would be in any other .NET application.

Simply calling MarkDig's `ToHtml` method converts a `string` into Html.

```
@using Markdig;

private string BuildHtmlFromMarkdown(string value) => Markdig.Markdown.ToHtml(
    markdown: value,
    pipeline: new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()
);
```

## Loading External Content

To complete the experiment Blazor was used to load Markdown _.md_ files externally. Once again .NET was leveraged to add the feature without directly jumping into JavaScript by using `System.Net.Http` and `Http.GetAsync`.

The code for using `Http.GetAsync` is quite similar to how it would be used in a typical .NET application. An HttpResponseMessage is created to make the call to the resource using `GetAsync`. Once the response returns, we check to see if the response was successful using `httpResponse.IsSuccessStatusCode`. Finally, the resulting markdown file is returned, or a error message is passed along `await httpResponse.Content.ReadAsStringAsync() : httpResponse.ReasonPhrase`.

While these are all quite familiar routines, it's worth noting that some abstractions may be present in Blazor to invoke JavaScript under the hood to make the actual Http request.

```
    public string FileUrl { get; set; }
    public string ContentValue { get; set; }

    protected async override Task OnInitAsync()
    {
        ContentValue = await GetContentFromUrl("/sample-data/example.md");
        StateHasChanged();
    }

    private async void OnClicked(UIMouseEventArgs e)
    {
        ContentValue = await GetContentFromUrl(FileUrl);
        StateHasChanged();
    }

    private async Task<string> GetContentFromUrl(string path)
    {
        HttpResponseMessage httpResponse = await Http.GetAsync(path);
        return httpResponse.IsSuccessStatusCode ?
        await httpResponse.Content.ReadAsStringAsync() : httpResponse.ReasonPhrase;
    }

```
### The Component

To fully experience what Blazor has to offer in its current state, the Blazor component model was used to create a `<Markdown>` component. The component is capable of receiving a simple string (markdown) and coverting it to HTML.

The `Content` property is used to set a simple string as the Markdown to render as HTML.

```<Markdown Content="# Hello World">```

_output_

```<h1>Hello World</h1>```

Blazor's component model follows similar principals to modern JavaScript frameworks like Angular. Each component has a HTML template, in the case of Blazor we use Razor's .cshtml format. In addtion to the template, the components code is encapsulated with the component as C#.

The Markdown component's structure is pretty straight forward. The properties and methods of the component are bound and rendered using Razor. By making Content and FromUrl public properties, these values are recognized as component properties when we write `<Markdown PropertyName`.

```
// Markdown.cshtml
@if (Content == null)
{
    <span>Loading...</span>
}
else
{
    @((MarkupString)BuildHtmlFromMarkdown(Content))
}

@functions {

[Parameter]
string Content { get; set; }

private string BuildHtmlFromMarkdown(string value) => Markdig.Markdown.ToHtml(
    markdown: value,
    pipeline: new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()
);
```
The code above represents the **ideal** code for the component. 

### Update: Blazor 0.5.1 release 

Before Blazor 0.5.1, there was no method for rendering raw HTML and BlazeDown was forced to use unconventional hacks to implement the feature. With the release of 0.5.1 the type `MarkupString` was added to facilitate rendering HTML directly using Blazor. _WARNING: Rendering raw HTML constructed from any untrusted source is a major security risk!_

Rendering the HTML parsed from MarkDig to the component's view is a simple as casting the HTML results from `BuildHtmlFromMarkdown` to a `MarkupString`.

```
@((MarkupString)BuildHtmlFromMarkdown(Content))
```
### Data Binding in Blazor

The BlazeDown app uses data binding to update the HTML preview when a user enters content in a `<textarea>` on the page. By binding `<textarea>` and `<Markdown>` components together, the online-markdown-editor experience is completed. To ensure the data is always updated when the textarea's value is changed two-way data binding is used. Using the `bind` attribute on the `<textarea>` the data `ContentValue` is bound to the `value` of the `<textarea>`, in addition it is automatically updated when the textarea's `onchange` event is raised.

```
<div class="col-sm-6">
    <span class="label label-default label-hint">Editor</span>
    <div class="markdown-editor">
        <textarea bind="@ContentValue" />
    </div>
</div>
<div class="col-sm-6">
    <span class="label label-default label-hint">HTML Preview</span>
    <div class="markdown-view">
        <Markdown Content="@ContentValue"></Markdown>
    </div>
</div>

```
## Conclusion

Blazor is quite an amazing idea and worthy experiment. It's exciting to see how easy it was to create an experiment like BlazeDown. Using mostly C# and existing .NET libraries, a client-side Markdown editor was created with minimal effort.

It's exciting to see Blazor being built. The community is experimenting right along side the dev team providing feedback, issues and pull request on [the Blazor GitHub repo](https://github.com/aspnet/Blazor).