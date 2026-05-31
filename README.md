### SingleFinite.Mvvm
SingleFinite.Mvvm is a [Model-View-ViewModel](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel) library.
It's built for .NET Core and contains interfaces, classes, and services that provide a solid foundation for building a modern MVVM .NET application.

#### Getting started

The following example illustrates a simple "hello world" style app that shows how to get something up and running after adding the SingleFinite.Mvvm
nuget package as a dependency in your project.

To start, let's define a simple view model class that extends `ViewModel`:

```csharp
// MyViewModel.cs
//

using SingleFinite.Mvvm;

namespace  MyApp;

public class MyViewModel : ViewModel
{
}
```

and a corresponding view class that implements `IView` and will be used with the view model defined above:

```csharp
// MyView.cs
//

using SingleFinite.Mvvm;

namespace MyApp;

public class MyView(MyViewModel viewModel) : IView<MyViewModel>
{
    public MyViewModel ViewModel => viewModel;
}
```

Now that we have a view model and view defined we can configure an instance of `AppHost` that will host our application state:

```csharp
// Program.cs
//

using SingleFinite.Mvvm;

namespace MyApp;

var services = new ServiceCollection();

var appHost = new AppHostBuilder()
    .AddViews(views => views.Add<MyViewModel, MyView>())
    .Build(services);

var provider = services.BuildServiceProvider();

appHost.Start(provider);
```

As you can see above we use the `AddViews` method to register the `MyViewModel` and `MyView` types.  Now when we want to display an instance
of `MyViewModel` an instance of `MyView` will be used to display it.

> :information_source: Note
>
> There is an extension method for registering views that can be used to scan assemblies for `ViewModel` and `IView` types to register but we manually
> register them here for better clarity in the example.

Let's create a new instance of `MyViewModel` from the `AppHost` instance we created above:

```csharp
// Program.cs
//

// ...

var presenter = provider.GetRequiredService<IItemPresenter>();
var viewModel = presenter.Set<MyViewModel>();

Console.WriteLine($"My ViewModel => {viewModel.GetType().FullName}");
Console.WriteLine($"My View => {presenter.Current.GetType().FullName}");
```

SingleFinite.MVVM is built on top of the [Microsoft Dependency Injection library](Microsoft.Extensions.DependencyInjection) and the `AppHostBuilder` is
used to register services for the app.  We get a new instance of `IItemPresenter` from the `IServiceProvider` which we use to create a new instance of
our view model.

`IPresenter` services are used to create and manage the lifecycle of view models and their views.  Both the view model and view
will be created with a new dependency injection scope and will be injected with services as defined in their constructors.  Normally you will use
another platform specific library like [SingleFinite.Mvvm.Maui](https://github.com/singlefinite/SingleFinite.Mvvm.Maui/) to manage the `AppHost` instance and
to present views for `IPresenter` based services.

#### Learn more

Check out the project [Wiki](https://github.com/singlefinite/SingleFinite.Mvvm/wiki) for more detailed documentation on this library.
