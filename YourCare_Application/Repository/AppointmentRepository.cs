using Microsoft.EntityFrameworkCore;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        ApplicationDbContext _context;
        ITimetableRepository _timetableRepository;
        public AppointmentRepository(
            ApplicationDbContext context
            , ITimetableRepository timetableRepository)
        {
            _context = context;
            _timetableRepository = timetableRepository;
        }

        public async Task Add(Appointment appointment)
        {
            try
            {
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                throw;
            }
        }

        public async Task CancelAppointment(Appointment appointment)
        {
            try
            {
                await Update(appointment);

                var timetable = appointment.TimeTable;
                if (timetable != null)
                {
                    timetable.IsAvailable = true;
                    await _timetableRepository.Update(timetable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                throw;
            }
        }

        public async Task Delete(Appointment appointment)
        {
            try
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                throw;
            }
        }

        public async Task<List<Appointment>> GetAll()
        {
            try
            {
                return await _context.Appointments
               .Include(x => x.Doctor)
               .Include(x => x.TimeTable)
               .Include(x => x.PatientProfile)
               .Include(x => x.CreatedByUser)
               .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAll:" + ex.Message + " - " + ex.StackTrace);
                throw;
            }
        }

        public async Task<List<Appointment>> GetAllByDoctorId(string doctorId)
        {
            var result = new List<Appointment>();
            try
            {
                result = await _context.Appointments
               .Include(x => x.Doctor)
               .Include(x => x.TimeTable)
               .Include(x => x.PatientProfile)
               .Where(x => x.DoctorID == doctorId && x.Status != Constants.StatusConstant.Status.Đã_hủy).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<List<Appointment>> GetAllByUserId(string userId)
        {
            var result = new List<Appointment>();
            try
            {
                result = await _context.Appointments
                .Include(x => x.PatientProfile)
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .Where(x => x.CreatedBy == userId).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<Appointment> GetById(int id)
        {
            return await _context.Appointments
                .Include(x => x.PatientProfile)
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Appointment appointment)
        {
            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                throw;
            }
        }
    }
}
