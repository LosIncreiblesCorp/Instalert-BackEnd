using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Alert.Application.Internal.CommandServices;
public sealed class AlertCommandService(IAlertRecordRepository records, IAlertPreferencesRepository preferences, IOutboxMessageRepository outbox, IBusinessProfileQueryService business, ICurrentActor actor, IUnitOfWork work, TimeProvider clock, IPanicPreparationTokens proofs, IBusinessRevisionCommandService revisions) : IAlertCommandService
{
    public async Task<Result<PanicPreparationSnapshot, EPreparePanicError>> Handle(PreparePanicCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var businessId = command.BusinessId;
            var location = command.Location;
            var lat = command.Latitude;
            var lng = command.Longitude;
            await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
            Guard.Coordinates(lat, lng);
            Guard.Require((location?.Length ?? 0) <= 250, "validation", "location");
            var p = await preferences.FindByIdAsync(actor.UserId, cancellationToken);
            var now = clock.GetUtcNow();
            var preparation = new PanicPreparation(Guid.NewGuid().ToString(), businessId, actor.UserId, location ?? "", lat, lng, now.AddSeconds(p?.PanicGracePeriodSeconds ?? 5), now.AddMinutes(10));
            return new Result<PanicPreparationSnapshot, EPreparePanicError>.Success(new PanicPreparationSnapshot(proofs.Protect(preparation), preparation.NotBefore, preparation.ExpiresAt, (preparation.NotBefore - now).TotalSeconds));
        }
        catch (DomainException error)
        {
            return new Result<PanicPreparationSnapshot, EPreparePanicError>.Failure(error.Code switch
            {
                "validation" => EPreparePanicError.InvalidInput,
                "unauthenticated" => EPreparePanicError.Unauthenticated,
                "forbidden" => EPreparePanicError.Forbidden,
                "not_found" => EPreparePanicError.NotFound,
                "conflict" => EPreparePanicError.Conflict,
                "concurrency" => EPreparePanicError.ConcurrencyConflict,
                "transition" => EPreparePanicError.InvalidTransition,
                "already_completed" => EPreparePanicError.AlreadyCompleted,
                "unexpected" => EPreparePanicError.UnexpectedError,
                _ => EPreparePanicError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<AlertFact, EConfirmPanicError>> Handle(ConfirmPanicCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var preparationToken = command.PreparationToken;
            var location = command.Location;
            var latitude = command.Latitude;
            var longitude = command.Longitude;
            var p = proofs.Unprotect(preparationToken);
            Guard.Require(p.UserId == actor.UserId, "forbidden");
            var now = clock.GetUtcNow();
            Guard.Require(now >= p.NotBefore && now < p.ExpiresAt, "transition");
            await business.Handle(new RequireBusinessAccessQuery(p.BusinessId, actor.UserId, false), cancellationToken);
            var existing = await records.FindByIdAsync(p.Id, cancellationToken);
            if (existing is not null)
                return new Result<AlertFact, EConfirmPanicError>.Success(ToAlertFact(existing));
            Guard.Require((await records.FindByEmployeeIdAndKindAndStatusAsync(actor.UserId, "panic", "active", cancellationToken)).Count == 0, "conflict");
            Guard.Coordinates(latitude, longitude);
            Guard.Require((location?.Length ?? 0) <= 250, "validation", "location");
            var a = AlertRecord.Panic(p.Id, p.BusinessId, p.UserId, location ?? p.Location, latitude ?? p.Latitude, longitude ?? p.Longitude, now);
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(p.BusinessId), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            await records.AddAsync(a, cancellationToken);
            await Publish(a, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<AlertFact, EConfirmPanicError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, EConfirmPanicError>.Failure(error.Code switch
            {
                "validation" => EConfirmPanicError.InvalidInput,
                "unauthenticated" => EConfirmPanicError.Unauthenticated,
                "forbidden" => EConfirmPanicError.Forbidden,
                "not_found" => EConfirmPanicError.NotFound,
                "conflict" => EConfirmPanicError.Conflict,
                "concurrency" => EConfirmPanicError.ConcurrencyConflict,
                "transition" => EConfirmPanicError.InvalidTransition,
                "already_completed" => EConfirmPanicError.AlreadyCompleted,
                "unexpected" => EConfirmPanicError.UnexpectedError,
                _ => EConfirmPanicError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<AlertFact, ECreateIncidentReportError>> Handle(CreateIncidentReportCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var c = command;
            await business.Handle(new RequireBusinessAccessQuery(c.BusinessId, actor.UserId, false), cancellationToken);
            var a = AlertRecord.Report(c.BusinessId, actor.UserId, c.Kind, c.Category, c.Description, c.Location, c.Latitude, c.Longitude, c.Moment, clock.GetUtcNow());
            await records.AddAsync(a, cancellationToken);
            await Publish(a, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<AlertFact, ECreateIncidentReportError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, ECreateIncidentReportError>.Failure(error.Code switch
            {
                "validation" => ECreateIncidentReportError.InvalidInput,
                "unauthenticated" => ECreateIncidentReportError.Unauthenticated,
                "forbidden" => ECreateIncidentReportError.Forbidden,
                "not_found" => ECreateIncidentReportError.NotFound,
                "conflict" => ECreateIncidentReportError.Conflict,
                "concurrency" => ECreateIncidentReportError.ConcurrencyConflict,
                "transition" => ECreateIncidentReportError.InvalidTransition,
                "already_completed" => ECreateIncidentReportError.AlreadyCompleted,
                "unexpected" => ECreateIncidentReportError.UnexpectedError,
                _ => ECreateIncidentReportError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<AlertFact, EUpdateAlertLocationError>> Handle(UpdateAlertLocationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var location = command.Location;
            var latitude = command.Latitude;
            var longitude = command.Longitude;
            var a = await Own(id, cancellationToken);
            a.UpdateLocation(location, latitude, longitude);
            await Publish(a, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<AlertFact, EUpdateAlertLocationError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, EUpdateAlertLocationError>.Failure(error.Code switch
            {
                "validation" => EUpdateAlertLocationError.InvalidInput,
                "unauthenticated" => EUpdateAlertLocationError.Unauthenticated,
                "forbidden" => EUpdateAlertLocationError.Forbidden,
                "not_found" => EUpdateAlertLocationError.NotFound,
                "conflict" => EUpdateAlertLocationError.Conflict,
                "concurrency" => EUpdateAlertLocationError.ConcurrencyConflict,
                "transition" => EUpdateAlertLocationError.InvalidTransition,
                "already_completed" => EUpdateAlertLocationError.AlreadyCompleted,
                "unexpected" => EUpdateAlertLocationError.UnexpectedError,
                _ => EUpdateAlertLocationError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<AlertFact, EResolvePanicAlertError>> Handle(ResolvePanicAlertCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var a = await Own(id, cancellationToken);
            if (a.Resolve(clock.GetUtcNow()))
            {
                await Publish(a, cancellationToken);
                await work.CompleteAsync(cancellationToken);
            }

            return new Result<AlertFact, EResolvePanicAlertError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, EResolvePanicAlertError>.Failure(error.Code switch
            {
                "validation" => EResolvePanicAlertError.InvalidInput,
                "unauthenticated" => EResolvePanicAlertError.Unauthenticated,
                "forbidden" => EResolvePanicAlertError.Forbidden,
                "not_found" => EResolvePanicAlertError.NotFound,
                "conflict" => EResolvePanicAlertError.Conflict,
                "concurrency" => EResolvePanicAlertError.ConcurrencyConflict,
                "transition" => EResolvePanicAlertError.InvalidTransition,
                "already_completed" => EResolvePanicAlertError.AlreadyCompleted,
                "unexpected" => EResolvePanicAlertError.UnexpectedError,
                _ => EResolvePanicAlertError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<AlertFact, EUpdatePanicReportDraftError>> Handle(UpdatePanicReportDraftCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var c = command;
            var a = await Own(id, cancellationToken);
            Guard.Require(c.Moment is null || c.Moment <= clock.GetUtcNow(), "validation", "moment");
            a.Draft(c.Category, c.Description, c.Location, c.Latitude, c.Longitude, c.Moment);
            await Publish(a, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<AlertFact, EUpdatePanicReportDraftError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, EUpdatePanicReportDraftError>.Failure(error.Code switch
            {
                "validation" => EUpdatePanicReportDraftError.InvalidInput,
                "unauthenticated" => EUpdatePanicReportDraftError.Unauthenticated,
                "forbidden" => EUpdatePanicReportDraftError.Forbidden,
                "not_found" => EUpdatePanicReportDraftError.NotFound,
                "conflict" => EUpdatePanicReportDraftError.Conflict,
                "concurrency" => EUpdatePanicReportDraftError.ConcurrencyConflict,
                "transition" => EUpdatePanicReportDraftError.InvalidTransition,
                "already_completed" => EUpdatePanicReportDraftError.AlreadyCompleted,
                "unexpected" => EUpdatePanicReportDraftError.UnexpectedError,
                _ => EUpdatePanicReportDraftError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<AlertFact, ECompletePanicReportError>> Handle(CompletePanicReportCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var a = await Own(id, cancellationToken);
            a.Complete(clock.GetUtcNow());
            await Publish(a, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<AlertFact, ECompletePanicReportError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, ECompletePanicReportError>.Failure(error.Code switch
            {
                "validation" => ECompletePanicReportError.InvalidInput,
                "unauthenticated" => ECompletePanicReportError.Unauthenticated,
                "forbidden" => ECompletePanicReportError.Forbidden,
                "not_found" => ECompletePanicReportError.NotFound,
                "conflict" => ECompletePanicReportError.Conflict,
                "concurrency" => ECompletePanicReportError.ConcurrencyConflict,
                "transition" => ECompletePanicReportError.InvalidTransition,
                "already_completed" => ECompletePanicReportError.AlreadyCompleted,
                "unexpected" => ECompletePanicReportError.UnexpectedError,
                _ => ECompletePanicReportError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private async Task Publish(AlertRecord a, CancellationToken cancellationToken) => await outbox.AddAsync(new OutboxMessage("alert.changed.v1", ToAlertFact(a)), cancellationToken);
    private async Task<AlertRecord> Own(string id, CancellationToken cancellationToken)
    {
        var a = await records.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
        Guard.Require(a.EmployeeId == actor.UserId, "forbidden");
        await business.Handle(new RequireBusinessAccessQuery(a.BusinessId, actor.UserId, false), cancellationToken);
        return a;
    }

    private static AlertFact ToAlertFact(AlertRecord a) => new(a.Id, a.BusinessId, a.EmployeeId, a.Kind, a.Status, a.ReportStatus, a.Category, a.Description, a.Location, a.Latitude, a.Longitude, a.Moment, a.CreatedAt ?? DateTimeOffset.MinValue, a.ActivatedAt, a.ResolvedAt, a.CompletedAt, a.Revision);
}
