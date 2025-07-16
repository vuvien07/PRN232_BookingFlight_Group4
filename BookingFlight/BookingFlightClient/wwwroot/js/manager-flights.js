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
    
    initializePage();
    loadFlights();
});

async function initializePage() {
    try {
        await Promise.all([
            loadStatuses(),
            loadAirports(),
            loadPlanes(),
            loadCustomers()
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
        console.log('About to call getAuthToken()...');
        const token = getAuthToken();
        console.log('loadFlights: token being sent:', token ? token.substring(0, 20) + '...' : 'NO TOKEN');
        console.log('=== LOAD FLIGHTS DEBUG END ===');
        
        const response = await fetch(API_CONFIG.BASE_URL + API_CONFIG.ENDPOINTS.LIST, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include',
            body: JSON.stringify(requestData)
        });
        
        console.log('loadFlights response status:', response.status);
        console.log('loadFlights response ok:', response.ok);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('loadFlights error response:', errorText);
            
            if (response.status === 401) {
                showError('Authentication failed. Please ensure you are logged in as a Manager.');
                return;
            }
            if (response.status === 500) {
                showError('Server error. Please check if you have Manager privileges and try again.');
                return;
            }
            throw new Error(`HTTP ${response.status}`);
        }
        
        const result = await response.json();
        if (result.success) {
            displayFlights(result.data);
            updatePagination(result.pagination);
        } else {
            showError(result.message || 'Failed to load flights');
        }
    } catch (error) {
        console.error('Error loading flights:', error);
        showError('Error loading flights: ' + error.message);
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
        const response = await fetch('http://localhost:5077/api/Airports', {
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
        const response = await fetch('http://localhost:5077/api/Planes', {
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

async function loadCustomers() {
    try {
        const response = await fetch('http://localhost:5077/api/Customers', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            credentials: 'include'
        });
        
        if (response.ok) {
            const customers = await response.json();
            populateDropdown('customerId', customers, 'customerId', 'fullname');
        } else {
            console.warn('Failed to load customers from API, using mock data');
            // Mock data for development
            const mockCustomers = [
                { customerId: 1, fullname: 'John Doe', email: 'john@example.com' },
                { customerId: 2, fullname: 'Jane Smith', email: 'jane@example.com' },
                { customerId: 3, fullname: 'Bob Johnson', email: 'bob@example.com' }
            ];
            populateDropdown('customerId', mockCustomers, 'customerId', 'fullname');
        }
    } catch (error) {
        console.error('Error loading customers:', error);
        // Mock data for development
        const mockCustomers = [
            { customerId: 1, fullname: 'John Doe', email: 'john@example.com' },
            { customerId: 2, fullname: 'Jane Smith', email: 'jane@example.com' },
            { customerId: 3, fullname: 'Bob Johnson', email: 'bob@example.com' }
        ];
        populateDropdown('customerId', mockCustomers, 'customerId', 'fullname');
    }
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
    
    tbody.innerHTML = flights.map(flight => `
        <tr>
            <td>
                <strong>${escapeHtml(flight.flightCode)}</strong>
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
                    <button type="button" class="btn btn-sm btn-outline-primary" 
                            onclick="editFlight(${flight.flightId})" title="Edit">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-info" 
                            onclick="viewFlightDetails(${flight.flightId})" title="View Details">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-danger" 
                            onclick="deleteFlight(${flight.flightId}, '${escapeHtml(flight.flightCode)}')" title="Delete">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
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
    document.getElementById('customerId').value = flight.customerId || '';
    document.getElementById('statusId').value = flight.statusId || '';
}

// Form handling
async function handleFlightSubmit(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const flightData = {
        flightCode: formData.get('flightCode'),
        tax: parseFloat(formData.get('tax')),
        departureTime: formData.get('departureTime'),
        arrivalTime: formData.get('arrivalTime'),
        departureAirportId: parseInt(formData.get('departureAirportId')),
        arrivalAirportId: parseInt(formData.get('arrivalAirportId')),
        planeId: parseInt(formData.get('planeId')),
        customerId: parseInt(formData.get('customerId')),
        statusId: parseInt(formData.get('statusId'))
    };
    
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

// Delete function
function deleteFlight(flightId, flightCode) {
    document.getElementById('deleteFlightInfo').textContent = `Flight: ${flightCode}`;
    
    const modal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    modal.show();
    
    document.getElementById('confirmDeleteBtn').onclick = async function() {
        try {
            const response = await fetch(`${API_CONFIG.BASE_URL}/${flightId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getAuthToken()}`
                },
                credentials: 'include'
            });
            
            const result = await response.json();
            
            if (result.success) {
                showSuccess('Flight deleted successfully');
                loadFlights();
            } else {
                showError(result.message || 'Failed to delete flight');
            }
        } catch (error) {
            console.error('Error deleting flight:', error);
            showError('Error deleting flight');
        }
        
        modal.hide();
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
    // Try to get JWT token from cookies first, then localStorage, sessionStorage
    const cookieToken = getCookie('X-Access-Token');
    if (cookieToken) {
        return cookieToken;
    }
    
    return localStorage.getItem('authToken') || 
           sessionStorage.getItem('authToken') || 
           localStorage.getItem('token') || 
           sessionStorage.getItem('token') || '';
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
    showError('Authentication required. Please login again.');
    setTimeout(() => {
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
