// My Flights JavaScript - Main functionality for the flight management page

// Test basic functionality
console.log('🚀 MyFlights.js loaded successfully');

// Test global function availability
console.log('🔍 Global getAuthToken available:', typeof window.getAuthToken === 'function');

// Define global getAuthToken function if not available
if (typeof window.getAuthToken !== 'function') {
    console.log('🔧 Defining global getAuthToken function');
    window.getAuthToken = function() {
        console.log('🌐 Global getAuthToken called');
        
        // Helper function to get cookie
        const getCookie = (name) => {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts.length === 2) return parts.pop().split(';').shift();
            return null;
        };
        
        // Try to get JWT token from cookies first, then localStorage, sessionStorage
        const cookieToken = getCookie('X-Access-Token');
        const localStorageAuthToken = localStorage.getItem('authToken');
        const sessionStorageAuthToken = sessionStorage.getItem('authToken');
        const localStorageToken = localStorage.getItem('token');
        const sessionStorageToken = sessionStorage.getItem('token');
        const flightInfoToken = localStorage.getItem('flightInfoToken');
        const flightCheckoutToken = localStorage.getItem('flightCheckoutToken');
        const accessToken = localStorage.getItem('access_token') || sessionStorage.getItem('access_token');
        const altCookieToken = getCookie('authToken') || getCookie('token') || getCookie('jwt');
        
        const finalToken = cookieToken || 
                          localStorageAuthToken || 
                          sessionStorageAuthToken || 
                          localStorageToken || 
                          sessionStorageToken ||
                          accessToken ||
                          altCookieToken || '';
        
        // DO NOT include flight-related tokens like flightInfoToken, flightCheckoutToken
        
        console.log('🌐 Global getAuthToken result:', finalToken ? finalToken.substring(0, 30) + '...' : 'NO TOKEN');
        return finalToken;
    };
    console.log('✅ Global getAuthToken function defined');
}

class MyFlightsManager {
    constructor() {
        // Detect current host and adjust API URL accordingly
        const currentHost = window.location.hostname;
        const currentPort = window.location.port;
        console.log('🌐 Current host:port:', currentHost + ':' + currentPort);
        
        // SOLUTION: Try client proxy first (since user is logged in), then direct server
        this.apiUrls = [
            `/MyFlights/api`,  // Primary: Through client proxy (user is authenticated here)
            `http://${currentHost}:5077/api`   // Fallback: HTTP API server direct
        ];
        this.currentApiIndex = 0;
        this.apiBaseUrl = this.apiUrls[0];
        console.log('🔗 Primary API URL:', this.apiBaseUrl);
        console.log('🔗 Fallback API URLs:', this.apiUrls.slice(1));
        
        this.currentYear = new Date().getFullYear();
        this.currentMonth = new Date().getMonth() + 1;
        this.flightsData = {
            stats: {},
            calendar: {},
            flights: []
        };
        this.currentLanguage = this.getLanguageCookie() || 'vn';
        
        console.log('🔧 MyFlightsManager constructor called');
        console.log('📍 API Base URL:', this.apiBaseUrl);
        console.log('🌐 Language:', this.currentLanguage);
        
        this.init();
    }

    init() {
        console.log('🔄 MyFlightsManager init() called');
        this.setupEventListeners();
        this.loadInitialData();
        this.setupLanguageSupport();
        console.log('✅ MyFlightsManager initialized');
    }

