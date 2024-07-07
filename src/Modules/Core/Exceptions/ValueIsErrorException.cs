﻿namespace Modular.Ecommerce.Core.Exceptions;

public sealed class ValueIsErrorException(Exception innerException) : Exception(ErrorMessage, innerException)
{
    private const string ErrorMessage = "Value is Error";
}