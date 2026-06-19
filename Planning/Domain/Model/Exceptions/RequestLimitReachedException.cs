using System;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;

public class RequestLimitReachedException(ClientIdentity client)
    : Exception($"Request limit reached for client {client}.");

