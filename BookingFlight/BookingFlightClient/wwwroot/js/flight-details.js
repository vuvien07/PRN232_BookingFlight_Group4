// Flight Details JavaScript
const API_CONFIG = {
    BASE_URL: 'http://localhost:5077/api/FlightManage'
};

let currentFlightId = null;
let flightData = null;

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    // Get flight ID from URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    currentFlightId = urlParams.get('id');
    
    if (!currentFlightId) {
        showError('Flight ID not provided');
        return;
    }
    
    // Check if there's a specific action to focus on
    const action = urlParams.get('action');
    if (action) {
        // Set the appropriate tab as active after loading
        setTimeout(() => {
            switch(action) {
                case 'seats':
                    document.getElementById('seats-tab').click();
                    break;
                case 'services':
                    document.getElementById('services-tab').click();
                    break;
                case 'analytics':
                    document.getElementById('analytics-tab').click();
                    break;
            }
        }, 500);
    }
    
    loadFlightDetails();
});

// Get authentication token
function getAuthToken() {
    return localStorage.getItem('token') || sessionStorage.getItem('token');
}

// Show error message
function showError(message) {
    const errorAlert = document.getElementById('errorAlert');
    const errorMessage = document.getElementById('errorMessage');
    errorMessage.textContent = message;
    errorAlert.style.display = 'block';
    errorAlert.classList.add('show');
}

// Hide error message
function hideError() {
    const errorAlert = document.getElementById('errorAlert');
    errorAlert.style.display = 'none';
    errorAlert.classList.remove('show');
}

// Go back to flights list
function goBack() {
    window.location.href = '/FlightManage';
}

