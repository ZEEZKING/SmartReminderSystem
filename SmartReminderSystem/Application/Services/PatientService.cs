using AutoMapper;
using SmartReminderSystem.Application.DTOs.ResponseDto;
using SmartReminderSystem.Application.DTOs;
using SmartReminderSystem.Application.Interfaces;
using SmartReminderSystem.Domain.Entities;
using SmartReminderSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartReminderSystem.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PatientService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PatientResponseDto> CreatePatientAsync(PatientRequestDto patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return _mapper.Map<PatientResponseDto>(patient);
        }

        public async Task<PatientResponseDto> GetPatientByIdAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            return patient == null ? null : _mapper.Map<PatientResponseDto>(patient);
        }

        public async Task<IEnumerable<PatientResponseDto>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
        }

        public async Task<bool> DeletePatientAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
