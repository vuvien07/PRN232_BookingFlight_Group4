// Manager Flights Management JavaScript

// Configuration
const API_CONFIG = {
    BASE_URL: 'http://localhost:5077/api/FlightManage', // Direct backend URL
    ENDPOINTS: {
        LIST: '/list',
        GET: '',
        CREATE: '',
        UPDATE: '',
        DELETE: '',
        CHECK_CONFLICTS: '/check-conflicts',
        STATUSES: '/statuses'
    }
};

// Global variables
let currentPage = 1;
let pageSize = 10;
let totalPages = 0;
let currentFilters = {};
let isEditMode = false;
let currentFlightId = null;

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    console.log('Manager Flights page initialized');
    console.log('Current URL:', window.location.href);
    console.log('API Base URL:', API_CONFIG.BASE_URL);
    
    // Test API connectivity first
    testAPIConnectivity().then(() => {
        initializePage();
        loadFlights();
    }).catch(error => {
        console.error('API connectivity test failed:', error);
        showError('Failed to connect to API server. Please check if the backend is running.');
    });
});

// Test API connectivity
async function testAPIConnectivity() {
    try {
        console.log('Testing API connectivity...');
        const response = await fetch(API_CONFIG.BASE_URL.replace('/FlightManage', '') + '/health', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        console.log('Health check response:', response.status);
        
        if (!response.ok && response.status !== 404) {
            throw new Error(`API server not responding: ${response.status}`);
        }
        
        console.log('API connectivity test passed');
        return true;
    } catch (error) {
        console.error('API connectivity test failed:', error);
        
        // Try alternative base URL
        try {
            console.log('Trying alternative API URL...');
            const altResponse = await fetch('http://localhost:5077/api', {
                method: 'GET'
            });
            console.log('Alternative URL response:', altResponse.status);
        } catch (altError) {
            console.error('Alternative URL also failed:', altError);
        }
        
        throw error;
    }
}

async function initializePage() {
    try {
        await Promise.all([
            loadStatuses(),
            loadAirports(),
            loadPlanes(),
            loadServices()
        ]);
        
        setupEventListeners();
    } catch (error) {
        console.error('Error initializing page:', error);
        showError('Failed to initialize page');
    }
}

function setupEventListeners() {
    // Search input
    document.getElementById('searchInput').addEventListener('input', debounce(applyFilters, 500));
    
    // Status filter
    document.getElementById('statusFilter').addEventListener('change', applyFilters);
    
    // Date filters
    document.getElementById('departureFrom').addEventListener('change', applyFilters);
    document.getElementById('departureTo').addEventListener('change', applyFilters);
    
    // Form submission
    document.getElementById('flightForm').addEventListener('submit', handleFlightSubmit);
    
    // Airport validation
    document.getElementById('departureAirportId').addEventListener('change', validateAirports);
    document.getElementById('arrivalAirportId').addEventListener('change', validateAirports);
    
    // Conflict checking
    document.getElementById('departureTime').addEventListener('change', autoCheckConflicts);
    document.getElementById('arrivalTime').addEventListener('change', autoCheckConflicts);
    document.getElementById('planeId').addEventListener('change', autoCheckConflicts);
    document.getElementById('departureAirportId').addEventListener('change', autoCheckConflicts);
    document.getElementById('arrivalAirportId').addEventListener('change', autoCheckConflicts);
}

// Load functions
async function loadFlights() {
    try {
        showLoadingState();
        
        const requestData = {
            page: currentPage,
            pageSize: pageSize,
            ...currentFilters
        };
        
        console.log('=== LOAD FLIGHTS DEBUG START ===');
        console.log('Request data:', requestData);
        console.log('API URL:', API_CONFIG.BASE_URL + API_CONFIG.ENDPOINTS.LIST);
        
        const token = getAuthToken();
        console.log('Token available:', !!token);
        console.log('Token preview:', token ? token.substring(0, 30) + '...' : 'NO TOKEN');
        
        const fetchUrl = API_CONFIG.BASE_URL + API_CONFIG.ENDPOINTS.LIST;
        console.log('Full fetch URL:', fetchUrl);
        
        const response = await fetch(fetchUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include',
            body: JSON.stringify(requestData)
        });
        
        console.log('Response status:', response.status);
        console.log('Response headers:', Object.fromEntries(response.headers.entries()));
        
        let responseText = '';
        try {
            responseText = await response.text();
            console.log('Response text:', responseText);
        } catch (textError) {
            console.error('Error reading response text:', textError);
        }
        
        if (!response.ok) {
            console.error('HTTP Error Details:');
            console.error('- Status:', response.status);
            console.error('- Status Text:', response.statusText);
            console.error('- Response Body:', responseText);
            
            if (response.status === 401) {
                showError('Authentication failed. Please ensure you are logged in as a Manager.');
                handleAuthError();
                return;
            }
            if (response.status === 403) {
                showError('Access denied. You do not have Manager privileges.');
                return;
            }
            if (response.status === 404) {
                showError('API endpoint not found. Please check the server configuration.');
                return;
            }
            if (response.status >= 500) {
                showError('Server error. Please check if the backend server is running.');
                return;
            }
            
            throw new Error(`HTTP ${response.status}: ${response.statusText}\n${responseText}`);
        }
        
        let result;
        try {
            result = JSON.parse(responseText);
            console.log('Parsed result:', result);
        } catch (parseError) {
            console.error('Error parsing JSON response:', parseError);
            console.error('Response was:', responseText);
            throw new Error('Invalid JSON response from server');
        }
        
        if (result.success) {
            console.log('Successfully loaded flights:', result.data?.length || 0, 'items');
            displayFlights(result.data);
            updatePagination(result.pagination);
        } else {
            console.error('API returned success=false:', result);
            showError(result.message || 'Failed to load flights');
        }
        
        console.log('=== LOAD FLIGHTS DEBUG END ===');
    } catch (error) {
        console.error('=== LOAD FLIGHTS ERROR ===');
        console.error('Error type:', error.name);
        console.error('Error message:', error.message);
        console.error('Error stack:', error.stack);
        console.error('=== END ERROR ===');
        
        showError('Error loading flights: ' + error.message);
        
        // Show empty state
        displayFlights([]);
    } finally {
        hideLoadingState();
    }
}

