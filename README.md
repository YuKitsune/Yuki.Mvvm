# About

`Yuki.Mvvm` provides a lightweight, and easy to use MVVM implementation for .NET Standard 2.1 projects. This library is also compatable with anything which utilizes the `INotifyPropertyChanging`, `INotifyPropertyChanged` or `ICommand` interfaces.

# Usage

To make a ViewModel, simply inherit from the `ViewModel` class
```cs
public class MainWindowViewModel : ViewModel
{
    ...
}
```

## Properties

Here is a typical property declaration. The Getter and setter should invoke the base `ViewModel.Get<TValue>`  and `ViewModel.Set(TValue)` methods.
These methods will automatically handle invoking the `PropertyChanging`, `PropertyChanged`, and `CanExecuteChanged` events.
```cs
public string MyAwesomeString
{
    get => Get<string>();
    set => Set(value);
}
```

### Dependent Properties
Properties have the ability to depend on one another, which allows for one property to be changed, but multiple PropertyChanged methods to be invoked.

In this example, `FirstNumber` and `SecondNumber` are configured as normal, but `Result` depends on them, as it sums their values.
```cs
public int FirstNumber
{
    get => Get<int>();
    set => Set(value);
}

public int SecondNumber
{
    get => Get<int>();
    set => Set(value);
}

// Implicit dependency
// No matter what property changes (So long as the Set(TValue) method is invoked), a PropertyChanged event will be invoked for this property
public int Result
{
    get => FirstNumber + SecondNumber;
}

// Explicit dependency
// A PropertyChanged event will be invoked for this property whenever the value of FirstNumber or SecondNumber changes (from calling Set(TValue))
[Dependant(nameof(FirstNumber), nameof(SecondNumber))]
public int Result
{
    get => FirstNumber + SecondNumber;
}
```

## Commands
`Yuki.Mvvm` also provides different types of `ICommand` implementations:
```cs
public bool CanDoSomethingAsync
{
    get => Get<bool>();
    set => Set(value);
}

public RelayCommand DoSomethingCommand => Get(new RelayCommand(DoSomething));
public AsyncCommand DoSomethingAsyncCommand => Get(
    new AsyncCommand(
        DoSomethingAsync, 
        canExecute: () => CanDoSomethingAsync)
    );

public void DoSomething()
{
    ...
}

public async Task DoSomethingAsync()
{
    ...
}
```

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`feature/AmazingFeature`)
3. Commit your Changes
4. Push to the Branch
5. Open a Pull Request