    setupEventListeners() {
        document.addEventListener('shown.bs.tab', (event) => {
            const tabId = event.target.getAttribute('data-bs-target');
            this.handleTabChange(tabId);
        });

        const searchInput = document.getElementById('flightSearch');
        if (searchInput) {
            searchInput.addEventListener('input', this.debounce(() => {
                this.handleSearch();
            }, 500));

            searchInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') {
                    this.handleSearch();
                }
            });
        }

        this.setupCalendarNavigation();
    }

    setupCalendarNavigation() {
        const prevBtn = document.getElementById('prevMonth');
        const nextBtn = document.getElementById('nextMonth');

        if (prevBtn) {
            prevBtn.addEventListener('click', () => this.navigateMonth(-1));
        }

        if (nextBtn) {
            nextBtn.addEventListener('click', () => this.navigateMonth(1));
        }
    }

    setupLanguageSupport() {
        const langSelect = document.getElementById('languageSelect');
        if (langSelect) {
            langSelect.value = this.currentLanguage;
            langSelect.addEventListener('change', (e) => {
                this.changeLanguage(e.target.value);
            });
        }

        this.updateLanguageElements();
    }

    updateLanguageElements() {
        if (typeof window.updateLanguage === 'function') {
            window.updateLanguage(this.currentLanguage);
        }
    }

    async loadInitialData() {
        console.log('=== MyFlights: Starting to load initial data ===');
        this.showLoading(true);
        
        try {
            // Load flight statistics and calendar data
            await this.loadFlightStats();
            await this.loadCalendarData(this.currentYear, this.currentMonth);
        } catch (error) {
            console.error('Error loading initial data:', error);
            this.showError('Không thể tải dữ liệu. Vui lòng thử lại sau.');
            this.showLoading(false);
        }
    }

    async loadFlightStats() {
        try {
            console.log('🚀 Loading flight stats...');
            
            // Try each API URL until one works
            let lastError = null;
            let success = false;
            
            for (let i = 0; i < this.apiUrls.length && !success; i++) {
                const apiUrl = this.apiUrls[i];
                const isProxy = apiUrl.includes('MyFlights');
                console.log(`🌐 Attempting API URL ${i + 1}/${this.apiUrls.length}: ${apiUrl} (${isProxy ? 'proxy' : 'direct'})`);
                
                try {
                    const token = this.getAuthToken();
                    console.log(`🔑 Token available: ${token ? 'YES' : 'NO'}`);
                    
                    const apiEndpoint = isProxy ? `${apiUrl}/stats` : `${apiUrl}/MyFlight/stats`;
                    
                    const headers = {
                        'Content-Type': 'application/json'
                    };
                    
                    // Add Authorization header if we have a token
                    if (token) {
                        headers['Authorization'] = `Bearer ${token}`;
                    }
                    
                    const response = await fetch(apiEndpoint, {
                        method: 'GET',
                        headers: headers,
                        credentials: 'include'  // Always include cookies for proxy
                    });

                    console.log(`📊 Stats API response status (${apiUrl}):`, response.status);
                    
                    if (response.ok) {
                        const result = await response.json();
                        console.log('📈 Stats API result:', result);
                        
                        if (result.success) {
                            console.log(`✅ API call succeeded with ${apiUrl}!`);
                            this.apiBaseUrl = apiUrl; // Switch permanently to working URL
                            this.flightsData.stats = result.data;
                            this.updateStatsDisplay();
                            console.log('✅ Flight stats loaded successfully');
                            success = true;
                            break;
                        } else {
                            console.error('API returned success=false:', result);
                            lastError = new Error('API trả về lỗi: ' + (result.message || 'Unknown error'));
                        }
                    } else if (response.status === 401) {
                        console.error(`🔐 401 Unauthorized from ${apiUrl}`);
                        const responseText = await response.text();
                        lastError = {
                            is401: true,
                            status: response.status,
                            responseText,
                            apiUrl,
                            isProxy
                        };
                    } else {
                        const responseText = await response.text();
                        lastError = new Error(`HTTP error! status: ${response.status}, body: ${responseText}`);
                    }
                } catch (fetchError) {
                    console.error(`❌ Error with API URL ${apiUrl}:`, fetchError);
                    lastError = fetchError;
                }
            }
            
            // If all URLs failed, show error
            if (!success) {
                if (lastError && lastError.is401) {
                    this.showAuthenticationError(lastError);
                } else {
                    throw lastError || new Error('All API URLs failed');
                }
            }
        } catch (error) {
            console.error('Error loading flight stats:', error);
            this.showError('Không thể tải thống kê chuyến bay. Lỗi: ' + error.message);
        }
    }

    showAuthenticationError(error) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger text-center';
        errorDiv.innerHTML = `
            <h5><i class="fas fa-exclamation-triangle"></i> Cần đăng nhập</h5>
            <p><strong>Error:</strong> ${error.responseText || 'Không có quyền truy cập'}</p>
            <p>Vui lòng đăng nhập để xem thông tin chuyến bay của bạn.</p>
            <a href="/Login" class="btn btn-primary">
                <i class="fas fa-sign-in-alt"></i> Đăng nhập ngay
            </a>
        `;
        
        const container = document.querySelector('.container-fluid') || document.body;
        const existing = container.querySelector('.alert-danger, .alert-warning');
        if (existing) existing.remove();
        container.insertBefore(errorDiv, container.firstChild);
        
        this.showLoading(false);
    }

    updateStatsDisplay() {
        const stats = this.flightsData.stats;
        
        const upcomingElement = document.getElementById('upcomingCount');
        if (upcomingElement) upcomingElement.textContent = stats.upcomingFlights || 0;
        
        const completedElement = document.getElementById('completedCount');
        if (completedElement) completedElement.textContent = stats.completedFlights || 0;
        
        const totalElement = document.getElementById('totalCount');
        if (totalElement) totalElement.textContent = stats.totalFlights || 0;
        
        const spentElement = document.getElementById('totalSpent');
        if (spentElement) spentElement.textContent = this.formatCurrency(stats.totalSpent || 0);
    }

    async loadCalendarData(year, month) {
        try {
            console.log(`🗓️ Loading calendar data for ${year}/${month}`);
            
            let success = false;
            
            for (let i = 0; i < this.apiUrls.length && !success; i++) {
                const apiUrl = this.apiUrls[i];
                const isProxy = apiUrl.includes('MyFlights');
                console.log(`🌐 Attempting calendar API URL ${i + 1}/${this.apiUrls.length}: ${apiUrl} (${isProxy ? 'proxy' : 'direct'})`);
                
                try {
                    const token = this.getAuthToken();
                    
                    const apiEndpoint = isProxy ? `${apiUrl}/calendar?year=${year}&month=${month}` : `${apiUrl}/MyFlight/calendar?year=${year}&month=${month}`;
                    
                    const headers = {
                        'Content-Type': 'application/json'
                    };
                    
                    if (token) {
                        headers['Authorization'] = `Bearer ${token}`;
                    }
                    
                    const response = await fetch(apiEndpoint, {
                        method: 'GET',
                        headers: headers,
                        credentials: 'include'
                    });

                    console.log(`📅 Calendar API response status (${apiUrl}):`, response.status);
                    
                    if (response.ok) {
                        const result = await response.json();
                        if (result.success) {
                            console.log(`✅ Calendar API call succeeded with ${apiUrl}!`);
                            this.apiBaseUrl = apiUrl;
                            this.renderCalendar(result.data);
                            success = true;
                            break;
                        }
                    } else if (response.status === 401) {
                        console.error(`🔐 401 Unauthorized from calendar API ${apiUrl}`);
                        // Continue to next URL
                    } else {
                        console.error(`❌ Calendar API error from ${apiUrl}: ${response.status}`);
                    }
                } catch (error) {
                    console.error(`❌ Error with calendar API URL ${apiUrl}:`, error);
                }
            }
            
            if (!success) {
                console.error('❌ All calendar API URLs failed');
            }
            
            this.updateCalendarTitle(year, month);
        } catch (error) {
            console.error('Error loading calendar data:', error);
            this.showError('Không thể tải dữ liệu lịch.');
        }
    }

    renderCalendar(calendarData) {
        const calendarGrid = document.getElementById('calendarGrid');
        if (!calendarGrid) return;

        this.flightsData.calendar = calendarData;
        calendarGrid.innerHTML = '';

        const daysOfWeek = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
        daysOfWeek.forEach(day => {
            const dayHeader = document.createElement('div');
            dayHeader.className = 'calendar-day-header';
            dayHeader.textContent = day;
            calendarGrid.appendChild(dayHeader);
        });

        const firstDay = new Date(calendarData.year, calendarData.month - 1, 1);
        const lastDay = new Date(calendarData.year, calendarData.month, 0);
        const firstDayOfWeek = firstDay.getDay();
        const daysInMonth = lastDay.getDate();

        for (let i = 0; i < firstDayOfWeek; i++) {
            const emptyDay = document.createElement('div');
            emptyDay.className = 'calendar-day empty';
            calendarGrid.appendChild(emptyDay);
        }

        for (let day = 1; day <= daysInMonth; day++) {
            const dayElement = document.createElement('div');
            const dateString = `${calendarData.year}-${String(calendarData.month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
            
            dayElement.className = 'calendar-day';
            if (this.isToday(dateString)) {
                dayElement.classList.add('today');
            }

            const dayData = calendarData.calendarDays.find(d => 
                new Date(d.date).toDateString() === new Date(dateString).toDateString()
            );

            let dayContent = `<div class="day-number">${day}</div>`;
            
            if (dayData && dayData.hasFlights) {
                dayElement.classList.add('has-flights');
                dayContent += `<div class="flight-count">${dayData.flightCount} chuyến</div>`;
            }

            dayElement.innerHTML = dayContent;
            calendarGrid.appendChild(dayElement);
        }
        
        this.showLoading(false);
    }

    isToday(dateString) {
        const today = new Date().toDateString();
        const compareDate = new Date(dateString).toDateString();
        return today === compareDate;
    }

    updateCalendarTitle(year, month) {
        const titleElement = document.getElementById('calendarTitle');
        if (titleElement) {
            const monthNames = [
                'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
            ];
            titleElement.textContent = `${monthNames[month - 1]} ${year}`;
        }

        this.currentYear = year;
        this.currentMonth = month;
    }

    navigateMonth(direction) {
        let newMonth = this.currentMonth + direction;
        let newYear = this.currentYear;

        if (newMonth < 1) {
            newMonth = 12;
            newYear--;
        } else if (newMonth > 12) {
            newMonth = 1;
            newYear++;
        }

        this.loadCalendarData(newYear, newMonth);
    }

    handleSearch() {
        const searchTerm = document.getElementById('flightSearch')?.value;
        if (!searchTerm) return;
        this.searchFlights(searchTerm);
    }

    async searchFlights(searchTerm) {
        try {
            const token = this.getAuthToken();
            const response = await fetch(`${this.apiBaseUrl}/MyFlight/search?term=${encodeURIComponent(searchTerm)}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Failed to search flights');
            }

            const result = await response.json();
            if (result.success) {
                console.log('Search results:', result.data);
            }
        } catch (error) {
            console.error('Error searching flights:', error);
        }
    }

    showLoading(show) {
        const loadingElement = document.getElementById('loadingSpinner');
        if (loadingElement) {
            loadingElement.style.display = show ? 'flex' : 'none';
        }
        console.log('Loading:', show);
    }

    showError(message) {
        console.error('MyFlights Error:', message);
        alert(message);
    }

    getAuthToken() {
        console.log('=== MyFlights GET AUTH TOKEN DEBUG ===');
        
        // Try to get JWT token from cookies first, then localStorage, sessionStorage
        const cookieToken = this.getCookie('X-Access-Token');
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
        const altCookieToken = this.getCookie('authToken') || this.getCookie('token') || this.getCookie('jwt');
        console.log('Alternative cookies:', altCookieToken ? altCookieToken.substring(0, 30) + '...' : 'NOT FOUND');
        
        // Try to use global function if available
        let globalToken = null;
        if (typeof window.getAuthToken === 'function') {
            globalToken = window.getAuthToken();
            console.log('Global getAuthToken result:', globalToken ? globalToken.substring(0, 30) + '...' : 'NOT FOUND');
        }
        
        const finalToken = cookieToken || 
                          localStorageAuthToken || 
                          sessionStorageAuthToken || 
                          localStorageToken || 
                          sessionStorageToken ||
                          altCookieToken ||
                          globalToken || '';
        
        console.log('Final token selected:', finalToken ? finalToken.substring(0, 30) + '...' : 'NO TOKEN AVAILABLE');
        console.log('All cookies:', document.cookie);
        console.log('Current URL:', window.location.href);
        console.log('=== END MyFlights GET AUTH TOKEN DEBUG ===');
        
        return finalToken;
    }

    getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
        return null;
    }

    formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

    getLanguageCookie() {
        return this.getCookie('selectedLanguage');
    }

    changeLanguage(language) {
        this.currentLanguage = language;
        document.cookie = `selectedLanguage=${language}; path=/`;
        this.updateLanguageElements();
    }

    debounce(func, wait) {
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
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 DOM loaded, initializing MyFlights...');
    
    // Check if we have the required elements
    const requiredElements = ['loadingSpinner', 'upcomingCount', 'completedCount', 'totalCount', 'totalSpent', 'calendarGrid', 'calendarTitle'];
    const missingElements = requiredElements.filter(id => !document.getElementById(id));
    
    if (missingElements.length > 0) {
        console.warn('⚠️ Some required elements are missing:', missingElements);
    }
    
    // Initialize MyFlights manager
    window.myFlightsManager = new MyFlightsManager();
    console.log('✅ MyFlights manager initialized and attached to window');
});

console.log('📄 MyFlights.js script loaded successfully');