async function loadStatuses() {
    try {
        const response = await fetch(API_CONFIG.BASE_URL + API_CONFIG.ENDPOINTS.STATUSES, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                populateStatusDropdowns(result.data);
            }
        }
    } catch (error) {
        console.error('Error loading statuses:', error);
    }
}

async function loadAirports() {
    try {
        const response = await fetch('http://localhost:5077/api/FlightAirports', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (response.ok) {
            const airports = await response.json();
            populateAirportDropdowns(airports);
        } else {
            console.warn('Failed to load airports from API, using mock data');
            // Mock data for development
            const mockAirports = [
                { airportId: 1, airportName: 'Noi Bai International Airport', airportCode: 'HAN' },
                { airportId: 2, airportName: 'Tan Son Nhat International Airport', airportCode: 'SGN' },
                { airportId: 3, airportName: 'Da Nang International Airport', airportCode: 'DAD' }
            ];
            populateAirportDropdowns(mockAirports);
        }
    } catch (error) {
        console.error('Error loading airports:', error);
        // Mock data for development
        const mockAirports = [
            { airportId: 1, airportName: 'Noi Bai International Airport', airportCode: 'HAN' },
            { airportId: 2, airportName: 'Tan Son Nhat International Airport', airportCode: 'SGN' },
            { airportId: 3, airportName: 'Da Nang International Airport', airportCode: 'DAD' }
        ];
        populateAirportDropdowns(mockAirports);
    }
}

async function loadPlanes() {
    try {
        const response = await fetch('http://localhost:5077/api/FlightPlanes', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (response.ok) {
            const planes = await response.json();
            populateDropdown('planeId', planes, 'planeId', 'planeCode');
        } else {
            console.warn('Failed to load planes from API, using mock data');
            // Mock data for development
            const mockPlanes = [
                { planeId: 1, planeCode: 'VN-A350-001', planeName: 'Airbus A350' },
                { planeId: 2, planeCode: 'VN-B787-002', planeName: 'Boeing 787' },
                { planeId: 3, planeCode: 'VN-A321-003', planeName: 'Airbus A321' }
            ];
            populateDropdown('planeId', mockPlanes, 'planeId', 'planeCode');
        }
    } catch (error) {
        console.error('Error loading planes:', error);
        // Mock data for development
        const mockPlanes = [
            { planeId: 1, planeCode: 'VN-A350-001', planeName: 'Airbus A350' },
            { planeId: 2, planeCode: 'VN-B787-002', planeName: 'Boeing 787' },
            { planeId: 3, planeCode: 'VN-A321-003', planeName: 'Airbus A321' }
        ];
        populateDropdown('planeId', mockPlanes, 'planeId', 'planeCode');
    }
}

async function loadServices() {
    try {
        const response = await fetch('http://localhost:5077/api/FlightManage/services', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                populateServicesDropdown(result.data);
            } else {
                console.warn('Failed to load services:', result.message);
                populateServicesDropdown([]);
            }
        } else {
            console.warn('Failed to load services from API');
            populateServicesDropdown([]);
        }
    } catch (error) {
        console.error('Error loading services:', error);
        populateServicesDropdown([]);
    }
}

function populateServicesDropdown(services) {
    const select = document.getElementById('serviceIds');
    select.innerHTML = '';
    
    services.forEach(service => {
        const option = document.createElement('option');
        option.value = service.serviceId;
        option.textContent = service.serviceName;
        select.appendChild(option);
    });
}

// Display functions
function displayFlights(flights) {
    const tbody = document.getElementById('flightsTableBody');
    
    if (!flights || flights.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center text-muted py-4">
                    <i class="fas fa-plane-slash fa-2x mb-2"></i>
                    <br>No flights found
                </td>
            </tr>
        `;
        return;
    }
    
    const now = new Date();
    
    tbody.innerHTML = flights.map(flight => {
        const departureTime = new Date(flight.departureTime);
        const isPastFlight = departureTime <= now;
        
        return `
        <tr${isPastFlight ? ' class="table-secondary"' : ''}>
            <td>
                <strong>${escapeHtml(flight.flightCode)}</strong>
                ${isPastFlight ? '<small class="text-muted d-block">Departed</small>' : ''}
            </td>
            <td>
                <div class="route-info">
                    <span class="badge bg-primary me-1">${escapeHtml(flight.departureAirportCode)}</span>
                    <i class="fas fa-arrow-right mx-1"></i>
                    <span class="badge bg-secondary">${escapeHtml(flight.arrivalAirportCode)}</span>
                </div>
                <small class="text-muted d-block">
                    ${escapeHtml(flight.departureAirportName)} → ${escapeHtml(flight.arrivalAirportName)}
                </small>
            </td>
            <td>
                <div>${formatDateTime(flight.departureTime)}</div>
            </td>
            <td>
                <div>${formatDateTime(flight.arrivalTime)}</div>
            </td>
            <td>
                <span class="text-primary">${escapeHtml(flight.planeName || 'N/A')}</span>
            </td>
            <td>
                <span class="badge ${getStatusBadgeClass(flight.statusId)}">
                    ${escapeHtml(flight.statusName)}
                </span>
            </td>
            <td>
                <div class="seat-info">
                    <span class="text-success">${flight.availableSeats}</span>
                    <span class="text-muted">/ ${flight.totalSeats}</span>
                </div>
                <small class="text-muted">Available / Total</small>
            </td>
            <td>
                <div class="btn-group" role="group">
                    ${isPastFlight ? `
                        <button type="button" class="btn btn-sm btn-outline-info" 
                                onclick="viewFlightDetails(${flight.flightId})" title="View Details">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-secondary" 
                                onclick="manageFlightSeats(${flight.flightId})" title="Manage Seats">
                            <i class="fas fa-chair"></i>
                        </button>
                        <small class="text-muted ms-2">Flight departed</small>
                    ` : `
                        <button type="button" class="btn btn-sm btn-outline-primary" 
                                onclick="editFlight(${flight.flightId})" title="Edit">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-info" 
                                onclick="viewFlightDetails(${flight.flightId})" title="View Details">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-success" 
                                onclick="manageFlightSeats(${flight.flightId})" title="Manage Seats">
                            <i class="fas fa-chair"></i>
                        </button>
                        <button type="button" class="btn btn-sm ${flight.statusId === 1 ? 'btn-outline-warning' : 'btn-outline-success'}" 
                                onclick="toggleFlightStatus(${flight.flightId}, ${flight.statusId}, '${escapeHtml(flight.flightCode)}')" 
                                title="${flight.statusId === 1 ? 'Deactivate Flight' : 'Activate Flight'}">
                            <i class="fas ${flight.statusId === 1 ? 'fa-pause' : 'fa-play'}"></i>
                        </button>
                    `}
                </div>
            </td>
        </tr>
    `}).join('');
}

function updatePagination(pagination) {
    totalPages = pagination.totalPages;
    currentPage = pagination.currentPage;
    
    const paginationContainer = document.getElementById('paginationContainer');
    const paginationElement = document.getElementById('pagination');
    
    if (totalPages <= 1) {
        paginationContainer.style.display = 'none';
        return;
    }
    
    paginationContainer.style.display = 'block';
    
    let paginationHTML = '';
    
    // Previous button
    paginationHTML += `
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage - 1})" aria-label="Previous">
                <span aria-hidden="true">&laquo;</span>
            </a>
        </li>
    `;
    
    // Page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);
    
    for (let i = startPage; i <= endPage; i++) {
        paginationHTML += `
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
            </li>
        `;
    }
    
    // Next button
    paginationHTML += `
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage + 1})" aria-label="Next">
                <span aria-hidden="true">&raquo;</span>
            </a>
        </li>
    `;
    
    paginationElement.innerHTML = paginationHTML;
}

// Modal functions
function showAddFlightModal() {
    isEditMode = false;
    currentFlightId = null;
    document.getElementById('flightModalLabel').textContent = 'Add New Flight';
    document.getElementById('flightForm').reset();
    
    // Hide status field for new flights
    const statusGroup = document.getElementById('statusGroup');
    if (statusGroup) {
        statusGroup.style.display = 'none';
    }
    
    // Reset services selection
    const serviceSelect = document.getElementById('serviceIds');
    if (serviceSelect) {
        serviceSelect.selectedIndex = -1;
        Array.from(serviceSelect.options).forEach(option => option.selected = false);
    }
    
    clearValidationErrors();
    hideConflictAlert();
    
    const modal = new bootstrap.Modal(document.getElementById('flightModal'));
    modal.show();
}

async function editFlight(flightId) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/${flightId}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            const flight = result.data;
            
            // Check if departure time is in the past
            const departureTime = new Date(flight.departureTime);
            const now = new Date();
            
            if (departureTime <= now) {
                showError('Cannot edit flights that have already departed or are departing now.');
                return;
            }
            
            isEditMode = true;
            currentFlightId = flightId;
            document.getElementById('flightModalLabel').textContent = 'Edit Flight';
            populateFlightForm(result.data);
            clearValidationErrors();
            hideConflictAlert();
            
            const modal = new bootstrap.Modal(document.getElementById('flightModal'));
            modal.show();
        } else {
            showError(result.message || 'Failed to load flight details');
        }
    } catch (error) {
        console.error('Error loading flight:', error);
        showError('Error loading flight details');
    }
}

function populateFlightForm(flight) {
    document.getElementById('flightCode').value = flight.flightCode || '';
    document.getElementById('tax').value = flight.tax || '';
    document.getElementById('departureTime').value = formatDateTimeForInput(flight.departureTime);
    document.getElementById('arrivalTime').value = formatDateTimeForInput(flight.arrivalTime);
    document.getElementById('departureAirportId').value = flight.departureAirportId || '';
    document.getElementById('arrivalAirportId').value = flight.arrivalAirportId || '';
    document.getElementById('planeId').value = flight.planeId || '';
    
    // Don't show status field even for edit mode - status is auto-managed
    const statusGroup = document.getElementById('statusGroup');
    if (statusGroup) {
        statusGroup.style.display = 'none';
    }
}

// Form handling
async function handleFlightSubmit(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    
    // Get selected service IDs
    const serviceSelect = document.getElementById('serviceIds');
    const selectedServices = Array.from(serviceSelect.selectedOptions).map(option => parseInt(option.value));
    
    const flightData = {
        flightCode: formData.get('flightCode'),
        tax: parseFloat(formData.get('tax')),
        departureTime: formData.get('departureTime'),
        arrivalTime: formData.get('arrivalTime'),
        departureAirportId: parseInt(formData.get('departureAirportId')),
        arrivalAirportId: parseInt(formData.get('arrivalAirportId')),
        planeId: parseInt(formData.get('planeId')),
        serviceIds: selectedServices
    };

    // Note: statusId is no longer included - it's automatically managed by the system
    
    // Validate that departure and arrival airports are different
    if (flightData.departureAirportId === flightData.arrivalAirportId) {
        showError('Departure and Arrival airports must be different!');
        document.getElementById('arrivalAirportId').classList.add('is-invalid');
        const feedback = document.getElementById('arrivalAirportId').nextElementSibling;
        if (feedback && feedback.classList.contains('invalid-feedback')) {
            feedback.textContent = 'Arrival airport must be different from departure airport';
        }
        return;
    }

    // Validate departure time must be at least 2 days from now (only for new flights)
    if (!isEditMode) {
        const departureTime = new Date(flightData.departureTime);
        const minimumTime = new Date();
        minimumTime.setDate(minimumTime.getDate() + 2);
        
        if (departureTime < minimumTime) {
            showError('Departure time must be at least 2 days from now!');
            document.getElementById('departureTime').classList.add('is-invalid');
            const feedback = document.getElementById('departureTime').nextElementSibling;
            if (feedback && feedback.classList.contains('invalid-feedback')) {
                feedback.textContent = `Departure time must be at least ${minimumTime.toLocaleString()}`;
            }
            return;
        }
    }
    
    if (isEditMode) {
        flightData.flightId = currentFlightId;
    }
    
    try {
        const url = isEditMode 
            ? `${API_CONFIG.BASE_URL}/${currentFlightId}` 
            : API_CONFIG.BASE_URL;
        const method = isEditMode ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include',
            body: JSON.stringify(flightData)
        });
        
        const result = await response.json();
        
        if (result.success) {
            showSuccess(`Flight ${isEditMode ? 'updated' : 'created'} successfully`);
            const modal = bootstrap.Modal.getInstance(document.getElementById('flightModal'));
            modal.hide();
            loadFlights();
        } else {
            if (result.errors) {
                displayValidationErrors(result.errors);
            } else {
                showError(result.message || `Failed to ${isEditMode ? 'update' : 'create'} flight`);
            }
        }
    } catch (error) {
        console.error('Error saving flight:', error);
        showError(`Error ${isEditMode ? 'updating' : 'creating'} flight`);
    }
}

// Conflict checking
async function checkConflicts() {
    const conflictData = {
        flightId: isEditMode ? currentFlightId : null,
        departureTime: document.getElementById('departureTime').value,
        arrivalTime: document.getElementById('arrivalTime').value,
        planeId: parseInt(document.getElementById('planeId').value),
        departureAirportId: parseInt(document.getElementById('departureAirportId').value),
        arrivalAirportId: parseInt(document.getElementById('arrivalAirportId').value)
    };
    
    // Validation
    if (!conflictData.departureTime || !conflictData.arrivalTime || 
        !conflictData.planeId || !conflictData.departureAirportId || !conflictData.arrivalAirportId) {
        showWarning('Please fill in all required fields before checking conflicts');
        return;
    }
    
    if (new Date(conflictData.departureTime) >= new Date(conflictData.arrivalTime)) {
        showWarning('Departure time must be before arrival time');
        return;
    }
    
    try {
        showInfo('Checking for conflicts with AI...');
        
        const response = await fetch(API_CONFIG.BASE_URL + API_CONFIG.ENDPOINTS.CHECK_CONFLICTS, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include',
            body: JSON.stringify(conflictData)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            displayConflictResults(result.data);
        } else {
            showError(result.message || 'Failed to check conflicts');
        }
    } catch (error) {
        console.error('Error checking conflicts:', error);
        showError('Error checking conflicts');
    }
}

function displayConflictResults(conflictResult) {
    const alertDiv = document.getElementById('conflictAlert');
    const detailsDiv = document.getElementById('conflictDetails');
    const aiAnalysisDiv = document.getElementById('aiAnalysis');
    const recommendationsDiv = document.getElementById('aiRecommendations');
    
    if (!conflictResult.hasConflict) {
        alertDiv.className = 'alert alert-success';
        alertDiv.querySelector('.alert-heading').textContent = 'No Conflicts Detected';
        detailsDiv.innerHTML = '<p>✅ Your flight schedule looks good! No conflicts found.</p>';
        aiAnalysisDiv.style.display = 'none';
        alertDiv.style.display = 'block';
        return;
    }
    
    // Show conflicts
    alertDiv.className = 'alert alert-warning';
    alertDiv.querySelector('.alert-heading').textContent = 'Flight Schedule Conflicts Detected';
    
    let conflictHTML = '<ul class="mb-0">';
    conflictResult.conflicts.forEach(conflict => {
        conflictHTML += `
            <li>
                <strong>${conflict.flightCode}</strong> - ${conflict.description}
                <br><small class="text-muted">
                    ${formatDateTime(conflict.departureTime)} → ${formatDateTime(conflict.arrivalTime)}
                </small>
            </li>
        `;
    });
    conflictHTML += '</ul>';
    detailsDiv.innerHTML = conflictHTML;
    
    // Show AI analysis
    if (conflictResult.aiAnalysis) {
        aiAnalysisDiv.style.display = 'block';
        recommendationsDiv.innerHTML = `
            <div class="ai-analysis">
                <div class="mb-2">
                    <strong>AI Analysis:</strong>
                    <p class="mt-1">${escapeHtml(conflictResult.aiAnalysis)}</p>
                </div>
                ${conflictResult.recommendations && conflictResult.recommendations.length > 0 ? `
                <div>
                    <strong>Recommendations:</strong>
                    <ul class="mt-1">
                        ${conflictResult.recommendations.map(rec => `<li>${escapeHtml(rec)}</li>`).join('')}
                    </ul>
                </div>
                ` : ''}
            </div>
        `;
    } else {
        aiAnalysisDiv.style.display = 'none';
    }
    
    alertDiv.style.display = 'block';
}

// Auto conflict checking
let conflictCheckTimeout;
function autoCheckConflicts() {
    clearTimeout(conflictCheckTimeout);
    conflictCheckTimeout = setTimeout(() => {
        const form = document.getElementById('flightForm');
        if (form.checkValidity()) {
            checkConflicts();
        }
    }, 1000);
}

// Airport validation function
function validateAirports() {
    const departureAirportId = document.getElementById('departureAirportId').value;
    const arrivalAirportId = document.getElementById('arrivalAirportId').value;
    
    const departureElement = document.getElementById('departureAirportId');
    const arrivalElement = document.getElementById('arrivalAirportId');
    
    // Clear previous validation states
    departureElement.classList.remove('is-invalid');
    arrivalElement.classList.remove('is-invalid');
    
    const departureFeedback = departureElement.nextElementSibling;
    const arrivalFeedback = arrivalElement.nextElementSibling;
    
    if (departureFeedback && departureFeedback.classList.contains('invalid-feedback')) {
        departureFeedback.textContent = '';
    }
    if (arrivalFeedback && arrivalFeedback.classList.contains('invalid-feedback')) {
        arrivalFeedback.textContent = '';
    }
    
    // Validate if both airports are selected and same
    if (departureAirportId && arrivalAirportId && departureAirportId === arrivalAirportId) {
        arrivalElement.classList.add('is-invalid');
        if (arrivalFeedback && arrivalFeedback.classList.contains('invalid-feedback')) {
            arrivalFeedback.textContent = 'Arrival airport must be different from departure airport';
        }
        showWarning('Departure and Arrival airports must be different!');
        return false;
    }
    
    return true;
}

// Toggle Flight Status function
function toggleFlightStatus(flightId, currentStatusId, flightCode) {
    const newStatusId = currentStatusId === 1 ? 2 : 1; // Toggle between Active (1) and Inactive (2)
    const newStatusName = newStatusId === 1 ? 'Active' : 'Inactive';
    const actionText = newStatusId === 1 ? 'activate' : 'deactivate';
    
    // Update modal content
    document.getElementById('statusChangeFlightInfo').textContent = `Flight: ${flightCode}`;
    document.getElementById('statusChangeMessage').textContent = `Are you sure you want to ${actionText} this flight?`;
    document.getElementById('statusChangeNote').textContent = 
        newStatusId === 1 ? 'This will make the flight available for booking.' : 'This will make the flight unavailable for booking.';
    document.getElementById('statusChangeButtonText').textContent = `${actionText.charAt(0).toUpperCase() + actionText.slice(1)} Flight`;
    
    // Update button style based on action
    const confirmBtn = document.getElementById('confirmStatusChangeBtn');
    confirmBtn.className = `btn ${newStatusId === 1 ? 'btn-success' : 'btn-warning'}`;
    confirmBtn.innerHTML = `<i class="fas ${newStatusId === 1 ? 'fa-play' : 'fa-pause'}"></i> ${actionText.charAt(0).toUpperCase() + actionText.slice(1)} Flight`;
    
    const modal = new bootstrap.Modal(document.getElementById('statusChangeConfirmModal'));
    modal.show();
    
    confirmBtn.onclick = async function() {
        try {
            console.log(`Attempting to change flight ${flightId} status from ${currentStatusId} to ${newStatusId}`);
            
            const response = await fetch(`${API_CONFIG.BASE_URL}/${flightId}/status`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getAuthToken()}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({
                    statusId: newStatusId
                })
            });
            
            console.log(`Response status: ${response.status} ${response.statusText}`);
            
            if (!response.ok) {
                // Log the response text for debugging
                const errorText = await response.text();
                console.error(`HTTP Error: ${response.status} - ${errorText}`);
                
                if (response.status === 401) {
                    showError('Authentication failed. Please login again.');
                } else if (response.status === 403) {
                    showError('Access denied. You can only manage your own flights.');
                } else if (response.status === 404) {
                    showError('Flight not found.');
                } else {
                    showError(`Failed to ${actionText} flight: HTTP ${response.status}`);
                }
                return;
            }
            
            const result = await response.json();
            console.log('Change status result:', result);
            
            if (result.success) {
                showSuccess(`Flight ${actionText}d successfully`);
                
                // Update the status in the table realtime
                updateFlightStatusInTable(flightId, newStatusId, newStatusName);
                
                // Close modal after successful update
                modal.hide();
                
                // Don't reload flights immediately - the table is already updated
                // loadFlights();
            } else {
                console.error('API returned success=false:', result);
                showError(result.message || `Failed to ${actionText} flight`);
            }
        } catch (error) {
            console.error(`Error ${actionText}ing flight:`, error);
            showError(`Error ${actionText}ing flight: ${error.message}`);
        }
    };
}

// Filter functions
function applyFilters() {
    currentFilters = {
        searchTerm: document.getElementById('searchInput').value.trim() || null,
        statusId: document.getElementById('statusFilter').value || null,
        departureFrom: document.getElementById('departureFrom').value || null,
        departureTo: document.getElementById('departureTo').value || null
    };
    
    currentPage = 1;
    loadFlights();
}

function clearFilters() {
    document.getElementById('searchInput').value = '';
    document.getElementById('statusFilter').value = '';
    document.getElementById('departureFrom').value = '';
    document.getElementById('departureTo').value = '';
    
    currentFilters = {};
    currentPage = 1;
    loadFlights();
}

function changePage(page) {
    if (page >= 1 && page <= totalPages && page !== currentPage) {
        currentPage = page;
        loadFlights();
    }
}

// Utility functions
function populateStatusDropdowns(statuses) {
    const filterSelect = document.getElementById('statusFilter');
    const formSelect = document.getElementById('statusId');
    
    // Filter dropdown
    filterSelect.innerHTML = '<option value="">All Status</option>';
    statuses.forEach(status => {
        filterSelect.innerHTML += `<option value="${status.statusId}">${escapeHtml(status.statusName)}</option>`;
    });
    
    // Form dropdown
    formSelect.innerHTML = '';
    statuses.forEach(status => {
        formSelect.innerHTML += `<option value="${status.statusId}">${escapeHtml(status.statusName)}</option>`;
    });
}

function populateAirportDropdowns(airports) {
    populateDropdown('departureAirportId', airports, 'airportId', 'airportName', 'airportCode');
    populateDropdown('arrivalAirportId', airports, 'airportId', 'airportName', 'airportCode');
}

function populateDropdown(elementId, items, valueField, textField, codeField = null) {
    const select = document.getElementById(elementId);
    const currentValue = select.value;
    
    select.innerHTML = `<option value="">Select ${elementId.replace('Id', '').replace(/([A-Z])/g, ' $1').trim()}</option>`;
    
    items.forEach(item => {
        const text = codeField ? `${item[codeField]} - ${item[textField]}` : item[textField];
        const option = new Option(text, item[valueField]);
        if (currentValue && currentValue == item[valueField]) {
            option.selected = true;
        }
        select.add(option);
    });
}

function getStatusBadgeClass(statusId) {
    switch (statusId) {
        case 1: return 'bg-success';
        case 2: return 'bg-secondary';
        case 3: return 'bg-warning';
        case 4: return 'bg-danger';
        default: return 'bg-secondary';
    }
}

function formatDateTime(dateTimeString) {
    if (!dateTimeString) return 'N/A';
    const date = new Date(dateTimeString);
    return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function formatDateTimeForInput(dateTimeString) {
    if (!dateTimeString) return '';
    const date = new Date(dateTimeString);
    return date.toISOString().slice(0, 16);
}

// UI State functions
function showLoadingState() {
    document.getElementById('loadingSpinner').style.display = 'block';
    document.getElementById('flightsTableContainer').style.display = 'none';
}

function hideLoadingState() {
    document.getElementById('loadingSpinner').style.display = 'none';
    document.getElementById('flightsTableContainer').style.display = 'block';
}

function hideConflictAlert() {
    document.getElementById('conflictAlert').style.display = 'none';
}

function clearValidationErrors() {
    const invalidInputs = document.querySelectorAll('.is-invalid');
    invalidInputs.forEach(input => {
        input.classList.remove('is-invalid');
    });
    
    const errorMessages = document.querySelectorAll('.invalid-feedback');
    errorMessages.forEach(msg => {
        msg.textContent = '';
    });
}

function displayValidationErrors(errors) {
    clearValidationErrors();
    
    Object.keys(errors).forEach(field => {
        const input = document.getElementById(field);
        if (input) {
            input.classList.add('is-invalid');
            const feedback = input.nextElementSibling;
            if (feedback && feedback.classList.contains('invalid-feedback')) {
                feedback.textContent = errors[field].join(', ');
            }
        }
    });
}

// Utility functions
function getAuthToken() {
    console.log('=== GET AUTH TOKEN DEBUG ===');
    
    // Try to get JWT token from cookies first, then localStorage, sessionStorage
    const cookieToken = getCookie('X-Access-Token');
    console.log('Cookie token (X-Access-Token):', cookieToken ? cookieToken.substring(0, 30) + '...' : 'NOT FOUND');
    
    const localStorageAuthToken = localStorage.getItem('authToken');
    console.log('localStorage authToken:', localStorageAuthToken ? localStorageAuthToken.substring(0, 30) + '...' : 'NOT FOUND');
    
    const sessionStorageAuthToken = sessionStorage.getItem('authToken');
    console.log('sessionStorage authToken:', sessionStorageAuthToken ? sessionStorageAuthToken.substring(0, 30) + '...' : 'NOT FOUND');
    
    const localStorageToken = localStorage.getItem('token');
    console.log('localStorage token:', localStorageToken ? localStorageToken.substring(0, 30) + '...' : 'NOT FOUND');
    
    const sessionStorageToken = sessionStorage.getItem('token');
    console.log('sessionStorage token:', sessionStorageToken ? sessionStorageToken.substring(0, 30) + '...' : 'NOT FOUND');
    
    // Check for more cookie variations
    const altCookieToken = getCookie('authToken') || getCookie('token') || getCookie('jwt');
    console.log('Alternative cookies:', altCookieToken ? altCookieToken.substring(0, 30) + '...' : 'NOT FOUND');
    
    const finalToken = cookieToken || 
                      localStorageAuthToken || 
                      sessionStorageAuthToken || 
                      localStorageToken || 
                      sessionStorageToken ||
                      altCookieToken || '';
    
    console.log('Final token selected:', finalToken ? finalToken.substring(0, 30) + '...' : 'NO TOKEN AVAILABLE');
    console.log('All cookies:', document.cookie);
    console.log('=== END GET AUTH TOKEN DEBUG ===');
    
    return finalToken;
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

function escapeHtml(text) {
    if (text == null) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function handleAuthError() {
    console.error('Authentication error detected');
    showError('Authentication required. Please login again.');
    
    // Clear any stored tokens
    localStorage.removeItem('authToken');
    localStorage.removeItem('token');
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('token');
    
    // Clear cookies by setting them to expire
    document.cookie = 'X-Access-Token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
    
    setTimeout(() => {
        console.log('Redirecting to login page...');
        window.location.href = '/Authentication/Login';
    }, 2000);
}

// Notification functions
function showSuccess(message) {
    console.log('Success:', message);
    // You can integrate with a toast library here
    alert(message);
}

function showError(message) {
    console.error('Error:', message);
    // You can integrate with a toast library here
    alert('Error: ' + message);
}

function showWarning(message) {
    console.warn('Warning:', message);
    // You can integrate with a toast library here
    alert('Warning: ' + message);
}

function showInfo(message) {
    console.info('Info:', message);
    // You can integrate with a toast library here - for now just log
}

// Debug functions for development
window.debugFlights = {
    testAPI: testAPIConnectivity,
    loadFlights: loadFlights,
    getToken: getAuthToken,
    clearTokens: function() {
        localStorage.clear();
        sessionStorage.clear();
        document.cookie.split(";").forEach(function(c) { 
            document.cookie = c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/"); 
        });
        console.log('All tokens and storage cleared');
    },
    showDebugInfo: function() {
        console.log('=== DEBUG INFO ===');
        console.log('Current page:', currentPage);
        console.log('Page size:', pageSize);
        console.log('Current filters:', currentFilters);
        console.log('API Config:', API_CONFIG);
        console.log('Auth token:', getAuthToken() ? 'Available' : 'Missing');
        console.log('=== END DEBUG INFO ===');
    }
};

console.log('Debug functions available: window.debugFlights');

// View flight details function
function viewFlightDetails(flightId) {
    if (!flightId) {
        showError('Flight ID is required');
        return;
    }
    
    // Navigate to flight details page
    window.location.href = `/FlightDetails?id=${flightId}`;
}

// Flight Seats Management Functions
let currentFlightSeatsFlightId = null;

function manageFlightSeats(flightId) {
    if (!flightId) {
        showError('Flight ID is required');
        return;
    }
    
    currentFlightSeatsFlightId = flightId;
    
    // Update modal title
    document.getElementById('flightSeatsModalLabel').textContent = `Manage Flight Seats - Flight ID: ${flightId}`;
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('flightSeatsModal'));
    modal.show();
    
    // Load flight seats data
    loadFlightSeats(flightId);
    loadAvailableSeats(flightId);
    
    // Setup form listeners for this specific flight
    setupFlightSeatsFormListeners();
}

function setupFlightSeatsFormListeners() {
    // Add flight seat form
    const addForm = document.getElementById('addFlightSeatForm');
    if (addForm) {
        addForm.removeEventListener('submit', handleAddFlightSeat); // Remove existing listener
        addForm.addEventListener('submit', handleAddFlightSeat);
    }
    
    // Seat occupied checkbox
    const occupiedCheckbox = document.getElementById('seatOccupied');
    if (occupiedCheckbox) {
        occupiedCheckbox.removeEventListener('change', toggleTicketIdInput);
        occupiedCheckbox.addEventListener('change', toggleTicketIdInput);
    }
}

function toggleTicketIdInput() {
    const occupied = document.getElementById('seatOccupied').checked;
    const ticketGroup = document.getElementById('ticketIdGroup');
    
    if (occupied) {
        ticketGroup.style.display = 'block';
    } else {
        ticketGroup.style.display = 'none';
        document.getElementById('ticketIdInput').value = '';
    }
}

async function loadFlightSeats(flightId) {
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${flightId}/seats`, {
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
            displayFlightSeats(result.data);
        } else {
            throw new Error(result.message || 'Failed to load flight seats');
        }
    } catch (error) {
        console.error('Error loading flight seats:', error);
        showError('Failed to load flight seats: ' + error.message);
        displayFlightSeats([]);
    }
}

