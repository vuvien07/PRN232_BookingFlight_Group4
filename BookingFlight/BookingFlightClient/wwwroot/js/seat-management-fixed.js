// Seat Management JavaScript for 150 seats aircraft layout
// Based on Boeing 737-800 configuration (25 rows x 6 seats)

// Configuration
const API_CONFIG = {
    BASE_URL: 'http://localhost:5077/api/FlightSeat', // Backend API URL  
    FLIGHT_ENDPOINT: 'http://localhost:5077/api/FlightSeat/flights', // Backend endpoint for flights
    SEAT_ENDPOINT: 'http://localhost:5077/api/FlightSeat', // Base endpoint for flight seats
};

// Global variables
let selectedFlightId = null;
let flights = [];
let seats = [];

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    console.log('Seat Management page initialized');
    console.log('Current URL:', window.location.href);
    console.log('Mock Mode: Auto-generating flights and seats');
    
    initializePage();
    loadMockFlightsAuto(); // Auto load mock flights instead of API call
});

// Initialize page
function initializePage() {
    console.log('Initializing seat management page...');
    
    // Initialize flight dropdown
    const flightSelect = document.getElementById('flightSelect');
    if (flightSelect) {
        flightSelect.addEventListener('change', function() {
            selectedFlightId = this.value;
            console.log('=== FLIGHT SELECTION DEBUG ===');
            console.log('Flight selected:', selectedFlightId);
            console.log('Selected option text:', this.options[this.selectedIndex].text);
            console.log('Selected option value:', this.value);
            console.log('================================');
            
            if (selectedFlightId && selectedFlightId !== '') {
                generateAndRender150Seats(selectedFlightId); // Always use mock seats
            } else {
                clearSeatsGrid();
            }
        });
    }

    // Load mock flights automatically on page load
    console.log('Auto-loading mock flights on page load...');
    loadMockFlightsAuto();
}

// Auto load mock flights (no API call needed)
function loadMockFlightsAuto() {
    console.log('Auto-generating mock flights...');
    
    const mockFlights = [
        {
            flightId: 1,
            flightNumber: 'VN101',
            departureAirport: 'HAN',
            arrivalAirport: 'SGN',
            departureTime: new Date().toISOString(),
            plane: 'Boeing 737-800'
        },
        {
            flightId: 2,
            flightNumber: 'VN202',
            departureAirport: 'SGN',
            arrivalAirport: 'DAD',
            departureTime: new Date(Date.now() + 2 * 60 * 60 * 1000).toISOString(),
            plane: 'Airbus A321'
        },
        {
            flightId: 3,
            flightNumber: 'VN303',
            departureAirport: 'DAD',
            arrivalAirport: 'HAN',
            departureTime: new Date(Date.now() + 4 * 60 * 60 * 1000).toISOString(),
            plane: 'Boeing 737-800'
        },
        {
            flightId: 4,
            flightNumber: 'VN404',
            departureAirport: 'SGN',
            arrivalAirport: 'CAN',
            departureTime: new Date(Date.now() + 6 * 60 * 60 * 1000).toISOString(),
            plane: 'Airbus A320'
        },
        {
            flightId: 5,
            flightNumber: 'VN505',
            departureAirport: 'HAN',
            arrivalAirport: 'PQC',
            departureTime: new Date(Date.now() + 8 * 60 * 60 * 1000).toISOString(),
            plane: 'Boeing 787'
        }
    ];
    
    flights = mockFlights;
    populateFlightDropdown(flights);
    showSuccessMessage(`Auto-loaded ${flights.length} mock flights successfully`);
    console.log('Mock flights loaded:', flights);
}

