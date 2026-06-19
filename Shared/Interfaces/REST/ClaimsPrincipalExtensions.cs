using System.Security.Claims;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Shared.Interfaces.REST;

/// <summary>
/// Extensions to extract claims from ElectroLink JWT.
/// 
/// Payload example:
///   nameidentifier → userId  (us-{guid})
///   emailaddress   → email
///   profileId      → prof-{guid}
///   businessRole   → "Technician" | "HomeOwner" | "Company"
///   roleSubjectId  → tech-{guid} | ho-{guid} | comp-{guid}
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new UnauthorizedAccessException("userId claim not found.");

    public static string GetEmail(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Email)
           ?? throw new UnauthorizedAccessException("email claim not found.");

    public static string GetProfileId(this ClaimsPrincipal user)
        => user.FindFirstValue("profileId")
           ?? throw new UnauthorizedAccessException("profileId claim not found.");

    public static string GetBusinessRole(this ClaimsPrincipal user)
        => user.FindFirstValue("businessRole")
           ?? throw new UnauthorizedAccessException("businessRole claim not found.");

    /// <summary>
    /// Returns technicianId, homeownerId or companyId according the authenticated user business role.
    /// </summary>
    public static string GetRoleSubjectId(this ClaimsPrincipal user)
        => user.FindFirstValue("roleSubjectId")
           ?? throw new UnauthorizedAccessException("roleSubjectId claim not found.");

    // ── Helpers de rol ────────────────────────────────────────────────────
    public static bool IsTechnician(this ClaimsPrincipal user)
        => user.FindFirstValue("businessRole") == "Technician";

    public static bool IsHomeOwner(this ClaimsPrincipal user)
        => user.FindFirstValue("businessRole") == "HomeOwner";

    public static bool IsCompany(this ClaimsPrincipal user)
        => user.FindFirstValue("businessRole") == "Company";

    /// <summary>
    /// Returns TechnicianId. Throws if User is not Technician.
    /// </summary>
    public static string GetTechnicianId(this ClaimsPrincipal user)
    {
        if (!user.IsTechnician())
            throw new UnauthorizedAccessException("Current user is not a Technician.");
        return user.GetRoleSubjectId();
    }

    /// <summary>
    /// Returns el homeownerId. Throws if User is not HomeOwner.
    /// </summary>
    public static string GetHomeOwnerId(this ClaimsPrincipal user)
    {
        if (!user.IsHomeOwner())
            throw new UnauthorizedAccessException("Current user is not a HomeOwner.");
        return user.GetRoleSubjectId();
    }

    /// <summary>
    /// Returns ClientIdentity from JWT claims (businessRole + roleSubjectId).
    /// Works for HomeOwner and Company roles. Throws for Technician.
    /// </summary>
    public static ClientIdentity GetClientIdentity(this ClaimsPrincipal user)
    {
        var role = GetBusinessRole(user);
        var subjectId = GetRoleSubjectId(user);
        return ClientIdentity.From(role, subjectId);
    }

}