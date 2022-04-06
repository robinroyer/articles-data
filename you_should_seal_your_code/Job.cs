namespace you_should_seal_your_code;

using System;
using System.Linq;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;



public class AbstractClass
{
    public virtual int Action() => 1;
}

public class ConcreteClass : AbstractClass
{
    public override int Action() => 2;
}

public sealed class SealedConcreteClass : AbstractClass
{
    public override int Action() => 2;
}


// === benchmarking

[SimpleJob(RunStrategy.ColdStart)]
[MinColumn, MaxColumn, MedianColumn]
public class MethodJob
{
    [Params(10000000)]
    public int Size;
    private readonly Random _random = new();
    private ConcreteClass _concrete = new();
    private SealedConcreteClass _sealed = new();

    [Benchmark(Baseline = true)]
    public int WorkConcrete() =>
        Enumerable
            .Range(0, Size)
            .Select(_ => _concrete.Action() + _random.Next(0, 2))
            .Sum();

    [Benchmark]
    public int WorkConcreteSealed() =>
        Enumerable
            .Range(0, Size)
            .Select(_ => _sealed.Action() + _random.Next(0, 2))
            .Sum();
}





public class NonSealedType { }
public sealed class SealedType { }


[SimpleJob(RunStrategy.ColdStart)]
[MinColumn, MaxColumn, MedianColumn]
public class TypeJob
{
    public object _object = "coucou";

    [Benchmark(Baseline = true)]
    public bool WorkConcrete() => _object is NonSealedType;

    [Benchmark]
    public bool WorkConcreteSealed() => _object is SealedType;
}
