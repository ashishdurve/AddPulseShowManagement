using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public class ShowRepository : BaseRepository<MSSQLDbContext>, IShowRepository
    {
        private readonly MSSQLDbContext _context;
        public ShowRepository(MSSQLDbContext context) : base(context)
        {
            _context = context;
        }
        public new MSSQLDbContext dbContext() => base.dbContext;

        public List<Shows> GetShowList(string startDate = "", string endDate = "")
        {
            try
            {
                var shows = this.ExecuteRows<Shows>(CommandType.StoredProcedure, "APS_01_GetShowsList",                    
                    ("@StartDate", startDate),
                    ("@EndDate", endDate)) ?? new List<Shows>();
                return shows;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Shows> GetShows()
        {
            try
            {    
                List<Shows> shows = _context.Shows.Where(y => y.IsActive == true).ToList() ?? new List<Shows>();
                return shows;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<Teams> GetTeamsOfShow(long showID=0)
        {
            try
            {
                List<Teams> team = _context.Teams.Where(y => (y.IsActive == true) && (y.ShowID == showID)).ToList() ?? new List<Teams>();

                return team;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Shows SaveUpdateShow(Shows show)
        {
            try
            {
                var showChk = this.dbContext().Shows.Find(show.ShowID);
                Shows show1 = showChk ?? new Shows();

                if (showChk != null)
                {
                    if (string.IsNullOrEmpty(show.CreatedDate.ToString()))
                    {
                        show.CreatedDate = showChk.CreatedDate;
                    }

                    if (string.IsNullOrEmpty(show.CreatedBy.ToString()))
                    {
                        show.CreatedBy = showChk.CreatedBy;
                    }

                    var local = this.dbContext().Set<Shows>()
                                        .Local
                                        .FirstOrDefault(entry => entry.ShowID.Equals(show.ShowID));

                    // check if local is not null  
                    if (local != null)
                    {
                        // detach
                        this.dbContext().Entry(local).State = EntityState.Detached;
                    }

                    this.dbContext().Entry(show).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    this.dbContext().Shows.Add(show);
                }
                this.dbContext().SaveChanges();

                return show;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
