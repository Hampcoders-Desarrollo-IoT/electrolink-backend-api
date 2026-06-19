namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

/// <summary>
/// Represents the lifecycle state transitions of a profile.
/// Implements the State pattern — each method transitions the profile
/// to the next valid status.
/// </summary>
public interface ICompletable
{
    EProfileStatus Status { get; }

    /// <summary>
    /// Transition from Active to Deactivated.
    /// </summary>
    void Deactivate();

    /// <summary>
    /// Transition from Deactivated to Active.
    /// </summary>
    void Reactivate();

    /// <summary>
    /// Transition from Active to Suspended (payment failure).
    /// </summary>
    void Suspend();
}
