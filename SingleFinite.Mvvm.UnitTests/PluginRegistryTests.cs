// MIT License
// Copyright (c) 2026 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using SingleFinite.Mvvm.Internal.Services;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class PluginRegistryTests
{
    [TestMethod]
    public void GetPlugins_Method_Returns_Registered_Plugins()
    {
        var pluginCollection = new PluginCollection();
        pluginCollection.Add<ExampleAPlugin1, ExampleAPluginHost>();
        pluginCollection.Add<ExampleAPlugin2, ExampleAPluginHost>();
        pluginCollection.Add<ExampleBPlugin1, ExampleBPluginHost>();

        var pluginRegistry = new PluginRegistry(pluginCollection);

        var pluginsForA = pluginRegistry.GetPlugins(new ExampleAPluginHost());
        Assert.HasCount(2, pluginsForA);
        Assert.IsTrue(
            pluginsForA.Any(
                descriptor =>
                    descriptor.PluginType == typeof(ExampleAPlugin1) &&
                    descriptor.PluginHostType == typeof(ExampleAPluginHost)
            )
        );
        Assert.IsTrue(
            pluginsForA.Any(
                descriptor =>
                    descriptor.PluginType == typeof(ExampleAPlugin2) &&
                    descriptor.PluginHostType == typeof(ExampleAPluginHost)
            )
        );

        var pluginsForB = pluginRegistry.GetPlugins(new ExampleBPluginHost());
        Assert.HasCount(1, pluginsForB);
        Assert.IsTrue(
            pluginsForB.Any(
                descriptor =>
                    descriptor.PluginType == typeof(ExampleBPlugin1) &&
                    descriptor.PluginHostType == typeof(ExampleBPluginHost)
            )
        );
    }

    [TestMethod]
    public void GetPlugins_Method_Returns_Empty_List_When_No_Registered_Plugins()
    {
        var pluginCollection = new PluginCollection
        {
            new PluginDescriptor(
                PluginType: typeof(ExampleAPlugin1),
                PluginHostType: typeof(ExampleAPluginHost)
            ),
            new PluginDescriptor(
                PluginType: typeof(ExampleAPlugin2),
                PluginHostType: typeof(ExampleAPluginHost)
            )
        };

        var pluginRegistry = new PluginRegistry(pluginCollection);

        var pluginsForB = pluginRegistry.GetPlugins(new ExampleBPluginHost());
        Assert.HasCount(0, pluginsForB);
    }

    #region Types

    private class ExampleAPluginHost : ViewModel, IPluginHost
    {
    }

    private class ExampleAPlugin1 : Plugin<ExampleAPluginHost>
    {
    }

    private class ExampleAPlugin2 : Plugin<ExampleAPluginHost>
    {
    }

    private class ExampleBPluginHost : ViewModel, IPluginHost
    {
    }

    private class ExampleBPlugin1 : Plugin<ExampleBPluginHost>
    {
    }

    #endregion
}
