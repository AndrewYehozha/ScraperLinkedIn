using AutoMapper;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;

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
                        cfg.CreateMap<Company, CompanyImportViewModel>()
                           .ForMember(ci => ci.LinkedIn, opt => opt.MapFrom(c => c.LinkedInURL))
                           .ForMember(ci => ci.Founders, opt => opt.MapFrom(c => c.Founders))
                           .ForMember(ci => ci.HeadquartersLocation, opt => opt.MapFrom(c => c.HeadquartersLocation))
                           .ForMember(ci => ci.OrganizationName, opt => opt.MapFrom(c => c.OrganizationName))
                           .ForMember(ci => ci.OrganizationNameURL, opt => opt.MapFrom(c => c.OrganizationURL))
                           .ForMember(ci => ci.Website, opt => opt.MapFrom(c => c.Website));

                        cfg.CreateMap<CompanyImportViewModel, Company>()
                           .ForMember(c => c.LinkedInURL, opt => opt.MapFrom(ci => ci.LinkedIn))
                           .ForMember(c => c.Founders, opt => opt.MapFrom(ci => ci.Founders))
                           .ForMember(c => c.HeadquartersLocation, opt => opt.MapFrom(ci => ci.HeadquartersLocation))
                           .ForMember(c => c.OrganizationName, opt => opt.MapFrom(ci => ci.OrganizationName))
                           .ForMember(c => c.OrganizationURL, opt => opt.MapFrom(ci => ci.OrganizationNameURL))
                           .ForMember(c => c.Website, opt => opt.MapFrom(ci => ci.Website));

                        cfg.CreateMap<Company, CompanyEmployeesViewModel>()
                           .ForMember(ce => ce.Id, opt => opt.MapFrom(c => c.Id))
                           .ForMember(ce => ce.LinkedIn, opt => opt.MapFrom(c => c.LinkedInURL))
                           .ForMember(ce => ce.Founders, opt => opt.MapFrom(c => c.Founders))
                           .ForMember(ce => ce.HeadquartersLocation, opt => opt.MapFrom(c => c.HeadquartersLocation))
                           .ForMember(ce => ce.OrganizationName, opt => opt.MapFrom(c => c.OrganizationName))
                           .ForMember(ce => ce.OrganizationNameURL, opt => opt.MapFrom(c => c.OrganizationURL))
                           .ForMember(ce => ce.Website, opt => opt.MapFrom(c => c.Website))
                           .ForMember(ce => ce.LogoCompanyUrl, opt => opt.MapFrom(c => c.LogoUrl))
                           .ForMember(ce => ce.Specialties, opt => opt.MapFrom(c => c.Specialties))
                           .ForMember(ce => ce.ExecutionStatus, opt => opt.MapFrom(c => (ExecutionStatuses)c.ExecutionStatusID));
                        //.ForMember(ce => ce.Employees, opt => opt.MapFrom(c => c.Profiles != null ? Instance.Map<IEnumerable<Profile>, IEnumerable<ProfilesViewModel>>(c.Profiles) : new List<ProfilesViewModel>()));

                        cfg.CreateMap<CompanyEmployeesViewModel, Company>()
                           .ForMember(c => c.Id, opt => opt.MapFrom(ce => ce.Id))
                           .ForMember(c => c.LinkedInURL, opt => opt.MapFrom(ce => ce.LinkedIn))
                           .ForMember(c => c.Founders, opt => opt.MapFrom(ce => ce.Founders))
                           .ForMember(c => c.HeadquartersLocation, opt => opt.MapFrom(ce => ce.HeadquartersLocation))
                           .ForMember(c => c.OrganizationName, opt => opt.MapFrom(ce => ce.OrganizationName))
                           .ForMember(c => c.OrganizationURL, opt => opt.MapFrom(ce => ce.OrganizationNameURL))
                           .ForMember(c => c.Website, opt => opt.MapFrom(ce => ce.Website))
                           .ForMember(c => c.LogoUrl, opt => opt.MapFrom(ce => ce.LogoCompanyUrl))
                           .ForMember(c => c.Specialties, opt => opt.MapFrom(ce => ce.Specialties))
                           .ForMember(c => c.ExecutionStatusID, opt => opt.MapFrom(ce => (int)ce.ExecutionStatus))
                           .ForMember(c => c.Profiles, opt => opt.Ignore())
                           .ForMember(c => c.ExecutionStatus, opt => opt.Ignore());
                        //.ForMember(c => c.Profiles, opt => opt.MapFrom(ce => Instance.Map<IEnumerable<ProfilesViewModel>, IEnumerable<Profile> > (ce.Employees)));

                        cfg.CreateMap<Profile, ProfileViewModel>()
                           .ForMember(c => c.Id, opt => opt.MapFrom(ce => ce.Id))
                           .ForMember(c => c.FirstName, opt => opt.MapFrom(ce => ce.FirstName))
                           .ForMember(c => c.LastName, opt => opt.MapFrom(ce => ce.LastName))
                           .ForMember(c => c.FullName, opt => opt.MapFrom(ce => ce.FullName))
                           .ForMember(c => c.Job, opt => opt.MapFrom(ce => ce.Job))
                           .ForMember(c => c.ProfileUrl, opt => opt.MapFrom(ce => ce.ProfileUrl))
                           .ForMember(c => c.AllSkills, opt => opt.MapFrom(ce => ce.AllSkills))
                           .ForMember(c => c.ExecutionStatusID, opt => opt.MapFrom(ce => (ExecutionStatuses)ce.ExecutionStatusID))
                           .ForMember(c => c.CompanyID, opt => opt.MapFrom(ce => ce.CompanyID))
                           .ForMember(c => c.ProfileStatusID, opt => opt.MapFrom(ce => ce.ProfileStatusID))
                           .ForMember(c => c.Skills, opt => opt.Ignore());

                        cfg.CreateMap<ProfileViewModel, Profile>()
                           .ForMember(c => c.Id, opt => opt.MapFrom(ce => ce.Id))
                           .ForMember(c => c.FirstName, opt => opt.MapFrom(ce => ce.FirstName))
                           .ForMember(c => c.LastName, opt => opt.MapFrom(ce => ce.LastName))
                           .ForMember(c => c.FullName, opt => opt.MapFrom(ce => ce.FullName))
                           .ForMember(c => c.Job, opt => opt.MapFrom(ce => ce.Job))
                           .ForMember(c => c.ProfileUrl, opt => opt.MapFrom(ce => ce.ProfileUrl))
                           .ForMember(c => c.AllSkills, opt => opt.MapFrom(ce => ce.AllSkills))
                           .ForMember(c => c.ExecutionStatusID, opt => opt.MapFrom(ce => (int)ce.ExecutionStatusID))
                           .ForMember(c => c.CompanyID, opt => opt.MapFrom(ce => ce.CompanyID))
                           .ForMember(c => c.ProfileStatusID, opt => opt.MapFrom(ce => ce.ProfileStatusID))
                           .ForMember(c => c.Company, opt => opt.Ignore())
                           .ForMember(c => c.ExecutionStatus, opt => opt.Ignore())
                           .ForMember(c => c.ProfileStatus, opt => opt.Ignore());
                    });
                    _instance = config.CreateMapper();
                }

                return _instance;
            }
        }
    }
}