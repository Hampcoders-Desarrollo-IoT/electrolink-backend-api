using Microsoft.Extensions.DependencyInjection;

namespace Hampcoders.Electrolink.API.Shared.Infrastructure.Authorization;

public static class AccessPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string MakerForAssignedService = "MakerForAssignedService";
    public const string CompanyOwnerDashboard = "CompanyOwnerDashboard";
    public const string TechnicianForAssignedService = "TechnicianForAssignedService";
    public const string HomeownerBasic = "HomeownerBasic";

    public static IServiceCollection AddAccessPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdminOnly, policy =>
                policy.RequireClaim("access_role", "Admin", "SuperAdmin"));

            options.AddPolicy(CompanyOwnerDashboard, policy =>
                policy.RequireClaim("businessRole", "Company"));

            options.AddPolicy(HomeownerBasic, policy =>
                policy.RequireClaim("businessRole", "HomeOwner"));

            options.AddPolicy(MakerForAssignedService, policy =>
                policy.RequireClaim("access_role", "Maker"));

            options.AddPolicy(TechnicianForAssignedService, policy =>
                policy.RequireClaim("businessRole", "Technician"));
        });

        return services;
    }
}
