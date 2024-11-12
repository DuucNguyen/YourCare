using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Xml;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Repository
{
    public class TimetableRepository : ITimetableRepository
    {

        private ApplicationDbContext _context;


        public TimetableRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Add(Timetable timetable)
        {
            _context.Timetables.Add(timetable);
            _context.SaveChanges();
        }

        public async Task AddRange(List<Timetable> timetables)
        {
            try
            {
                // get properties
                var doctorIds = timetables.Select(t => t.DoctorID).Distinct();
                var dates = timetables.Select(t => t.Date).Distinct();
                var startTimes = timetables.Select(t => t.StartTime).Distinct();
                var EndTimes = timetables.Select(t => t.EndTime).Distinct();

                // Get existing
                var existingTimetables = await _context.Timetables
                    .Where(dbTimetable =>
                    doctorIds.Contains(dbTimetable.DoctorID)
                    && dates.Contains(dbTimetable.Date)
                    && startTimes.Contains(dbTimetable.StartTime)
                    && EndTimes.Contains(dbTimetable.EndTime)
                    ).ToListAsync();

                var newTimetables = new List<Timetable>();
                foreach (var timetable in timetables)
                {
                    var isExists = existingTimetables.Any(dbTimetable =>
                        dbTimetable.DoctorID == timetable.DoctorID &&
                        dbTimetable.Date == timetable.Date &&
                        dbTimetable.StartTime == timetable.StartTime &&
                        dbTimetable.EndTime == timetable.EndTime);

                    if (!isExists)
                    {
                        newTimetables.Add(timetable);
                    }
                }

                //Add new only
                if (newTimetables.Any())
                {
                    await _context.BulkInsertAsync(newTimetables);
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                Console.WriteLine("Dublicate");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task Deactivate(int timeTaleId)
        {
            try
            {
                var find = _context.Timetables.FirstOrDefault(x => x.Id == timeTaleId);
                if (find == null) throw new Exception("TimeTable not Found");

                find.IsAvailable = false;

                _context.Timetables.Update(find);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} - Where {ex.StackTrace}");
            }
        }

        public async Task Delete(Timetable timetable)
        {
            _context.Timetables.Remove(timetable);
            _context.SaveChanges();
        }

        public async Task<List<Timetable>> GetAll()
        {
            return _context.Timetables.ToList();
        }

        public async Task<Timetable> GetById(int id)
        {
            return await _context.Timetables.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Timetable>> GetInRange(string doctorID, DateTime startDate, DateTime endDate)
        {
            return await _context.Timetables
                .Where(x => x.DoctorID == doctorID
                && x.Date.Date >= startDate
                && x.Date.Date <= endDate)
                .ToListAsync();
        }



        public async Task Update(Timetable timetable)
        {
            try
            {
                _context.Entry(timetable).State = EntityState.Modified;

                _context.Timetables.Update(timetable);
                _context.SaveChanges();
            }
            catch (Exception)
            {

            }
        }


    }
}
