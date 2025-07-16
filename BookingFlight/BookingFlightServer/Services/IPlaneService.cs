using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
    public interface IPlaneService
    {
        Task<IEnumerable<PlaneResponseDTO>> GetPlanesByManagerIdAsync(int managerId);
        Task<IEnumerable<PlaneResponseDTO>> GetPlanesWithFiltersAsync(PlaneListRequestDTO request);
        Task<int> GetTotalPlanesCountAsync(PlaneListRequestDTO request);
        Task<PlaneResponseDTO?> GetPlaneByIdAsync(int planeId);
        Task<PlaneResponseDTO> CreatePlaneAsync(PlaneCreateRequestDTO request);
        Task<PlaneResponseDTO> UpdatePlaneAsync(PlaneUpdateRequestDTO request);
        Task<bool> DeletePlaneAsync(int planeId);
        Task<(bool CanDelete, string Message)> CanDeletePlaneAsync(int planeId);
        Task<IEnumerable<StatusResponseDTO>> GetPlaneStatusesAsync();
    }
}
