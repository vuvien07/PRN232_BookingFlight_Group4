// Seat Management JavaScript
// Based on manager-flights.js logic

// Configuration
const API_CONFIG = {
    BASE_URL: 'http://localhost:5077/api/Seat', // Backend API URL
    FLIGHT_ENDPOINT: '/SeatManagement/api/flights/list', // Client-side endpoint for flights
    ENDPOINTS: {
        FLIGHTS: '/flights',
        SEATS: '/seats',
        UPDATE_SEAT: '/update-seat',
        GET_FLIGHT_SEATS: '/flight-seats'
    }
};

// Global variables
let currentPage = 1;
let pageSize = 10;
let totalPages = 0;
let currentFilters = {};
let selectedFlightId = null;
let flights = [];
let seats = [];

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    console.log('Seat Management page initialized');
    console.log('Current URL:', window.location.href);
    console.log('API Base URL:', API_CONFIG.BASE_URL);
    
    // Test API connectivity first
    testAPIConnectivity().then(() => {
        initializePage();
        loadFlights();
    }).catch(error => {
        console.error('API connectivity test failed:', error);
        showError('Failed to connect to API server. Please check if the backend is running.');
        // Load mock data as fallback
        loadMockData();
    });
});

// Test API connectivity
async function testAPIConnectivity() {
    try {
        console.log('Testing API connectivity...');
        const response = await fetch('http://localhost:5077/api/health', {
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
        throw error;
    }
}

async function initializePage() {
    try {
        setupEventListeners();
        console.log('Page initialized successfully');
    } catch (error) {
        console.error('Error initializing page:', error);
        showError('Failed to initialize page');
    }
}

function setupEventListeners() {
    // Flight selection change
    const flightSelect = document.getElementById('flightSelect');
    if (flightSelect) {
        flightSelect.addEventListener('change', function() {
            selectedFlightId = this.value;
            console.log('Flight selected:', selectedFlightId);
            if (selectedFlightId) {
                loadFlightSeats(selectedFlightId);
            } else {
                clearSeatsGrid();
            }
        });
    }

    // Test buttons
    document.getElementById('testFlights')?.addEventListener('click', loadMockFlights);
    document.getElementById('testSeats')?.addEventListener('click', loadMockSeats);
    document.getElementById('testAPI')?.addEventListener('click', testAPIConnectivity);
    document.getElementById('debugInfo')?.addEventListener('click', showDebugInfo);
}

// Load functions
async function loadFlights() {
    try {
        console.log('=== LOAD FLIGHTS DEBUG START ===');
        showLoadingState();
        
        const token = getAuthToken();
        console.log('Token available:', !!token);
        console.log('Token preview:', token ? token.substring(0, 30) + '...' : 'NO TOKEN');
        
        // Try client-side endpoint first
        const clientEndpoint = API_CONFIG.FLIGHT_ENDPOINT;
        console.log('Trying client endpoint:', clientEndpoint);
        
        const response = await fetch(clientEndpoint, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include'
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
                console.log('Client endpoint not found, trying backend API...');
                return await loadFlightsFromBackend();
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
        
        if (result && result.success !== false) {
            flights = Array.isArray(result) ? result : (result.data || result.flights || []);
            console.log('Flights loaded successfully:', flights.length);
            populateFlightDropdown(flights);
            hideLoadingState();
        } else {
            console.error('API returned error:', result.message || 'Unknown error');
            throw new Error(result.message || 'Failed to load flights');
        }
        
        console.log('=== LOAD FLIGHTS DEBUG END ===');
        
    } catch (error) {
        console.error('Error loading flights:', error);
        hideLoadingState();
        showError('Failed to load flights. Loading mock data as fallback.');
        loadMockFlights();
    }
}

async function loadFlightsFromBackend() {
    try {
        console.log('Trying backend API for flights...');
        const token = getAuthToken();
        
        const backendUrl = 'http://localhost:5077/api/FlightManage/list';
        console.log('Backend URL:', backendUrl);
        
        const requestData = {
            page: 1,
            pageSize: 100
        };
        
        const response = await fetch(backendUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include',
            body: JSON.stringify(requestData)
        });
        
        console.log('Backend response status:', response.status);
        
        if (!response.ok) {
            throw new Error(`Backend API error: ${response.status}`);
        }
        
        const responseText = await response.text();
        const result = JSON.parse(responseText);
        
        if (result.success) {
            flights = result.data || [];
            console.log('Flights loaded from backend:', flights.length);
            populateFlightDropdown(flights);
            hideLoadingState();
        } else {
            throw new Error(result.message || 'Backend API returned error');
        }
        
    } catch (error) {
        console.error('Backend API also failed:', error);
        throw error;
    }
}

async function loadFlightSeats(flightId) {
    try {
        console.log('=== LOAD FLIGHT SEATS DEBUG START ===');
        console.log('Loading seats for flight:', flightId);
        
        showSeatsLoadingState();
        
        const token = getAuthToken();
        console.log('Token available:', !!token);
        
        // Try backend API endpoint
        const apiUrl = `${API_CONFIG.BASE_URL}/flight/${flightId}/seats`;
        console.log('API URL:', apiUrl);
        
        const response = await fetch(apiUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include'
        });
        
        console.log('Seats response status:', response.status);
        
        if (!response.ok) {
            console.error('Failed to load seats from API, status:', response.status);
            if (response.status === 404) {
                console.log('Seats endpoint not found, loading mock data...');
                loadMockSeats();
                return;
            }
            throw new Error(`HTTP ${response.status}`);
        }
        
        const responseText = await response.text();
        const result = JSON.parse(responseText);
        
        if (result && result.success !== false) {
            seats = Array.isArray(result) ? result : (result.data || result.seats || []);
            console.log('Seats loaded successfully:', seats.length);
            displaySeatsGrid(seats);
            hideSeatsLoadingState();
        } else {
            throw new Error(result.message || 'Failed to load seats');
        }
        
        console.log('=== LOAD FLIGHT SEATS DEBUG END ===');
        
    } catch (error) {
        console.error('Error loading flight seats:', error);
        hideSeatsLoadingState();
        showError('Failed to load seats. Loading mock data as fallback.');
        loadMockSeats();
    }
}

// Mock data functions
function loadMockData() {
    console.log('Loading mock data...');
    loadMockFlights();
    loadMockSeats();
}

function loadMockFlights() {
    console.log('Loading mock flights...');
    const mockFlights = [
        { flightId: 1, flightNumber: 'VN101', departureCity: 'Ho Chi Minh', arrivalCity: 'Ha Noi', departureDate: '2024-01-15' },
        { flightId: 2, flightNumber: 'VN102', departureCity: 'Ha Noi', arrivalCity: 'Da Nang', departureDate: '2024-01-16' },
        { flightId: 3, flightNumber: 'VN103', departureCity: 'Da Nang', arrivalCity: 'Ho Chi Minh', departureDate: '2024-01-17' }
    ];
    
    flights = mockFlights;
    populateFlightDropdown(mockFlights);
    showSuccess('Mock flights loaded successfully');
}

function loadMockSeats() {
    console.log('Loading mock seats...');
    const mockSeats = [];
    
    // Generate mock seats: A1-F10 (60 seats)
    const rows = ['A', 'B', 'C', 'D', 'E', 'F'];
    const cols = 10;
    
    for (let row of rows) {
        for (let col = 1; col <= cols; col++) {
            const seatNumber = `${row}${col}`;
            const isBooked = Math.random() < 0.3; // 30% chance of being booked
            mockSeats.push({
                seatId: mockSeats.length + 1,
                seatNumber: seatNumber,
                seatClass: col <= 3 ? 'Business' : 'Economy',
                isAvailable: !isBooked,
                isBooked: isBooked,
                passengerName: isBooked ? `Passenger ${mockSeats.length + 1}` : null
            });
        }
    }
    
    seats = mockSeats;
    displaySeatsGrid(mockSeats);
    showSuccess('Mock seats loaded successfully');
}

// UI functions
function populateFlightDropdown(flightData) {
    const flightSelect = document.getElementById('flightSelect');
    if (!flightSelect) {
        console.error('Flight select element not found');
        return;
    }
    
    // Clear existing options
    flightSelect.innerHTML = '<option value="">Select a flight...</option>';
    
    flightData.forEach(flight => {
        const option = document.createElement('option');
        option.value = flight.flightId || flight.id;
        option.textContent = `${flight.flightNumber || flight.number} - ${flight.departureCity || flight.from} to ${flight.arrivalCity || flight.to} (${flight.departureDate || flight.date})`;
        flightSelect.appendChild(option);
    });
    
    console.log('Flight dropdown populated with', flightData.length, 'flights');
}

function displaySeatsGrid(seatData) {
    const seatsContainer = document.getElementById('seatsContainer');
    if (!seatsContainer) {
        console.error('Seats container not found');
        return;
    }
    
    // Clear existing content
    seatsContainer.innerHTML = '';
    
    if (!seatData || seatData.length === 0) {
        seatsContainer.innerHTML = '<div class="alert alert-info">No seats found for this flight.</div>';
        return;
    }
    
    // Group seats by row
    const seatsByRow = {};
    seatData.forEach(seat => {
        const row = seat.seatNumber ? seat.seatNumber.charAt(0) : 'X';
        if (!seatsByRow[row]) {
            seatsByRow[row] = [];
        }
        seatsByRow[row].push(seat);
    });
    
    // Create seats grid
    const gridHtml = `
        <div class="seats-legend mb-3">
            <span class="legend-item"><span class="seat-demo available"></span> Available</span>
            <span class="legend-item"><span class="seat-demo booked"></span> Booked</span>
            <span class="legend-item"><span class="seat-demo business"></span> Business</span>
            <span class="legend-item"><span class="seat-demo economy"></span> Economy</span>
        </div>
        <div class="seats-grid">
            ${Object.keys(seatsByRow).sort().map(row => `
                <div class="seat-row">
                    <div class="row-label">${row}</div>
                    <div class="seats">
                        ${seatsByRow[row].sort((a, b) => {
                            const numA = parseInt(a.seatNumber.slice(1));
                            const numB = parseInt(b.seatNumber.slice(1));
                            return numA - numB;
                        }).map(seat => `
                            <div class="seat ${getSeatClasses(seat)}" 
                                 data-seat-id="${seat.seatId || seat.id}"
                                 data-seat-number="${seat.seatNumber}"
                                 title="${getSeatTooltip(seat)}">
                                ${seat.seatNumber}
                            </div>
                        `).join('')}
                    </div>
                </div>
            `).join('')}
        </div>
        <div class="mt-3">
            <p><strong>Total Seats:</strong> ${seatData.length}</p>
            <p><strong>Available:</strong> ${seatData.filter(s => s.isAvailable).length}</p>
            <p><strong>Booked:</strong> ${seatData.filter(s => s.isBooked).length}</p>
        </div>
    `;
    
    seatsContainer.innerHTML = gridHtml;
    
    // Add click event listeners to seats
    seatsContainer.querySelectorAll('.seat').forEach(seatElement => {
        seatElement.addEventListener('click', function() {
            const seatId = this.dataset.seatId;
            const seatNumber = this.dataset.seatNumber;
            handleSeatClick(seatId, seatNumber);
        });
    });
    
    console.log('Seats grid displayed with', seatData.length, 'seats');
}

function getSeatClasses(seat) {
    let classes = [];
    
    if (seat.isAvailable) {
        classes.push('available');
    }
    if (seat.isBooked) {
        classes.push('booked');
    }
    if (seat.seatClass === 'Business') {
        classes.push('business');
    } else {
        classes.push('economy');
    }
    
    return classes.join(' ');
}

function getSeatTooltip(seat) {
    if (seat.isBooked && seat.passengerName) {
        return `${seat.seatNumber} - ${seat.seatClass} - Booked by: ${seat.passengerName}`;
    } else if (seat.isBooked) {
        return `${seat.seatNumber} - ${seat.seatClass} - Booked`;
    } else {
        return `${seat.seatNumber} - ${seat.seatClass} - Available`;
    }
}

function handleSeatClick(seatId, seatNumber) {
    console.log('Seat clicked:', seatId, seatNumber);
    // Here you can add seat management functionality
    // For now, just show info
    showSuccess(`Seat ${seatNumber} clicked (ID: ${seatId})`);
}

function clearSeatsGrid() {
    const seatsContainer = document.getElementById('seatsContainer');
    if (seatsContainer) {
        seatsContainer.innerHTML = '<div class="alert alert-secondary">Please select a flight to view seats.</div>';
    }
}

function showLoadingState() {
    const flightSelect = document.getElementById('flightSelect');
    if (flightSelect) {
        flightSelect.innerHTML = '<option value="">Loading flights...</option>';
        flightSelect.disabled = true;
    }
}

function hideLoadingState() {
    const flightSelect = document.getElementById('flightSelect');
    if (flightSelect) {
        flightSelect.disabled = false;
    }
}

function showSeatsLoadingState() {
    const seatsContainer = document.getElementById('seatsContainer');
    if (seatsContainer) {
        seatsContainer.innerHTML = '<div class="alert alert-info">Loading seats...</div>';
    }
}

function hideSeatsLoadingState() {
    // This is handled by displaySeatsGrid
}

function showDebugInfo() {
    const debugInfo = {
        selectedFlightId: selectedFlightId,
        flightsCount: flights.length,
        seatsCount: seats.length,
        authToken: getAuthToken() ? 'Available' : 'Not found',
        apiConfig: API_CONFIG,
        cookies: document.cookie
    };
    
    console.log('=== DEBUG INFO ===');
    console.log(debugInfo);
    console.log('=== END DEBUG INFO ===');
    
    alert('Debug info logged to console. Press F12 to view.');
}

// Authentication functions (copied from manager-flights.js)
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
    alert(`Success: ${message}`);
}

function showError(message) {
    console.error('Error:', message);
    alert(`Error: ${message}`);
}

// Export functions for global access
window.SeatManagement = {
    loadFlights: loadFlights,
    loadFlightSeats: loadFlightSeats,
    loadMockFlights: loadMockFlights,
    loadMockSeats: loadMockSeats,
    showDebugInfo: showDebugInfo
};