// Load functions
async function loadFlights() {
    try {
        console.log('=== LOAD FLIGHTS DEBUG START ===');
        showLoadingState();
        
        const token = getAuthToken();
        console.log('Token available:', !!token);
        
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
        
        console.log('Client Response status:', response.status);
        
        if (!response.ok) {
            console.log('Client endpoint failed');
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        console.log('Raw response data:', data);
        
        if (data && data.success !== false) {
            let flightData = [];
            
            // Handle different response formats
            if (data.data && Array.isArray(data.data)) {
                flightData = data.data;
            } else if (Array.isArray(data)) {
                flightData = data;
            } else if (data.flights && Array.isArray(data.flights)) {
                flightData = data.flights;
            }
            
            if (flightData.length === 0) {
                console.log('No flights found');
                showError('No flights available');
                return;
            }
            
            flights = flightData;
            populateFlightDropdown(flights);
            showSuccessMessage('Flights loaded successfully');
            
        } else {
            throw new Error(data.message || 'Failed to load flights');
        }
        
    } catch (error) {
        console.error('Failed to load flights:', error);
        showError(`Failed to load flights: ${error.message}`);
    } finally {
        hideLoadingState();
        console.log('=== LOAD FLIGHTS DEBUG END ===');
    }
}

async function loadFlightSeats(flightId) {
    try {
        console.log('=== LOAD FLIGHT SEATS DEBUG START ===');
        console.log('Flight ID:', flightId);
        
        showSeatsLoadingState();
        
        const token = getAuthToken();
        const endpoint = `${API_CONFIG.SEAT_ENDPOINT}/${flightId}/seats`;
        console.log('Seats endpoint:', endpoint);
        
        const response = await fetch(endpoint, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            credentials: 'include'
        });
        
        console.log('Seats Response status:', response.status);
        
        if (!response.ok) {
            console.log('Seats endpoint failed, generating mock data...');
            // Generate 150 seats mock data
            generateAndRender150Seats(flightId);
            return;
        }
        
        const data = await response.json();
        console.log('Seats raw response data:', data);
        
        let seatsData = [];
        if (data.data && Array.isArray(data.data)) {
            seatsData = data.data;
        } else if (Array.isArray(data)) {
            seatsData = data;
        }
        
        if (seatsData.length === 0) {
            console.log('No seats found, generating 150 mock seats...');
            generateAndRender150Seats(flightId);
            return;
        }
        
        seats = seatsData;
        renderSeatsGrid(seats);
        showSuccessMessage(`${seats.length} seats loaded successfully`);
        
    } catch (error) {
        console.error('Failed to load flight seats:', error);
        console.log('Generating 150 mock seats as fallback...');
        generateAndRender150Seats(flightId);
    } finally {
        hideSeatsLoadingState();
        console.log('=== LOAD FLIGHT SEATS DEBUG END ===');
    }
}

// Generate and render 150 seats for aircraft
function generateAndRender150Seats(flightId) {
    console.log('Generating 150 seats for flight:', flightId);
    
    const mockSeats = [];
    let seatId = 1;
    
    // Get flight info for seat patterns
    const selectedFlight = flights.find(f => f.flightId == flightId);
    const flightNumber = selectedFlight ? selectedFlight.flightNumber : `FL${flightId}`;
    
    // Different seat patterns based on flightId
    const occupancyRate = getOccupancyRateByFlight(flightId);
    const unavailableRate = getUnavailableRateByFlight(flightId);
    
    console.log(`Flight ${flightNumber}: ${occupancyRate * 100}% occupied, ${unavailableRate * 100}% unavailable`);
    
    // Generate 25 rows x 6 seats = 150 seats
    for (let row = 1; row <= 25; row++) {
        const positions = ['A', 'B', 'C', 'D', 'E', 'F'];
        
        positions.forEach(position => {
            // Use flight-specific patterns for seat status
            const random = Math.random();
            const isOccupied = random < occupancyRate;
            const isUnavailable = !isOccupied && random < (occupancyRate + unavailableRate);
            const isAvailable = !isOccupied && !isUnavailable;
            
            const seat = {
                seatId: seatId++,
                seatNumber: `${row}${position}`,
                flightId: flightId,
                isSat: isOccupied,
                isOccupied: isOccupied,
                isAvailable: isAvailable,
                ticketId: isOccupied ? Math.floor(Math.random() * 1000) + 1 : null,
                seat: {
                    seatId: seatId - 1,
                    seatNumber: `${row}${position}`,
                    classId: row <= 3 ? 1 : (row <= 6 ? 2 : 3), // Business, Premium, Economy
                    statusId: isAvailable ? 1 : (isOccupied ? 2 : 3) // Available, Occupied, Unavailable
                }
            };
            
            mockSeats.push(seat);
        });
    }
    
    seats = mockSeats;
    renderSeatsGrid(seats);
    
    const availableCount = seats.filter(s => s.isAvailable).length;
    const occupiedCount = seats.filter(s => s.isOccupied).length;
    const unavailableCount = seats.filter(s => !s.isAvailable && !s.isOccupied).length;
    
    showSuccessMessage(`Flight ${flightNumber}: ${availableCount} available, ${occupiedCount} occupied, ${unavailableCount} unavailable seats`);
}

// Get occupancy rate based on flight ID for variety
function getOccupancyRateByFlight(flightId) {
    const patterns = {
        1: 0.75, // VN101 - High occupancy (peak route)
        2: 0.45, // VN202 - Medium occupancy
        3: 0.25, // VN303 - Low occupancy
        4: 0.85, // VN404 - Very high occupancy (international)
        5: 0.60  // VN505 - Medium-high occupancy
    };
    return patterns[flightId] || 0.50; // Default 50%
}

