using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Modular.Ecommerce.Core.Exceptions;

namespace Modular.Ecommerce.Core.Types;

public static class Result
{
    public static Result<T> Ok<T>(T ok)
    {
        return new Result<T>(ok);
    }
    
    public static Result<T> Failure<T>(Exception exception)
    {
        return new Result<T>(exception);
    }

    public static readonly Result<Unit> UnitResult = new(Unit.Value);

    public static Result<B> ToError<A, B>(this Result<A> res)
    {
        if (res.IsSuccess)
        {
            throw new ValueIsSuccessException<A>(res.Value);
        }
        
        return Failure<B>(res.ErrorValue);
    }

    public static bool TryGetOk<T>(this Result<T> result, [NotNullWhen(true)] out T? value)
    {
        if (result.IsSuccess)
        {
            value = result.Value!;
            return true;
        }

        value = default;
        return false;
    }
}


public readonly struct Result<T> : IEquatable<Result<T>>
{
    private enum ResultState : byte
    {
        Succeed,
        Error,
    }

    internal readonly T? Success;
    internal readonly Exception? Error;
    
    public T Value => IsSuccess ? Success : throw new ValueIsErrorException(Error);
    
    public Exception ErrorValue => IsSuccess ? throw new ValueIsSuccessException<T>(Success) : Error;
    public object Case => IsSuccess ? Success : Error;

    private readonly ResultState _state;

    [MemberNotNullWhen(true, nameof(Success))]
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(false, nameof(ErrorValue))]
    public bool IsSuccess => _state == ResultState.Succeed;

    internal Result(T success)
    {
        ArgumentNullException.ThrowIfNull(success);
        Success = success;
        Error = default;
        _state = ResultState.Succeed;
    }

    internal Result(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Success = default;
        Error = error;
        _state = ResultState.Error;
    }
    
    
    [System.Diagnostics.Contracts.Pure]
    public Result<TR> Map<TR>(Func<T, TR> func)
    {
        if (!IsSuccess)
        {
            return Result.Failure<TR>(Error);
        }
        return Result.Ok(func(Success));
    }

    [System.Diagnostics.Contracts.Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TRes Match<TRes>(Func<T, TRes> ok, Func<Exception, TRes> error)
    {
        if (!IsSuccess)
        {
            return error(Error);
        }
        return ok(Success);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<T> ok, Action<Exception> error)
    {
        if (IsSuccess)
        {
            ok(Success);
        }
        else
        {
            error(Error);
        }
    }

    [System.Diagnostics.Contracts.Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TRes> MatchTask<TRes>(Func<T, CancellationToken, Task<TRes>> ok,
        Func<Exception, CancellationToken, Task<TRes>> error, CancellationToken token = default)
    {
        if (IsSuccess)
        {
            return ok(Success, token);
        }
        return error(Error, token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task MatchTask(Func<T, CancellationToken, Task> ok, Func<Exception, CancellationToken, Task> error,
        CancellationToken cancellationToken = default)
    {
        if (IsSuccess)
        {
            return ok(Success, cancellationToken);
        }
        return error(Error, cancellationToken);
    }

    public bool Equals(Result<T> other)
    {
        return EqualityComparer<T?>.Default.Equals(Success, other.Success) && Equals(Error, other.Error) && _state == other._state;
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Success, Error, (int)_state);
    }

    public static bool operator ==(Result<T> left, Result<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<T> left, Result<T> right)
    {
        return !(left == right);
    }
}