function displayFlightSeats(seats) {
    const tbody = document.getElementById('flightSeatsTableBody');
    const loadingText = document.getElementById('flightSeatsLoadingText');
    const emptyText = document.getElementById('flightSeatsEmptyText');
    
    loadingText.style.display = 'none';
    
    if (!seats || seats.length === 0) {
        tbody.innerHTML = '';
        emptyText.style.display = 'block';
        return;
    }
    
    emptyText.style.display = 'none';
    
    tbody.innerHTML = seats.map(seat => `
        <tr>
            <td>
                <strong>${escapeHtml(seat.seatNumber)}</strong>
            </td>
            <td>
                <span class="badge bg-info">${escapeHtml(seat.className)}</span>
                <small class="text-muted d-block">$${seat.classPrice}</small>
            </td>
            <td>
                <span class="badge ${seat.seatStatusId === 1 ? 'bg-success' : 'bg-warning'}">
                    ${escapeHtml(seat.seatStatusName)}
                </span>
            </td>
            <td>
                <span class="badge ${seat.isSat ? 'bg-danger' : 'bg-success'}">
                    ${seat.isSat ? 'Occupied' : 'Available'}
                </span>
            </td>
            <td>
                ${seat.ticketCode ? `<small class="text-muted">${escapeHtml(seat.ticketCode)}</small>` : '-'}
            </td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button type="button" class="btn btn-outline-primary btn-sm" 
                            onclick="editFlightSeat(${seat.flightId}, ${seat.seatId})" title="Edit">
                        <i class="fas fa-edit"></i>
                    </button>
                    ${!seat.isSat ? `
                        <button type="button" class="btn btn-outline-danger btn-sm" 
                                onclick="removeFlightSeat(${seat.flightId}, ${seat.seatId})" title="Remove">
                            <i class="fas fa-trash"></i>
                        </button>
                    ` : ''}
                </div>
            </td>
        </tr>
    `).join('');
}

