using Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Entities;

public class Technician
{
  public TechnicianId TechnicianId { get; private set; } = null!;
  public ProfileId ProfileId { get; private set; } = null!;

  public IReadOnlyList<ESpecialty> Specialties => _specialtyEntities.Select(e => e.Specialty).ToList().AsReadOnly();
  public int ExperienceYears { get; private set; }
  public ServiceArea ServiceArea { get; private set; } = null!;
  public bool IsIoTCertified { get; private set; }
  public CertificationData? IoTCertification { get; private set; }

  public string AboutMe { get; private set; } = string.Empty;
  public double AverageRating { get; private set; }
  public int ActiveServiceCount { get; private set; }

  private List<PortfolioItem> _portfolioItems = new();
  public IReadOnlyList<PortfolioItem> PortfolioItems => _portfolioItems.AsReadOnly();

  private List<TechnicianSpecialty> _specialtyEntities = new();

  public static Technician Create(
    TechnicianId id,
    ProfileId profileId,
    IEnumerable<ESpecialty> specialties,
    int experienceYears,
    string aboutMe,
    ServiceArea serviceArea)
  {
    var specialtiesList = specialties?.ToList() ?? new List<ESpecialty>();
    if (specialtiesList.Count == 0)
      throw new AtLeastOneSpecialtyRequiredException();

    var technician = new Technician
    {
      TechnicianId    = id,
      ProfileId       = profileId,
      _specialtyEntities = specialtiesList.Select(s => new TechnicianSpecialty(id, s)).ToList(),
      ExperienceYears = experienceYears,
      ServiceArea     = serviceArea,
      AboutMe         = aboutMe,
    };
    return technician;
  }
  
  public void UpdateExperienceYears(int experienceYears)
  {
    ExperienceYears = experienceYears;
  }
  
  public void UpdateSpecialties(IEnumerable<ESpecialty> specialties)
  {
    var list = specialties?.ToList() ?? new List<ESpecialty>();
    if (list.Count == 0)
      throw new AtLeastOneSpecialtyRequiredException();

    _specialtyEntities = list.Select(s => new TechnicianSpecialty(TechnicianId, s)).ToList();
  }
  
  public void UpdateAboutMe(string? aboutMe)
  {
    AboutMe = aboutMe ?? string.Empty;
  }
  
  public void UpdateServiceArea(double lat, double lon, double radiusKm)
  {
    ServiceArea = ServiceArea.FromPointAndRadius(lat, lon, radiusKm);
  }

  public void UpdateRating(int newRating)
  {
    ActiveServiceCount++;
    AverageRating = ((AverageRating * (ActiveServiceCount - 1)) + newRating) / ActiveServiceCount;
  }

  public void AddPortfolioItem(PortfolioItem item)
  {
    _portfolioItems.Add(item);
  }

  public void GrantIoTCertification(CertificationData certification)
  {
    IsIoTCertified = true;
    IoTCertification = certification;
  }

  public void RevokeIoTCertification()
  {
    IsIoTCertified = false;
    IoTCertification = null;
  }
}
