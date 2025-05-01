using AutoMapper;
using SmartReminderSystem.Application.DTOs.ResponseDto;
using SmartReminderSystem.Application.DTOs;
using SmartReminderSystem.Domain.Entities;

namespace SmartReminderSystem.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Patient, PatientRequestDto>().ReverseMap();
            CreateMap<Patient, PatientResponseDto>();

            CreateMap<Appointment, AppointmentRequestDto>().ReverseMap();
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName));
        }
    }
}
