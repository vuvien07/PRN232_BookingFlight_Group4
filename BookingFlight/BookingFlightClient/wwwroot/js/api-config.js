// API Configuration
const API_CONFIG = {
    BASE_URL: 'http://localhost:5077', // Changed from HTTPS to HTTP
    ENDPOINTS: {
        SERVICES: {
            LIST: '/api/manager/services/list',
            DETAILS: '/api/Manager/services',
            CREATE: '/api/Manager/services',
            UPDATE: '/api/Manager/services',
            DELETE: '/api/Manager/services',
            STATUSES: '/api/Manager/services/statuses'
        },
        ITEMS: {
            LIST: '/api/Manager/items/list',
            ACTIVE: '/api/Manager/items',
            CREATE: '/api/Manager/items',
            UPDATE: '/api/Manager/items',
            DELETE: '/api/Manager/items'
        }
    }
};

// Helper function to build API URLs
function buildApiUrl(endpoint, params = {}) {
    let url = API_CONFIG.BASE_URL + endpoint;
    
    // Replace parameters in URL
    Object.keys(params).forEach(key => {
        url = url.replace(`:${key}`, params[key]);
    });
    
    return url;
}

// Export for use in other files
window.API_CONFIG = API_CONFIG;
window.buildApiUrl = buildApiUrl;
