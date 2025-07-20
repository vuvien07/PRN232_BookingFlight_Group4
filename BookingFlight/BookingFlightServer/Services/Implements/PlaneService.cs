using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories.Interfaces;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class PlaneService : IPlaneService
    {
        private readonly IPlaneRepository _planeRepository;
        private readonly BookingFlightContext _context;

        public PlaneService(IPlaneRepository planeRepository, BookingFlightContext context)
        {
            _planeRepository = planeRepository;
            _context = context;
        }

        public async Task<IEnumerable<PlaneResponseDTO>> GetPlanesByManagerIdAsync(int managerId)
        {
            var planes = await _planeRepository.GetPlanesByManagerIdAsync(managerId);
            return planes.Select(MapToPlaneResponseDTO);
        }

        public async Task<IEnumerable<PlaneResponseDTO>> GetPlanesWithFiltersAsync(PlaneListRequestDTO request)
        {
            var planes = await _planeRepository.GetPlanesWithFiltersAsync(
                request.Search, 
                request.StatusId, 
                request.ManagerId, 
                request.Page, 
                request.PageSize);
            
            return planes.Select(MapToPlaneResponseDTO);
        }

        public async Task<int> GetTotalPlanesCountAsync(PlaneListRequestDTO request)
        {
            return await _planeRepository.GetTotalPlanesCountAsync(
                request.Search, 
                request.StatusId, 
                request.ManagerId);
        }

        public async Task<PlaneResponseDTO?> GetPlaneByIdAsync(int planeId)
        {
            var plane = await _planeRepository.GetPlaneByIdAsync(planeId);
            return plane != null ? MapToPlaneResponseDTO(plane) : null;
        }

        public async Task<PlaneResponseDTO> CreatePlaneAsync(PlaneCreateRequestDTO request)
        {
            // Check if plane code already exists
            if (await _planeRepository.IsPlaneCodeExistsAsync(request.PlaneCode))
            {
                throw new InvalidOperationException($"Plane code '{request.PlaneCode}' already exists.");
            }

            var plane = new Plane
            {
                PlaneCode = request.PlaneCode,
                Model = request.Model,
                Manufacture = request.Manufacture,
                YearOfManufacture = request.YearOfManufacture,
                StatusId = request.StatusId,
                ManagerId = request.ManagerId
            };

            await _planeRepository.AddAsync(plane);

            // Reload with includes for response
            var createdPlane = await _planeRepository.GetPlaneByIdAsync(plane.PlaneId ?? 0);
            return MapToPlaneResponseDTO(createdPlane!);
        }

        public async Task<PlaneResponseDTO> UpdatePlaneAsync(PlaneUpdateRequestDTO request)
        {
            Console.WriteLine($"PlaneService.UpdatePlaneAsync called for PlaneId: {request.PlaneId}");
            
            var plane = await _planeRepository.GetPlaneByIdForUpdateAsync(request.PlaneId);
            if (plane == null)
            {
                throw new InvalidOperationException($"Plane with ID {request.PlaneId} not found.");
            }

            Console.WriteLine($"Found plane: {plane.PlaneCode} - Current values: Model={plane.Model}, Manufacture={plane.Manufacture}, Year={plane.YearOfManufacture}, Status={plane.StatusId}");

            // Check if plane code already exists (excluding current plane)
            if (await _planeRepository.IsPlaneCodeExistsAsync(request.PlaneCode, request.PlaneId))
            {
                throw new InvalidOperationException($"Plane code '{request.PlaneCode}' already exists.");
            }

            Console.WriteLine($"Updating plane with new values: Model={request.Model}, Manufacture={request.Manufacture}, Year={request.YearOfManufacture}, Status={request.StatusId}");

            plane.PlaneCode = request.PlaneCode;
            plane.Model = request.Model;
            plane.Manufacture = request.Manufacture;
            plane.YearOfManufacture = request.YearOfManufacture;
            plane.StatusId = request.StatusId;
            plane.ManagerId = request.ManagerId;

            Console.WriteLine($"Before update - Plane values: Model={plane.Model}, Manufacture={plane.Manufacture}, Year={plane.YearOfManufacture}, Status={plane.StatusId}");
            
            await _planeRepository.UpdateAsync(plane);
            
            Console.WriteLine("Update completed, reloading plane from database...");

            // Reload with includes for response
            var updatedPlane = await _planeRepository.GetPlaneByIdAsync(request.PlaneId);
            Console.WriteLine($"Reloaded plane values: Model={updatedPlane?.Model}, Manufacture={updatedPlane?.Manufacture}, Year={updatedPlane?.YearOfManufacture}, Status={updatedPlane?.StatusId}");
            
            return MapToPlaneResponseDTO(updatedPlane!);
        }

        public async Task<bool> DeletePlaneAsync(int planeId)
        {
            var plane = await _planeRepository.GetPlaneByIdAsync(planeId);
            if (plane == null)
            {
                return false;
            }

            // Check if plane can be deleted
            var canDelete = await CanDeletePlaneAsync(planeId);
            if (!canDelete.CanDelete)
            {
                throw new InvalidOperationException(canDelete.Message);
            }

            await _planeRepository.DeleteAsync(plane);
            return true;
        }

        public async Task<(bool CanDelete, string Message)> CanDeletePlaneAsync(int planeId)
        {
            var plane = await _planeRepository.GetPlaneByIdAsync(planeId);
            if (plane == null)
            {
                return (false, "Plane not found");
            }

            // Check if plane has any flights
            var hasFlights = await _context.Flights.AnyAsync(f => f.PlaneId == planeId);
            if (hasFlights)
            {
                return (false, "Cannot delete plane because it has associated flights");
            }

            // Check if plane has any seats
            var hasSeats = await _context.Seats.AnyAsync(s => s.PlaneId == planeId);
            if (hasSeats)
            {
                return (false, "Cannot delete plane because it has associated seats");
            }

            return (true, "Plane can be deleted");
        }

        public async Task<IEnumerable<StatusResponseDTO>> GetPlaneStatusesAsync()
        {
            // Get all statuses first to see what's available
            var allStatuses = await _context.Statuses.ToListAsync();
            Console.WriteLine($"All statuses in database:");
            foreach (var status in allStatuses)
            {
                Console.WriteLine($"- StatusId: {status.StatusId}, StatusName: {status.StatusName}, StatusType: {status.StatusType}");
            }
            
            // Filter statuses for planes (assuming StatusType = "Plane" or similar)
            var statuses = allStatuses.Where(s => s.StatusType.ToLower().Contains("plane") || 
                                                   s.StatusName.ToLower() == "active" || 
                                                   s.StatusName.ToLower() == "inactive").ToList();
            
            if (!statuses.Any())
            {
                // If no plane-specific statuses found, use first few statuses
                statuses = allStatuses.Take(2).ToList();
                Console.WriteLine("No plane-specific statuses found, using first 2 statuses");
            }
            
            Console.WriteLine($"Using {statuses.Count} statuses for planes:");
            foreach (var status in statuses)
            {
                Console.WriteLine($"- StatusId: {status.StatusId}, StatusName: {status.StatusName}");
            }
            
            return statuses.Select(s => new StatusResponseDTO
            {
                StatusId = s.StatusId,
                StatusName = s.StatusName
            });
        }

        private static PlaneResponseDTO MapToPlaneResponseDTO(Plane plane)
        {
            return new PlaneResponseDTO
            {
                PlaneId = plane.PlaneId ?? 0,
                PlaneCode = plane.PlaneCode,
                Model = plane.Model,
                Manufacture = plane.Manufacture,
                YearOfManufacture = plane.YearOfManufacture,
                StatusId = plane.StatusId,
                StatusName = plane.Status?.StatusName ?? "",
                ManagerId = plane.ManagerId,
                ManagerName = plane.Manager?.Fullname ?? ""
            };
        }
    }
}
