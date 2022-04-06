// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


// BenchmarkDotNet
//     .Running
//     .BenchmarkRunner
//     .Run(
//         typeof(you_should_seal_your_code.MethodJob).Assembly,
//         BenchmarkDotNet
//             .Configs
//             .ManualConfig
//             .CreateMinimumViable()
//             .WithOption(BenchmarkDotNet.Configs.ConfigOptions.DisableLogFile, true));



BenchmarkDotNet
    .Running
    .BenchmarkRunner
    .Run(
        typeof(you_should_seal_your_code.TypeJob).Assembly,
        BenchmarkDotNet
            .Configs
            .ManualConfig
            .CreateMinimumViable()
            .WithOption(BenchmarkDotNet.Configs.ConfigOptions.DisableLogFile, true));