// Load flight details
async function loadFlightDetails() {
    try {
        showLoading();
        hideError();
        
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/details`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                window.location.href = '/Login';
                return;
            }
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            flightData = result.data;
            displayFlightDetails(flightData);
        } else {
            throw new Error(result.message || 'Failed to load flight details');
        }
    } catch (error) {
        console.error('Error loading flight details:', error);
        showError('Failed to load flight details: ' + error.message);
    } finally {
        hideLoading();
    }
}

// Show loading spinner
function showLoading() {
    document.getElementById('loadingSpinner').classList.remove('d-none');
    document.getElementById('flightDetailsContent').classList.add('d-none');
}

// Hide loading spinner
function hideLoading() {
    document.getElementById('loadingSpinner').classList.add('d-none');
    document.getElementById('flightDetailsContent').classList.remove('d-none');
}

// Display flight details
function displayFlightDetails(data) {
    // Update page title
    document.title = `Flight ${data.flightCode} - Details`;
    
    // Display basic flight information
    displayBasicInfo(data);
    
    // Display route information
    displayRouteInfo(data);
    
    // Display statistics
    displayStatistics(data.statistics);
    
    // Display seats
    displaySeats(data.seats, data.seatStatistics);
    
    // Display services
    displayServices(data.services);
    
    // Display analytics
    displayAnalytics(data.seatStatistics);
}

// Display basic flight information
function displayBasicInfo(data) {
    const basicInfoHtml = `
        <tr>
            <td class="fw-semibold">Flight Code:</td>
            <td>${escapeHtml(data.flightCode)}</td>
        </tr>
        <tr>
            <td class="fw-semibold">Aircraft:</td>
            <td>${escapeHtml(data.plane.planeName)} (${escapeHtml(data.plane.planeCode)})</td>
        </tr>
        <tr>
            <td class="fw-semibold">Status:</td>
            <td><span class="badge ${getStatusBadgeClass(data.statusId)}">${escapeHtml(data.statusName)}</span></td>
        </tr>
        <tr>
            <td class="fw-semibold">Tax:</td>
            <td>${(data.tax * 100).toFixed(1)}%</td>
        </tr>
        <tr>
            <td class="fw-semibold">Manager:</td>
            <td>${escapeHtml(data.manager.fullname)}</td>
        </tr>
    `;
    
    document.getElementById('flightBasicInfo').innerHTML = basicInfoHtml;
}

// Display route information
function displayRouteInfo(data) {
    const departureTime = new Date(data.departureTime);
    const arrivalTime = new Date(data.arrivalTime);
    
    // Calculate flight duration
    const duration = arrivalTime - departureTime;
    const hours = Math.floor(duration / (1000 * 60 * 60));
    const minutes = Math.floor((duration % (1000 * 60 * 60)) / (1000 * 60));
    
    const routeHtml = `
        <div class="d-flex align-items-center justify-content-between mb-3">
            <div class="text-center">
                <h5 class="mb-0">${escapeHtml(data.departureAirport.airportCode)}</h5>
                <small class="text-muted">${escapeHtml(data.departureAirport.city)}</small>
            </div>
            <div class="text-center flex-grow-1 mx-3">
                <i class="fas fa-plane text-primary"></i>
                <div class="small text-muted">${hours}h ${minutes}m</div>
            </div>
            <div class="text-center">
                <h5 class="mb-0">${escapeHtml(data.arrivalAirport.airportCode)}</h5>
                <small class="text-muted">${escapeHtml(data.arrivalAirport.city)}</small>
            </div>
        </div>
    `;
    
    document.getElementById('routeInfo').innerHTML = routeHtml;
    
    // Update timeline
    document.getElementById('departureInfo').innerHTML = `
        <strong>${departureTime.toLocaleTimeString()}</strong><br>
        <small>${escapeHtml(data.departureAirport.airportName)}</small>
    `;
    
    document.getElementById('arrivalInfo').innerHTML = `
        <strong>${arrivalTime.toLocaleTimeString()}</strong><br>
        <small>${escapeHtml(data.arrivalAirport.airportName)}</small>
    `;
}

// Display statistics
function displayStatistics(stats) {
    document.getElementById('totalSeats').textContent = stats.totalSeats;
    document.getElementById('occupiedSeats').textContent = stats.occupiedSeats;
    document.getElementById('occupancyRate').textContent = `${stats.occupancyRate}%`;
    document.getElementById('totalRevenue').textContent = `$${stats.totalRevenue.toLocaleString()}`;
}

// Display seats
function displaySeats(seats, seatStats) {
    // Create class filter buttons
    const classFilterHtml = `
        <button type="button" class="btn btn-outline-secondary active" data-class="all">
            All Classes
        </button>
        ${seatStats.map(stat => `
            <button type="button" class="btn btn-outline-secondary" data-class="${escapeHtml(stat.className)}">
                ${escapeHtml(stat.className)} (${stat.totalSeats})
            </button>
        `).join('')}
    `;
    
    document.getElementById('seatClassFilter').innerHTML = classFilterHtml;
    
    // Add click event listeners to filter buttons
    document.querySelectorAll('#seatClassFilter .btn').forEach(btn => {
        btn.addEventListener('click', function() {
            // Update active button
            document.querySelectorAll('#seatClassFilter .btn').forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            
            // Filter seats
            const className = this.dataset.class;
            filterSeats(className);
        });
    });
    
    // Display seats table
    displaySeatsTable(seats);
}

// Display seats table
function displaySeatsTable(seats) {
    const seatsHtml = seats.map(seat => `
        <tr class="${seat.isSat ? 'seat-occupied' : 'seat-available'}" data-class="${escapeHtml(seat.className)}">
            <td>
                <strong>${escapeHtml(seat.seatNumber)}</strong>
            </td>
            <td>
                <span class="badge bg-secondary">${escapeHtml(seat.className)}</span>
            </td>
            <td>$${seat.classPrice.toLocaleString()}</td>
            <td>
                ${seat.isSat ? 
                    '<span class="badge bg-danger">Occupied</span>' : 
                    '<span class="badge bg-success">Available</span>'
                }
            </td>
            <td>${seat.customerName ? escapeHtml(seat.customerName) : '-'}</td>
            <td>${seat.ticketNumber ? escapeHtml(seat.ticketNumber) : '-'}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button type="button" class="btn btn-outline-primary" onclick="editSeat(${seat.seatId})" title="Edit">
                        <i class="fas fa-edit"></i>
                    </button>
                    ${!seat.isSat ? `
                        <button type="button" class="btn btn-outline-danger" onclick="removeSeat(${seat.seatId})" title="Remove">
                            <i class="fas fa-trash"></i>
                        </button>
                    ` : ''}
                </div>
            </td>
        </tr>
    `).join('');
    
    document.getElementById('seatsTableBody').innerHTML = seatsHtml;
}

// Filter seats by class
function filterSeats(className) {
    const rows = document.querySelectorAll('#seatsTableBody tr');
    rows.forEach(row => {
        if (className === 'all' || row.dataset.class === className) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// Display services
function displayServices(services) {
    if (services.length === 0) {
        document.getElementById('servicesContainer').innerHTML = `
            <div class="col-12 text-center py-4">
                <i class="fas fa-concierge-bell text-muted fs-1"></i>
                <p class="text-muted mt-3">No services assigned to this flight</p>
                <button type="button" class="btn btn-primary" onclick="manageFlightServices()">
                    <i class="fas fa-plus"></i> Add Services
                </button>
            </div>
        `;
        return;
    }
    
    const servicesHtml = services.map(service => `
        <div class="col-md-6 col-lg-4 mb-3">
            <div class="card service-card h-100">
                <div class="card-body">
                    <h6 class="card-title">
                        <i class="fas fa-concierge-bell text-primary"></i>
                        ${escapeHtml(service.serviceName)}
                    </h6>
                    <p class="card-text text-muted small">${escapeHtml(service.detail)}</p>
                    <div class="d-flex justify-content-between align-items-center">
                        <span class="badge ${getStatusBadgeClass(service.statusId)}">${escapeHtml(service.statusName)}</span>
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeService(${service.serviceId})" title="Remove">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
    
    document.getElementById('servicesContainer').innerHTML = servicesHtml;
}

// Display analytics
function displayAnalytics(seatStats) {
    // Revenue breakdown
    const revenueHtml = seatStats.map(stat => `
        <div class="d-flex justify-content-between align-items-center mb-2 p-2 bg-light rounded">
            <div>
                <strong>${escapeHtml(stat.className)}</strong>
                <small class="text-muted d-block">${stat.occupiedSeats}/${stat.totalSeats} seats</small>
            </div>
            <div class="text-end">
                <strong class="text-success">$${stat.revenue.toLocaleString()}</strong>
            </div>
        </div>
    `).join('');
    
    document.getElementById('revenueByClass').innerHTML = revenueHtml;
}

// Manage flight seats
function manageFlightSeats() {
    // Open the flight seats modal
    const modal = new bootstrap.Modal(document.getElementById('flightSeatsModal'));
    modal.show();
    
    // Load modal data
    loadModalSeats();
    loadAvailableSeatsForModal();
}

// Manage flight services  
function manageFlightServices() {
    // Open the flight services modal
    const modal = new bootstrap.Modal(document.getElementById('flightServicesModal'));
    modal.show();
    
    // Load modal data
    loadModalServices();
    loadAvailableServicesForModal();
}

// Edit seat
function editSeat(seatId) {
    // Implement seat editing functionality
    console.log('Edit seat:', seatId);
    // You can open a modal or redirect to edit page
}

// Remove seat
async function removeSeat(seatId) {
    if (!confirm('Are you sure you want to remove this seat from the flight?')) {
        return false;
    }
    
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/seats/${seatId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            // Reload flight details
            loadFlightDetails();
            showSuccess('Seat removed successfully');
            return true;
        } else {
            throw new Error(result.message || 'Failed to remove seat');
        }
    } catch (error) {
        console.error('Error removing seat:', error);
        showError('Failed to remove seat: ' + error.message);
        return false;
    }
}

