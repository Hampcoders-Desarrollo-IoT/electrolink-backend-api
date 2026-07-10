using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Hampcoders.Electrolink.API.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class ProfileRepository(AppDbContext context)
  : BaseRepository<Profile, ProfileId>(context), IProfileRepository
{
  public async Task<IEnumerable<Profile>> FindByRoleAsync(EBusinessRole role)
  {
    return await Context.Set<Profile>()
      .Include(p => p.Homeowner)
      .Include(p => p.Technician)
      .Include(p => p.Company)
      .Where(p => p.BusinessRole != null && p.BusinessRole.Value == role)
      .ToListAsync();
  }

  public async Task<IEnumerable<(string technicianId, string profileId, string fullName)>> 
    FindTechniciansInAreaAsync(double lat, double lon)
  {
    var point = new Point(lon, lat) { SRID = 4326 };

    var data = await Context.Set<Profile>()
      .Where(p => p.Status == EProfileStatus.Active 
                  && p.BusinessRole == EBusinessRole.Technician 
                  && p.Technician != null 
                  && p.Technician.ServiceArea.Area.Contains(point))
      .Select(p => new {
        TechnicianId = p.Technician!.TechnicianId.Value,
        ProfileId = p.ProfileId.Value,
        FullName = p.PersonalData!.FullName
      })
      .AsNoTracking()
      .ToListAsync();

    var results = data.Select(p => (
      technicianId: p.TechnicianId,
      profileId: p.ProfileId,
      fullName: p.FullName
    )).ToList();

    return results;
  }

  public async Task<(string ProfileId, string ProfileStatus, string? BusinessRole, string? RoleSubjectId, string? SubscriptionTier)?> FindProfileClaimsByUserIdAsync(UserId userId)
  {
    var result = await Context.Set<Profile>()
      .Where(p => p.UserId == userId)
      .Select(p => new
      {
        ProfileId = p.ProfileId.Value,
        ProfileStatus = p.Status,
        Role = p.BusinessRole,
        SubscriptionTier = p.SubscriptionTier,
        HomeownerId = p.Homeowner != null ? p.Homeowner.HomeownerId.Value : null,
        TechnicianId = p.Technician != null ? p.Technician.TechnicianId.Value : null,
        CompanyId = p.Company != null ? p.Company.CompanyId.Value : null
      })
      .FirstOrDefaultAsync();
    
    return result == null ? null : (
      result.ProfileId,
      result.ProfileStatus.ToString(),
      result.Role?.ToString(),
      result.Role == EBusinessRole.HomeOwner ? result.HomeownerId 
        : result.Role == EBusinessRole.Technician ? result.TechnicianId 
        : result.Role == EBusinessRole.Company ? result.CompanyId 
        : null,
      result.SubscriptionTier
    );
  }

  public async Task<bool> ExistsByUserIdAsync(UserId userId)
  {
    return await Context.Set<Profile>().AnyAsync(p => p.UserId == userId);
  }
  
  public async Task<bool> DniExistsAsync(Dni dni, ProfileId? excludeProfileId = null)
  {
    return await Context.Set<Profile>()
      .AnyAsync(p => p.PersonalData != null && p.PersonalData.Dni == dni && (excludeProfileId == null || p.ProfileId != excludeProfileId));
  }

  public async Task<bool> TaxIdExistsAsync(TaxId taxId, ProfileId? excludeProfileId = null)
  {
    return await Context.Set<Profile>()
      .AnyAsync(p => p.Company != null && p.Company.CompanyData.TaxId == taxId && (excludeProfileId == null || p.ProfileId != excludeProfileId));
  }

  public async Task<bool> IsHomeownerActiveAsync(HomeownerId homeownerId)
  {
    return await Context.Set<Profile>()
      .AnyAsync(p => p.Homeowner != null && p.Homeowner.HomeownerId == homeownerId);
  }

  public async Task<bool> IsCompanyActiveAsync(CompanyId companyId)
  {
    return await Context.Set<Profile>()
      .AnyAsync(p => p.Company != null && p.Company.CompanyId == companyId);
  }

  public async Task<Profile?> FindByTechnicianIdAsync(TechnicianId technicianId)
  {
    return await Context.Set<Profile>()
      .Include(p => p.Technician)
      .Include(p => p.Company)
      .FirstOrDefaultAsync(p => p.Technician != null && p.Technician.TechnicianId == technicianId);
  }

  public async Task<Profile?> FindByCompanyIdAsync(CompanyId companyId)
  {
    return await Context.Set<Profile>()
      .Include(p => p.Company)
      .FirstOrDefaultAsync(p => p.Company != null && p.Company.CompanyId == companyId);
  }

  public async Task<Profile?> FindByHomeownerIdAsync(HomeownerId homeownerId)
  {
    return await Context.Set<Profile>()
      .Include(p => p.Homeowner)
      .FirstOrDefaultAsync(p => p.Homeowner != null && p.Homeowner.HomeownerId == homeownerId);
  }

  public override async Task<Profile?> FindByIdAsync(ProfileId id)
  {
    return await Context.Set<Profile>()
      .Include(p => p.Homeowner)
      .Include(p => p.Technician)
      .Include(p => p.Company)
      .FirstOrDefaultAsync(p => p.ProfileId == id);
  }

  public async Task<Profile?> FindByUserIdAsync(UserId userId)
  {
    return await Context.Set<Profile>()
      .Include(p => p.Homeowner)
      .Include(p => p.Technician)
      .Include(p => p.Company)
      .FirstOrDefaultAsync(p => p.UserId == userId);
  }
}
