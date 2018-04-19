# BlazeDown

Blazor + Markdown = BlazeDown!

## This is an experiment built on top of an experiment to kick the tires of **Blazor**. 

This is a proof of concept using [Blazor](https://blogs.msdn.microsoft.com/webdev/2018/04/17/blazor-0-2-0-release-now-available/) to create an online Markdown editor.

This experiment is in no way intented to be a product, or example of best practices. It's here because it *it's possible* and that's all.

## Client Side C#

BlazeDown was built using Blazor, a client side **experimental framework** for building native web applications using .NET and C#. That's right, this app was created with .NET and is running natively in the browser using [WebAssembly](https://blogs.msdn.microsoft.com/webdev/2018/02/06/blazor-experimental-project/).

Thanks to Blazor the app requires no plugins, this is because the code is compiled to WebAssembly, something your browser understands. Nearly everything you see here was written in .NET using C# with a few exceptions. Since Blazor is in the experimental phase *(I can't stress that enough)*, some small workarounds are required for certain features.

## Building BlazeDown