// Remove service
async function removeService(serviceId) {
    if (!confirm('Are you sure you want to remove this service from the flight?')) {
        return false;
    }
    
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/services/${serviceId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            // Reload flight details
            loadFlightDetails();
            showSuccess('Service removed successfully');
            return true;
        } else {
            throw new Error(result.message || 'Failed to remove service');
        }
    } catch (error) {
        console.error('Error removing service:', error);
        showError('Failed to remove service: ' + error.message);
        return false;
    }
}

// Refresh seats
function refreshSeats() {
    loadFlightDetails();
}

// Show success message
function showSuccess(message) {
    // You can implement a success notification here
    console.log('Success:', message);
}

// Get status badge class
function getStatusBadgeClass(statusId) {
    switch (statusId) {
        case 1: return 'bg-success';
        case 2: return 'bg-warning';
        case 3: return 'bg-danger';
        case 4: return 'bg-secondary';
        default: return 'bg-secondary';
    }
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    if (text == null) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Modal management functions
async function loadModalSeats() {
    if (!flightData || !flightData.seats) return;
    
    const modalSeatsHtml = flightData.seats.map(seat => `
        <tr class="${seat.isSat ? 'table-danger' : 'table-success'}">
            <td><strong>${escapeHtml(seat.seatNumber)}</strong></td>
            <td><span class="badge bg-secondary">${escapeHtml(seat.className)}</span></td>
            <td>
                ${seat.isSat ? 
                    '<span class="badge bg-danger">Occupied</span>' : 
                    '<span class="badge bg-success">Available</span>'
                }
            </td>
            <td>${seat.customerName ? escapeHtml(seat.customerName) : '-'}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button type="button" class="btn btn-outline-primary" onclick="editModalSeat(${seat.seatId})" title="Edit">
                        <i class="fas fa-edit"></i>
                    </button>
                    ${!seat.isSat ? `
                        <button type="button" class="btn btn-outline-danger" onclick="removeModalSeat(${seat.seatId})" title="Remove">
                            <i class="fas fa-trash"></i>
                        </button>
                    ` : ''}
                </div>
            </td>
        </tr>
    `).join('');
    
    document.getElementById('modalSeatsTableBody').innerHTML = modalSeatsHtml;
}

async function loadAvailableSeatsForModal() {
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/available-seats`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            populateAvailableSeatsDropdown(result.data);
        } else {
            throw new Error(result.message || 'Failed to load available seats');
        }
    } catch (error) {
        console.error('Error loading available seats:', error);
        populateAvailableSeatsDropdown([]);
    }
}

function populateAvailableSeatsDropdown(seats) {
    const select = document.getElementById('availableSeatSelect');
    const infoDiv = document.getElementById('availableSeatsInfo');
    
    select.innerHTML = '<option value="">Select a seat...</option>';
    
    if (!seats || seats.length === 0) {
        select.innerHTML += '<option value="" disabled>No available seats</option>';
        infoDiv.innerHTML = '<small class="text-muted">All seats from this plane have been added to the flight.</small>';
        return;
    }
    
    // Group seats by class for better organization
    const seatsByClass = seats.reduce((acc, seat) => {
        const className = seat.className || 'Unknown';
        if (!acc[className]) acc[className] = [];
        acc[className].push(seat);
        return acc;
    }, {});
    
    // Add seats grouped by class
    Object.keys(seatsByClass).forEach(className => {
        const classSeats = seatsByClass[className];
        select.innerHTML += `<optgroup label="${escapeHtml(className)} (${classSeats.length} seats)">`;
        classSeats.forEach(seat => {
            select.innerHTML += `
                <option value="${seat.seatId}">
                    ${escapeHtml(seat.seatNumber)} - $${seat.classPrice}
                </option>
            `;
        });
        select.innerHTML += '</optgroup>';
    });
    
    // Update info
    infoDiv.innerHTML = `
        <small class="text-info">
            <i class="fas fa-info-circle"></i> ${seats.length} seats available to add
        </small>
    `;
}

async function loadModalServices() {
    if (!flightData || !flightData.services) return;
    
    if (flightData.services.length === 0) {
        document.getElementById('currentServicesContainer').innerHTML = `
            <div class="text-center py-4">
                <i class="fas fa-concierge-bell text-muted fs-3"></i>
                <p class="text-muted mt-2">No services assigned</p>
            </div>
        `;
        return;
    }
    
    const servicesHtml = flightData.services.map(service => `
        <div class="card mb-2">
            <div class="card-body py-2">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1">${escapeHtml(service.serviceName)}</h6>
                        <small class="text-muted">${escapeHtml(service.detail)}</small>
                    </div>
                    <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeModalService(${service.serviceId})" title="Remove">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        </div>
    `).join('');
    
    document.getElementById('currentServicesContainer').innerHTML = servicesHtml;
}

async function loadAvailableServicesForModal() {
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/services`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            populateAvailableServicesDropdown(result.data);
        } else {
            throw new Error(result.message || 'Failed to load available services');
        }
    } catch (error) {
        console.error('Error loading available services:', error);
        populateAvailableServicesDropdown([]);
    }
}

