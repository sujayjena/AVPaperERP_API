using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface ITerritoryRepository
    {
        #region State 

        Task<int> SaveState(State_Request parameters);

        Task<IEnumerable<State_Response>> GetStateList(BaseSearchEntity parameters);

        Task<State_Response?> GetStateById(long Id);

        Task<IEnumerable<StateDataValidationErrors>> ImportState(List<ImportedState> parameters);

        #endregion

        #region District 

        Task<int> SaveDistrict(District_Request parameters);

        Task<IEnumerable<District_Response>> GetDistrictList(BaseSearchEntity parameters);

        Task<District_Response?> GetDistrictById(long Id);

        Task<IEnumerable<DistrictDataValidationErrors>> ImportDistrict(List<ImportedDistrict> parameters);

        #endregion

        #region City 

        Task<int> SaveCity(City_Request parameters);

        Task<IEnumerable<City_Response>> GetCityList(BaseSearchEntity parameters);

        Task<City_Response?> GetCityById(long Id);

        Task<IEnumerable<CityDataValidationErrors>> ImportCity(List<ImportedCity> parameters);

        #endregion

        #region Territories 

        Task<int> SaveTerritories(Territories_Request parameters);

        Task<IEnumerable<Territories_Response>> GetTerritoriesList(BaseSearchEntity parameters);

        Task<Territories_Response?> GetTerritoriesById(long Id);

        Task<IEnumerable<Territories_State_Dist_City_Response>> GetTerritories_State_Dist_City_List_ById(Territories_State_Dist_City_Search parameters);

        Task<IEnumerable<TerritoriesDataValidationErrors>> ImportTerritories(List<ImportedTerritories> parameters);

        #endregion
    }
}
