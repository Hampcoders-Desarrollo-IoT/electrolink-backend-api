using Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Entities;

public class HomeOwner
{
  public HomeownerId HomeownerId { get; private set; }
  public ProfileId ProfileId { get; private set; }
  public EContactTime PreferredContactTime { get; private set; }
  public CommunicationPreferences CommunicationPreferences { get; private set; }
  public EmergencyContact? EmergencyContact { get; private set; }
  public double AverageRating { get; private set; }
  public int ActiveServiceCount { get; private set; }

  public void UpdateRating(int newRating)
  {
      ActiveServiceCount++;
      AverageRating = ((AverageRating * (ActiveServiceCount - 1)) + newRating) / ActiveServiceCount;
  }

  private HomeOwner() { }

  public static HomeOwner Create(HomeownerId id, ProfileId profileId, EContactTime preferredContactTime,
    CommunicationPreferences communicationPreferences, EmergencyContact? emergencyContact = null)
  {
    return new HomeOwner
    {
      HomeownerId = id,
      ProfileId = profileId,
      PreferredContactTime = preferredContactTime,
      CommunicationPreferences = communicationPreferences,
      EmergencyContact = emergencyContact
    };
  }

  public void UpdatePreferences(CommunicationPreferences preferences)
  {
    if (!preferences.HasAtLeastOneChannelActive())
      throw new AtLeastOneNotificationChannelRequiredException();

    CommunicationPreferences = preferences;
  }

  public void UpdateCommunicationPreferences(CommunicationPreferences preferences)
  {
    UpdatePreferences(preferences);
  }

  public void UpdatePreferredContactTime(EContactTime preferredContactTime)
  {
    PreferredContactTime = preferredContactTime;
  }

  public void UpdateEmergencyContact(EmergencyContact? emergencyContact)
  {
    EmergencyContact = emergencyContact;
  }

  public void UpdateContactDetails(EContactTime preferredContactTime, CommunicationPreferences preferences, EmergencyContact? emergencyContact = null)
  {
    if (!preferences.HasAtLeastOneChannelActive())
      throw new AtLeastOneNotificationChannelRequiredException();

    PreferredContactTime = preferredContactTime;
    CommunicationPreferences = preferences;
    EmergencyContact = emergencyContact;
  }
}
