// MIT License
// Copyright (c) 2025 Single Finite
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

using System.ComponentModel;
using SingleFinite.Essentials;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public sealed class INotifyPropertyChangedExtensionsTests
{
    [TestMethod]
    public void ObservePropertyChanged_Emits_When_Event_Raised()
    {
        var component = new ExampleComponent();
        var observedNames = new List<string?>();

        var observer = component.ObservePropertyChanged()
            .OnEach(observedNames.Add);

        Assert.IsEmpty(observedNames);

        component.Number = 9;

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Number", observedNames[0]);
    }

    [TestMethod]
    public void ObservePropertyChanged_With_PropertyName_Emits_When_Event_Raised()
    {
        var component = new ExampleComponent();
        var observedNames = new List<string?>();

        var observer = component.ObservePropertyChanged(
            property: () => component.Text
        ).OnEach(observedNames.Add);

        Assert.IsEmpty(observedNames);

        component.Number = 9;

        Assert.IsEmpty(observedNames);

        component.Text = "Hello";

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Text", observedNames[0]);
    }

    #region Types

    private class ExampleComponent : INotifyPropertyChanged
    {
        public int Number
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Number)));
                }
            }
        }

        public string? Text
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Text)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    #endregion
}
