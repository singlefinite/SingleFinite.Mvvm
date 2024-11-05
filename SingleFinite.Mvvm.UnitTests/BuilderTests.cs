// MIT License
// Copyright (c) 2024 SingleFinite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.Extensions.DependencyInjection;
using SingleFinite.Mvvm.Internal.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class BuilderTests
{
    [TestMethod]
    public void Build_Method_With_Generic_Type_Creates_Objects_With_Services_And_Parameters()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ExampleService>();
        var serviceProvider = services.BuildServiceProvider();
        var exampleService = serviceProvider.GetRequiredService<ExampleService>();

        var builder = new Builder(serviceProvider);

        var example = builder.Build<ExampleClass>("test 1", 81);

        Assert.AreEqual(exampleService, example.Service);
        Assert.AreEqual("test 1", example.Text);
        Assert.AreEqual(81, example.Number);
    }

    [TestMethod]
    public void Build_Method_With_Object_Type_Creates_Objects_With_Services_And_Parameters()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ExampleService>();
        var serviceProvider = services.BuildServiceProvider();
        var exampleService = serviceProvider.GetRequiredService<ExampleService>();

        var builder = new Builder(serviceProvider);

        var example = (ExampleClass)builder.Build(typeof(ExampleClass), "test 1", 81);

        Assert.AreEqual(exampleService, example.Service);
        Assert.AreEqual("test 1", example.Text);
        Assert.AreEqual(81, example.Number);
    }

    [TestMethod]
    public void Build_Method_With_Missing_Parameters_Throws()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ExampleService>();
        var serviceProvider = services.BuildServiceProvider();

        var builder = new Builder(serviceProvider);

        Assert.ThrowsException<InvalidOperationException>(() => builder.Build<ExampleClass>());
    }

    [TestMethod]
    public void Build_Method_With_Missing_Service_Throws()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var builder = new Builder(serviceProvider);

        Assert.ThrowsException<InvalidOperationException>(() => builder.Build<ExampleClass>("test 1", 81));
    }

    #region Types

    private class ExampleService
    {
    }

    private class ExampleClass(ExampleService service, string text, int number)
    {
        public ExampleService Service { get; } = service;
        public string Text { get; } = text;
        public int Number { get; } = number;
    }

    #endregion
}
