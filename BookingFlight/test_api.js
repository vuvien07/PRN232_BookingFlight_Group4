console.log('Testing Flight Seat Management API...');

const API_BASE_URL = 'http://localhost:5077/api';

async function testFlightAPI() {
    try {
        console.log('Testing GET /api/Flight/GetAllFlights');
        const response = await fetch(`${API_BASE_URL}/Flight/GetAllFlights`);
        console.log('Response status:', response.status);
        console.log('Response ok:', response.ok);
        
        if (!response.ok) {
            console.error('Response not OK:', response.statusText);
            return;
        }
        
        const text = await response.text();
        console.log('Raw response length:', text.length);
        console.log('Raw response (first 500 chars):', text.substring(0, 500));
        
        if (!text.trim()) {
            console.error('Empty response received');
            return;
        }
        
        try {
            const data = JSON.parse(text);
            console.log('Parsed data:', data);
            
            if (data.success) {
                console.log('API call successful!');
                console.log('Number of flights:', data.data?.length || 0);
            } else {
                console.log('API returned success=false:', data.message);
            }
        } catch (jsonError) {
            console.error('JSON Parse Error:', jsonError.message);
            console.error('Response text:', text);
        }
        
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

// Test Flight Seat API
async function testFlightSeatAPI() {
    try {
        // First get a flight ID
        const flightResponse = await fetch(`${API_BASE_URL}/Flight/GetAllFlights`);
        const flightData = await flightResponse.json();
        
        if (flightData.success && flightData.data && flightData.data.length > 0) {
            const flightId = flightData.data[0].flightId;
            console.log(`Testing FlightSeat API with flightId: ${flightId}`);
            
            // Test seats endpoint
            console.log('Testing GET /api/FlightSeat/{flightId}/seats');
            const seatsResponse = await fetch(`${API_BASE_URL}/FlightSeat/${flightId}/seats`);
            console.log('Seats response status:', seatsResponse.status);
            
            const seatsText = await seatsResponse.text();
            console.log('Seats response:', seatsText.substring(0, 200));
            
            // Test statistics endpoint
            console.log('Testing GET /api/FlightSeat/{flightId}/seats/statistics');
            const statsResponse = await fetch(`${API_BASE_URL}/FlightSeat/${flightId}/seats/statistics`);
            console.log('Stats response status:', statsResponse.status);
            
            const statsText = await statsResponse.text();
            console.log('Stats response:', statsText.substring(0, 200));
        }
    } catch (error) {
        console.error('FlightSeat API test error:', error);
    }
}

// Run tests
testFlightAPI().then(() => {
    console.log('Flight API test completed');
    return testFlightSeatAPI();
}).then(() => {
    console.log('All tests completed');
});
