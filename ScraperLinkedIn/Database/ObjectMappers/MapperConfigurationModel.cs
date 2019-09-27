using AutoMapper;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System.Collections.Generic;

namespace ScraperLinkedIn.Database.ObjectMappers
{
    public static class MapperConfigurationModel
    {
        private static IMapper _instance;
        public static IMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Company, CompanyEmployeesViewModel>()
                           .ForMember(ce => ce.LinkedIn, opt => opt.MapFrom(c => c.LinkedInURL))
                           .ForMember(ce => ce.OrganizationNameURL, opt => opt.MapFrom(c => c.OrganizationURL))
                           .ForMember(ce => ce.LogoCompanyUrl, opt => opt.MapFrom(c => c.LogoUrl))
                           .ForMember(ce => ce.Categories, opt => opt.MapFrom(c => c.Industry))
                           .ForMember(ce => ce.NumberofEmployees, opt => opt.MapFrom(c => c.AmountEmployees))
                           .ForMember(ce => ce.ExecutionStatus, opt => opt.MapFrom(c => (ExecutionStatuses)c.ExecutionStatusID))
                           .ForMember(ce => ce.Employees, opt => opt.MapFrom(c => c.Profiles != null ? Instance.Map<IEnumerable<Profile>, IEnumerable<ProfileViewModel>>(c.Profiles) : new List<ProfileViewModel>()));

                        cfg.CreateMap<CompanyEmployeesViewModel, Company>()
                           .ForMember(c => c.LinkedInURL, opt => opt.MapFrom(ce => ce.LinkedIn))
                           .ForMember(c => c.OrganizationURL, opt => opt.MapFrom(ce => ce.OrganizationNameURL))
                           .ForMember(c => c.LogoUrl, opt => opt.MapFrom(ce => ce.LogoCompanyUrl))
                           .ForMember(ce => ce.Industry, opt => opt.MapFrom(c => c.Categories))
                           .ForMember(ce => ce.AmountEmployees, opt => opt.MapFrom(c => c.NumberofEmployees))
                           .ForMember(c => c.ExecutionStatusID, opt => opt.MapFrom(ce => (int)ce.ExecutionStatus))
                           .ForMember(c => c.Profiles, opt => opt.Ignore())
                           .ForMember(c => c.ExecutionStatus, opt => opt.Ignore());

                        cfg.CreateMap<Profile, ProfileViewModel>()
                           .ForMember(c => c.ExecutionStatus, opt => opt.MapFrom(ce => (ExecutionStatuses)ce.ExecutionStatusID))
                           .ForMember(c => c.ProfileStatus, opt => opt.MapFrom(ce => (ProfileStatuses)ce.ProfileStatusID))
                           .ForMember(c => c.CompanyName, opt => opt.MapFrom(ce => ce.Company.OrganizationName))
                           .ForMember(c => c.Skills, opt => opt.Ignore());

                        cfg.CreateMap<ProfileViewModel, Profile>()
                           .ForMember(c => c.ExecutionStatusID, opt => opt.MapFrom(ce => (int)ce.ExecutionStatus))
                           .ForMember(c => c.ProfileStatusID, opt => opt.MapFrom(ce => (int)ce.ProfileStatus))
                           .ForMember(c => c.Company, opt => opt.Ignore())
                           .ForMember(c => c.ExecutionStatus, opt => opt.Ignore())
                           .ForMember(c => c.ProfileStatus, opt => opt.Ignore());

                        cfg.CreateMap<SuitableProfile, ResultViewModel>();

                        cfg.CreateMap<ResultViewModel, SuitableProfile>()
                        .ForMember(c => c.DateTimeCreation, opt => opt.Ignore());
                        
                        cfg.CreateMap<Setting, SettingsViewModel>()
                           .ForMember(c => c.IntervalType, opt => opt.MapFrom(ce => (IntervalTypes)ce.IntervalType))
                           .ForMember(c => c.ScraperStatus, opt => opt.MapFrom(ce => (ScraperStatuses)ce.ScraperStatusID));

                        cfg.CreateMap<SettingsViewModel, Setting>()
                           .ForMember(c => c.IntervalType, opt => opt.MapFrom(ce => (int)ce.IntervalType))
                           .ForMember(c => c.ScraperStatusID, opt => opt.MapFrom(ce => (int)ce.ScraperStatus))
                           .ForMember(c => c.ScraperStatus, opt => opt.Ignore());
                    });
                    _instance = config.CreateMapper();
                }

                return _instance;
            }
        }
    }
}