async function loadAvailableSeats(flightId) {
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${flightId}/available-seats`, {
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
        showError('Failed to load available seats: ' + error.message);
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

async function handleAddFlightSeat(event) {
    event.preventDefault();
    
    const seatId = document.getElementById('availableSeatSelect').value;
    const isSat = document.getElementById('seatOccupied').checked;
    const ticketId = document.getElementById('ticketIdInput').value;
    
    if (!seatId) {
        showError('Please select a seat');
        return;
    }
    
    if (!currentFlightSeatsFlightId) {
        showError('Flight ID is missing');
        return;
    }
    
    try {
        const token = getAuthToken();
        const requestData = {
            seatId: parseInt(seatId),
            isSat: isSat,
            ticketId: ticketId ? parseInt(ticketId) : null
        };
        
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightSeatsFlightId}/seats`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify(requestData)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            showSuccess('Flight seat added successfully');
            
            // Reset form
            document.getElementById('addFlightSeatForm').reset();
            document.getElementById('ticketIdGroup').style.display = 'none';
            
            // Check if flight status was changed and update UI realtime
            if (result.data.flightStatusChanged) {
                console.log(`Flight ${currentFlightSeatsFlightId} status changed to: ${result.data.newFlightStatusName}`);
                
                // Update flight status in the main flights table
                updateFlightStatusInTable(currentFlightSeatsFlightId, result.data.newFlightStatusId, result.data.newFlightStatusName);
                
                // Show notification about status change
                showSuccess(`Flight seat added successfully! Flight status automatically changed to ${result.data.newFlightStatusName}`);
            }
            
            // Reload data
            loadFlightSeats(currentFlightSeatsFlightId);
            loadAvailableSeats(currentFlightSeatsFlightId);
        } else {
            throw new Error(result.message || 'Failed to add flight seat');
        }
    } catch (error) {
        console.error('Error adding flight seat:', error);
        showError('Failed to add flight seat: ' + error.message);
    }
}

