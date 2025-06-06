﻿// MIT License
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

using SingleFinite.Mvvm.Internal;
using SingleFinite.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using SingleFinite.Essentials;

namespace SingleFinite.Mvvm.UnitTests;

[TestClass]
public class ViewModelTests
{
    [TestMethod]
    public void Dispose_Method_Disposes_All_Descendants()
    {
        using var context = new TestContext();
        var scope = context.ServiceProvider.CreateLinkedScope();

        var viewModel1 = new NestingViewModel(scope.ServiceProvider.GetRequiredService<IPresentableStack>());
        var viewModel2 = viewModel1.CreateChild("second");
        var viewModel3 = viewModel2.CreateChild("third");

        Assert.IsFalse(viewModel1.IsDisposed);
        Assert.IsFalse(viewModel2.IsDisposed);
        Assert.IsFalse(viewModel3.IsDisposed);

        viewModel1.ClearChildren();

        Assert.IsFalse(viewModel1.IsDisposed);
        Assert.IsTrue(viewModel2.IsDisposed);
        Assert.IsTrue(viewModel3.IsDisposed);
    }

    [TestMethod]
    public void Lifecycle_Events_Fire_In_Expected_Order()
    {
        var onInitializeCount = 0;
        var onActivateCount = 0;
        var onDeactivateCount = 0;
        var onDisposeCount = 0;
        var isActiveChanged = new List<bool>();

        var viewModel = new SimpleViewModel();
        var viewModelInterface = (IViewModel)viewModel;

        viewModel.Initialized
            .Observe()
            .OnEach(() => onInitializeCount++);
        viewModel.Activated
            .Observe()
            .OnEach(() => onActivateCount++);
        viewModel.Deactivated
            .Observe()
            .OnEach(() => onDeactivateCount++);
        viewModel.Disposed
            .Observe()
            .OnEach(() => onDisposeCount++);
        viewModel.IsActiveChanged
            .Observe()
            .OnEach(isActiveChanged.Add);

        Assert.AreEqual(0, onInitializeCount);
        Assert.AreEqual(0, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Initialize();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(0, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Initialize();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(0, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Activate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);

        Assert.AreEqual(1, isActiveChanged.Count);
        Assert.IsTrue(isActiveChanged[0]);
        isActiveChanged.Clear();

        Assert.IsTrue(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Activate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(0, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsTrue(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Deactivate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);

        Assert.AreEqual(1, isActiveChanged.Count);
        Assert.IsFalse(isActiveChanged[0]);
        isActiveChanged.Clear();

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Deactivate();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(0, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsFalse(viewModel.IsDisposed);

        viewModelInterface.Dispose();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(1, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsTrue(viewModel.IsDisposed);

        viewModelInterface.Dispose();

        Assert.AreEqual(1, onInitializeCount);
        Assert.AreEqual(1, onActivateCount);
        Assert.AreEqual(1, onDeactivateCount);
        Assert.AreEqual(1, onDisposeCount);
        Assert.AreEqual(0, isActiveChanged.Count);

        Assert.IsFalse(viewModel.IsActive);
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public void DerivedProperty_Get_Raises_Events()
    {
        var observedEvents = new List<(object?, PropertyChangedEventArgs)>();
        var simpleViewModel = new SimpleViewModel();
        var simpleView = new SimpleView(simpleViewModel);

        ((INotifyPropertyChanged)simpleView).PropertyChanged += (sender, args) =>
        {
            observedEvents.Add((sender, args));
        };

        Assert.AreEqual(0, observedEvents.Count);
        Assert.AreEqual("positive", simpleView.DerivedNumber);

        simpleViewModel.Number = -5;

        Assert.AreEqual(1, observedEvents.Count);
        var (sender, args) = observedEvents[0];
        Assert.AreEqual(simpleView, sender);
        Assert.AreEqual("DerivedNumber", args.PropertyName);
        Assert.AreEqual("negative", simpleView.DerivedNumber);
    }

    [TestMethod]
    public void DerivedProperty_Set_Raises_Events()
    {
        var observedEvents = new List<(object?, PropertyChangedEventArgs)>();
        var simpleViewModel = new SimpleViewModel();
        var simpleView = new SimpleView(simpleViewModel);

        ((INotifyPropertyChanged)simpleViewModel).PropertyChanged += (sender, args) =>
        {
            observedEvents.Add((sender, args));
        };

        Assert.AreEqual(0, observedEvents.Count);
        Assert.AreEqual(0, simpleViewModel.Number);

        simpleView.DerivedNumber = "negative";

        Assert.AreEqual(1, observedEvents.Count);
        var (sender, args) = observedEvents[0];
        Assert.AreEqual(simpleViewModel, sender);
        Assert.AreEqual("Number", args.PropertyName);
        Assert.AreEqual(-1, simpleViewModel.Number);
    }

    [TestMethod]
    public void DerivedProperty_Event_Raised_Once_For_Multiple_Source_Changes()
    {
        var observedEvents = new List<(object?, PropertyChangedEventArgs)>();
        var viewModel = new MultiDerivedViewModel();
        var view = new MultiDerivedView(viewModel);

        ((INotifyPropertyChanged)view).PropertyChanged += (sender, args) =>
        {
            observedEvents.Add((sender, args));
        };

        Assert.AreEqual(0, observedEvents.Count);
        Assert.AreEqual("0:", view.Label);

        viewModel.Number = 9;

        Assert.AreEqual(1, observedEvents.Count);
        var (sender, args) = observedEvents[0];
        Assert.AreEqual(view, sender);
        Assert.AreEqual("Label", args.PropertyName);
        Assert.AreEqual("9:9", view.Label);
    }

    #region Types

    private class MultiDerivedViewModel : ViewModel
    {
        public int Number
        {
            get;
            set => ChangeProperty(ref field, value);
        }

        public string? Text
        {
            get;
            private set => ChangeProperty(ref field, value);
        }

        protected override void OnChanged()
        {
            Text = Number.ToString();
        }
    }

    private class MultiDerivedView : IView<MultiDerivedViewModel>
    {
        public MultiDerivedView(MultiDerivedViewModel viewModel)
        {
            ViewModel = viewModel;

            ViewModel.AddDerivedProperty(
                derivedPropertyOwner: this,
                derivedPropertyName: nameof(Label),
                sourcePropertyNames: [
                    nameof(ViewModel.Number),
                    nameof(ViewModel.Text)
                ]
            );
        }

        public MultiDerivedViewModel ViewModel { get; }

        public string Label => $"{ViewModel.Number}:{ViewModel.Text}";
    }

    private class SimpleViewModel : ViewModel
    {
        public int Number
        {
            get;
            set => ChangeProperty(ref field, value);
        }
    }

    private class SimpleView : IView<SimpleViewModel>
    {
        public SimpleView(SimpleViewModel viewModel)
        {
            ViewModel = viewModel;

            ViewModel.AddDerivedProperty(
                derivedPropertyOwner: this,
                derivedPropertyName: nameof(DerivedNumber),
                sourcePropertyNames: nameof(ViewModel.Number)
            );
        }

        public SimpleViewModel ViewModel { get; }

        public string DerivedNumber
        {
            get => ViewModel.Number >= 0 ?
                "positive" :
                "negative";

            set => ViewModel.Number = value == "positive" ?
                1 :
                -1;
        }
    }

    private class ExampleViewModel(
        IBackgroundDispatcher backgroundDispatcher
    ) : ViewModel
    {
        public int Number
        {
            get;
            set => ChangeProperty(
                field: ref field,
                value: value,
                onPropertyChanged: () => _numberChanged.Emit(field)
            );
        }

        public string Text
        {
            get;
            private set => ChangeProperty(ref field, value);
        } = "";

        protected override void OnInitialize()
        {
            NumberChanged
                .Observe()
                .OnEach(number => Text = $"Number {number}")
                .On(this);
        }

        public void RunOnBackground(Action<CancellationToken> action)
        {
            backgroundDispatcher.Run(action);
        }

        public Observable<int> NumberChanged => _numberChanged.Observable;
        private readonly ObservableSource<int> _numberChanged = new();
    }

    private class NestingView(NestingViewModel viewModel) : IView<NestingViewModel>
    {
        public NestingViewModel ViewModel => viewModel;
    }

    private class NestingViewModel(IPresentableStack presenterStack) : ViewModel
    {
        private string _name = "first";
        public string Name => _name;

        public NestingViewModel CreateChild(string name)
        {
            var child = presenterStack.Push<NestingViewModel>();
            child._name = name;
            return child;
        }

        public void ClearChildren()
        {
            presenterStack.Clear();
        }
    }

    #endregion
}
