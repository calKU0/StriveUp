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
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.ActivityLikes.Count))
                .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.Activity.Id))
                .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Activity.Description))
                .ForMember(dest => dest.DurationSeconds, opt => opt.MapFrom(src => src.DurationSeconds));

            // CreateUserActivityDto <-> DTO
            CreateMap<CreateUserActivityDto, UserActivity>();

            CreateMap<UserActivityDto, UserActivity>();

            // GeoPoint <-> DTO
            CreateMap<GeoPoint, GeoPointDto>().ReverseMap();
            CreateMap<ActivityElevation, ActivityElevationDto>();
            CreateMap<ActivityElevation, ActivityElevationDto>().ReverseMap();

            CreateMap<SegmentConfig, SegmentDto>();

            // Comment  <-> DTO
            CreateMap<ActivityComment, ActivityCommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            // Activity <-> DTO
            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.MeasurementType, opt => opt.MapFrom(src => src.Config.MeasurementType))
                .ForMember(dest => dest.ElevationRelevant, opt => opt.MapFrom(src => src.Config.ElevationRelevant))
                .ForMember(dest => dest.Indoor, opt => opt.MapFrom(src => src.Config.Indoor))
                .ForMember(dest => dest.UseHeartRate, opt => opt.MapFrom(src => src.Config.UseHeartRate))
                .ForMember(dest => dest.SpeedRelevant, opt => opt.MapFrom(src => src.Config.SpeedRelevant))
                .ForMember(dest => dest.DistanceRelevant, opt => opt.MapFrom(src => src.Config.DistanceRelevant))
                .ForMember(dest => dest.Segments, opt => opt.MapFrom(src => src.SegmentConfigs));


            // Medal <-> DTO
            CreateMap<Medal, MedalDto>();

            // MedalEarned <-> DTO
            CreateMap<MedalEarned, MedalDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Medal.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Medal.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Medal.Description))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Medal.Level))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.Medal.TargetValue))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Medal.Frequency))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Medal.Points))
                .ForMember(dest => dest.DateEarned, opt => opt.MapFrom(src => src.DateEarned))
                .ForMember(dest => dest.ProgressPercent, opt => opt.Ignore())
                .ForMember(dest => dest.DistanceToEarn, opt => opt.Ignore());

            // AppUser <-> DTO
            CreateMap<AppUser, UserProfileDto>()
                .ForMember(dest => dest.Medals, opt => opt.MapFrom(src => src.MedalsEarned))
                .ForMember(dest => dest.LevelNumber, opt => opt.MapFrom(src => src.Level.LevelNumber))
                .ForMember(dest => dest.LevelTotalXP, opt => opt.MapFrom(src => src.Level.TotalXP))
                .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers.Select(f => f.Follower)))
                .ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Following.Select(f => f.Followed)));

            CreateMap<EditUserProfileDto, AppUser>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ActivityHrDto, ActivityHr>();
            CreateMap<ActivityHrDto, ActivityHr>().ReverseMap();

            CreateMap<ActivitySpeedDto, ActivitySpeed>();
            CreateMap<ActivitySpeedDto, ActivitySpeed>().ReverseMap();

            CreateMap<AppUser, FollowDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.IsFollowed, opt => opt.Ignore());

            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.ActorName, opt => opt.Ignore())
                .ForMember(dest => dest.ActorAvatar, opt => opt.Ignore());

            CreateMap<CreateNotificationDto, Notification>();

            CreateMap<AppUser, SimpleUserDto>();

            CreateMap<UserSynchro, UserSynchroDto>()
                .ForMember(dest => dest.SynchroProviderId, opt => opt.MapFrom(src => src.SynchroId))
                .ForMember(dest => dest.SynchroProviderName, opt => opt.MapFrom(src => src.SynchroProvider.Name))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.SynchroProvider.IconUrl));

            //CreateMap<CreateUserSynchroDto, UserSynchro>()
            //    .ForMember(dest => dest.SynchroId, opt => opt.MapFrom(src => src.SynchroProviderId))
            //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            //    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<SynchroProvider, UserSynchroDto>()
                .ForMember(dest => dest.SynchroProviderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SynchroProviderName, opt => opt.MapFrom(src => src.Name));

            CreateMap<UpdateUserSynchroDto, UserSynchro>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateTokenDto, UserSynchro>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.RefreshToken, opt => opt.Condition(src => src.RefreshToken != null));
        }
    }
}