// Get unavailable rate based on flight ID
function getUnavailableRateByFlight(flightId) {
    const patterns = {
        1: 0.05, // VN101 - Few maintenance issues
        2: 0.10, // VN202 - Some seats under maintenance
        3: 0.02, // VN303 - Almost all available
        4: 0.08, // VN404 - Some premium seats blocked
        5: 0.15  // VN505 - More seats blocked for upgrades
    };
    return patterns[flightId] || 0.05; // Default 5%
}

function renderSeatsGrid(seatsData) {
    const seatsContainer = document.getElementById('seatsContainer');
    if (!seatsContainer) {
        console.error('Seats container not found');
        return;
    }

    if (!seatsData || seatsData.length === 0) {
        seatsContainer.innerHTML = '<div class="alert alert-info">No seats found for this flight.</div>';
        return;
    }

    // Create aircraft layout with 150 seats (25 rows x 6 seats)
    let html = '<div class="seats-grid">';
    
    // Aircraft header
    html += '<div class="aircraft-header">✈️ Aircraft Seat Map - 150 Seats</div>';
    
    // Header with seat position labels
    html += '<div class="seat-row header-row">';
    html += '<div class="seat-label">Row</div>';
    html += '<div class="seat-label">A</div>';
    html += '<div class="seat-label">B</div>';
    html += '<div class="seat-label">C</div>';
    html += '<div class="seat-label">D</div>';
    html += '<div class="seat-label">E</div>';
    html += '<div class="seat-label">F</div>';
    html += '</div>';

    // Create seat map from data
    const seatMap = new Map();
    seatsData.forEach(seat => {
        const seatNumber = seat.seat?.seatNumber || seat.seatNumber || `${Math.floor((seat.seatId - 1) / 6) + 1}${String.fromCharCode(65 + ((seat.seatId - 1) % 6))}`;
        seatMap.set(seatNumber, {
            id: seat.seatId || seat.id,
            seatNumber: seatNumber,
            isOccupied: seat.isSat || seat.isOccupied || false,
            isAvailable: !seat.isSat && (seat.seat?.statusId === 1 || seat.statusId === 1 || seat.isAvailable),
            seatClass: getSeatClass(seatNumber),
            classId: seat.seat?.classId || seat.classId || 3,
            ticketId: seat.ticketId
        });
    });

    // Create 25 rows of seats
    for (let row = 1; row <= 25; row++) {
        html += '<div class="seat-row">';
        html += `<div class="seat-label">${row}</div>`;
        
        const positions = ['A', 'B', 'C', 'D', 'E', 'F'];
        positions.forEach((position, index) => {
            const seatNumber = `${row}${position}`;
            const seat = seatMap.get(seatNumber);
            
            if (seat) {
                const statusClass = seat.isOccupied ? 'occupied' : 
                                  seat.isAvailable ? 'available' : 'unavailable';
                const classType = seat.seatClass.toLowerCase();
                
                html += `<div class="seat ${statusClass} ${classType}" 
                             data-seat-id="${seat.id}" 
                             data-seat-number="${seat.seatNumber}"
                             data-row="${row}" 
                             data-position="${position}"
                             title="Seat ${seat.seatNumber} - ${seat.seatClass}${seat.isOccupied ? ' (Occupied)' : seat.isAvailable ? ' (Available)' : ' (Unavailable)'}">
                             ${seat.seatNumber}
                         </div>`;
            } else {
                // Create default seat if no data
                const defaultClass = row <= 3 ? 'business' : (row <= 6 ? 'premium' : 'economy');
                const defaultStatus = Math.random() > 0.7 ? 'occupied' : 'available';
                
                html += `<div class="seat ${defaultStatus} ${defaultClass}" 
                             data-seat-number="${seatNumber}"
                             data-row="${row}" 
                             data-position="${position}"
                             title="Seat ${seatNumber} - ${defaultClass} (${defaultStatus})">
                             ${seatNumber}
                         </div>`;
            }
            
            // Add aisle space after seat C (index 2)
            if (index === 2) {
                html += '<div style="width: 15px;"></div>';
            }
        });
        
        html += '</div>';
    }

    html += '</div>';
    
    // Add legend
    html += `
        <div class="seats-legend">
            <div class="legend-item">
                <div class="seat-demo available" style="position: relative;">
                    <span style="position: absolute; top: 1px; right: 2px; font-size: 10px; color: #00ff00; font-weight: bold; text-shadow: 1px 1px 2px rgba(0,0,0,0.8);">✓</span>
                </div>
                <span>Available ✓</span>
            </div>
            <div class="legend-item">
                <div class="seat-demo occupied" style="position: relative;">
                    <span style="position: absolute; top: 1px; right: 2px; font-size: 10px; color: #ffff00; font-weight: bold; text-shadow: 1px 1px 2px rgba(0,0,0,0.8);">✗</span>
                </div>
                <span>Occupied ✗</span>
            </div>
            <div class="legend-item">
                <div class="seat-demo unavailable" style="position: relative;">
                    <span style="position: absolute; top: 1px; right: 2px; font-size: 10px; color: #ff6600; font-weight: bold; text-shadow: 1px 1px 2px rgba(0,0,0,0.8);">⊘</span>
                </div>
                <span>Unavailable ⊘</span>
            </div>
            <div class="legend-item">
                <div class="seat-demo business"></div>
                <span>Business Class</span>
            </div>
            <div class="legend-item">
                <div class="seat-demo premium"></div>
                <span>Premium Class</span>
            </div>
            <div class="legend-item">
                <div class="seat-demo economy"></div>
                <span>Economy Class</span>
            </div>
        </div>
    `;

    seatsContainer.innerHTML = html;
    
    // Add seat click listeners
    addSeatClickListeners();
    
    console.log(`Rendered ${seatsData.length} seats in aircraft layout`);
}

