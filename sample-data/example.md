# BlazeDown

Blazor + Markdown = BlazeDown!

## This is an experiment built on top of an experiment to kick the tires of **Blazor**. 

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

## Conclusion

Blazor is quite an amazing idea and worthy experiment. It's exciting to see how easy it was to create an experiment like BlazeDown. Using mostly C# and existing .NET libraries, a client-side Markdown editor was created with minimal effort.

It's exciting to see Blazor being built. The community is experimenting right along side the dev team providing feedback, issues and pull request on [the Blazor GitHub repo](https://github.com/aspnet/Blazor).