using AutoMapper;
using ShiftDock.Application.DTOs.Assignments;
using ShiftDock.Application.DTOs.Organizations;
using ShiftDock.Application.DTOs.Projects;
using ShiftDock.Application.DTOs.Users;
using ShiftDock.Domain.Entities;
using System.Text.Json;

namespace ShiftDock.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserResponse>();
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Organization mappings
        CreateMap<Organization, OrganizationResponse>();
        CreateMap<CreateOrganizationRequest, Organization>();
        CreateMap<UpdateOrganizationRequest, Organization>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Project mappings
        CreateMap<Project, ProjectResponse>();
        CreateMap<CreateProjectRequest, Project>();
        CreateMap<UpdateProjectRequest, Project>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Shift mappings
        CreateMap<Shift, ShiftDto>();

        // Worker Assignment mappings
        CreateMap<WorkerAssignment, WorkerAssignmentResponse>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.User.Phone));

        CreateMap<CreateAssignmentRequest, WorkerAssignment>();
    }
}