// Helper function to determine seat class
function getSeatClass(seatNumber) {
    const row = parseInt(seatNumber);
    if (row <= 3) return 'business';  // Rows 1-3: Business Class (18 seats)
    if (row <= 7) return 'premium';   // Rows 4-7: Premium Class (24 seats)
    return 'economy';                 // Rows 8-25: Economy Class (108 seats)
}

// Add event listeners for seat clicks
function addSeatClickListeners() {
    const seats = document.querySelectorAll('.seat:not(.empty)');
    seats.forEach(seat => {
        seat.addEventListener('click', function() {
            if (!this.classList.contains('occupied') && !this.classList.contains('unavailable')) {
                const seatNumber = this.getAttribute('data-seat-number');
                const seatId = this.getAttribute('data-seat-id');
                const currentStatus = this.classList.contains('occupied') ? 'occupied' : 'available';
                
                console.log(`Seat ${seatNumber} (ID: ${seatId}) clicked - Current status: ${currentStatus}`);
                
                // Manager can change seat status here
                // For demonstration, we'll just log the action
                updateSeatStatus(seatId, seatNumber, currentStatus);
            }
        });
    });
}

// Update seat status (placeholder for manager functionality)
function updateSeatStatus(seatId, seatNumber, currentStatus) {
    console.log(`Manager can update seat ${seatNumber} (ID: ${seatId}) from status: ${currentStatus}`);
    // This would call an API to update the seat status
    // For now, just log the action
}

// UI Helper functions
function populateFlightDropdown(flightData) {
    const flightSelect = document.getElementById('flightSelect');
    if (!flightSelect) {
        console.error('Flight select element not found');
        return;
    }

    // Clear existing options except the first one
    while (flightSelect.options.length > 1) {
        flightSelect.remove(1);
    }

    if (!flightData || flightData.length === 0) {
        console.log('No flight data to populate');
        return;
    }

    console.log('=== POPULATING FLIGHT DROPDOWN ===');
    flightData.forEach((flight, index) => {
        const option = document.createElement('option');
        
        console.log(`Processing flight ${index + 1}:`, flight);
        
        // Handle different flight object structures
        const flightId = flight.flightId || flight.id || flight.FlightId || flight.ID || 1;
        const flightNumber = flight.flightNumber || flight.number || flight.FlightNumber || flight.Number || flight.code || `FL${flightId}`;
        const departure = flight.departureAirport || flight.departure || flight.DepartureAirport || flight.from || flight.origin || 'HAN';
        const arrival = flight.arrivalAirport || flight.arrival || flight.ArrivalAirport || flight.to || flight.destination || 'SGN';
        const departureTime = flight.departureTime || flight.DepartureTime || flight.departureDate || flight.date;
        
        console.log(`Parsed values for flight ${index + 1}:`, { flightId, flightNumber, departure, arrival, departureTime });
        
        option.value = flightId;
        
        // Format display text
        let displayText = flightNumber;
        if (departure && arrival) {
            displayText += ` - ${departure} to ${arrival}`;
        }
        if (departureTime) {
            const date = new Date(departureTime);
            if (!isNaN(date.getTime())) {
                displayText += ` (${date.toLocaleDateString()})`;
            }
        }
        
        console.log(`Flight option: value="${flightId}", text="${displayText}"`);
        option.textContent = displayText;
        flightSelect.appendChild(option);
    });

    console.log(`Populated dropdown with ${flightData.length} flights`);
    console.log('=====================================');
}