function populateAvailableServicesDropdown(services) {
    const select = document.getElementById('availableServiceSelect');
    
    select.innerHTML = '<option value="">Select a service...</option>';
    
    if (!services || services.length === 0) {
        select.innerHTML += '<option value="" disabled>No available services</option>';
        return;
    }
    
    services.forEach(service => {
        select.innerHTML += `
            <option value="${service.serviceId}">
                ${escapeHtml(service.serviceName)} - ${escapeHtml(service.detail)}
            </option>
        `;
    });
}

// Modal action functions
function editModalSeat(seatId) {
    editSeat(seatId);
}

async function removeModalSeat(seatId) {
    const success = await removeSeat(seatId);
    if (success) {
        // Reload modal and main data
        loadFlightDetails();
        loadModalSeats();
        loadAvailableSeatsForModal();
    }
}

async function removeModalService(serviceId) {
    const success = await removeService(serviceId);
    if (success) {
        // Reload modal and main data
        loadFlightDetails();
        loadModalServices();
    }
}

async function regenerateAllSeats() {
    if (!confirm('Are you sure you want to regenerate all seats? This will remove all current seat assignments.')) {
        return;
    }
    
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/seats/regenerate`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            let successMessage = 'Flight seats regenerated successfully';
            
            // Check if flight status was changed and update UI realtime
            if (result.data && result.data.flightStatusChanged) {
                console.log(`Flight ${currentFlightId} status changed to: ${result.data.newFlightStatusName}`);
                
                // Update flight status in the page header
                updateFlightStatusDisplay(result.data.newFlightStatusId, result.data.newFlightStatusName);
                
                // Update success message to include status change
                successMessage = `Flight seats regenerated successfully! Flight status automatically changed to ${result.data.newFlightStatusName}`;
            }
            
            showSuccess(successMessage);
            // Reload all data
            loadFlightDetails();
            loadModalSeats();
            loadAvailableSeatsForModal();
        } else {
            throw new Error(result.message || 'Failed to regenerate seats');
        }
    } catch (error) {
        console.error('Error regenerating seats:', error);
        showError('Failed to regenerate seats: ' + error.message);
    }
}

// Initialize modal forms
document.addEventListener('DOMContentLoaded', function() {
    // Add flight seat form handler
    document.getElementById('addFlightSeatForm').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const seatId = document.getElementById('availableSeatSelect').value;
        const isOccupied = document.getElementById('seatOccupied').checked;
        const ticketId = document.getElementById('ticketIdInput').value;
        
        if (!seatId) {
            showError('Please select a seat');
            return;
        }
        
        try {
            const token = getAuthToken();
            const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/seats`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({
                    seatId: parseInt(seatId),
                    isSat: isOccupied,
                    ticketId: ticketId ? parseInt(ticketId) : null
                })
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const result = await response.json();
            if (result.success) {
                let successMessage = 'Flight seat added successfully';
                
                // Check if flight status was changed and update UI realtime
                if (result.data.flightStatusChanged) {
                    console.log(`Flight ${currentFlightId} status changed to: ${result.data.newFlightStatusName}`);
                    
                    // Update flight status in the page header
                    updateFlightStatusDisplay(result.data.newFlightStatusId, result.data.newFlightStatusName);
                    
                    // Update success message to include status change
                    successMessage = `Flight seat added successfully! Flight status automatically changed to ${result.data.newFlightStatusName}`;
                }
                
                showSuccess(successMessage);
                // Reset form
                this.reset();
                // Reload data
                loadFlightDetails();
                loadModalSeats();
                loadAvailableSeatsForModal();
            } else {
                throw new Error(result.message || 'Failed to add seat');
            }
        } catch (error) {
            console.error('Error adding seat:', error);
            showError('Failed to add seat: ' + error.message);
        }
    });
    
    // Add service form handler
    document.getElementById('addServiceForm').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const serviceId = document.getElementById('availableServiceSelect').value;
        
        if (!serviceId) {
            showError('Please select a service');
            return;
        }
        
        try {
            const token = getAuthToken();
            const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightId}/services`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({
                    serviceId: parseInt(serviceId)
                })
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const result = await response.json();
            if (result.success) {
                showSuccess('Service added successfully');
                // Reset form
                this.reset();
                // Reload data
                loadFlightDetails();
                loadModalServices();
            } else {
                throw new Error(result.message || 'Failed to add service');
            }
        } catch (error) {
            console.error('Error adding service:', error);
            showError('Failed to add service: ' + error.message);
        }
    });

/**
 * Updates flight status display in the flight details page realtime
 */
function updateFlightStatusDisplay(newStatusId, newStatusName) {
    try {
        // Find and update status badge in the flight info section
        const statusBadges = document.querySelectorAll('.badge, .status-badge');
        for (const badge of statusBadges) {
            if (badge.textContent.includes('Active') || badge.textContent.includes('Inactive')) {
                // Update status text
                badge.textContent = newStatusName;
                
                // Update status badge classes
                badge.className = 'badge ' + (newStatusId === 1 ? 'bg-success' : 'bg-secondary');
                
                // Add a brief highlight effect to show the change
                badge.style.animation = 'pulse 1s ease-in-out';
                setTimeout(() => {
                    badge.style.animation = '';
                }, 1000);
                
                console.log(`✅ Updated flight status display to ${newStatusName}`);
                break;
            }
        }
        
        // Also check for any status text that might be in other elements
        const statusText = document.querySelector('[data-status], .flight-status');
        if (statusText) {
            statusText.textContent = newStatusName;
            statusText.className = statusText.className.replace(/text-\w+/, newStatusId === 1 ? 'text-success' : 'text-secondary');
        }
    } catch (error) {
        console.error('Error updating flight status display:', error);
    }
}
    
    // Handle seat occupied checkbox
    document.getElementById('seatOccupied').addEventListener('change', function() {
        const ticketGroup = document.getElementById('ticketIdGroup');
        if (this.checked) {
            ticketGroup.style.display = 'block';
        } else {
            ticketGroup.style.display = 'none';
            document.getElementById('ticketIdInput').value = '';
        }
    });
});
