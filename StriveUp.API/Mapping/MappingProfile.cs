using AutoMapper;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Shared.DTOs.Profile;

namespace StriveUp.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // UserActivity <-> DTO
            CreateMap<UserActivity, UserActivityDto>()
                .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.ActivityLikes.Count))
                .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
                .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.Activity.Id))
                .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Activity.Description))
                .ForMember(dest => dest.DurationSeconds, opt => opt.MapFrom(src => src.DurationSeconds));

            // CreateUserActivityDto <-> DTO
            CreateMap<CreateUserActivityDto, UserActivity>();
            CreateMap<UserActivityDto, UserActivity>();

            // GeoPoint <-> DTO
            CreateMap<GeoPoint, GeoPointDto>().ReverseMap();

            // Comment  <-> DTO
            CreateMap<ActivityComment, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            // Activity <-> DTO
            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.MeasurementType, opt => opt.MapFrom(src => src.Config.MeasurementType))
                .ForMember(dest => dest.ElevationRelevant, opt => opt.MapFrom(src => src.Config.ElevationRelevant))
                .ForMember(dest => dest.IndoorCapable, opt => opt.MapFrom(src => src.Config.IndoorCapable));


            // Medal <-> DTO
            CreateMap<Medal, MedalDto>();

            // MedalEarned <-> DTO
            CreateMap<MedalEarned, MedalDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Medal.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Medal.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Medal.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Medal.ImageUrl))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Medal.Level))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.Medal.TargetValue))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Medal.Frequency))
                .ForMember(dest => dest.DateEarned, opt => opt.MapFrom(src => src.DateEarned));

            // AppUser <-> DTO
            CreateMap<AppUser, UserProfileDto>()
                .ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.UserActivities))
                .ForMember(dest => dest.Medals, opt => opt.MapFrom(src => src.MedalsEarned));

            CreateMap<EditUserProfileDto, AppUser>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ActivityHr, ActivityHrDto>();
            CreateMap<ActivityHrDto, ActivityHr>();
        }
    }
}
