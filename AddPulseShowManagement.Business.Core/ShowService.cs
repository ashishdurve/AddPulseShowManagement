using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Repo;

namespace AddPulseShowManagement.Business.Core
{
    public class ShowService: IShowService
    {
        private readonly IShowRepository _repository;
        public ShowService(IShowRepository repository)
        {
            this._repository = repository;
        }

        public async Task<List<Shows>> GetShowList(string startdate = "", string endate = "")
        {
            try
            {
                return _repository.GetShowList(startdate, endate);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public  List<Shows> GetShows()
        {
            try
            {
                return _repository.GetShows();
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
                return _repository.SaveUpdateShow(show);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Teams> GetTeamsOfShow(long showID = 0)
        {
            try
            {
                return _repository.GetTeamsOfShow(showID);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