async function removeFlightSeat(flightId, seatId) {
    if (!confirm('Are you sure you want to remove this seat from the flight?')) {
        return;
    }
    
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${flightId}/seats/${seatId}`, {
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
            showSuccess('Flight seat removed successfully');
            
            // Reload data
            loadFlightSeats(flightId);
            loadAvailableSeats(flightId);
        } else {
            throw new Error(result.message || 'Failed to remove flight seat');
        }
    } catch (error) {
        console.error('Error removing flight seat:', error);
        showError('Failed to remove flight seat: ' + error.message);
    }
}

async function regenerateAllSeats() {
    if (!currentFlightSeatsFlightId) {
        showError('Flight ID is missing');
        return;
    }
    
    if (!confirm('This will regenerate all seats for this flight. Any existing seat assignments will be removed. Continue?')) {
        return;
    }
    
    try {
        const token = getAuthToken();
        const response = await fetch(`${API_CONFIG.BASE_URL}/${currentFlightSeatsFlightId}/seats/regenerate`, {
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
                console.log(`Flight ${currentFlightSeatsFlightId} status changed to: ${result.data.newFlightStatusName}`);
                
                // Update flight status in the main flights table
                updateFlightStatusInTable(currentFlightSeatsFlightId, result.data.newFlightStatusId, result.data.newFlightStatusName);
                
                // Update success message to include status change
                successMessage = `Flight seats regenerated successfully! Flight status automatically changed to ${result.data.newFlightStatusName}`;
            }
            
            showSuccess(successMessage);
            
            // Reload data
            loadFlightSeats(currentFlightSeatsFlightId);
            loadAvailableSeats(currentFlightSeatsFlightId);
        } else {
            throw new Error(result.message || 'Failed to regenerate flight seats');
        }
    } catch (error) {
        console.error('Error regenerating flight seats:', error);
        showError('Failed to regenerate flight seats: ' + error.message);
    }
}

function editFlightSeat(flightId, seatId) {
    // For now, just show info - you can implement editing later if needed
    showInfo(`Editing flight seat: Flight ${flightId}, Seat ${seatId}`);
}

/**
 * Updates flight status in the main flights table realtime without page reload
 */
function updateFlightStatusInTable(flightId, newStatusId, newStatusName) {
    try {
        console.log(`Updating flight ${flightId} status to ${newStatusName} (${newStatusId}) in table`);
        
        // Find the flight row in the table
        const flightsTable = document.querySelector('#flightsTable tbody');
        if (!flightsTable) {
            console.log('Flights table not found, skipping status update');
            return;
        }

        // Find the row with the matching flight ID by looking for the toggle button
        const rows = flightsTable.querySelectorAll('tr');
        for (const row of rows) {
            // Look for the toggle status button with this flight ID
            const toggleButton = row.querySelector(`button[onclick*="toggleFlightStatus(${flightId},"]`);
            if (toggleButton) {
                console.log(`Found row for flight ${flightId}`);
                
                // Find the status badge in this row (it's in the 6th column, index 5)
                const statusCell = row.children[5]; // Status column
                if (statusCell) {
                    const statusBadge = statusCell.querySelector('.badge');
                    if (statusBadge) {
                        // Update status text
                        statusBadge.textContent = newStatusName;
                        
                        // Update status badge classes
                        statusBadge.className = 'badge ' + (newStatusId === 1 ? 'bg-success' : 'bg-secondary');
                        
                        // Add a brief highlight effect to show the change
                        statusBadge.style.animation = 'pulse 1s ease-in-out';
                        setTimeout(() => {
                            statusBadge.style.animation = '';
                        }, 1000);
                        
                        console.log(`✅ Updated flight ${flightId} status badge to ${newStatusName}`);
                    }
                }
                
                // Also update the toggle button itself
                const newActionText = newStatusId === 1 ? 'Deactivate Flight' : 'Activate Flight';
                const newIcon = newStatusId === 1 ? 'fa-pause' : 'fa-play';
                const newButtonClass = newStatusId === 1 ? 'btn-outline-warning' : 'btn-outline-success';
                
                // Update button class
                toggleButton.className = `btn btn-sm ${newButtonClass}`;
                
                // Update button icon
                const buttonIcon = toggleButton.querySelector('i');
                if (buttonIcon) {
                    buttonIcon.className = `fas ${newIcon}`;
                }
                
                // Update button title
                toggleButton.title = newActionText;
                
                // Update onclick to reflect new current status
                const newOnclick = `toggleFlightStatus(${flightId}, ${newStatusId}, '${escapeHtml(row.children[0].querySelector('strong').textContent)}')`;
                toggleButton.setAttribute('onclick', newOnclick);
                
                console.log(`✅ Updated flight ${flightId} toggle button`);
                break;
            }
        }
    } catch (error) {
        console.error('Error updating flight status in table:', error);
    }
}