function clearSeatsGrid() {
    const seatsContainer = document.getElementById('seatsContainer');
    if (seatsContainer) {
        seatsContainer.innerHTML = '<div class="alert alert-secondary">Please select a flight to view seats.</div>';
    }
}

// Loading states
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
    // Seats container will be updated by renderSeatsGrid
}

// Test functions
function loadMockFlights() {
    console.log('Loading mock flights...');
    
    const mockFlights = [
        {
            flightId: 1,
            flightNumber: 'VN101',
            departureAirport: 'HAN',
            arrivalAirport: 'SGN',
            departureTime: new Date().toISOString()
        },
        {
            flightId: 2,
            flightNumber: 'VN202',
            departureAirport: 'SGN',
            arrivalAirport: 'DAD',
            departureTime: new Date().toISOString()
        }
    ];
    
    flights = mockFlights;
    populateFlightDropdown(flights);
    showSuccessMessage('Mock flights loaded successfully');
}

function loadMockSeats() {
    if (selectedFlightId) {
        generateAndRender150Seats(selectedFlightId);
    } else {
        generateAndRender150Seats(1);
    }
}

// Test backend direct connectivity
async function testBackendDirect() {
    try {
        console.log('=== TESTING BACKEND DIRECT ===');
        
        const flightId = selectedFlightId || 1;
        console.log('Testing with flightId:', flightId);
        
        // Test backend directly
        const backendUrl = `http://localhost:5077/api/FlightSeat/${flightId}/seats`;
        console.log('Backend URL:', backendUrl);
        
        const response = await fetch(backendUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        console.log('Backend Response status:', response.status);
        
        if (response.ok) {
            const data = await response.json();
            console.log('Backend Response data:', data);
            showSuccessMessage(`Backend test successful! Found ${data.data?.length || 0} seats for flight ${flightId}`);
        } else {
            const errorText = await response.text();
            console.log('Backend error:', errorText);
            showError(`Backend test failed: ${response.status} - ${errorText}`);
        }
    } catch (error) {
        console.error('Backend direct test failed:', error);
        showError('Backend direct test failed: ' + error.message);
    }
}

// Utility functions
function getAuthToken() {
    // Try to get token from localStorage first
    let token = localStorage.getItem('authToken');
    
    if (!token) {
        // Try to get from cookie
        const cookies = document.cookie.split(';');
        for (let cookie of cookies) {
            const [name, value] = cookie.trim().split('=');
            if (name === 'authToken' || name === 'jwt' || name === 'token') {
                token = value;
                break;
            }
        }
    }
    
    if (!token) {
        // Try to get from sessionStorage
        token = sessionStorage.getItem('authToken');
    }
    
    return token;
}

function showSuccessMessage(message) {
    console.log('Success:', message);
    
    // Create and show a success message element
    const existingMessage = document.querySelector('.success-message');
    if (existingMessage) {
        existingMessage.remove();
    }
    
    const messageDiv = document.createElement('div');
    messageDiv.className = 'success-message';
    messageDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #d4edda;
        color: #155724;
        padding: 10px 15px;
        border: 1px solid #c3e6cb;
        border-radius: 4px;
        box-shadow: 0 2px 5px rgba(0,0,0,0.2);
        z-index: 1000;
        max-width: 300px;
    `;
    messageDiv.textContent = message;
    
    document.body.appendChild(messageDiv);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        if (messageDiv.parentNode) {
            messageDiv.parentNode.removeChild(messageDiv);
        }
    }, 3000);
}

function showError(message) {
    console.error('Error:', message);
    
    // Show error message in the appropriate container
    const flightSelect = document.getElementById('flightSelect');
    const seatsContainer = document.getElementById('seatsContainer');
    
    if (message.includes('flights')) {
        // Error loading flights
        if (flightSelect) {
            flightSelect.innerHTML = '<option value="">Error loading flights</option>';
        }
    } else if (message.includes('seats')) {
        // Error loading seats
        if (seatsContainer) {
            seatsContainer.innerHTML = `<div class="alert alert-danger">Error: ${message}</div>`;
        }
    }
}

// Export functions for global access
window.SeatManagement = {
    loadFlights: loadFlights,
    loadFlightSeats: loadFlightSeats,
    loadMockFlights: loadMockFlights,
    generateAndRender150Seats: generateAndRender150Seats
};
