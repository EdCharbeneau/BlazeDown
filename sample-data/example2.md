# BlazeDown 2

Blazor + Markdown = BlazeDown!

## This is an experiment built on top of an experiment to kick the tires of **Blazor**. 

This is a proof of concept using [Blazor](https://blogs.msdn.microsoft.com/webdev/2018/04/17/blazor-0-2-0-release-now-available/) to create an online Markdown editor.

This experiment is in no way intented to be a product, or example of best practices. It's here because it *it's possible* and that's all.

## Client Side C#

BlazeDown was built using Blazor, a client side **experimental framework** for building native web applications using .NET and C#. That's right, this app was created with .NET and is running natively in the browser using [WebAssembly](https://blogs.msdn.microsoft.com/webdev/2018/02/06/blazor-experimental-project/).

Thanks to Blazor the app requires no plugins, this is because the code is compiled to WebAssembly, something your browser understands. Nearly everything you see here was written in .NET using C# with a few exceptions. Since Blazor is in the experimental phase *(I can't stress that enough)*, some small workarounds are required for certain features.

## Building BlazeDown

This experiment was to test out Blazor and see how easy (or difficult) it was to use a .NET Standard library on the client. The beauty of BlazeDown is that writing a Markdown parser was completely unecessary because one already existed for .NET.

## The Markdown Processor

BlazeDown takes advatnage of the .NET ecosystem. It uses the [MarkDig](https://www.nuget.org/packages/Markdig/) an extensible Markdown processor for .NET. Since MarkDig is compatiable with .NET Standard 1.1+ it worked flawlessly with Blazor. Having the freedom to reuse existing .NET libraries on the client is in my opinion what makes Blazor an iteresting option for developoers.

Utilizing MarkDig in Blazor followed the standard procedure of grabbing the package from [NuGet](https://www.nuget.org). Once the package was installed, MarkDig is initialize just as it would be in any other .NET application.

Simply calling MarkDig's `ToHtml` method converts a `string` into Html.

```
	@using Markdig;

    private string RenderHtmlContent(string value) => Markdig.Markdown.ToHtml(
    markdown: value,
    pipeline: new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()
    );
```
To complete the experiment Blazor was used to load Markdown _.md_ files externally. Once again .NET was leveraged to add the feature without directly jumping into JavaScript by using `System.Net.Http` and `Http.GetAsync`.

The code for using `Http.GetAsync` is quite similar to how it would be used in a typical .NET application. An HttpResponseMessage is created to make the call to the resource using `GetAsync`. Once the response returns, we check to see if the response was successful using `httpResponse.IsSuccessStatusCode`. Finally, the resulting markdown file is returned, or a error message is passed along `await httpResponse.Content.ReadAsStringAsync() : httpResponse.ReasonPhrase`.

While these are all quite famailiar routines, it's worth noting that some abstractions may be present in Blazor to invoke JavaScript under the hood to make the actual Http request.

```
    protected override async Task OnInitAsync()
    {
        if (Content == null)
            Content = String.IsNullOrEmpty(FromUrl) ?
                "Content or FromUrl property is not set or invalid" : await InitContentFromUrl();
    }

    private async Task<string> InitContentFromUrl()
    {
        HttpResponseMessage httpResponse = await Http.GetAsync(FromUrl);
        return httpResponse.IsSuccessStatusCode ?
        await httpResponse.Content.ReadAsStringAsync() : httpResponse.ReasonPhrase;
    }
```
### The Component

To fully experience what Blazor has to offer in its current state, the Blazor component model was used to create a `<Markdown>` component. The component is capable of receiving a simple string, or fetching data from an external resource.

The `Content` property is used to set a simple string as the Markdown to render as HTML.

```<Markdown Content="# Hello World">```

_output_

```<h1>Hello World</h1>```

The `FromUrl` property defines a URL for the component to fetch data from.

```<Markdown Content="/helloworld.md">```

_output fetched via Http_

```<h1>Hello World</h1>```

Blazor's component model follows similar principals to modern JavaScript frameworks like Angular. Each component has a HTML template, in the case of Blazor we use Razor's .cshtml format. In addtion to the template, the components code is encapsulated with the component as C#.

The Markdown component's structure is pretty straight forward. The properties and methods of the component are bound and rendered using Razor. By making Content and FromUrl public properties, these values are recognized as component properties when we write `<Markdown PropertyName`.

When the component is created it needs to fetch an a resource if one is defined in the `FromUrl` property. Telling Blazor to initialize a fetch when the component loads is as simple as overriding the `OnInitAsync` behavior from the component's base class.

```
// Markdown.cshtml
@if (Content == null)
{
    <span>Loading...</span>
}
else
{
    <div id="markdown-component">
        <!-- Omitted for further explination -->
		@RenderHtml()
    </div>
}

    public string Content { get; set; }
    public string FromUrl { get; set; }

    protected override async Task OnInitAsync()
    {
        if (String.IsNullOrEmpty(Content))
            Content = String.IsNullOrEmpty(FromUrl) ?
                "Content or FromUrl property is not set or invalid" : await InitContentFromUrl();
    }

    private async Task<string> InitContentFromUrl()
    {
        HttpResponseMessage httpResponse = await Http.GetAsync(FromUrl);
        return httpResponse.IsSuccessStatusCode ?
        await httpResponse.Content.ReadAsStringAsync() : httpResponse.ReasonPhrase;
    }

    private string RenderHtmlContent() => Markdig.Markdown.ToHtml(
        markdown: Content,
        pipeline: new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()
    );
```
The code above represents the **ideal** code for the component. 

### The Bad Parts

The ideal code mentioned above requried quite a few hacks, remember that this is a 0.2.0 release and Blazor is very early in development. Browsing the source code on GitHub will reveal that much more is going on behind the scense.

**Rendering Raw HTML**

The first major issue is that Raw Html cannot be rendered with Blazor. This means when MarkDig calls `ToHtml` the literal HTML is rendered on screen as string, ex: `<h1>Even the H1 tags are visible</h1>`. In order to make Raw HTML render properly two methods are used.

First, an ugly hack is needed to display the HTML when the component is initialized. Since the DOM hasn't rendered yet, we can't even properly use JavaScript to combat this issue. The actual code attaches to the `onerror` event of an image to forcing the HTML to render via JavaScript.

```
    // This is a totally hacky way to get HTML injected on the page until https://github.com/aspnet/Blazor/issues/167 is resolved
    <div>
        <img src="" onerror="(function (e) { e.parentElement.innerHTML = '@HttpUtility.JavaScriptStringEncode(RenderHtmlContent())'; })(this)" />
    </div>
```

Second, when the `Content` property is set, JavaScript is called via interop to find our component's container to render the HTML directly to the DOM.

```
    public string Content
    {
        get { return content; }
        set
        {
            content = value;
            if(isComponentInitialized)
                // Once this issue is fixed https://github.com/aspnet/Blazor/issues/167
                // then the interop will no longer be needed. This will also take care of that lame <img hack in the markup above.
                // Once raw HTML output is available, the component will simply use RenderHtmlContent function directly from Razor.
                HtmlRendererInterop.RenderMarkdownAsHtml(RenderHtmlContent(value));
        }
    }
```

```
Blazor.registerFunction('MarkdownComponent.HtmlRendererInterop.RenderMarkdown', function (message) {
   
    let el = document.getElementById("markdown-component");
    if (el) { el.innerHTML = message; }
    else {
        console.log("HTML not rendered");
    }
});

```

While ugly and quite cool at the same time, it's necessary. Blazor still has a lot of work to eliminate the need for interop and quirky hacks like these.

### Data Binding in Blazor

In addition to the rendering hacks, data binding is still quite new and changing in 0.2.0. Data binding works in Blazor by simple conventions that are quite easy to understand and implment.

Here's the example shown in the MSDN article celebrating the [0.2.0 release](https://blogs.msdn.microsoft.com/webdev/2018/04/17/blazor-0-2-0-release-now-available/).


```
@* in Counter.cshtml *@
<div>...html omitted for brevity...</div>
@functions {
    public int Value { get; set; } = 1;
    public Action<int> ValueChanged { get; set; }
}

@* in another file *@
<Counter bind-Value="@CurrentValue" />
@functions {
    public int CurrentValue { get; set; }
}
```

The BlazeDown app uses data binding to update the HTML preview when a user enters content in a `<textarea>` on the page. By binding `<textarea>` and `<Markdown>` components together, the online-markdown-editor experience is completed.

It's worth noting that the data binding in current release don't use the conventions added in 0.2.0. At the time of writing this worked with some exceptions (uncaught exceptions). Using the bind-Property convention outlined above caused [Visual Studio to crash](https://github.com/aspnet/Blazor/issues/597). In addition, the data binding [didn't appear to work in a two-way binding](https://github.com/aspnet/Blazor/issues/610) scenario. Eventually this binding method was avoided and a more implicit approach was used.

```
<div class="row">
    <div class="col-sm-6">
        <div class="markdown-editor">
            <textarea bind="@ContentValue" />
        </div>
    </div>
    <div class="col-sm-6">
        <div class="markdown-view">
            <Markdown Content="@ContentValue" ContentChanged="@OnContentChanged" FromUrl="/sample-data/example.md"></Markdown>
        </div>
    </div>
</div>


@functions {
    public string ContentValue { get; set; }

    void OnContentChanged(string newValue)
    {
        ContentValue = newValue;
        StateHasChanged();
    }
```
## Conclusion

Blazor is quite an amazing idea and worthy experiment. It's exciting to see how easy it was to create an experiment like BlazeDown. Using mostly C# and existing .NET libraries, a client-side Markdown editor was created with minimal effort.

While Blazor clearly has flaws, **as one would expect from an early in development experiment**, if Blazor was a matured product and the hacks shown above were not necessary the entire process of creating BlazeDown would have been simple and straight forward. Even with the inconvinences mentioned above, the BlazeDown experiment is quite a success.

It's exciting to see Blazor being built. The community is experimenting right along side the dev team providing feedback, issues and pullrequets on [the Blazor GitHub repo](https://github.com/aspnet/Blazor).